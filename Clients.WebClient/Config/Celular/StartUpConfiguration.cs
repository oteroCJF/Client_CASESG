using Api.Gateway.WebClient.Proxy.Celular.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.Celular.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Celular.Convenios.Commands;
using Api.Gateway.WebClient.Proxy.Celular.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.Celular.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.Celular.EntregablesContrato.Queries;
using Api.Gateway.WebClient.Proxy.Celular.ServiciosContrato.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Gateway.WebClient.Proxy.Celular.ServiciosContrato.Commands;

namespace Clients.WebClient.Config.Celular
{
    public static class StartUpConfiguration
    {
        public static IServiceCollection AddAppsettingBinding(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            return service;
        }

        public static IServiceCollection AddProxiesCelularQueries(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<IQContratoCelularProxy, QContratoCelularProxy>();
            service.AddHttpClient<IQSContratoCelularProxy, QSContratoCelularProxy>();
            service.AddHttpClient<IQConvenioCelularProxy, QConvenioCelularProxy>();
            service.AddHttpClient<IQEContratoCelularProxy, QEContratoCelularProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesCelularCommands(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<ICContratoCelularProxy, CContratoCelularProxy>();
            service.AddHttpClient<ICSContratoCelularProxy, CSContratoCelularProxy>();
            service.AddHttpClient<ICConvenioCelularProxy, CConvenioCelularProxy>();
            service.AddHttpClient<ICEContratoCelularProxy, CEContratoCelularProxy>();

            return service;
        }
    }
}
