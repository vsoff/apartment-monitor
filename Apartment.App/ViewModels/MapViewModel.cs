using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Apartment.App.Components;
using Apartment.DataProvider.Avito.Common;
using Apartment.Options;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace Apartment.App.ViewModels
{
    // TODO: Сейчас этот код нарушает MVVM, надо что-то с этим придумать.
    public class MapViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// Объект карты.
        /// </summary>
        /// <remarks>Только для биндинга во вьюху, использоваться может исключительно в <see cref="MapViewModel"/>.</remarks>
        public GMapControl Map { get; }

        public string CurrentPositionText { get; private set; }

        public MapViewModel(ApplicationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            Map = CreateMap(options);
        }

        /// <summary>
        /// Создаёт новый экземпляр карты.
        /// </summary>
        private GMapControl CreateMap(ApplicationOptions options)
        {
            var startPosition = new PointLatLng(options.StartPosition.Latitude, options.StartPosition.Longitude);

            var mapControl = new GMapControl();
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            // choose your provider here
            mapControl.MapProvider = OpenStreetMapProvider.Instance;
            mapControl.MinZoom = 2;
            mapControl.MaxZoom = 17;
            mapControl.Position = startPosition;
            // whole world zoom
            mapControl.Zoom = 14;
            // lets the map use the mousewheel to zoom
            mapControl.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            // lets the user drag the map
            mapControl.CanDragMap = true;
            // lets the user drag the map with the left mouse button
            mapControl.DragButton = MouseButton.Left;
            mapControl.OnPositionChanged += point =>
            {
                startPosition = point;
                CurrentPositionText = $"Lat: {startPosition.Lat}, Lng: {startPosition.Lng}, Zoom: {mapControl.Zoom}";
            };

            const string newPolyPointsTag = "NewPolyTag";
            const string newPolyTag = "NewPolyPointsTag";
            mapControl.MouseDoubleClick += (o, args) =>
            {
                var polygons = mapControl.Markers.Where(x => (string) x.Tag == newPolyTag).ToArray();
                foreach (var maker in polygons)
                    mapControl.Markers.Remove(maker);

                var polyPositions = mapControl.Markers.Select(x => x.Position).ToArray();
                mapControl.Markers.Clear();

                var poly = new RegionPolygon(polyPositions, new Polygon
                {
                    Stroke = Brushes.CornflowerBlue,
                    Fill = Brushes.LightSkyBlue,
                    Opacity = 0.6,
                    StrokeThickness = 2
                })
                {
                    Tag = newPolyTag,
                };

                mapControl.Markers.Add(poly);
            };
            mapControl.MouseRightButtonDown += (o, args) =>
            {
                var marker = new GMapMarker(startPosition)
                {
                    Shape = new Ellipse
                    {
                        Width = 12,
                        Height = 12,
                        Stroke = Brushes.Red,
                        StrokeThickness = 3
                    },
                    Tag = newPolyPointsTag
                };

                mapControl.Markers.Add(marker);
            };

            return mapControl;
        }

        /// <summary>
        /// Центрирует карту по указанным координатам.
        /// </summary>
        public void SetCurrentPosition(PointLatLng position) => Map.Position = position;

        public void SetDrawableApartments(IEnumerable<ApartmentData> apartments)
        {
            Map.Markers.Clear();
            foreach (var apartment in apartments)
            {
                var component = new ApartmentMarker(apartment.PriceText);
                Map.Markers.Add(new GMapMarker(apartment.Location)
                {
                    Shape = component,
                    //Tag = "Apartment"
                });
                component.MouseUp += (sender, args) => { MessageBox.Show(apartment.ToString(), apartment.Id); };
            }
        }

        public void Dispose() => Map?.Dispose();
    }
}