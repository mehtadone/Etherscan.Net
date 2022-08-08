using System;

namespace EthScanNet.Lib.Models.Exceptions
{
    public class EtherscanApiRateLimitException : Exception
    {
        public EtherscanApiRateLimitException(string message): base(message)
        {
        }
    }   
}