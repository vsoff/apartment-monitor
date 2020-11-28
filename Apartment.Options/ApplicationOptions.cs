namespace Apartment.Options
{
    public class ApplicationOptions
    {
        //public const string Section = "ApartmentApplication";

        // TODO: Потом эта настройка уйдет. Она пока что на время разработки.
        public string AvitoUrl { get; set; }
        public StartPosition StartPosition { get; set; }
    }

    public class StartPosition
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}