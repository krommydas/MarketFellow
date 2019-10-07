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
            //return ConfigureRequest(configuration)
            //    .ContinueWith((response) => new Models.TradingPair[] { Unbox(response.Result.Content.ReadAsAsync<Object>().Result, configuration) }.AsEnumerable());

            return ConfigureRequest(configuration)
                .ContinueWith((response) => Unbox(response.Result.Content.ReadAsAsync<Object>().Result, configuration));
        }

        private IEnumerable<Models.TradingPair> Unbox(object boxedResult, TradingPairConfiguration configuration)
        {
            List<Models.TradingPair> result = new List<Models.TradingPair>();

            if (boxedResult is null) return result;

            Newtonsoft.Json.Linq.JArray items = boxedResult as Newtonsoft.Json.Linq.JArray;
            if (items == null) return result;

            foreach (var item in items)
            {
                if (item is null || !item.HasValues) continue;

                String id = item.Value<String>(configuration.IDField);
                if (String.IsNullOrEmpty(id)) continue;

                String name = item.Value<String>(configuration.NameField);
                if (String.IsNullOrEmpty(name)) continue;

                result.Add(new Models.TradingPair() { ID = id, Name = name });
            }

            return result;
        }

        private Task<HttpResponseMessage> ConfigureRequest(TradingPairConfiguration configuration)
        {
            if (!Uri.IsWellFormedUriString(configuration.Url, UriKind.RelativeOrAbsolute))
                throw new UriFormatException("trading pair request url issues");

            if(configuration.AdditionalHeaders != null)
            {
                foreach (var header in configuration.AdditionalHeaders)
                    _Client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            return _Client.GetAsync(new Uri(configuration.Url));
        }
    }
}
