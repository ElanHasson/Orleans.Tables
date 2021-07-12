using System;

namespace Orleans.KeyValueStore.Grains.Interfaces
{
    public struct ResponseStream
    {
        public Guid ResponseStreamId { get; private set; }

        public ResponseStream(Guid responseStreamId)
        {
            ResponseStreamId = responseStreamId;
        }
    }
}