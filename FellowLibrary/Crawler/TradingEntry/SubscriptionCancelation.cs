using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FellowLibrary.Crawler
{
    class SubscriptionCancelation : IDisposable
    {
        public SubscriptionCancelation()
        {
            _CancelationSource = new CancellationTokenSource();
        }

        CancellationTokenSource _CancelationSource;

        public CancellationToken Token { get { return _CancelationSource.Token; } }

        public void Dispose()
        {
            _CancelationSource.Cancel();
        }
    }
}
