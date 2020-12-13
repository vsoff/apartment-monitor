using System;

namespace Apartment.Common.Workers
{
    public interface IWorker : IDisposable
    {
        void Start();
        void Stop();
    }
}