using System;
using System.IO;
using Apartment.Core.Loggers;
using Autofac;
using Serilog;
using ILogger = Apartment.Common.Loggers.ILogger;

namespace Apartment.Core
{
    public class LoggerModule : Module
    {
        private static ILogger _logger;

        public static ILogger GetLogger()
        {
            if (_logger == null)
            {
                var fullPathFormat = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"{DateTime.Now:yyyy.MM.dd}_main.txt");
                var serilogLogger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.File(fullPathFormat)
                    .CreateLogger();

                _logger = new SerilogLogger(serilogLogger);
            }

            return _logger;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(GetLogger()).SingleInstance();
        }
    }
}