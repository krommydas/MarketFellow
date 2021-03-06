﻿using GraphQL.Types;
using GraphQL;
using MongoDB.Driver;

namespace MarketFellowApi.Models
{
    public class TradeEntryFilters : ObjectGraphType
    {
        public TradeEntryFilters()
        {
            Field<IntGraphType>("marketProvider");

            Field<ShortGraphType>("tradingPair");
        }
    }
}
