using System;

namespace Apartment.Options
{
    public class MonitoringServiceOptions
    {
        //public const string Section = "MonitoringService";

        public DatabaseOptions DataBase { get; set; }
        public DebugOptions Debug { get; set; }
        public TimeSpan WorkerInterval { get; set; }
    }

    // TODO: Надо сделать настройки иммутабельными.
    public class ApplicationOptions
    {
        //public const string Section = "ApartmentApplication";

        public DatabaseOptions DataBase { get; set; }
        public StartPosition StartPosition { get; set; }
        public DebugOptions Debug { get; set; }
        public bool UseOriginalProvider { get; set; }
    }

    public class DebugOptions
    {
        public bool UseProviderCache { get; set; }
    }

    public class DatabaseOptions
    {
        public string DataBaseConnectionString { get; set; }
        public DataProviderType DataProviderType { get; set; }
    }

    public class StartPosition
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public enum DataProviderType
    {
        Unknown = 0,
        MsSql = 1,
        MySql = 2
    }
}