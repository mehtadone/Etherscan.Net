using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using EthScanNet.Lib.Enums;
using EthScanNet.Lib.Extensions;
using EthScanNet.Lib.Models.ApiResponses;
using EthScanNet.Lib.Models.EScan;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EthScanNet.Lib.Models.ApiRequests
{
    internal class EScanRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string Module { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Action { get; }

        private readonly EScanClient _eScanClient;
        private readonly Type _responseType;

        //public Type returnType { get; set; }
        internal EScanRequest(EScanClient eScanClient, Type responseType, EScanModules module, EScanActions action)
        {
            if (responseType.BaseType != typeof(EScanResponse) && responseType.BaseType != typeof(EScanJsonRpcResponse))
            {
                const string type = "response type should inherit from 'EScanResponse' or 'EScanJsonRpcResponse'";
                throw new(type);
            }

            _eScanClient = eScanClient;
            _responseType = responseType;
            
            Module = module.ToString();
            Action = action.ToString();
        }

        internal async Task<dynamic> SendAsync()
        {
            using var client = _eScanClient.HttpClientFactory.CreateClient();
            
            var requestUrl = EScanClient.BaseUrl;
            
            var queryString = this.ToQueryString();
            var finalUrl = requestUrl + queryString + "&apiKey=" + _eScanClient.ApiKeyToken;
            
            Debug.WriteLine(finalUrl);

            await _eScanClient.RateLimiter.DelayAsync();
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, finalUrl);
            try
            {
                var result = await client.SendAsync(requestMessage);
                if (!result.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Server issue (" + result.StatusCode + "): " + result.ReasonPhrase);
                }

                var resultStream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);
                
                if (_responseType.BaseType == typeof(EScanJsonRpcResponse))
                {
                    var genericJsonResponse = await resultStream.Get<EScanGenericJsonResponse>().ConfigureAwait(false);
                    if (genericJsonResponse.JsonRpc != null)
                    {
                        dynamic jsonResponseObject = resultStream.Get(_responseType);
                        return jsonResponseObject;
                    }
                }

                var genericResponse = await resultStream.Get<EScanGenericResponse>().ConfigureAwait(false);
                if (genericResponse.Message.StartsWith("NotOk", StringComparison.OrdinalIgnoreCase))
                {
                    if (genericResponse.ResultMessage.GetType() != typeof(JArray))
                    {
                        throw new("Error with API result: (" + genericResponse.ResultMessage + ")");
                    }

                    throw new("Error with API result: (Unknown)");
                }

                dynamic responseObject = await resultStream.Get(_responseType).ConfigureAwait(false);
                return responseObject;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        internal async Task<dynamic> SendAsync<T>(T payload)
        {
            using var client = _eScanClient.HttpClientFactory.CreateClient();
            var requestUrl = EScanClient.BaseUrl;

            await _eScanClient.RateLimiter.DelayAsync();

            var payloadJson = JsonConvert.SerializeObject(payload);
            var payloadDictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(payloadJson);
            
            payloadDictionary.Add("apikey", _eScanClient.ApiKeyToken);
            payloadDictionary.Add("module", this.Module);
            payloadDictionary.Add("action", this.Action);

            var content = new FormUrlEncodedContent(payloadDictionary);

            try
            {
                var result = await client.PostAsync(requestUrl, content);
                if (!result.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Server issue (" + result.StatusCode + "): " + result.ReasonPhrase);
                }

                var resultStream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);
                if (_responseType.BaseType == typeof(EScanJsonRpcResponse))
                {
                    var genericJsonResponse = await resultStream.Get<EScanGenericJsonResponse>().ConfigureAwait(false);
                    if (genericJsonResponse.JsonRpc != null)
                    {
                        dynamic jsonResponseObject = await resultStream.Get(_responseType);
                        return jsonResponseObject;
                    }
                }

                var genericResponse = await resultStream.Get<EScanGenericResponse>().ConfigureAwait(false);
                if (!genericResponse.IsOk)
                {
                    if (genericResponse.ResultMessage.GetType() != typeof(JArray))
                    {
                        throw new("Error with API result: (" + genericResponse.ResultMessage + ")");
                    }

                    throw new("Error with API result: (Unknown)");
                }

                dynamic responseObject = await resultStream.Get(_responseType).ConfigureAwait(false);
                return responseObject;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}