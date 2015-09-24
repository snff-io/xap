using System;

namespace XAP.Engine
{
    public class EngineWorkerException : Exception
    {
        public EngineWorkerException(string message, Exception inner)
            : base(message, inner)
        {

        }

    }
}
