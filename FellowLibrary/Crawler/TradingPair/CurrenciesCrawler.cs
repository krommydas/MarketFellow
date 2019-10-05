using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Linq;

namespace FellowLibrary.Crawler
{
    public class TradingPairCrawler
    {
        HttpClient _Client;

        public TradingPairCrawler(HttpClient client)
        {
            _Client = client;
        }

        public Task<IEnumerable<Models.TradingPair>> GetTradingPairs(TradingPairConfiguration configuration)
        {
            return _Client.GetAsync(configuration.Url)
                .ContinueWith((response) => response.Result.Content.ReadAsAsync<IEnumerable<Object>>()
                .Result.Select(boxed => Unbox(boxed, configuration)));
        }

        private Models.TradingPair Unbox(object boxedResult, TradingPairConfiguration configuration)
        {
            if (!(boxedResult is Dictionary<String, Object>))
                return null;

            Dictionary<String, Object> dict = boxedResult as Dictionary<String, Object>;

            if (!dict.ContainsKey(configuration.IDField) || !dict.ContainsKey(configuration.NameField))
                return null;

            object idField;
            if (!dict.TryGetValue(configuration.IDField, out idField) || !(idField is string))
                return null;

            object nameField;
            if (!dict.TryGetValue(configuration.NameField, out nameField) || !(nameField is string))
                return null;

            return new Models.TradingPair() { ID = (string)idField, Name = (string)nameField };
        }
    }
}
