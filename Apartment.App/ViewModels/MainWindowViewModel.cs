using System;
using Apartment.DataProvider.Avito.Common;

namespace Apartment.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IApartmentsProvider _apartmentsProvider;

        public MainWindowViewModel(IApartmentsProvider apartmentsProvider)
        {
            _apartmentsProvider = apartmentsProvider ?? throw new ArgumentNullException(nameof(apartmentsProvider));
        }

        public string Test => "test string";
    }
}