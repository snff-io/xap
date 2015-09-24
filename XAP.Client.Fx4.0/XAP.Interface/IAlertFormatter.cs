using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public interface IAlertFormatter
    {
        void Format(AlertInstance alert, string serviceInventoryUriBase);
    }
}
