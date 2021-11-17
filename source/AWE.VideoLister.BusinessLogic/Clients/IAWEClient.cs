using AWE.VideoLister.BusinessLogic.DTO;

namespace AWE.VideoLister.BusinessLogic.Clients
{
    /// <summary>
    /// Encapsulates a HTTPClient for AWE-related HTTP calls
    /// </summary>
    internal interface IAWEClient
    {
        /// <summary>
        /// Returns the content list by arguments specified in the query string
        /// </summary>
        public Task<ContentListingDto> GetContentListAsync(string queryString);

        /// <summary>
        /// Returns a file by URL
        /// </summary>
        public Task<FileDataDto> GetFileAsync(string url);
    }
}
