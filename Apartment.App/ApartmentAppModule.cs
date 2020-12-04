using System.IO;
using Apartment.App.ViewModels;
using Apartment.App.Views;
using Apartment.DataProvider;
using Apartment.DataProvider.Avito;
using Apartment.Options;
using Autofac;
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