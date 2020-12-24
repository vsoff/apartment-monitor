using System;
using System.Collections.ObjectModel;
using Apartment.Common.Models;
using Apartment.Core.Extensions;

namespace Apartment.App.ViewModels
{
    public class ApartmentDataViewModel : ViewModelBase
    {
        public string Id => _apartment.ExternalId;
        public int? Price => _apartment.Price;
        public string Url => _apartment.Url;
        public DateTime CreatedAtUtc => _apartment.CreatedAtUtc;
        public DateTime PublishingDate => _apartment.PublishingDateUtc;
        public string Title => _apartment.Title;
        public string Address => _apartment.Address;
        public bool IsNewest { get; }
        public ObservableCollection<string> ImageUrls { get; }

        private readonly ApartmentInfo _apartment;

        public ApartmentDataViewModel(ApartmentInfo apartment)
        {
            _apartment = apartment ?? throw new ArgumentNullException(nameof(apartment));
            IsNewest = apartment.IsNewest();
            ImageUrls = new ObservableCollection<string>(_apartment.ImageUrls);
        }
    }
}