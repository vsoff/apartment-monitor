using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Apartment.App.Components;
using Apartment.App.ViewModels;
using Apartment.App.Views;
using Apartment.DataProvider;
using Apartment.DataProvider.Avito.Avito;
using Apartment.Options;
using Autofac;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Newtonsoft.Json;

namespace Apartment.App
{
    internal class ApartmentAppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainWindowViewModel>().AsSelf();
            builder.RegisterType<MapViewModel>().AsSelf();

            builder.RegisterType<AvitoApartmentsProvider>().As<IApartmentsProvider>();

            RegisterOptions(builder);
        }

        private void RegisterOptions(ContainerBuilder builder)
        {
            // Собираем конфигурацию.
            // TODO: Стоит пересобрать конфиг по другому.
            var configJson = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<ApplicationOptions>(configJson);
            builder.RegisterInstance(config).AsSelf();
        }
    }
}