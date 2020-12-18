using System.Collections.Generic;
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
                ColorHex = data.ColorHex,
                PointsJson = JsonConvert.SerializeObject(data.Locations)
            };
        }

        public static Region ToCore(this RegionEntity data)
        {
            if (data == null) return null;
            var locations = JsonConvert.DeserializeObject<ICollection<PointLatLng>>(data.PointsJson);
            return new Region(data.Id, data.Name, data.ColorHex, locations);
        }
    }
}