using System.IO;
using Apartment.Common.Workers;
using Apartment.Core;
using Apartment.Data;
using Apartment.DataProvider;
using Apartment.DataProvider.Avito;
using Apartment.Options;
using Autofac;
using Newtonsoft.Json;

namespace Apartment.MonitorService
{
    internal class ApartmentsMonitorServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApartmentsMonitorWorker>().AsSelf().SingleInstance();
            builder.RegisterType<ApplicationContextProvider>().As<IDatabaseContextProvider>().SingleInstance();
            builder.RegisterType<AvitoApartmentsProvider>().As<IApartmentsProvider>().SingleInstance();
            builder.RegisterType<DefaultWorkerController>().As<IWorkerController>().SingleInstance();
            builder.RegisterType<ApartmentService>().AsSelf().SingleInstance();

            RegisterOptions(builder);
        }

        private void RegisterOptions(ContainerBuilder builder)
        {
            // Собираем конфигурацию.
            // TODO: Стоит пересобрать конфиг по другому.
            var configJson = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<MonitoringServiceOptions>(configJson);
            builder.RegisterInstance(config).AsSelf();
            builder.RegisterInstance(config.DataBase).AsSelf();
            builder.RegisterInstance(config.Debug).AsSelf();
        }
    }
}