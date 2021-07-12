using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using Orleans.KeyValueStore.Grains;
using Orleans.KeyValueStore.Grains.Interfaces;
using Orleans.Streams;

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
                        siloBuilder.AddMemoryGrainStorage("circleStorage");
                        siloBuilder.AddMemoryGrainStorage("nodeStorage");
                        siloBuilder.AddMemoryGrainStorage("PubSubStore");
                        siloBuilder.AddSimpleMessageStreamProvider("nodeResponse", configurator =>
                        {
                            configurator.FireAndForgetDelivery = true;
                        });
                        siloBuilder.AddMemoryGrainStorageAsDefault();
                    }
                    else
                    {
                        throw new NotImplementedException("Only setup for development mode.");
                    }

                    siloBuilder.UseDashboard(options =>
                    {
                        options.Port = 8888;
                    });
                    
                    siloBuilder.ConfigureApplicationParts(parts =>
                    {
                        parts.AddApplicationPart(typeof(ContainerGrain).Assembly).WithReferences();
                    });
                    
                    
                });
    }
}
