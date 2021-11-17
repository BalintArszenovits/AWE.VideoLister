using System.Text.Json;
using AWE.VideoLister.BusinessLogic.DTO;

namespace AWE.VideoLister.BusinessLogic.Providers
{
    internal class ConfigurationProvider : IConfigurationProvider
    {
        private readonly string appSettingsPath = "appsettings.json";
        private AppSettingsDto appSettings;

        /// <inheritdoc />
        public string HttpClientBaseAddress => appSettings.HttpClientBaseAddress;
        /// <inheritdoc />
        public string TempFileDirectoryPath => Path.Combine(Path.GetTempPath(), appSettings.TempFileDirectoryName);

        /// <inheritdoc />
        public async Task InitializeConfigurationProviderAsync()
        {
            string json = await File.ReadAllTextAsync(appSettingsPath);
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            appSettings = JsonSerializer.Deserialize<AppSettingsDto>(json, options);
        }
    }
}
