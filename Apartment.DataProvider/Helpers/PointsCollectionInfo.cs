using GMap.NET;

namespace Apartment.DataProvider.Helpers
{
    internal readonly struct PointsCollectionInfo
    {
        public RectLatLng Rect { get; }
        public PointLatLng Center { get; }

        public PointsCollectionInfo(RectLatLng rect, PointLatLng center)
        {
            Rect = rect;
            Center = center;
        }
    }
}