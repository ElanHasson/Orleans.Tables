using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using Orleans.KeyValueStore.Grains;
using Orleans.KeyValueStore.Grains.Interfaces;

namespace Orleans.ObjectStorage.TestApp
{
    class Program
    {
        async static Task Main(string[] args)
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .ConfigureApplicationParts(c => c.AddApplicationPart(typeof(ContainerGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .AddSimpleMessageStreamProvider("nodeResponse")
                .Build();
            await client.Connect(async error =>
            {
                Console.WriteLine("Waiting to retry...");
                await Task.Delay(TimeSpan.FromSeconds(2));
                Console.WriteLine("Retrying connection...");
                return true;
            });

            var bucket = client.GetGrain<IContainerGrain>("bucket");


            foreach (var i in Enumerable.Range(1, 5))
            {
                await bucket.AddObject($"Item-{i:##,###}", i);
            }

            foreach (var i in Enumerable.Range(1, 5))
            {
                await bucket.GetObject<int>($"Item-{i:##,###}");
            }

            await foreach (var (key, value) in client.Get<KeyValuePair<string, object>>(streamId => bucket.GetAll(streamId),
                CancellationToken.None))
            {
                Console.WriteLine($"{key} = {value}");
            }
            Console.Write("ALL DONE!");
            await Task.Delay(-1);
        }
    }
}