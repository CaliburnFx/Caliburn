using System.Threading;
using Caliburn.Core.Threading;

namespace Tests.Caliburn.Fakes
{
    public class FakeThreadPool : IThreadPool
    {
        public bool QueueUserWorkItem(WaitCallback callback)
        {
            callback(null);
            return true;
        }
    }
}