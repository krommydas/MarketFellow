using System;
using System.Collections.Generic;
using System.Text;

namespace FellowLibrary.Crawler
{
   public sealed class TradingPairConfiguration
   {
        public String Url { get; set; }

        public String IDField { get; set; }

        public String NameField { get; set; }

        public Dictionary<String, String> AdditionalHeaders { get; set; }
   }
}
