namespace AWE.VideoLister.BusinessLogic.Providers
{
    /// <summary>
    /// Provides application settings
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Base address for the HTTP client
        /// </summary>
        public string HttpClientBaseAddress { get; }

        /// <summary>
        /// Application working directory path
        /// </summary>
        public string TempFileDirectoryPath { get; }

        /// <summary>
        /// Initializes Configuration Provider
        /// </summary>
        public Task InitializeConfigurationProviderAsync();
    }
}
