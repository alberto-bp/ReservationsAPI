using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Timeout;
using ReservationsAPI.Entities;
using ReservationsAPI.Services;
using Restaurant.Common.Extensions;

namespace ReservationsAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private static Random random = new Random();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongo()
                    .AddMongoRepository<Reservation>("reservations")
                    .AddMongoRepository<Room>("rooms")
                    .AddMassTransitWithRabbitMQ();

            // AddRoomsClient(services);

            services.AddControllers(option =>
            {
                option.SuppressAsyncSuffixInActionNames = false;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReservationsAPI", Version = "v1" });
            });

            services.AddMongoDbAndRabbitMQHealthChecks(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReservationsAPI v1"));

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions {
                    Predicate = check => check.Tags.Contains("ready")
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions {
                    Predicate = _ => false
                });
            });
        }

        private static void AddRoomsClient(IServiceCollection services)
        {
            services.AddHttpClient<RoomsService>(c =>
            {
                c.BaseAddress = new Uri("http://localhost:5004");
            })
            .AddTransientHttpErrorPolicy(config => config.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                3,
                TimeSpan.FromSeconds(10)
            )).AddTransientHttpErrorPolicy(config => config.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                3,
                retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)) + TimeSpan.FromMilliseconds(random.Next(0, 1000))
            )).AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(2)));
        }
    }
}
