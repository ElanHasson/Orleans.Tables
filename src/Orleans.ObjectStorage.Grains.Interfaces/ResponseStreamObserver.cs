using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Orleans.KeyValueStore.Grains.Interfaces
{
    public class ResponseStreamObserver<T> : IAsyncObserver<T>
    {
        private readonly Func<T, Task> onItemReceived;
        private bool isCompleted;

        public ResponseStreamObserver(Func<T, Task> onItemReceived)
        {
            this.onItemReceived = onItemReceived;
        }


        public async Task OnNextAsync(T item, StreamSequenceToken token = null)
        {
            await onItemReceived(item);
        }

        public async Task OnCompletedAsync() => this.isCompleted = true;

        public bool IsCompleted => isCompleted;

        public Task OnErrorAsync(Exception ex)
        {
            Console.WriteLine(ex);
            return Task.CompletedTask;
        }
    }
}