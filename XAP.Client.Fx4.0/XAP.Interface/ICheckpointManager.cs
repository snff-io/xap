using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public interface ICheckpointManager
    {
        bool TryLockReporter(IReporter reporter);

        string GetCheckpoint(IReporter reporter);

        void SetCheckpoint(IReporter reporter, string checkpoint);

        bool TryReleaseReporter(IReporter reporter);
    }
}
