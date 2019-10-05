using System;
using System.Collections.Generic;
using System.Text;

namespace FellowLibrary.Models
{
    public sealed class TradingPair
    {
        public String ID { get; set; }

        public int MarketProviderSource { get; set; }

        public String Name { get; set; }
    }
}
