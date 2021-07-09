using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Grpc.Grains.Row;
using Orleans.Hosting;
using Orleans.KeyValueStore.Grains.Interfaces;

namespace Orleans.KeyValueStore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseOrleans((ctx, siloBuilder) =>
                {
                    if (ctx.HostingEnvironment.IsDevelopment())
                    {
                        siloBuilder.UseLocalhostClustering();
                        siloBuilder.AddMemoryGrainStorageAsDefault();

                        siloBuilder.AddMemoryGrainStorage("rowStorage");
                        siloBuilder.AddMemoryGrainStorage("KeyValueStore");
                    }
                    else
                    {
                        throw new NotImplementedException("Only setup for development mode.");
                        // In order to support multiple hosts forming a cluster, they must listen on different ports.
                        // Use the --InstanceId X option to launch subsequent hosts.
                        var instanceId = ctx.Configuration.GetValue<int>("InstanceId");
                        siloBuilder.UseLocalhostClustering(
                            siloPort: 11111 + instanceId,
                            gatewayPort: 30000 + instanceId,
                            primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11111));

                    }

                    siloBuilder.UseDashboard(options =>
                    {
                        options.Port = 8888;
                    });
                    
                    siloBuilder.ConfigureApplicationParts(parts =>
                    {
                        parts.AddApplicationPart(typeof(RowDefinitionGrain).Assembly).WithReferences();
                    });
                    
                    
                });
    }
}
