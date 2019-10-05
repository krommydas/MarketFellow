using System;
using System.Linq;
using System.Collections.Generic;
using GraphQL.Types;
using FellowLibrary.DatabaseAccess;
using System.Collections;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Net.Http;

namespace MarketFellow.Models
{
    public class MarketFellowQuery : ObjectGraphType
    {
        public MarketFellowQuery(DatabaseContext storeProvider, HttpClient httpClient, FellowLibrary.Crawler.MarketProvidersConfiguration marketProviderConfiguration)
        {
            Field<ListGraphType<MarketProviderType>>("marketProviders", 
                resolve: context => marketProviderConfiguration.Select(x => new FellowLibrary.Models.MarketProvider() { ID = x.ID, Name = x.Name }));

            Field<ListGraphType<TradingPairType>>("tradingPairs", 
                arguments: new QueryArguments(new QueryArgument<IntGraphType>(){ Name = "marketProvider" }), 
                resolve: context => GetTradingPairs(httpClient, marketProviderConfiguration, context));

            Field<ListGraphType<TradeEntryType>>("tradingEntries",
                 arguments: new QueryArguments(new QueryArgument<IntGraphType>() { Name = "marketProvider" },
                                               new QueryArgument<StringGraphType>() { Name = "tradingPair" }),      
                 resolve: context => GetTradeEntries(storeProvider, context));
        }

        private object GetTradingPairs(HttpClient httpClient, IEnumerable<FellowLibrary.Crawler.MarketProviderConfiguration> marketProviderConfiguration, 
            ResolveFieldContext<object> context)
        {
            if (context == null) throw new ArgumentNullException();

            var provider = context.GetArgument<int?>("marketProvider");
            if (provider.HasValue) marketProviderConfiguration = marketProviderConfiguration.Where(x => x.ID == provider.Value);

            var pairsProvider = new FellowLibrary.Crawler.TradingPairCrawler(httpClient);

            return Task.WhenAll(marketProviderConfiguration.Select(x => pairsProvider.GetTradingPairs(x.TradingPairConfiguration)));
        }

        private object GetTradeEntries(DatabaseContext storeProvider, ResolveFieldContext<object> context)
        {
            if (context == null) throw new ArgumentNullException();

            var mongoFilterBuilder = Builders<FellowLibrary.Models.TradeEntry>.Filter;

            var finalFilter = mongoFilterBuilder.Where(x => true);

            var marketProvider = context.GetArgument<int?>("marketProvider");
            if (marketProvider.HasValue) finalFilter = finalFilter & mongoFilterBuilder.Eq(x => x.Provider, marketProvider.Value);

            var tradingPair = context.GetArgument<String>("tradingPair");
            if (tradingPair != null) finalFilter = finalFilter & mongoFilterBuilder.Eq(x => x.TradingPair, tradingPair);

            return storeProvider.Trades.FindAsync(finalFilter);
        }
    }
}
