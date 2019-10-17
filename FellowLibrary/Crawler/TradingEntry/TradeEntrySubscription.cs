using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FellowLibrary.Crawler
{
    public class TradeEntrySubscription : IObservable<Models.TradeEntry>
    {
        private IObserver<Models.TradeEntry> _Observer;

        private TradingEntryCrawler _Crawler;

        public static async Task<TradeEntrySubscription> Start(MarketProviderConfiguration forProvider, String forTradingPair)
        {
            TradeEntrySubscription instance = new TradeEntrySubscription();

            instance._Crawler = new TradingEntryCrawler(forProvider, forTradingPair);

            CancellationTokenSource timeOutProvider = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            await instance._Crawler.Initiate(timeOutProvider.Token);

            if (timeOutProvider.IsCancellationRequested)
                throw new WebSocketException("could not start the web socket connection");

            return instance;
        }

        public IDisposable Subscribe(IObserver<Models.TradeEntry> observer)
        {
            if (_Observer != null)
                throw new OverflowException("subscriptions are limited to 1 at a time");

            _Observer = observer;

            SubscriptionCancelation source = new SubscriptionCancelation();

            Receive(source.Token).Start();

            return source;
        }

        private async Task Receive(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                try
                {
                    Models.TradeEntry item = await _Crawler.Receive(token);
                    _Observer.OnNext(item);
                }
                catch(Exception e)
                {
                    _Observer.OnError(e);
                }
            }

            await _Crawler.Stop(new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
        }
    }
}
