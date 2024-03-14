using KONSI.CHALLENGE.CACHE;
using KONSI.CHALLENGE.QUEUE;
using KONSI.CHALLENGE.SEARCH;
using KONSI.CHALLENGE.SERVICES.Connections;
using KONSI.CHALLENGE.SERVICES.Interfaces;
using KONSI.CHALLENGE.SERVICES.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KONSI.CHALLENGE.INFRA.IOC.InfrastructureSettings
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IApplicationRestConnection, ApplicationRestConnection>();
            services.AddScoped<IConsultationBenefitsService, ConsultationBenefitsService>();
            services.AddScoped<IMainService, MainService>();
            services.AddScoped<IElasticsearchClient, ElasticsearchClient>();
            services.AddScoped<IRedisCache, RedisCache>();
            services.AddScoped<IRabbitMQConsumer, RabbitMQConsumer>();

            return services;
        }
    }
}