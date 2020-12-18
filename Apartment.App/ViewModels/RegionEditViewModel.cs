using System;
using System.Windows.Media;
using Apartment.App.Common;
using Region = Apartment.Common.Models.Region;

namespace Apartment.App.ViewModels
{
    public class RegionEditViewModel : ViewModelBase
    {
        private readonly Region _region;

        public int Id => _region.Id;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        private string _name;

        public SolidColorBrush Color
        {
            get => _color;
            set => _color = value;
        }

        private SolidColorBrush _color;

        public RegionEditViewModel(Region region)
        {
            _region = region ?? throw new ArgumentNullException(nameof(region));
            _region = region;
            _name = _region.Name;
            _color = new SolidColorBrush(region.ColorHex.ParseColor());
        }

        public Region GetNewRegion()
        {
            return new Region(_region.Id, _name, _color.Color.ToHexString(), _region.Locations);
        }
    }
}