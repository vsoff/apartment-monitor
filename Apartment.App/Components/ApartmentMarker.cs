using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Apartment.App.Models;
using GMap.NET.WindowsPresentation;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace Apartment.App.Components
{
    public sealed class ApartmentMarker : GMapMarker
    {
        public ApartmentMarker(ApartmentsGroup apartmentGroup, Action<ApartmentsGroup> clickHandler) : base(apartmentGroup.Location)
        {
            const double defaultOpacity = 0.7;
            var component = new ApartmentMarkerShape(apartmentGroup.Title, apartmentGroup.HasNewest, apartmentGroup.AllIsOld) {Opacity = defaultOpacity};
            component.MouseUp += (sender, args) => clickHandler(apartmentGroup);
            int? cachedZIndex = null;
            component.MouseEnter += (sender, args) =>
            {
                component.Opacity = 1;
                cachedZIndex ??= ZIndex;
                ZIndex = cachedZIndex.Value + 1;
            };
            component.MouseLeave += (sender, args) =>
            {
                component.Opacity = defaultOpacity;
                if (cachedZIndex != null)
                    ZIndex = cachedZIndex.Value;
            };
            Shape = component;
            //var comp = new TestShape(apartmentGroup);
            //comp.MouseLeftButtonUp += (sender, args) => clickHandler(apartmentGroup);
            //Shape = comp;
        }

        private sealed class TestShape : UIElement
        {
            private readonly ApartmentsGroup _apartmentGroup;

            public TestShape(ApartmentsGroup apartmentGroup)
            {
                _apartmentGroup = apartmentGroup ?? throw new ArgumentNullException(nameof(apartmentGroup));
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawLine(new System.Windows.Media.Pen(Brushes.Blue, 2), new Point(0, 0), new Point(10, 10));
                drawingContext.DrawRectangle(Brushes.Blue, new System.Windows.Media.Pen(Brushes.Blue, 2), new Rect(new Point(10, 10), new Point(5, 5)));
                //drawingContext.DrawText(new FormattedText(_apartmentGroup.Title,CultureInfo.CurrentCulture, FlowDirection.LeftToRight,new Typeface(), ), new Point(0, 0));
            }
        }

        private sealed class ApartmentMarkerShape : Border
        {
            private const double MarkSize = 12;

            private Color GetColor(bool isNewest, bool isOld)
            {
                if (isNewest)
                    return Colors.Crimson;

                if (isOld)
                    return Colors.DarkGray;

                return Colors.CornflowerBlue;
            }

            public ApartmentMarkerShape(string text, bool isNewest, bool isOld)
            {
                Color color = GetColor(isNewest, isOld);
                Brush borderBrush = new SolidColorBrush(color) {Opacity = 1};
                Brush backgroundBrush = new SolidColorBrush(Colors.White) {Opacity = 0.8};
                Child = new StackPanel
                {
                    Children =
                    {
                        new Ellipse
                        {
                            Width = MarkSize,
                            Height = MarkSize,
                            Stroke = borderBrush,
                            StrokeThickness = 3,
                            StrokeDashOffset = -6
                        },
                        new Label {Content = text}
                    },
                };

                BorderThickness = new Thickness(1);
                Background = backgroundBrush;
                BorderBrush = borderBrush;
            }

            protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
            {
                Margin = new Thickness(-sizeInfo.NewSize.Width / 2, -MarkSize / 2, 0, 0);
            }
        }
    }
}