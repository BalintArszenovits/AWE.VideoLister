using System.Text;
using AWE.VideoLister.BusinessLogic.Clients;
using AWE.VideoLister.BusinessLogic.DTO;
using AWE.VideoLister.ViewModel;
using AWE.VideoLister.ViewModel.Enum;

namespace AWE.VideoLister.BusinessLogic.Providers
{
    /// <inheritdoc />
    internal class ContentProvider : IContentProvider
    {
        private readonly string tempFileDirectoryPath;
        private readonly IAWEClient aweClient;
        private readonly ICredentialProvider credentialProvider;

        public ContentProvider(IAWEClient aweClient, ICredentialProvider credentialProvider, IConfigurationProvider configurationProvider)
        {
            tempFileDirectoryPath = configurationProvider.TempFileDirectoryPath;
            this.aweClient = aweClient;
            this.credentialProvider = credentialProvider;
        }

        /// <inheritdoc />
        public async Task<ContentListingViewModel> GetContentListAsync(FilteringViewModel filters, PaginationViewModel pagination)
        {
            //Creates working directory of does not exists
            if (!Directory.Exists(tempFileDirectoryPath))
            {
                Directory.CreateDirectory(tempFileDirectoryPath);
            }

            //Get credentials
            CredentialsDto credentials = await credentialProvider.GetCredentialsAsync();

            //Get query string
            string queryString = GetQueryString(filters, pagination, credentials.PSID, credentials.AccessKey);

            //Get content listing
            ContentListingDto contentListingDto = await aweClient.GetContentListAsync(queryString);

            //Resolve content listing viewmodel for UI
            ContentListingViewModel contentListingViewModel = await ResolveContent(contentListingDto);

            return contentListingViewModel;
        }

        /// <summary>
        /// Builds the query string for querying content
        /// </summary>
        private static string GetQueryString(FilteringViewModel filters, PaginationViewModel pagination, string psid, string accessKey)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("?");

            //Add credentials
            sb.Append($"psid={psid}&");
            sb.Append($"accessKey={accessKey}&");

            //Add filtering options
            foreach (SexualOrientation item in filters.SexualOrientation)
            {
                sb.Append($"sexualOrientation={Enum.GetName(typeof(SexualOrientation), item).ToLowerInvariant()}&");
            }

            foreach (Quality item in filters.Quality)
            {
                sb.Append($"quality={Enum.GetName(typeof(Quality), item).ToLowerInvariant()}&");
            }

            sb.Append($"forcedPerformers={filters.ForcedPerformers}&");

            //Add paging options
            sb.Append($"limit={pagination.Limit}&");
            sb.Append($"pageIndex={pagination.CurrentPage}&");

            //Defaults
            sb.Append("pstool=421_1&");
            sb.Append("ms_notrack=1&");
            sb.Append("program=revs&");
            sb.Append("campaign_id=&");
            sb.Append("type=&");
            sb.Append("site=jasmin&");
            sb.Append("primaryColor=%238AC437&");
            sb.Append("labelColor=%23212121");

            return sb.ToString();
        }

        /// <summary>
        /// Resolves content resources
        /// </summary>
        private async Task<ContentListingViewModel> ResolveContent(ContentListingDto contentListingDto)
        {
            ContentListingViewModel contentListingViewModel = new ContentListingViewModel();

            //Resolve videos, if there is any
            if (contentListingDto.Data.Videos.Any())
            {
                List<VideoViewModel> videoViewModels = new List<VideoViewModel>();

                List<Task<VideoViewModel>> videoTasks = contentListingDto.Data.Videos
                    .Select(video => ResolveVideo(video))
                    .ToList();

                await Task.WhenAll(videoTasks);

                videoTasks.ForEach(videoTask => {
                    //Add videos only, which did not encounter any kind of issues
                    if (videoTask.Status == TaskStatus.RanToCompletion)
                    {
                        videoViewModels.Add(videoTask.Result);
                    }
                });

                contentListingViewModel.Videos = videoViewModels;
            }

            contentListingViewModel.Pagination = contentListingDto.Data.Pagination.ToViewModel();

            return contentListingViewModel;
        }

        /// <summary>
        /// Resolves the video URLs in the dto file, downloads and writes them to the hard drive, returns a Video ViewModel with the path to these files
        /// </summary>
        private async Task<VideoViewModel> ResolveVideo(VideoContentDto videoDto)
        {
            VideoViewModel videoViewModel = new VideoViewModel();

            //Get Preview images
            if (videoDto.PreviewImages.Any())
            {
                //Get preview images from the server
                List<Task<FileDataDto>> previewImages = videoDto.PreviewImages
                    .Select(url => aweClient.GetFileAsync(FormatUrl(url)))
                    .ToList();

                await Task.WhenAll(previewImages);

                List<FileDataDto> previewImageData = new List<FileDataDto>();

                previewImages.ForEach(task => {
                    //Add succesful request results only
                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        previewImageData.Add(task.Result);
                    }
                });

                List<string> previewImagesPaths = new List<string>();

                //Write preview images to the disk
                previewImageData.ForEach(fileData => {
                    if (fileData != null)
                    {
                        string previewImagePath = Path.Combine(tempFileDirectoryPath, $"{Guid.NewGuid()}{GetFileExtension(fileData.MediaType)}");
                        File.WriteAllBytes(previewImagePath, fileData.Data);
                        previewImagesPaths.Add(previewImagePath);
                    }
                });

                videoViewModel.PreviewImages = previewImagesPaths;
            }

            //Get profile image
            if (!string.IsNullOrWhiteSpace(videoDto.ProfileImage))
            {
                Task<FileDataDto> profileImageDataTask = aweClient.GetFileAsync(FormatUrl(videoDto.ProfileImage));

                await Task.WhenAll(new List<Task>() { profileImageDataTask });

                //If HTTP call was succesful, write profile image to the disk
                if (profileImageDataTask.Result != null)
                {
                    FileDataDto profileImageData = profileImageDataTask.Result;
                    string profileImagePath = Path.Combine(tempFileDirectoryPath, $"{Guid.NewGuid()}{GetFileExtension(profileImageData.MediaType)}");
                    File.WriteAllBytes(profileImagePath, profileImageData.Data);
                    videoViewModel.ProfileImage = profileImagePath;
                }
            }

            //Get cover image
            if (!string.IsNullOrWhiteSpace(videoDto.CoverImage))
            {
                Task<FileDataDto> coverImageDataTask = aweClient.GetFileAsync(FormatUrl(videoDto.CoverImage));

                await Task.WhenAll(new List<Task>() { coverImageDataTask });

                //If HTTP call was succesful, write cover image to the disk
                if (coverImageDataTask.Status == TaskStatus.RanToCompletion)
                {
                    FileDataDto coverImageData = coverImageDataTask.Result;
                    string coverImagePath = Path.Combine(tempFileDirectoryPath, $"{Guid.NewGuid()}{GetFileExtension(coverImageData.MediaType)}");
                    File.WriteAllBytes(coverImagePath, coverImageData.Data);
                    videoViewModel.CoverImage = coverImagePath;
                }
            }

            videoViewModel.Tags = videoDto.Tags;
            videoViewModel.DetailsUrl = videoDto.DetailsUrl;
            videoViewModel.IsHd = videoDto.IsHd;
            videoViewModel.Quality = videoDto.Quality;
            videoViewModel.TargetUrl = videoDto.TargetUrl;
            videoViewModel.Title = videoDto.Title;
            videoViewModel.Uploader = videoDto.Uploader;
            videoViewModel.UploaderLink = videoDto.UploaderLink;

            return videoViewModel;
        }

        /// <summary>
        /// Gets the extension of the file resource by MIME type
        /// </summary>
        private static string GetFileExtension(string contentType)
        {
            return MimeTypes.MimeTypeMap.GetExtension(contentType);
        }

        /// <summary>
        /// Formats the URL if it is in an invalid format for HttpClient
        /// </summary>
        private static string FormatUrl(string url)
        {
            if (url.StartsWith("//"))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("https:");
                sb.Append(url);
                return sb.ToString();
            }

            return url;
        }
    }
}
