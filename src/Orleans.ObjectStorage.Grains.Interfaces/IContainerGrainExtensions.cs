using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Orleans.KeyValueStore.Grains.Interfaces
{
    public static class IContainerGrainExtensions
    {
        public static async Task<ResponseStream> Get<T>(this IClusterClient clusterClient,
            Func<Guid, Task<ResponseStream>> grainCall, Func<T, Task> onItemReceived)
        {
            var responseStreamId = Guid.NewGuid();
            var stream = clusterClient.GetStreamProvider("nodeResponse")
                .GetStream<T>(responseStreamId, "default");
            
            var streamObserver = new ResponseStreamObserver<T>(onItemReceived);
            var handle = await stream.SubscribeAsync(streamObserver);
            
            var result = await grainCall(responseStreamId);
            //TODO: Replace with Async signal
            do
            {
                Console.WriteLine("Got All of them!");
                await Task.Delay(100);
            } while (!streamObserver.IsCompleted);

            return new ResponseStream(responseStreamId);
        }
    }
}