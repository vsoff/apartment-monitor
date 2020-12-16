using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Apartment.App.ViewModels;

namespace Apartment.App.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();

            DataContext = viewModel;
            _viewModel = viewModel;
        }

        protected override void OnActivated(EventArgs e)
        {
            if (_viewModel.InitializeCommand.CanExecute(null))
                _viewModel.InitializeCommand.Execute(null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ColorPickerColor.Background = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                ColorPickerColor.Content = colorDialog.Color.Name.ToLower();
            }
        }
    }
}