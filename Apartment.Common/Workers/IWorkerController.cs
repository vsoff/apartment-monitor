using System;

namespace Apartment.Common.Workers
{
    public interface IWorkerController
    {
        IWorker CreateWorker(Action action, TimeSpan interval, bool startImmediately = true);
        IWorker StartWorker(Action action, TimeSpan interval, bool startImmediately = true);
    }
}