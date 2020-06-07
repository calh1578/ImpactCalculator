using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ImpactCalculator.WebClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton(Configuration);

            var calcURL = GetSetting("CALCULATOR_URL");

            if (string.IsNullOrWhiteSpace(calcURL))
            {
                calcURL = "https://localhost:5001";
            }
            else
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            }

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            Console.WriteLine(calcURL);

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddGrpcClient<Calculator.CalculatorClient>(cc =>
            {
                cc.Address = new Uri(calcURL);

            })
            .ConfigureChannel(op =>
            {
                op.HttpClient = new HttpClient(handler);
            });
        }

        private string GetSetting(string key)
        {
            var value = Configuration[key];

            return value;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            if (string.IsNullOrWhiteSpace(GetSetting("CALCULATOR_URL")))
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
