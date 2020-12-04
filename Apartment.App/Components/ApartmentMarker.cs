using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Apartment.App.Models;
using GMap.NET.WindowsPresentation;

namespace Apartment.App.Components
{
    public sealed class ApartmentMarker : GMapMarker
    {
        public ApartmentMarker(ApartmentsGroup apartmentGroup, Action<ApartmentsGroup> clickHandler) : base(apartmentGroup.Location)
        {
            const double defaultOpacity = 0.7;
            var component = new ApartmentMarkerShape(apartmentGroup.Title) {Opacity = defaultOpacity};
            component.MouseUp += (sender, args) => clickHandler(apartmentGroup);
            var initZIndex = ZIndex;
            component.MouseEnter += (sender, args) =>
            {
                component.Opacity = 1;
                ZIndex = initZIndex + 1;
            };
            component.MouseLeave += (sender, args) =>
            {
                component.Opacity = defaultOpacity;
                ZIndex = initZIndex;
            };
            Shape = component;
        }

        private sealed class ApartmentMarkerShape : Border
        {
            private readonly Brush _borderBrush = new SolidColorBrush(Colors.CornflowerBlue) {Opacity = 1};
            private readonly Brush _backgroundBrush = new SolidColorBrush(Colors.White) {Opacity = 0.8};
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