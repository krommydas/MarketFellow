using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FellowLibrary.Crawler
{
    public class TradingEntryCrawler
    {
        public TradingEntryCrawler(MarketProviderConfiguration forProvider, String forTradingPair)
        {
            if (forProvider == null || forProvider.TradesConfiguration == null || !forProvider.TradesConfiguration.IsValid())
                throw new MissingMemberException("trades are not configured well");

            if (String.IsNullOrEmpty(forTradingPair))
                throw new MissingMemberException("subscription trading pair is missing");

            _Configuration = forProvider;
            _TradingPair = forTradingPair;

            _Client = new ClientWebSocket();
        }

        ClientWebSocket _Client;
        MarketProviderConfiguration _Configuration;
        String _TradingPair;   

        public async Task Initiate(CancellationToken cancelationToken)
        {
            if (cancelationToken.IsCancellationRequested)
                return;

            if (_Client.State != WebSocketState.None)
                throw new WebSocketException("can not re start a connection");

            await Connect(cancelationToken);

            if (cancelationToken.IsCancellationRequested)
            {
                await _Client.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "cancelation requested", cancelationToken);
                return;
            }             

            await SubscribeToTradingChannel(cancelationToken);
        }

        public async Task<Models.TradeEntry> Receive(CancellationToken cancelationToken)
        {
            if (_Client.State != WebSocketState.Open)
                throw new WebSocketException("web scoket connection is not open");

            ArraySegment<byte> messageBuffer = WebSocket.CreateServerBuffer(500);
            await ReceivePartial(cancelationToken, messageBuffer);

            return Unbox(FromByteArray(messageBuffer));
        }

        public async Task Stop(CancellationToken cancelationToken)
        {
            if (_Client.State != WebSocketState.Open)
                throw new WebSocketException("there is no open connection to stop");       

            await _Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "termination requested", cancelationToken);

            _Client.Dispose();
        }

        private Task Connect(CancellationToken onCancel)
        {
            //if (!Uri.IsWellFormedUriString(_Configuration.TradesConfiguration.Url, UriKind.Absolute))
            //    throw new UriFormatException();

            return _Client.ConnectAsync(new Uri(_Configuration.TradesConfiguration.Url), onCancel);
        }

        private Task SubscribeToTradingChannel(CancellationToken onCancel) 
        {
            String withTradingPair = _Configuration.TradesConfiguration.SubscriptionMessage.Replace("@tradingPair@", _TradingPair);

            return _Client.SendAsync(ToByteArray(withTradingPair), WebSocketMessageType.Binary, true, onCancel);
        }

        private async Task ReceivePartial(CancellationToken onCancel, ArraySegment<byte> messageBuffer)
        {
            if(messageBuffer.Count > 0)
                onCancel.ThrowIfCancellationRequested();

            var response = await _Client.ReceiveAsync(messageBuffer, onCancel);

            if(!response.EndOfMessage)
                await ReceivePartial(onCancel, messageBuffer);
        }

        private ArraySegment<byte> ToByteArray(String json)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(json);
            return new ArraySegment<byte>(encoded);
        }
        private object FromByteArray(ArraySegment<byte> message)
        {
            String textMessage = Encoding.UTF8.GetString(message.ToArray());
            return JsonConvert.DeserializeObject(textMessage);
        }

        private Models.TradeEntry Unbox(object boxedResult)
        {
            if (boxedResult is null) throw new SocketException();

            Newtonsoft.Json.Linq.JObject item = boxedResult as Newtonsoft.Json.Linq.JObject;
            if (item == null) throw new FormatException();

            foreach(var tag in _Configuration.TradesConfiguration.TradeEntryTags)
            {
                JToken tagValue;
                if(!item.TryGetValue(tag.Key, out tagValue) || !Equals(tagValue, tag.Value))
                    throw new FormatException();
            }

            decimal? price = item.Value<decimal?>(_Configuration.TradesConfiguration.PriceField);
            if (!price.HasValue) throw new FormatException();

            String tradingPairID = item.Value<String>(_Configuration.TradesConfiguration.TradingPairID);
            if (String.IsNullOrEmpty(tradingPairID)) throw new FormatException();

            DateTime? time = item.Value<DateTime?>(_Configuration.TradesConfiguration.TimeField);
            if (!time.HasValue) throw new FormatException();

            return new Models.TradeEntry() { DateTime = time.Value, Price = price.Value, TradingPair = tradingPairID , Provider = _Configuration.ID };
        }
    }
}
