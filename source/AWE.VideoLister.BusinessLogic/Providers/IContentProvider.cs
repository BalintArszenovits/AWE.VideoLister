using AWE.VideoLister.ViewModel;

namespace AWE.VideoLister.BusinessLogic.Providers
{
    /// <summary>
    /// Provides AWE content
    /// </summary>
    public interface IContentProvider
    {
        /// <summary>
        /// Returns a content list which contains information and previews about available conent
        /// </summary>
        public Task<ContentListingViewModel> GetContentListAsync(FilteringViewModel filters, PaginationViewModel pagination);
    }
}
