using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Apartment.App.Models;
using Apartment.App.ViewModels;

namespace Apartment.App.Views
{
    /// <summary>
    /// Interaction logic for ApartmentsGroupWindow.xaml
    /// </summary>
    public partial class ApartmentsGroupWindow : Window
    {
        public ApartmentsGroupWindow(ApartmentsGroup group)
        {
            InitializeComponent();
            DataContext = new ApartmentsGroupViewModel(group);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo {UseShellExecute = true, FileName = e.Uri.AbsoluteUri};
            Process.Start(psi);
            e.Handled = true;
        }
    }
}