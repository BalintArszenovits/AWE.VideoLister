namespace AWE.VideoLister.ViewModel
{
    public class ContentListingViewModel
    {
        public IEnumerable<VideoViewModel> Videos { get; set; }
        public PaginationViewModel Pagination { get; set; }

        public ContentListingViewModel()
        {
            Videos = new List<VideoViewModel>();
        }
    }
}
