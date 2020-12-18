using System;
using System.Collections.ObjectModel;
using System.Linq;
using Apartment.App.Models;
using Apartment.Common.Models;
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

    public class ApartmentDataViewModel : ViewModelBase
    {
        public string Id => _apartment.ExternalId;
        public int? Price => _apartment.Price;
        public string Url => _apartment.Url;
        public DateTime PublishingDate => _apartment.PublishingDate;
        public string Title => _apartment.Title;
        public string Address => _apartment.Address;
        public ObservableCollection<string> ImageUrls { get; }

        private readonly ApartmentInfo _apartment;

        public ApartmentDataViewModel(ApartmentInfo apartment)
        {
            _apartment = apartment ?? throw new ArgumentNullException(nameof(apartment));
            ImageUrls = new ObservableCollection<string>(_apartment.ImageUrls);
        }
    }
}