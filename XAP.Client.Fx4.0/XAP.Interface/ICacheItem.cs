using System;

namespace XAP.Interface
{
    public interface ICacheItem
    {
        DateTime? ValidUntil { get; }
    }
}
