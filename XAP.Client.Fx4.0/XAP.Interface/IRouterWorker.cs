﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public interface IRouterWorker
    {
        void Initialize(IContext context);
        void Run();
        void Stop();
    }
}