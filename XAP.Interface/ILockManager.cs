using System;

namespace XAP.Interface
{
    public interface ILockManager
    {
        bool TryGetLock(string resourceId, out Guid lockToken, out string lockData);

        bool TryReleaseLock(string resourceId, Guid lockToken, string lockData);
    }
}
