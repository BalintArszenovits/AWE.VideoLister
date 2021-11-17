using AWE.VideoLister.ViewModel;

namespace AWE.VideoLister.BusinessLogic.DTO
{
    internal class PaginationDto
    {
        public int Total { get; set; }
        public int Count { get; set; }
        public int PerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public PaginationViewModel ToViewModel()
        {
            return new PaginationViewModel
            {
                Total = Total,
                Count = Count,
                CurrentPage = CurrentPage,
                PerPage = PerPage,
                TotalPages = TotalPages,
                Limit = PerPage
            };
        }
    }
}
