using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Orleans.KeyValueStore.Grains.Interfaces
{
    public class ResponseStreamObserver<T> : IAsyncObserver<T>
    {
        private readonly Func<T, StreamSequenceToken, Task> onItemReceived;
        private readonly Func<Task> onCompleted;
        private readonly Func<Exception, Task> onErrorAsync;

        public ResponseStreamObserver(Func<T, StreamSequenceToken, Task> onItemReceived, Func<Task> onCompleted, Func<Exception, Task> onErrorAsync)
        {
            this.onItemReceived = onItemReceived;
            this.onCompleted = onCompleted;
            this.onErrorAsync = onErrorAsync;
        }


        /// <inheritdoc />
        public  Task OnNextAsync(T item, StreamSequenceToken token = null) => onItemReceived(item, token);

        /// <inheritdoc />
        public  Task OnCompletedAsync() => this.onCompleted();

        public Task OnErrorAsync(Exception ex) => this.onErrorAsync(ex);
    }
}