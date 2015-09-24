using System;
using System.Collections.Generic;

namespace XAP.Common
{
    public class Factory
    {
        readonly Dictionary<Type, Func<object>> _locator;

        public Factory()
        {
            _locator = new Dictionary<Type, Func<object>>();
        }

        public void Register<T>(Func<object> createFunc)
        {
            _locator[typeof(T)] = createFunc;
        }

        public T CreateInstance<T>()
        {
            Tracing.XapTrace.TraceInformation("creating instance of {0}", typeof(T).ToString());
            Func<object> createFunc;
            
            if (_locator.TryGetValue(typeof(T), out createFunc))
            {
                T instance = (T)createFunc();
                return instance;
            }

            return default(T);
        }
    }
}
