using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using EthScanNet.Lib.EScanApi;
using EthScanNet.Lib.Helper;
using RateLimiter;

namespace EthScanNet.Lib
{
    public class EScanClient
    {
        [Obsolete("Please use Client.Network.Url. Property will be deprecated in v2.")]
        public static string BaseUrl { get; private set; }
        
        public EScanNetwork Network { get; }
        
        public Accounts Accounts { get; }
        public Tokens Tokens { get;  }
        public Stats Stats { get; }
        public Contracts Contracts { get; set; }

        public Proxy Proxy { get; }
        
        public string ApiKeyToken { get; }
        
        internal TimeLimiter RateLimiter { get; set; }
        
        internal IHttpClientFactory HttpClientFactory { get; }

        /// <summary>
        /// The base connection of the API, use this to access the features from within the account
        /// </summary>
        /// <param name="network">The ETH based EtherScan network to which to connect</param>
        /// <param name="apiKeyToken">The API key needed to be able to access more results</param>
        /// <param name="httpClientFactory">A factory abstraction for a component that can create <see cref="HttpClient"/> instances with custom configuration for a given logical name.</param>
        /// <param name="throttleMs">Delay between transaction requests, set to 200ms, as registered users are allowed 5/seconds</param>
        public EScanClient(EScanNetwork network, string apiKeyToken, IHttpClientFactory httpClientFactory, params RateLimitOptions[] options)
        {
            BaseUrl = network;
            Network = network;
            ApiKeyToken = apiKeyToken;
            HttpClientFactory = httpClientFactory;

            Accounts = new(this);
            Tokens = new(this);
            Stats = new(this);
            Proxy = new(this);
            Contracts = new(this);
            
            RateLimiter = ConfigureRateLimiter(options);
        }

        private TimeLimiter ConfigureRateLimiter(RateLimitOptions[] options)
        {
            var rateLimitOptions = new List<RateLimitOptions>(options);
            
            if (!options.Any())
            {
                rateLimitOptions.Add(new RateLimitOptions()
                {
                    Count = 5,
                    TimeSpan = TimeSpan.FromSeconds(1)
                });
            }
            
            var constraints = new List<IAwaitableConstraint>();
			
            foreach (var option in rateLimitOptions)
            {
                var constraint = new CountByIntervalAwaitableConstraint(option.Count, option.TimeSpan);
                constraints.Add(constraint);
            }
			
            return TimeLimiter.Compose(constraints.ToArray());
        }
    }
}