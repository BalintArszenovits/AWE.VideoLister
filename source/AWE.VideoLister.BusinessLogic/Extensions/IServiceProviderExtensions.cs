using Microsoft.Extensions.DependencyInjection;
using AWE.VideoLister.BusinessLogic.Providers;

namespace AWE.VideoLister.BusinessLogic.Extensions
{
    /// <summary>
    /// Extensions class for IServiceProvider
    /// </summary>
    public static class IServiceProviderExtensions
    {
        /// <summary>
        /// Initializes services which required to be initialized
        /// </summary>
        public async static Task InitializeAweServices(this IServiceProvider serviceProvider)
        {
            IConfigurationProvider configurationProviderserviceProvider = serviceProvider.GetService<IConfigurationProvider>();
            await configurationProviderserviceProvider?.InitializeConfigurationProviderAsync();
        }
    }
}
