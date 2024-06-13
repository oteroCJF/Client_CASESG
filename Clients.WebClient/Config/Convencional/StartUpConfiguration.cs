using Api.Gateway.WebClient.Proxy.Convencional.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.Convencional.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Convencional.Convenios.Commands;
using Api.Gateway.WebClient.Proxy.Convencional.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.Convencional.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.Convencional.EntregablesContrato.Queries;
using Api.Gateway.WebClient.Proxy.Convencional.ServiciosContrato.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Gateway.WebClient.Proxy.Convencional.ServiciosContrato.Commands;

namespace Clients.WebClient.Config.Convencional
{
    public static class StartUpConfiguration
    {
        public static IServiceCollection AddAppsettingBinding(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            return service;
        }

        public static IServiceCollection AddProxiesConvencionalQueries(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<IQContratoConvencionalProxy, QContratoConvencionalProxy>();
            service.AddHttpClient<IQSContratoConvencionalProxy, QSContratoConvencionalProxy>();
            service.AddHttpClient<IQConvenioConvencionalProxy, QConvenioConvencionalProxy>();
            service.AddHttpClient<IQEContratoConvencionalProxy, QEContratoConvencionalProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesConvencionalCommands(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<ICContratoConvencionalProxy, CContratoConvencionalProxy>();
            service.AddHttpClient<ICSContratoConvencionalProxy, CSContratoConvencionalProxy>();
            service.AddHttpClient<ICConvenioConvencionalProxy, CConvenioConvencionalProxy>();
            service.AddHttpClient<ICEContratoConvencionalProxy, CEContratoConvencionalProxy>();

            return service;
        }
    }
}
