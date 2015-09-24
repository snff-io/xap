using System;
using System.Collections.Generic;
using System.Linq;
using XAP.Interface;

namespace XAP.BuiltIn
{
    public class MemoryLockManager : ILockManager
    {
        readonly List<LockData> _locks;

        class LockData
        {
            public string ResourceId { get; set; }

            public Guid Token { get; set; }

            public string Data { get; set; }

            public DateTime Modified { get; set; }
        }

        public MemoryLockManager()
        {
            _locks = new List<LockData>();
        }

        public bool TryGetLock(string resourceId, out Guid lockToken, out string lockData)
        {
            ClearDeadLocks();

            var newLockToken = Guid.NewGuid();
            lock (_locks)
            {
                LockData lockRec = _locks.FirstOrDefault(x => x.ResourceId.Equals(resourceId));

                if (lockRec == null) //No prior lock
                {
                    lockRec = new LockData
                    {
                        ResourceId = resourceId,
                        Token = newLockToken,
                        Data = String.Empty,
                        Modified = DateTime.UtcNow
                    };

                    _locks.Add(lockRec);

                    lockToken = lockRec.Token;
                    lockData = lockRec.Data;

                    return true;
                }
                if (lockRec.Token.Equals(Guid.Empty)) //No current lock
                {
                    lockRec.Token = newLockToken;
                    lockRec.Modified = DateTime.UtcNow;

                    lockToken = lockRec.Token;
                    lockData = lockRec.Data;

                    return true;
                }
                lockToken = Guid.Empty;
                lockData = String.Empty;

                return false;
            }
        }

        public bool TryReleaseLock(string resourceId, Guid lockToken, string lockData)
        {
            if (lockToken == Guid.Empty)
            {
                throw new ArgumentException("lockToken can not be Guid.Empty...");
            }

            ClearDeadLocks();

            lock (_locks)
            {
                var lockRec = _locks.FirstOrDefault(x => x.ResourceId.Equals(resourceId));

                if (lockRec == null) //weird
                {
                    _locks.Add(new LockData
                    {
                         Data = lockData,
                         Token = Guid.Empty,
                         ResourceId = resourceId,
                         Modified = DateTime.UtcNow
                    });
                    return true;
                }
                if (lockRec.Token.Equals(lockToken)) //normal
                {
                    lockRec.Token = Guid.Empty;
                    lockRec.Data = lockData;
                    lockRec.Modified = DateTime.UtcNow;
                    return true;
                }
                return false;
            }
        }

        private void ClearDeadLocks()
        {
            lock (_locks)
            {
                var deadTime = TimeSpan.FromHours(1);

                var deadLocks = _locks
                    .Where(x => x.Modified < DateTime.UtcNow.Subtract(deadTime)
                    && !x.Token.Equals(Guid.Empty));

                foreach (var l in deadLocks)
                {
                    l.Token = Guid.Empty;
                }
            }
        }
    }
}
