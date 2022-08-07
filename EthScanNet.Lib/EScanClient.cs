using System;
using System.Net.Http;
using System.Reflection.Emit;
using EthScanNet.Lib.EScanApi;
using EthScanNet.Lib.Helper;

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
        
        public RateLimiter RateLimiter { get; set; }
        
        public IHttpClientFactory HttpClientFactory { get; }

        /// <summary>
        /// The base connection of the API, use this to access the features from within the account
        /// </summary>
        /// <param name="network">The ETH based EtherScan network to which to connect</param>
        /// <param name="apiKeyToken">The API key needed to be able to access more results</param>
        /// <param name="httpClientFactory">A factory abstraction for a component that can create <see cref="HttpClient"/> instances with custom configuration for a given logical name.</param>
        /// <param name="throttleMs">Delay between transaction requests, set to 200ms, as registered users are allowed 5/seconds</param>
        public EScanClient(EScanNetwork network, string apiKeyToken, IHttpClientFactory httpClientFactory, Action<RateLimiter> configureRateLimiter = null)
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
            
            RateLimiter = ConfigureRateLimiter(configureRateLimiter);
        }

        private RateLimiter ConfigureRateLimiter(Action<RateLimiter> configureRateLimiter)
        {
            var rateLimiter = new EthScanRateLimiter
            {
                Count = 5,
                Duration = TimeSpan.FromMilliseconds(1000)
            };
            
            configureRateLimiter?.Invoke(rateLimiter);

            return rateLimiter;
        }
    }
}