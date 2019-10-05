using FellowLibrary.Models;
using GraphQL.Types;

public class TradeEntryType : ObjectGraphType<TradeEntry>
{
    public TradeEntryType()
    {
        Field("price", x => x.Price);
        Field("time", x => x.DateTime);
        Field("tradePair", x => x.TradingPair);
    }
}