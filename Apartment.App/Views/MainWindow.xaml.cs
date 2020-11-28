using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Apartment.App.Components;
using Apartment.App.ViewModels;
using Apartment.DataProvider.Avito.Avito;
using Apartment.DataProvider.Avito.Common;
using Apartment.Options;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace Apartment.App.Views
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationOptions _options;

        public MainWindow(MainWindowViewModel viewModel, ApplicationOptions options)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            InitializeComponent();
            DataContext = viewModel;
        }

        protected override void OnClosed(EventArgs e)
        {
            Carte.Dispose();
            base.OnClosed(e);
        }

        private void Carte_Loaded(object sender, RoutedEventArgs e)
        {
            var startPosition = new PointLatLng(_options.StartPosition.Latitude, _options.StartPosition.Longitude);

            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            // choose your provider here
            Carte.MapProvider = OpenStreetMapProvider.Instance;
            Carte.MinZoom = 2;
            Carte.MaxZoom = 17;
            Carte.Position = startPosition;
            // whole world zoom
            Carte.Zoom = 14;
            // lets the map use the mousewheel to zoom
            Carte.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            // lets the user drag the map
            Carte.CanDragMap = true;
            // lets the user drag the map with the left mouse button
            Carte.DragButton = MouseButton.Left;
            Carte.OnPositionChanged += point =>
            {
                startPosition = point;
                PositionTextBox.Text = startPosition.ToString() + Carte.Zoom;
            };

            const string newPolyPointsTag = "NewPolyTag";
            const string newPolyTag = "NewPolyPointsTag";
            Carte.MouseDoubleClick += (o, args) =>
            {
                var polygons = Carte.Markers.Where(x => (string) x.Tag == newPolyTag).ToArray();
                foreach (var maker in polygons)
                    Carte.Markers.Remove(maker);

                var polyPositions = Carte.Markers.Select(x => x.Position).ToArray();
                Carte.Markers.Clear();

                var poly = new RegionPolygon(polyPositions, new Polygon
                {
                    Stroke = Brushes.DarkRed,
                    Fill = Brushes.Aqua,
                    Opacity = 0.7
                })
                {
                    Tag = newPolyTag
                };

                Carte.Markers.Add(poly);
            };
            Carte.MouseRightButtonDown += (o, args) =>
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

                Carte.Markers.Add(marker);
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var apartments = new ApartmentData[0];
            ApartmentsListBox.Items.Clear();
            foreach (var apartment in apartments)
            {
                ApartmentsListBox.Items.Add(apartment);
                var component = new ApartmentMarker( /*apartment.PriceText*/"sometext");
                Carte.Markers.Add(new GMapMarker(apartment.Location)
                {
                    Shape = component,
                    Tag = "Apartment",
                    Offset = new Point(-component.RenderSize.Width, -6)
                });
            }
        }

        private void ApartmentsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var item = e.AddedItems[0] as ApartmentData;
            if (item == null)
                return;

            Carte.Position = item.Location;
        }
    }
}