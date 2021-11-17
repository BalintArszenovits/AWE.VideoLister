using System.Text.Json;
using AWE.VideoLister.BusinessLogic.DTO;

namespace AWE.VideoLister.BusinessLogic.Providers
{
    /// <inheritdoc />
    internal class CredentialProvider : ICredentialProvider
    {
        private readonly string credentialsFileName = "credentials.json";
        private CredentialsDto cachedCredentials;

        /// <inheritdoc />
        public async Task<CredentialsDto> GetCredentialsAsync()
        {
            //If credentials are cached, return the cached credentials
            if (cachedCredentials != null)
            {
                return cachedCredentials;
            }

            string json = await File.ReadAllTextAsync(credentialsFileName);

            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            CredentialsDto credentials = JsonSerializer.Deserialize<CredentialsDto>(json, options);

            //Cache credentials
            cachedCredentials = credentials;

            return credentials;
        }
    }
}
