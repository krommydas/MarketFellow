using FellowLibrary.Models;
using GraphQL.Types;

namespace MarketFellowApi.Models
{
    public class TradeEntryType : ObjectGraphType<TradeEntry>
    {
        public TradeEntryType()
        {
            Field("price", x => x.Price);
            Field("time", x => x.DateTime);
            Field("tradePair", x => x.TradingPair);
            Field("provider", x => x.Provider);
            Field("id", x => x.ID);
        }
    }
}