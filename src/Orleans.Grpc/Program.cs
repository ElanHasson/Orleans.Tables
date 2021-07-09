using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;

namespace Orleans.Grpc
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseOrleans((ctx, siloBuilder) =>
                {
                    if (ctx.HostingEnvironment.IsDevelopment())
                    {
                        siloBuilder.UseLocalhostClustering();
                        
                       
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
                    
                    siloBuilder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Program).Assembly));
                    
                    
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
              
                });

        }
    }
}