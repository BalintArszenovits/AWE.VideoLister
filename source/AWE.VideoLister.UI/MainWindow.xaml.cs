using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AWE.VideoLister.BusinessLogic.Providers;
using AWE.VideoLister.ViewModel;
using AWE.VideoLister.ViewModel.Enum;

namespace AWE.VideoLister.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //User control dimensions constants
        private const int ProfileImageWidth = 227;
        private const int ProfileImageHeight = 171;

        private const int PreviewImageWidth = 400;
        private const int PreviewImageHeight = 226;

        private const int DetailsStackPanelWidth = 300;
        private const int DetailsStackPanelHeight = 226;

        private const int ScrollViewerWidth = 753;

        private readonly IContentProvider contentProvider;
        private readonly ILoggingProvider loggingProvider;
        private readonly DispatcherTimer dispatcherTimer;

        public MainWindow(IContentProvider contentProvider, ILoggingProvider loggingProvider)
        {
            InitializeComponent();
            InitializeComboBoxes();

            dispatcherTimer = new DispatcherTimer();
            InitializeLoggingTimer();

            this.contentProvider = contentProvider;
            this.loggingProvider = loggingProvider;
        }

        private async void btnFetch_Click(object sender, RoutedEventArgs e)
        {
            //Handle issue with IntegerUpDown default value
            if (string.IsNullOrWhiteSpace(numPageNumber.Text))
            {
                numPageNumber.Text = "1";
            }

            int pageNumber = int.Parse(numPageNumber.Text);
            pageNumber = Math.Clamp(pageNumber, 1, int.MaxValue);

            numPageNumber.Text = pageNumber.ToString();

            ContentListingViewModel contentListingViewModel = await FetchContentAsync(pageNumber);
            UpdateMainContentView(contentListingViewModel);

            tbTotal.Text = contentListingViewModel.Pagination.Total.ToString();
            tbTotalPages.Text = contentListingViewModel.Pagination.TotalPages.ToString();
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            //Handle issue with IntegerUpDown default value
            if (string.IsNullOrWhiteSpace(numPageNumber.Text))
            {
                numPageNumber.Text = "0";
            }

            int pageNumber = int.Parse(numPageNumber.Text);
            ++pageNumber;
            pageNumber = Math.Clamp(pageNumber, 1, int.MaxValue);

            numPageNumber.Text = pageNumber.ToString();

            ContentListingViewModel contentListingViewModel = await FetchContentAsync(pageNumber);
            UpdateMainContentView(contentListingViewModel);

            tbTotal.Text = contentListingViewModel.Pagination.Total.ToString();
            tbTotalPages.Text = contentListingViewModel.Pagination.TotalPages.ToString();
        }

        private async void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //Handle issue with IntegerUpDown default value
            if (string.IsNullOrWhiteSpace(numPageNumber.Text))
            {
                numPageNumber.Text = "2";
            }

            int pageNumber = int.Parse(numPageNumber.Text);
            --pageNumber;
            pageNumber = Math.Clamp(pageNumber, 1, int.MaxValue);

            numPageNumber.Text = pageNumber.ToString();

            ContentListingViewModel contentListingViewModel = await FetchContentAsync(pageNumber);
            UpdateMainContentView(contentListingViewModel);

            tbTotal.Text = contentListingViewModel.Pagination.Total.ToString();
            tbTotalPages.Text = contentListingViewModel.Pagination.TotalPages.ToString();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            string message = default(string);

            if (loggingProvider.TryGetLastMessage(out message))
            {
                StringBuilder sb = new StringBuilder(tbLog.Text);
                sb.Append($"{message}{Environment.NewLine}");
                tbLog.Text = sb.ToString();
                tbLog.ScrollToEnd();
            }
        }

        private void InitializeComboBoxes()
        {
            cbSexualOrientation.Items.Clear();
            cbSexualOrientation.ItemsSource = Enum.GetValues(typeof(SexualOrientation)).Cast<SexualOrientation>();
            cbSexualOrientation.SelectedIndex = 0;

            cbQuality.Items.Clear();
            cbQuality.ItemsSource = Enum.GetValues(typeof(Quality)).Cast<Quality>();
            cbQuality.SelectedIndex = 0;
        }

        private void InitializeLoggingTimer()
        {
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dispatcherTimer.Start();
        }

        private async Task<ContentListingViewModel> FetchContentAsync(int pageNumber)
        {
            loggingProvider.LogInfo("Fetch content...");

            FilteringViewModel filters = new FilteringViewModel()
            {
                SexualOrientation = new List<SexualOrientation>() { (SexualOrientation)cbSexualOrientation.SelectedValue },
                Quality = new List<Quality>() { (Quality)cbQuality.SelectedValue },
                ForcedPerformers = string.Empty
            };

            PaginationViewModel pagination = new PaginationViewModel()
            {
                Limit = 25,
                CurrentPage = pageNumber
            };

            return await contentProvider.GetContentListAsync(filters, pagination);
        }

        private void UpdateMainContentView(ContentListingViewModel contentListingViewModel)
        {
            spContent.Children.Clear();

            foreach (VideoViewModel item in contentListingViewModel.Videos)
            {
                if (item.ProfileImage != null)
                {
                    spContent.Children.Add(CreateStackPanelRow(item));
                }
            }

            loggingProvider.LogInfo("Fetching content finished");
        }

        private static StackPanel CreateStackPanelRow(VideoViewModel videoViewModel)
        {
            //mainStackPanel will contain both infoStackPanel and previewStackPanel
            StackPanel mainStackPanel = new StackPanel();
            mainStackPanel.Orientation = Orientation.Horizontal;
            mainStackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            //infoStackPanel contains both image and details stack panels
            StackPanel infoStackPanel = new StackPanel();
            infoStackPanel.Orientation = Orientation.Horizontal;

            //If profile image exists, add profile image. If not, add the first preview image, if there is any.
            if (!string.IsNullOrWhiteSpace(videoViewModel.ProfileImage))
            {
                infoStackPanel.Children.Add(CreateImageStackPanel(
                    videoViewModel.ProfileImage, 
                    ProfileImageWidth, 
                    ProfileImageHeight));
            }
            else if (videoViewModel.PreviewImages.Any())
            {
                infoStackPanel.Children.Add(CreateImageStackPanel(
                    videoViewModel.PreviewImages.First(),
                    PreviewImageWidth,
                    PreviewImageHeight));
            }
            else
            {
                infoStackPanel.Children.Add(CreateImageStackPanel(
                    null,
                    PreviewImageWidth,
                    PreviewImageHeight));
            }

            //Add video details
            infoStackPanel.Children.Add(CreateDetailsStackPanel(videoViewModel));

            //Add infoStackPanel into mainStackPanel
            mainStackPanel.Children.Add(infoStackPanel);

            //Add previewStackPanel into mainStackPanel
            mainStackPanel.Children.Add(CreateScrollViewer(CreatePreviewStackPanel(videoViewModel)));

            return mainStackPanel;
        }

        private static StackPanel CreateImageStackPanel(string imagePath, int width, int height)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(CreateImage(imagePath, width, height));
            return stackPanel;
        }

        private static StackPanel CreateDetailsStackPanel(VideoViewModel videoViewModel)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Width = DetailsStackPanelWidth;
            stackPanel.Height = DetailsStackPanelHeight;

            stackPanel.Children.Add(CreateLabel("Title", videoViewModel.Title));
            stackPanel.Children.Add(CreateLabel("Uploader", videoViewModel.Uploader));
            stackPanel.Children.Add(CreateLabel("Duration", videoViewModel.Duration.ToString()));
            stackPanel.Children.Add(CreateLabel("High Definition", videoViewModel.IsHd.ToString()));
            stackPanel.Children.Add(CreateLabel("Quality", videoViewModel.Quality));
            stackPanel.Children.Add(CreateTagsLabel(videoViewModel.Tags));

            return stackPanel;

        }

        private static StackPanel CreatePreviewStackPanel(VideoViewModel videoViewModel)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            foreach (string item in videoViewModel.PreviewImages)
            {
                stackPanel.Children.Add(CreateImage(item, PreviewImageWidth, PreviewImageHeight));
            }

            return stackPanel;
        }

        private static ScrollViewer CreateScrollViewer(StackPanel stackPanel)
        {
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = stackPanel;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.HorizontalAlignment = HorizontalAlignment.Stretch;
            scrollViewer.Width = ScrollViewerWidth;

            return scrollViewer;
        }

        private static Label CreateLabel(string title, string text)
        {
            Label label = new Label();
            label.Content = $"{title}: {text}";
            label.Padding = new Thickness(5, 0, 5, 0);
            return label;
        }

        private static Label CreateTagsLabel(IEnumerable<string> tags)
        {
            Label label = new Label();
            label.Padding = new Thickness(5, 0, 5, 0);

            StringBuilder sb = new StringBuilder();
            sb.Append("Tags: ");

            int newLineCounter = 1;

            foreach (string item in tags)
            {
                sb.Append($"{item}, ");

                if (newLineCounter == 5)
                {
                    sb.Append(Environment.NewLine);
                    newLineCounter = 1;
                }

                newLineCounter++;
            }

            sb.Remove(sb.Length - 1, 1);

            label.Content = sb.ToString();
            return label;
        }

        private static Image CreateImage(string imagePath, int width, int height)
        {
            Image image = new Image();
            BitmapImage imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.UriSource = new Uri(imagePath);
            imageSource.EndInit();
            image.Source = imageSource;
            image.Stretch = Stretch.UniformToFill;
            image.Width = width;
            image.Height = height;
            return image;
        }
    }
}
