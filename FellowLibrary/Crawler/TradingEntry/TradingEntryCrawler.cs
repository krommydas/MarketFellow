using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace FellowLibrary.Crawler
{
    public class TradingEntryCrawler: IObservable<Models.TradeEntry>
    {
        public TradingEntryCrawler(ClientWebSocket connectionProvider, MarketProviderConfiguration forProvider, String forTradingPair)
        {
            _Client = connectionProvider;
            _Configuration = forProvider;
            _TradingPair = forTradingPair;
        }

        ClientWebSocket _Client;
        MarketProviderConfiguration _Configuration;
        String _TradingPair;

        private IObserver<Models.TradeEntry> _observer;

        public IDisposable Subscribe(IObserver<Models.TradeEntry> observer)
        {
            _observer = observer;

            WebSocketCancelation cancelationProvider = new WebSocketCancelation(_Client);

            cancelationProvider.Disconnect().ContinueWith(x => Start(_Configuration, _TradingPair));
            
            return new WebSocketCancelation(_Client);
        }

        private Task Start(MarketProviderConfiguration configuration, String tradingPairID)
        {
            return Connect(configuration, tradingPairID).ContinueWith(x => SubscribeToTradingChannel(configuration, tradingPairID))
                .ContinueWith(x => Receive(configuration));
        }

        private Task Connect(MarketProviderConfiguration configuration, String tradingPairID)
        {
            //if (_Client.State == WebSocketState)
            //    _Client.Close();
            if (!Uri.IsWellFormedUriString(configuration.TradingEntryConfiguration.Url, UriKind.Absolute))
                throw new UriFormatException();

            return _Client.ConnectAsync(new Uri(configuration.TradingEntryConfiguration.Url), new System.Threading.CancellationToken());
        }

        private Task SubscribeToTradingChannel(MarketProviderConfiguration configuration, String tradingPairID) 
        {
            if (String.IsNullOrEmpty(configuration.TradingEntryConfiguration.SubscriptionMessage))
                throw new ArgumentNullException();

            String withTradingPair = configuration.TradingEntryConfiguration.SubscriptionMessage.Replace("@tradingPair@", tradingPairID);
            return _Client.SendAsync(ToByteArray(withTradingPair), WebSocketMessageType.Binary, true, new System.Threading.CancellationToken());
        }

        private void Receive(MarketProviderConfiguration configuration)
        {
            ArraySegment<byte> response = new ArraySegment<byte>();
            _Client.ReceiveAsync(response, new System.Threading.CancellationToken())
                .ContinueWith(x => {

                    Models.TradeEntry tradeEntry = Unbox(FromByteArray(response), configuration);
                    _observer.OnNext(tradeEntry);

                    if (!x.IsCanceled || _Client.State != WebSocketState.CloseSent)
                        Receive(configuration);
                });
            
        }

        private ArraySegment<byte> ToByteArray(String json)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, json);
            }

            return new ArraySegment<byte>(ms.ToArray());
        }
        private object FromByteArray(ArraySegment<byte> message)
        {
            MemoryStream ms = new MemoryStream(message.ToArray());
            Object result;
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        private Models.TradeEntry Unbox(object boxedResult, MarketProviderConfiguration configuration)
        {
            if (boxedResult is null) throw new SocketException();

            Newtonsoft.Json.Linq.JObject item = boxedResult as Newtonsoft.Json.Linq.JObject;
            if (item == null) throw new FormatException();

            foreach(var tag in configuration.TradingEntryConfiguration.TradeEntryTags)
            {
                JToken tagValue;
                if(!item.TryGetValue(tag.Key, out tagValue) || !Equals(tagValue, tag.Value))
                    throw new FormatException();
            }

            decimal? price = item.Value<decimal?>(configuration.TradingEntryConfiguration.PriceField);
            if (!price.HasValue) throw new FormatException();

            String tradingPairID = item.Value<String>(configuration.TradingEntryConfiguration.TradingPairID);
            if (String.IsNullOrEmpty(tradingPairID)) throw new FormatException();

            DateTime? time = item.Value<DateTime?>(configuration.TradingEntryConfiguration.TimeField);
            if (!time.HasValue) throw new FormatException();

            return new Models.TradeEntry() { DateTime = time.Value, Price = price.Value, TradingPair = tradingPairID , Provider = configuration.ID };
        }
    }
}
