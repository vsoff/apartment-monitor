﻿using System.Collections.Generic;
using Apartment.Common.Models;
using Apartment.Data.Entities;
using GMap.NET;
using Newtonsoft.Json;

namespace Apartment.Core.Mappers
{
    public static class ApartmentMapper
    {
        public static ApartmentEntity ToEntity(this ApartmentInfo data)
        {
            if (data == null) return null;
            return new ApartmentEntity
            {
                ExternalId = data.ExternalId,
                Lat = data.Location.Lat,
                Lng = data.Location.Lng,
                Price = data.Price,
                Url = data.Url,
                ApartmentNumber = data.ApartmentNumber,
                Floor = data.Floor,
                FloorsCount = data.FloorsCount,
                RoomsCount = data.RoomsCount,
                Area = data.Area,
                Title = data.Title,
                Address = data.Address,
                PublishingDate = data.PublishingDateUtc,
                DisappearedDate = data.DisappearedDate,
                ImageUrlsJson = JsonConvert.SerializeObject(data.ImageUrls)
            };
        }

        public static ApartmentInfo ToCore(this ApartmentEntity data)
        {
            if (data == null) return null;
            return new ApartmentInfo
            {
                ExternalId = data.ExternalId,
                Location = new PointLatLng(data.Lat, data.Lng),
                Price = data.Price,
                Url = data.Url,
                ApartmentNumber = data.ApartmentNumber,
                Floor = data.Floor,
                FloorsCount = data.FloorsCount,
                RoomsCount = data.RoomsCount,
                Area = data.Area,
                Title = data.Title,
                Address = data.Address,
                PublishingDateUtc = data.PublishingDate,
                DisappearedDate = data.DisappearedDate,
                CreatedAtUtc = data.CreatedAtUtc,
                ImageUrls = JsonConvert.DeserializeObject<ICollection<string>>(data.ImageUrlsJson)
            };
        }
    }
}