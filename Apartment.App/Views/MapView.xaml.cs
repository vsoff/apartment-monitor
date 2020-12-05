using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Apartment.App.Components;
using Apartment.App.Models;
using Apartment.App.ViewModels;
using Apartment.DataProvider.Models;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Point = System.Windows.Point;

namespace Apartment.App.Views
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public PointLatLng LastPosition;
        private readonly Dictionary<object, GMapMarker> _markerByObjectMap;
        private MapViewModel _viewModel;

        public MapView()
        {
            InitializeComponent();
            DataContextChanged += MapView_DataContextChanged;

            _markerByObjectMap = new Dictionary<object, GMapMarker>();

            #region Map settings

            MapControl.MapProvider = OpenStreetMapProvider.Instance;
            MapControl.MinZoom = 2;
            MapControl.MaxZoom = 17;
            MapControl.Zoom = 14;
            MapControl.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            MapControl.CanDragMap = true;
            MapControl.DragButton = MouseButton.Left;

            #endregion

            #region Events

            MapControl.OnPositionChanged += point => { LastPosition = point; };
            Point? leftPoint = null;
            MouseLeftButtonUp += (sender, args) =>
            {
                if (!leftPoint.HasValue)
                    return;

                var newPoint = args.GetPosition(this);
                if (Math.Abs(newPoint.X - leftPoint.Value.X) < double.Epsilon && Math.Abs(newPoint.Y - leftPoint.Value.Y) < double.Epsilon)
                {
                    if (_viewModel.AddNewRegionPointCommand.CanExecute(null))
                        _viewModel.AddNewRegionPointCommand?.Execute(args);
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
                    if (_viewModel.FlushNewRegionPointCommand.CanExecute(null))
                        _viewModel.FlushNewRegionPointCommand?.Execute(args);
                }
            };
            MouseRightButtonDown += (sender, args) => { rightPoint = args.GetPosition(this); };

            #endregion
        }

        private void MapView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var viewModel = e.NewValue as MapViewModel;
            if (viewModel == null) throw new ArgumentNullException(nameof(_viewModel));
            _viewModel = viewModel;
            _viewModel.ItemsAdded += (o, data) => AddObject(data, MapLayer.Apartments);
            _viewModel.ItemsRemoved += (o, data) => RemoveObjectIfExists(data);
            _viewModel.CurrentPositionChanged += (o, position) => MapControl.Position = position;
            _viewModel.SetCurrentPosition(_viewModel.StartPosition);
        }

        private int GetZIndex(MapLayer layer)
        {
            switch (layer)
            {
                case MapLayer.Apartments: return 10;
                case MapLayer.Regions: return 1;
                case MapLayer.NewRegion: return 2;
                case MapLayer.NewRegionPoints: return 3;
                default: return 0;
            }
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private GMapMarker CreateMarker(object obj)
        {
            Type type = obj.GetType();

            if (type == typeof(NewRegionData))
            {
                var data = obj as NewRegionData;

                return new RegionPolygon(data.Locations, null, Colors.IndianRed);
            }

            if (type == typeof(NewRegionPoint))
            {
                var data = obj as NewRegionPoint;
                const double size = 8;
                return new GMapMarker(data.Location)
                {
                    Shape = new Rectangle
                    {
                        Stroke = Brushes.Red,
                        Fill = Brushes.White,
                        Width = size,
                        Height = size,
                        StrokeThickness = 1
                    },
                    Offset = new Point(-size / 2, -size / 2)
                };
            }

            if (type == typeof(ApartmentsGroup))
            {
                var data = obj as ApartmentsGroup;
                return new ApartmentMarker(data, x =>
                {
                    if (_viewModel.OpenMarkerInfoCommand.CanExecute(null))
                        _viewModel.OpenMarkerInfoCommand.Execute(x);
                });
            }

            if (type == typeof(ApartmentsRegion))
            {
                var data = obj as ApartmentsRegion;
                return new RegionPolygon(data.Locations, data.Name, Colors.LightSkyBlue);
            }

            throw new ArgumentException($"Неизвестный тип {type.FullName}", nameof(obj));
        }

        /// <summary>
        /// Добавляет объект на карту.
        /// </summary>
        public void AddObject(object obj, MapLayer layer)
        {
            if (obj is IEnumerable objects)
            {
                foreach (var objectsItem in objects)
                    AddObject(objectsItem, layer);
                return;
            }

            if (_markerByObjectMap.TryGetValue(obj, out _))
                throw new ArgumentException("Этот объект уже добавлен на карту");

            var marker = CreateMarker(obj);
            var zIndex = GetZIndex(layer);
            marker.ZIndex = zIndex;

            _markerByObjectMap.Add(obj, marker);
            MapControl.Markers.Add(marker);
        }

        /// <summary>
        /// Удаляет объект с карты.
        /// </summary>
        public void RemoveObjectIfExists(object obj)
        {
            if (obj is IEnumerable objects)
            {
                foreach (var objectsItem in objects)
                    RemoveObjectIfExists(objectsItem);
                return;
            }

            if (!_markerByObjectMap.TryGetValue(obj, out var marker))
                return;

            _markerByObjectMap.Remove(marker);
            MapControl.Markers.Remove(marker);
        }
    }

    // TODO: Удалить слои.
    public enum MapLayer
    {
        Undefined,
        NewRegion,
        NewRegionPoints,
        Regions,
        Apartments
    }
}