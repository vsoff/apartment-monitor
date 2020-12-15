using System.IO;
using Apartment.App.ViewModels;
using Apartment.App.Views;
using Apartment.Common;
using Apartment.Common.Loggers;
using Apartment.Core;
using Apartment.Core.Providers;
using Apartment.Core.Services;
using Apartment.Data;
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
            builder.RegisterInstance(LoggerModule.GetLogger()).As<ILogger>();
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainWindowViewModel>().AsSelf();
            builder.RegisterType<MapViewModel>().AsSelf();

            var config = RegisterOptions(builder);

            if (config.UseOriginalProvider)
                builder.RegisterType<AvitoApartmentsProvider>().As<IApartmentsProvider>();
            else
            {
                builder.RegisterType<ApartmentService>().AsSelf();
                builder.RegisterType<ApplicationContextProvider>().As<IDatabaseContextProvider>();
                builder.RegisterType<DatabaseApartmentsProvider>().As<IApartmentsProvider>();
            }
        }

        private ApplicationOptions RegisterOptions(ContainerBuilder builder)
        {
            // Собираем конфигурацию.
            // TODO: Стоит пересобрать конфиг по другому.
            var configJson = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<ApplicationOptions>(configJson);
            builder.RegisterInstance(config).AsSelf();
            builder.RegisterInstance(config.DataBase).AsSelf();
            builder.RegisterInstance(config.Debug).AsSelf();

            return config;
        }
    }
}