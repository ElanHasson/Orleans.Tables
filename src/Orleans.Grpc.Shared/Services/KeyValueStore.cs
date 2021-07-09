using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Orleans.Grpc.Shared.Services.Protos;
using Orleans.KeyValueStore.Grains.Interfaces;

namespace Orleans.Grpc.Shared.Services
{
    public partial class KeyValueStoreService  : Protos.KeyValueStoreService.KeyValueStoreServiceBase
    {
        private readonly IClusterClient client;

        public KeyValueStoreService(IClusterClient client)
        {
            this.client = client;
        }

        public override async Task<AddColumnReply> AddColumn(AddColumnRequest request, ServerCallContext context)
        {
            var grain = this.client.GetGrain<IRowDefinitionGrain>(request.UUID.ToStringUtf8());
            await grain.AddColumn(request.Definition.ColumnName);
            return new AddColumnReply();
        }
    }
}