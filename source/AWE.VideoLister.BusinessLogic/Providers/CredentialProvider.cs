using System.Text.Json;
using AWE.VideoLister.BusinessLogic.DTO;

namespace AWE.VideoLister.BusinessLogic.Providers
{
    /// <inheritdoc />
    internal class CredentialProvider : ICredentialProvider
    {
        private readonly ILoggingProvider loggingProvider;
        private readonly string credentialsFileName = "credentials.json";
        private CredentialsDto cachedCredentials;

        public CredentialProvider(ILoggingProvider loggingProvider)
        {
            this.loggingProvider = loggingProvider;
        }

        /// <inheritdoc />
        public async Task<CredentialsDto> GetCredentialsAsync()
        {
            //If credentials are cached, return the cached credentials
            if (cachedCredentials != null)
            {
                return cachedCredentials;
            }

            string json = default(string);

            try
            {
                json = await File.ReadAllTextAsync(credentialsFileName);
            }
            catch (IOException)
            {
                loggingProvider.LogError($"Credentials configuration file [credentials.json] could not be loaded");
                return null;
            }

            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            CredentialsDto credentials = JsonSerializer.Deserialize<CredentialsDto>(json, options);

            //Cache credentials
            cachedCredentials = credentials;

            return credentials;
        }
    }
}
