using System;
using System.Collections.Generic;
using System.Text;

namespace FellowLibrary.Crawler
{
    public  class TradingEntryConfiguration
    {
        public String Url { get; set; }

        public String PriceField { get; set; }

        public String TimeField { get; set; }

        public String SubscriptionMessage { get; set; }

        public String TradingPairID { get; set; }

        public Dictionary<String, object> TradeEntryTags { get; set; }
    }
}
