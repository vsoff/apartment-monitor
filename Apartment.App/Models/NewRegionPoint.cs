using GMap.NET;

namespace Apartment.App.Models
{
    public class NewRegionPoint
    {
        public PointLatLng Location { get; }

        public NewRegionPoint(PointLatLng location)
        {
            Location = location;
        }
    }
}