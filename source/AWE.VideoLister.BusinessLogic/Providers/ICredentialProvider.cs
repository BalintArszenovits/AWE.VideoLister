using AWE.VideoLister.BusinessLogic.DTO;

namespace AWE.VideoLister.BusinessLogic.Providers
{
    /// <summary>
    /// Provides user credentials
    /// </summary>
    internal interface ICredentialProvider
    {
        /// <summary>
        /// Returns user credentials
        /// </summary>
        public Task<CredentialsDto> GetCredentialsAsync();
    }
}
