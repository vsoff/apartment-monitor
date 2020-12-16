using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;
using Brush = System.Drawing.Brush;
using Region = Apartment.Common.Models.Region;

namespace Apartment.App.ViewModels
{
    public class RegionEditViewModel : ViewModelBase
    {
        private readonly Region _region;

        public int Id { get; }

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
            _color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_region.Color.R, _region.Color.G, _region.Color.B));
        }

        public Region GetNewRegion()
        {
            var color = _color.Color;
            return new Region(_region.Id, _name, System.Drawing.Color.FromArgb(color.R, color.G, color.B), _region.Locations);
        }
    }
}