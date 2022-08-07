using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EthScanNet.Lib.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<T> Get<T>(this Stream stream)
        {
            var result = await Get(stream, typeof(T)).ConfigureAwait(false);
            return (T)result;
        }
        
        public static Task<object> Get(this Stream stream, Type type)
        {
            using (var streamReader = new StreamReader(stream, leaveOpen: true))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                var result = Task.FromResult(serializer.Deserialize(jsonTextReader, type));
                
                stream.Position = 0;
                return result;
            }
        }
    }
}