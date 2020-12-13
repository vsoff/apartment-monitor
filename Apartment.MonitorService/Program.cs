using System;
using System.Threading;
using Apartment.Core;
using Autofac;

namespace Apartment.MonitorService
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LoggerModule.GetLogger();

            try
            {
                logger.Info("Приложение запущено");

                // Собираем контейнер.
                var builder = new ContainerBuilder();
                builder.RegisterModule<LoggerModule>();
                builder.RegisterModule<ApartmentsMonitorServiceModule>();
                var container = builder.Build();

                // Запускаем.
                container.Resolve<ApartmentsMonitorWorker>().Start();

                // Ожидаем завершения приложения.
                ManualResetEvent resetEvent = new ManualResetEvent(false);
                resetEvent.WaitOne();
                container.Dispose();

                logger.Info("Работа приложения завершена");
            }
            catch (Exception ex)
            {
                logger.Error("Произошла ошибка во время работы приложения", ex);
                throw;
            }
        }
    }
}