using FellowLibrary.Models;
using GraphQL.Types;

namespace MarketFellowApi.Models
{
    public class MarketProviderType : ObjectGraphType<FellowLibrary.Models.MarketProvider>
    {
        public MarketProviderType()
        {
            Field("id", x => x.ID);
            Field("name", x => x.Name);
        }
    }
}
