using System.Threading;
using Caliburn.Core.Threading;

namespace Tests.Caliburn.Fakes
{
    public class FakeThreadPool : IThreadPool
    {
        public bool QueueUserWorkItem(WaitCallback callback, object state)
        {
            callback(state);
            return true;
        }
    }
}