using System;
using System.Collections.ObjectModel;
using System.Linq;
using Apartment.App.Models;
using GMap.NET;

namespace Apartment.App.ViewModels
{
    public class ApartmentsGroupViewModel : ViewModelBase
    {
        public string Title { get; }
        public ObservableCollection<ApartmentDataViewModel> Apartments { get; }

        public ApartmentsGroupViewModel(ApartmentsGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            Title = group.Title;
            Apartments = new ObservableCollection<ApartmentDataViewModel>(
                group.Apartments
                    .OrderBy(x => x.Price)
                    .Select(x => new ApartmentDataViewModel(x)));
        }
    }
}