using AWE.VideoLister.BusinessLogic.Clients;
using AWE.VideoLister.BusinessLogic.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AWE.VideoLister.BusinessLogic.Extensions;

namespace AWE.VideoLister.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.RegisterAweServices();
            services.AddSingleton<MainWindow>();
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await serviceProvider.InitializeAweServices();
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
