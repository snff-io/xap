using System;
using System.Collections.Generic;

namespace XAP.Interface
{
    public interface IAlertQueue
    {
        void Enqueue(AlertInstance alert);

        void Enqueue(IEnumerable<AlertInstance> alerts);

        AlertInstance StartNext();

        void End(Guid xapId);

        AlertInstance Peek();

        int Count();
    }
}
