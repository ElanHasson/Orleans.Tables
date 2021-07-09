using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsistentSharp;
using Microsoft.Extensions.Logging;
using Orleans.KeyValueStore.Grains.Interfaces;
using Orleans.Runtime;

namespace Orleans.KeyValueStore.Grains
{
    public class ContainerGrain : Grain, IContainerGrain
    {
        private const long InitialNodeCount = 4;
        private readonly ILogger<ContainerGrain> logger;
        private readonly IPersistentState<SortedList<uint, string>> circle;
        private readonly ConsistentHash consistentHash;

        public ContainerGrain(ILogger<ContainerGrain> logger,
            [PersistentState("circle", "circleStorage")]
            IPersistentState<SortedList<uint, string>> circle)
        {
            this.logger = logger;
            this.circle = circle;
            this.consistentHash = new ConsistentHash();
        }

        public async Task<bool> AddObject<T>(string key, T @object)
        {
            var nodeToAddObjectTo = this.consistentHash.Get(key);
            var nodeGrain =  this.GrainFactory.GetGrain<INodeGrain>(this.GetNodeKey(nodeToAddObjectTo));
            return await nodeGrain.AddObject<T>(key, @object);
        }

        public async Task<T> GetObject<T>(string key)
        {
            var nodeToAddObjectTo = this.consistentHash.Get(key);
            var nodeGrain = this.GrainFactory.GetGrain<INodeGrain>(this.GetNodeKey(nodeToAddObjectTo));
            return await nodeGrain.GetObject<T>(key);
        }

        private string GetNodeKey(string nodeToAddObjectTo)
        {
            return $"{this.GetGrainIdentity()}-{nodeToAddObjectTo}";
        }

        public override async Task OnActivateAsync()
        {
            await this.Initialize();
        }

        private Task Initialize()
        {
            if (this.circle.RecordExists)
            {
                 return Task.CompletedTask;
            }

            this.circle.State = new SortedList<uint, string>();

            for (var i = 0; i < InitialNodeCount; i++)
            {
                this.consistentHash.Add($"node{i}");
            }

            this.circle.State = this.consistentHash.Circle;
            return Task.CompletedTask;

        }
    }
}