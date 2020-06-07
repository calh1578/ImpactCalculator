using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ImpactCalculator.CalculatorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConsole();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("COMPOSE")))
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(5000, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http2;
                        });
                    });
                }

                webBuilder.UseStartup<Startup>();
            });
    }
}