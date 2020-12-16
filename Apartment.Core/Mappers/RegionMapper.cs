using System;
using System.Collections.Generic;
using System.Drawing;
using Apartment.Common.Models;
using Apartment.Data.Entities;
using GMap.NET;
using Newtonsoft.Json;

namespace Apartment.Core.Mappers
{
    public static class RegionMapper
    {
        public static RegionEntity ToEntity(this Region data)
        {
            if (data == null) return null;
            return new RegionEntity
            {
                Id = data.Id,
                Name = data.Name,
                ColorR = data.Color.R,
                ColorG = data.Color.G,
                ColorB = data.Color.B,
                PointsJson = JsonConvert.SerializeObject(data.Locations)
            };
        }

        public static Region ToCore(this RegionEntity data)
        {
            if (data == null) return null;
            var color = Color.FromArgb(data.ColorR, data.ColorG, data.ColorB);
            var locations = JsonConvert.DeserializeObject<ICollection<PointLatLng>>(data.PointsJson);
            return new Region(data.Id, data.Name, color, locations);
        }
    }
}