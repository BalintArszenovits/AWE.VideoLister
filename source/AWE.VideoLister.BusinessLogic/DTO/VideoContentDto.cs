namespace AWE.VideoLister.BusinessLogic.DTO
{
    internal class VideoContentDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string ProfileImage { get; set; }
        public string CoverImage { get; set; }
        public IEnumerable<string> PreviewImages { get; set; }
        public string TargetUrl { get; set; }
        public string DetailsUrl { get; set; }
        public string Quality { get; set; }
        public bool IsHd { get; set; }
        public string Uploader { get; set; }
        public string UploaderLink { get; set; }
    }
}
