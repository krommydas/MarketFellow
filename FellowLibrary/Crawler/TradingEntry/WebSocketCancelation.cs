using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace FellowLibrary.Crawler
{
    public class WebSocketCancelation : IDisposable
    {
        public WebSocketCancelation(ClientWebSocket provider)
        {
            _Client = provider;
        }

        ClientWebSocket _Client;

        internal Task Disconnect()
        {
            return _Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "termination", new System.Threading.CancellationToken());
        }

        public void Dispose()
        {
            Disconnect().ContinueWith(x => _Client.Dispose());
        }
    }
}
