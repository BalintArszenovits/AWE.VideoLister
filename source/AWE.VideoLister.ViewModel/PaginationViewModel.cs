namespace AWE.VideoLister.ViewModel
{
    public class PaginationViewModel
    {
        public int Total { get; set; }
        public int Count { get; set; }
        public int PerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Limit { get; set; }
    }
}
