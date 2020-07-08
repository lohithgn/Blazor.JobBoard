using System;
using System.Net.Http;
using System.Threading.Tasks;
using JobBoard.Client.ApiClients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using Refit;

namespace JobBoard.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var baseAddress = builder.HostEnvironment.BaseAddress;
            if (builder.HostEnvironment.IsEnvironment("Development"))
            {
                baseAddress = "http://localhost:7071/";
            }

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
            
            builder.Services.AddRefitClient<IJobsClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));

            builder.Services.AddScoped<DialogService>();

            await builder.Build().RunAsync();
        }
    }
}
