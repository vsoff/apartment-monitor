using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Apartment.DataProvider;
using Apartment.DataProvider.Models;
using GMap.NET.WindowsPresentation;

namespace Apartment.App.Components
{
    public sealed class ApartmentMarker : GMapMarker
    {
        public ApartmentMarker(ApartmentData apartmentData, Action<ApartmentData> clickHandler) : base(apartmentData.Location)
        {
            var component = new ApartmentMarkerShape(apartmentData.PriceText);
            component.MouseUp += (sender, args) => clickHandler(apartmentData);
            Shape = component;
        }

        private sealed class ApartmentMarkerShape : Border
        {
            private readonly Brush _borderBrush = new SolidColorBrush(Colors.CornflowerBlue) {Opacity = 0.6};
            private readonly Brush _backgroundBrush = new SolidColorBrush(Colors.White) {Opacity = 0.3};
            private const double MarkSize = 12;

            public ApartmentMarkerShape(string text)
            {
                Child = new StackPanel
                {
                    Children =
                    {
                        new Ellipse
                        {
                            Width = MarkSize,
                            Height = MarkSize,
                            Stroke = Brushes.CornflowerBlue,
                            StrokeThickness = 3,
                            StrokeDashOffset = -6
                        },
                        new Label {Content = text}
                    },
                };

                BorderThickness = new Thickness(1);
                Background = _backgroundBrush;
                BorderBrush = _borderBrush;
            }

            protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
            {
                Margin = new Thickness(-sizeInfo.NewSize.Width / 2, -MarkSize / 2, 0, 0);
            }
        }
    }
}