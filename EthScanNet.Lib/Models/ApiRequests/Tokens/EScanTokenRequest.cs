using System;
using EthScanNet.Lib.Enums;
using EthScanNet.Lib.Models.EScan;

namespace EthScanNet.Lib.Models.ApiRequests.Tokens
{
    internal class EScanTokenRequest : EScanRequest
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public EScanAddress ContractAddress { get; }
        
        internal EScanTokenRequest(EScanAddress contractAddress, EScanModules module, EScanActions action, Type returnType) : base(returnType, module, action)
        {
            this.ContractAddress = contractAddress;
        }
    }
}