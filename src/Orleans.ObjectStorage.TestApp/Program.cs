using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.KeyValueStore.Grains;
using Orleans.KeyValueStore.Grains.Interfaces;

namespace Orleans.ObjectStorage.TestApp
{
    class Program
    {
       async  static Task Main(string[] args)
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .ConfigureApplicationParts(c=>c.AddApplicationPart(typeof(ContainerGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
            await client.Connect(async error =>
            {
             
                Console.WriteLine("Waiting to retry...");
                await Task.Delay(TimeSpan.FromSeconds(2));
                Console.WriteLine("Retrying connection...");
                return true;
            });

            var bucket = client.GetGrain<IContainerGrain>("bucket");

            
            foreach (var i in Enumerable.Range(1,10000000))
            {
                await bucket.AddObject($"Item-{i:##,###}", i);
            }

            foreach (var i in Enumerable.Range(1, 10000000))
            {
                await bucket.GetObject<int>($"Item-{i:##,###}");
            }


            await Task.Delay(TimeSpan.Zero);
        }
    }
}
