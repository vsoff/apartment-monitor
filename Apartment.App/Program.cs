using System;
using System.Net;
using Apartment.App.Views;
using Autofac;

namespace Apartment.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Добавляем поддержку протокола "TLS1.2".
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            // Формируем контейнер и билдим его.
            var builder = new ContainerBuilder();
            builder.RegisterModule<ApartmentAppModule>();
            var container = builder.Build();

            // Запускаем окно приложения.
            using var scope = container.BeginLifetimeScope();
            var main = scope.Resolve<MainWindow>();
            main.ShowDialog();
        }
    }
}