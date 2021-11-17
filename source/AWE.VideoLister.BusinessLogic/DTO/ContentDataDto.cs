namespace AWE.VideoLister.BusinessLogic.DTO
{
    internal class ContentDataDto
    {
        public IEnumerable<VideoContentDto> Videos { get; set; }
        public PaginationDto Pagination { get; set; }
    }
}
