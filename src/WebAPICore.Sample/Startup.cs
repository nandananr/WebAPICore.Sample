using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using StackExchange.Redis;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Internal;

namespace WebAPICore.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConnectionMultiplexer redis = null;
            if (redis == null)
            {
                ConfigurationOptions option = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    EndPoints = { "34.231.5.225" }
                };
                redis = ConnectionMultiplexer.Connect(option);
            }
            services.AddDataProtection()
                    .PersistKeysToRedis(redis, "DataProtection-Keys");



            services.AddMvc();

            //var redis = ConnectionMultiplexer.Connect("localhost:6379");
            //services.AddDataProtection();
            //services.Configure<KeyManagementOptions>(o =>
            //{
            //    o.XmlRepository = new RedisXmlRepository(() => redis.GetDatabase(), "DataProtection-Keys");
            //});

            // services.AddDataProtection().SetDefaultKeyLifetime(TimeSpan.FromDays(14));
            //services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
            //{
            //    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            //    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            //});

            //services.AddDataProtection().DisableAutomaticKeyGeneration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
