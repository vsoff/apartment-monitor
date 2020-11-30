using System;
using System.Windows.Controls;
using Apartment.App.ViewModels;

namespace Apartment.App.Views
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
            DataContextChanged += MapView_DataContextChanged;
        }

        private void MapView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // Прокинем карту на вьюху, чтобы слой view использовал карту из слоя viewModel.
            var viewModel = e.NewValue as MapViewModel;
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            Content = viewModel.Map ?? throw new ArgumentNullException(nameof(viewModel.Map), "Контрол карты не может быть null");
        }
    }
}