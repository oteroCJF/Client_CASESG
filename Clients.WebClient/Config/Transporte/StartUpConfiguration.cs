using Api.Gateway.WebClient.Proxy.Transporte.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.Transporte.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Transporte.Convenios.Commands;
using Api.Gateway.WebClient.Proxy.Transporte.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.Transporte.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.Transporte.EntregablesContrato.Queries;
using Api.Gateway.WebClient.Proxy.Transporte.ServiciosContrato.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Gateway.WebClient.Proxy.Transporte.ServiciosContrato.Commands;

namespace Clients.WebClient.Config.Transporte
{
    public static class StartUpConfiguration
    {
        public static IServiceCollection AddAppsettingBinding(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            return service;
        }

        public static IServiceCollection AddProxiesTransporteQueries(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<IQContratoTransporteProxy, QContratoTransporteProxy>();
            service.AddHttpClient<IQSContratoTransporteProxy, QSContratoTransporteProxy>();
            service.AddHttpClient<IQConvenioTransporteProxy, QConvenioTransporteProxy>();
            service.AddHttpClient<IQEContratoTransporteProxy, QEContratoTransporteProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesTransporteCommands(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<ICContratoTransporteProxy, CContratoTransporteProxy>();
            service.AddHttpClient<ICSContratoTransporteProxy, CSContratoTransporteProxy>();
            service.AddHttpClient<ICConvenioTransporteProxy, CConvenioTransporteProxy>();
            service.AddHttpClient<ICEContratoTransporteProxy, CEContratoTransporteProxy>();

            return service;
        }
    }
}
