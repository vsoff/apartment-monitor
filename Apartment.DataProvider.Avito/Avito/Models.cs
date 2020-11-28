namespace Apartment.DataProvider.Avito.Avito
{
    internal class MapApartmentsResponseWebModel
    {
        public object[] markers { get; set; }
        public Maparea mapArea { get; set; }
        public Options options { get; set; }
        public Rash[] rash { get; set; }
        public Features features { get; set; }
    }

    internal class Maparea
    {
        public Topleft topLeft { get; set; }
        public Bottomright bottomRight { get; set; }
    }

    internal class Topleft
    {
        public float lng { get; set; }
        public float lat { get; set; }
    }

    internal class Bottomright
    {
        public float lng { get; set; }
        public float lat { get; set; }
    }

    internal class Options
    {
        public int zoom { get; set; }
    }

    internal class Features
    {
    }

    internal class Rash
    {
        public string id { get; set; }
        public string type { get; set; }
        public int itemsCount { get; set; }
        public Coords coords { get; set; }
        public int radius { get; set; }
        public bool isFavorite { get; set; }
        public string favoritesIds { get; set; }
        public Price price { get; set; }
    }

    internal class Coords
    {
        public float lng { get; set; }
        public float lat { get; set; }
    }

    internal class Price
    {
        public int value { get; set; }
        public string title { get; set; }
    }
}