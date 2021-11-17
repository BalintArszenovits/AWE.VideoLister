using AWE.VideoLister.ViewModel.Enum;

namespace AWE.VideoLister.ViewModel
{
    public class FilteringViewModel
    {
        public IEnumerable<SexualOrientation> SexualOrientation { get; set; }
        public IEnumerable<Quality> Quality { get; set; }
        public string ForcedPerformers { get; set; }
    }
}
