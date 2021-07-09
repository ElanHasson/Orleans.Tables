using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Grpc.Shared.Services.Protos;

namespace Orleans.KeyValueStore.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            
            builder.Services.AddGrpcClient<KeyValueStoreService.KeyValueStoreServiceClient>(o =>
            {
                o.Address = new Uri("https://localhost:5001");
            }).ConfigurePrimaryHttpMessageHandler(
                () => new GrpcWebHandler(new HttpClientHandler()));;

            await builder.Build().RunAsync();
        }
    }
}
