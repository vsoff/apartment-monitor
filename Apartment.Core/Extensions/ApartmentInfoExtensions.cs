using System;
using System.Collections.Generic;
using System.Text;
using Apartment.Common.Models;

namespace Apartment.Core.Extensions
{
    public static class ApartmentInfoExtensions
    {
        private static readonly TimeSpan NewestApartmentsTimeout = TimeSpan.FromDays(2);
        private static readonly TimeSpan OldApartmentsTimeout = TimeSpan.FromDays(7);

        public static bool IsNewest(this ApartmentInfo info) => DateTime.UtcNow - info.CreatedAtUtc < NewestApartmentsTimeout;
        public static bool IsOld(this ApartmentInfo info) => DateTime.UtcNow - info.PublishingDateUtc > OldApartmentsTimeout;
    }
}