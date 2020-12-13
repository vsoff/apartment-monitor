using System;
using System.Collections.Generic;

namespace Apartment.Core
{
    public class DataWithHistory<T>
    {
        public T Data { get; set; }
        public IReadOnlyDictionary<DateTime, T> History { get; set; }
    }
}