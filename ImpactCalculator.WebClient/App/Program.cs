using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ImpactCalculator.WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(GetLevel("LogLevel", LogEventLevel.Information))
                .MinimumLevel.Override("Microsoft.AspNetCore", GetLevel("AspNetCoreLogLevel", LogEventLevel.Warning))
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Debug("Attempting to start web host");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Unable to start web host");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

        private static LogEventLevel GetLevel(string setting, LogEventLevel defaultLevel)
        {
            var userSetLevelString = Environment.GetEnvironmentVariable(setting);

            if (string.IsNullOrEmpty(userSetLevelString))
            {
                return defaultLevel;
            }

            LogEventLevel userSetLevel;

            if (!Enum.TryParse(userSetLevelString, out userSetLevel))
            {
                Trace.TraceWarning($"Unable to parse User Log Event Level: {userSetLevel}, Setting level to {defaultLevel}");

                return userSetLevel;
            }

            return userSetLevel;
        }
    }
}