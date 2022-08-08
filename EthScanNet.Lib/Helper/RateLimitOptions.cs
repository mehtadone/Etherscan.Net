using System;

namespace EthScanNet.Lib.Helper
{
    public class RateLimitOptions
    {
        public int Count { get; set; }
    
        public TimeSpan TimeSpan { get; set; }
    }   
}