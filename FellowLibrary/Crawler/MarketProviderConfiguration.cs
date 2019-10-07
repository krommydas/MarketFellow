using System;
using System.Collections.Generic;
using System.Text;

namespace FellowLibrary.Crawler
{
    public class MarketProviderConfiguration
    {
        public int ID { get; set; }

        public String Name { get; set; }

        public TradingPairConfiguration TradingPairConfiguration { get; set; }

        public TradingEntryConfiguration TradingEntryConfiguration { get; set; }
    }

    public class MarketProvidersConfiguration : List<MarketProviderConfiguration>
    {
      


    }
}
