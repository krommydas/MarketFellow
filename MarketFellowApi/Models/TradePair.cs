using FellowLibrary.Models;
using GraphQL.Types;

namespace MarketFellowApi.Models
{
    public class TradingPairType : ObjectGraphType<FellowLibrary.Models.TradingPair>
    {
        public TradingPairType()
        {
            Field("id", x => x.ID);
            Field("name", x => x.Name);
        }
    }
}
