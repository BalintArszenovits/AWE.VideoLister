using Microsoft.Extensions.DependencyInjection;
using AWE.VideoLister.BusinessLogic.Clients;
using AWE.VideoLister.BusinessLogic.Providers;

namespace AWE.VideoLister.BusinessLogic.Extensions
{
    /// <summary>
    /// Extensions class for IServiceProvider
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers internal services
        /// </summary>
        public static void RegisterAweServices(this IServiceCollection services)
        {
            services.AddSingleton<IConfigurationProvider, ConfigurationProvider>();
            services.AddSingleton<ICredentialProvider, CredentialProvider>();
            services.AddSingleton<IAWEClient, AWEClient>();
            services.AddSingleton<IContentProvider, ContentProvider>();
        }
    }
}
