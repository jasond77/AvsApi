
using Data.Repositories;
using DataInterface;
using DB;
using DomainInterface.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceInterface;
using Services;

namespace IoC
{
    public static class IocHelper
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Repositories
            // Service

            //services.AddScoped<IProductService, ProductService>();
            //services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICommonRepository, CommonRepository>();

            services.AddScoped<IHttpService, HttpService>();

            services.AddScoped<ILogService, LogService>();
            services.AddScoped<ILogRepository, LogRepository>();

            services.AddScoped<IMessageService, MessageService>();

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<IDbContext, DbContext>(serviceProvider =>
            {
                return new DbContext(configuration["ConnectionStrings:BaseDbKey"]);
            });

            services.AddSingleton(configuration);

            // Logger
            //container.RegisterType<ILogger, PortalLogger>();

        }
    }
}
