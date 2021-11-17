using System.Collections.Generic;
using System.Windows;
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
        private readonly IContentProvider contentProvider;

        public MainWindow(IContentProvider contentProvider)
        {
            InitializeComponent();
            this.contentProvider = contentProvider;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FilteringViewModel filters = new FilteringViewModel()
            {
                SexualOrientation = new List<SexualOrientation>() { SexualOrientation.Straight },
                Quality = new List<Quality>() { Quality.HD },
                ForcedPerformers = string.Empty
            };

            PaginationViewModel pagination = new PaginationViewModel()
            {
                Limit = 25
            };

            ContentListingViewModel viewModel = await contentProvider.GetContentListAsync(filters, pagination);
        }
    }
}
