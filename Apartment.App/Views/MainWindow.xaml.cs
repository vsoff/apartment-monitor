using System;
using System.Windows;
using Apartment.App.ViewModels;

namespace Apartment.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();

            DataContext = viewModel;
        }
    }
}