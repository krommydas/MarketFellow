using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketFellow.Models
{
    public class MarketFellowSchema : Schema
    {
        public MarketFellowSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<MarketFellowQuery>();
        }
    }
}
