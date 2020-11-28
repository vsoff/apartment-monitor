using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Apartment.App.Views
{
    public sealed class ApartmentMarker : Border
    {
        public ApartmentMarker(string text)
        {
            Child = new StackPanel
            {
                Children =
                {
                    new Ellipse
                    {
                        Width = 12,
                        Height = 12,
                        Stroke = Brushes.CornflowerBlue,
                        StrokeThickness = 3,
                        StrokeDashOffset = -6
                    },
                    new Label {Content = text}
                },
            };

            BorderThickness = new Thickness(1);
            var backBrush = new SolidColorBrush(Colors.White) {Opacity = 0.3};
            Background = backBrush;
            BorderBrush = Brushes.Blue;
            MouseUp += (sender, args) => { MessageBox.Show(text); };
        }
    }
}