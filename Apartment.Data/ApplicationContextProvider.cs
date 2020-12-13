using System;
using Apartment.Options;

namespace Apartment.Data
{
    public class ApplicationContextProvider : IDatabaseContextProvider
    {
        private readonly DatabaseOptions _options;

        public ApplicationContextProvider(DatabaseOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ApplicationContext Create()
        {
            return new ApplicationContext(_options.DataBaseConnectionString, _options.DataProviderType);
        }
    }
}