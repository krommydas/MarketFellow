using GraphQL.Resolvers;
using GraphQL.Subscription;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MarketFellowApi.Models
{
    public class MarketFellowSubscription : ObjectGraphType
    {
        public MarketFellowSubscription(ClientWebSocket clientWebSocket, FellowLibrary.Crawler.MarketProvidersConfiguration configuration)
        {
            FieldSubscribe<TradeEntryType>("tradeEntry", 
                resolve: context => context.Source as FellowLibrary.Models.TradeEntry, 
                subscribe: context => Subscribe(clientWebSocket, configuration, context),
                 arguments: new QueryArguments(new QueryArgument<IntGraphType>() { Name = "marketProvider" },
                                               new QueryArgument<StringGraphType>() { Name = "tradingPair" })
                );
        }

        private IObservable<object> Subscribe(ClientWebSocket clientWebSocket, FellowLibrary.Crawler.MarketProvidersConfiguration configuration,
            ResolveEventStreamContext context)
        {
            if (context == null) throw new ArgumentNullException();

            var marketProvider = context.GetArgument<int?>("marketProvider");
            if (!marketProvider.HasValue) throw new ArgumentNullException();

            var marketProviderConfiguration = configuration.FirstOrDefault(x => x.ID == marketProvider.Value);
            if (marketProviderConfiguration == null) throw new ArgumentException();

            var tradingPair = context.GetArgument<String>("tradingPair");
            if (String.IsNullOrEmpty(tradingPair)) throw new ArgumentNullException();

            return new FellowLibrary.Crawler.TradingEntryCrawler(clientWebSocket, marketProviderConfiguration, tradingPair);
        }
    }
}
