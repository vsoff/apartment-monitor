using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Apartment.DataProvider.Avito.Common;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace Apartment.App.Components
{
    public class ApartmentMapControl : GMapControl
    {
        public PointLatLng LastPosition;

        public event MouseButtonEventHandler MouseLeftButtonClick;
        public event MouseButtonEventHandler MouseRightButtonClick;

        public ApartmentMapControl(PointLatLng startPosition)
        {
            _markers = new MultiValueDictionary<MapLayer, GMapMarker>()
            {
                [MapLayer.NewRegion] = new List<GMapMarker>(),
                [MapLayer.Apartments] = new List<GMapMarker>(),
                [MapLayer.Regions] = new List<GMapMarker>()
            };

            #region Map

            MapProvider = OpenStreetMapProvider.Instance;
            MinZoom = 2;
            MaxZoom = 17;
            Position = startPosition;
            Zoom = 14;
            MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            CanDragMap = true;
            DragButton = MouseButton.Left;

            LastPosition = startPosition;
            OnPositionChanged += point => { LastPosition = point; };

            Point? leftPoint = null;
            MouseLeftButtonUp += (sender, args) =>
            {
                if (!leftPoint.HasValue)
                    return;

                var newPoint = args.GetPosition(this);
                if (Math.Abs(newPoint.X - leftPoint.Value.X) < double.Epsilon && Math.Abs(newPoint.Y - leftPoint.Value.Y) < double.Epsilon)
                {
                    MouseLeftButtonClick?.Invoke(this, args);
                }
            };
            MouseLeftButtonDown += (sender, args) => { leftPoint = args.GetPosition(this); };

            Point? rightPoint = null;
            MouseRightButtonUp += (sender, args) =>
            {
                if (!rightPoint.HasValue)
                    return;

                var newPoint = args.GetPosition(this);
                if (Math.Abs(newPoint.X - rightPoint.Value.X) < double.Epsilon && Math.Abs(newPoint.Y - rightPoint.Value.Y) < double.Epsilon)
                {
                    MouseRightButtonClick?.Invoke(this, args);
                }
            };
            MouseRightButtonDown += (sender, args) => { rightPoint = args.GetPosition(this); };

            #endregion
        }

        public ApartmentsRegion FlushRegion(string regionName)
        {
            var newRegionMarkers = _markers[MapLayer.NewRegion];
            var newRegionPoints = newRegionMarkers
                .Where(x => x.GetType() != typeof(RegionPolygon))
                .Select(x => x.Position).ToArray();
            if (newRegionPoints.Length < 3)
            {
                ClearLayer(MapLayer.NewRegion);
                return null;
            }

            var region = new ApartmentsRegion(regionName, newRegionPoints);
            AddRegionMarkers(new[] {region});
            ClearLayer(MapLayer.NewRegion);

            return region;
        }

        private int GetZIndex(MapLayer layer)
        {
            switch (layer)
            {
                case MapLayer.Apartments: return 10;
                case MapLayer.Regions: return 1;
                case MapLayer.NewRegion: return 2;
                default: return 0;
            }
        }

        private void AddMarker(MapLayer layer, GMapMarker marker)
        {
            marker.ZIndex = GetZIndex(layer);
            _markers[layer].Add(marker);
            Markers.Add(marker);
        }

        private void AddMarker(MapLayer layer, ICollection<GMapMarker> markers)
        {
            var zIndex = GetZIndex(layer);
            _markers[layer].AddRange(markers);
            foreach (var marker in markers)
            {
                marker.ZIndex = zIndex;
                Markers.Add(marker);
            }
        }

        public void AddRegionMarkers(IEnumerable<ApartmentsRegion> regions)
        {
            var markers = regions.Select(x => new RegionPolygon(x.Locations, new Polygon
            {
                Stroke = Brushes.CornflowerBlue,
                Fill = Brushes.LightSkyBlue,
                Opacity = 0.6,
                StrokeThickness = 2
            }));
            AddMarker(MapLayer.Regions, markers.ToArray());
        }

        public void AddApartmentMarkers(IEnumerable<ApartmentData> apartments)
        {
            var markers = apartments.Select(x => new ApartmentMarker(x, data => MessageBox.Show(data.ToString(), data.Id)));
            AddMarker(MapLayer.Apartments, markers.ToArray());
        }

        public void AddNewRegionMarker(PointLatLng point)
        {
            const string regionTag = "regionTag";
            var temp = _markers[MapLayer.NewRegion].Where(x => x.GetType() != typeof(RegionPolygon)).ToList();
            ClearLayer(MapLayer.NewRegion);

            temp.Add(new GMapMarker(point)
            {
                Shape = new Rectangle
                {
                    Width = 12,
                    Height = 12,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2
                },
                Offset = new Point(-6, -6)
            });

            temp.Add(new RegionPolygon(temp.Select(x => x.Position).ToArray(), new Polygon
            {
                Stroke = Brushes.DarkRed,
                Fill = Brushes.IndianRed,
                Opacity = 0.6,
                StrokeThickness = 2,
                Tag = regionTag
            }));

            AddMarker(MapLayer.NewRegion, temp);
        }

        public void ClearLayer(MapLayer layer)
        {
            foreach (var marker in _markers[layer])
                Markers.Remove(marker);

            _markers[layer].Clear();
        }

        private readonly MultiValueDictionary<MapLayer, GMapMarker> _markers;
    }

    public enum MapLayer
    {
        Undefined,
        NewRegion,
        Regions,
        Apartments
    }

    public class MultiValueDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            if (!TryGetValue(key, out var values))
            {
                values = new List<TValue>();
                Add(key, values);
            }

            values.Add(value);
        }
    }
}