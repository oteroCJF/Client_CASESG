using Api.Gateway.WebClient.Proxy.Agua.CedulasEvaluacion.Commands;
using Api.Gateway.WebClient.Proxy.Agua.CedulasEvaluacion.Queries;
using Api.Gateway.WebClient.Proxy.Agua.CFDIs.Commands;
using Api.Gateway.WebClient.Proxy.Agua.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.Agua.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Convenios.Commands;
using Api.Gateway.WebClient.Proxy.Agua.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Entregables.Commands;
using Api.Gateway.WebClient.Proxy.Agua.Entregables.Queries;
using Api.Gateway.WebClient.Proxy.Agua.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.Agua.EntregablesContrato.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Firmantes.Commands;
using Api.Gateway.WebClient.Proxy.Agua.Firmantes.Queries;
using Api.Gateway.WebClient.Proxy.Agua.LogCedulas.Commands;
using Api.Gateway.WebClient.Proxy.Agua.LogCedulas.Queries;
using Api.Gateway.WebClient.Proxy.Agua.LogEntregables.Commands;
using Api.Gateway.WebClient.Proxy.Agua.LogEntregables.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Repositorios.Commands;
using Api.Gateway.WebClient.Proxy.Agua.Repositorios.Queries;
using Api.Gateway.WebClient.Proxy.Agua.ServiciosContrato.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Gateway.WebClient.Proxy.Agua.Respuestas.Commands;
using Api.Gateway.WebClient.Proxy.Agua.Incidencias.Commands;
using Api.Gateway.WebClient.Proxy.Agua.Incidencias.Queries;
using Api.Gateway.WebClient.Proxy.Agua.ServiciosContrato.Commands;
using Api.Gateway.WebClient.Proxy.Agua.Oficios.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Oficios.Commands;

namespace Clients.WebClient.Config.Agua
{
    public static class StartUpConfiguration
    {
        public static IServiceCollection AddAppsettingBinding(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            return service;
        }

        public static IServiceCollection AddProxiesAguaQueries(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<IQRepositorioAguaProxy, QRepositorioAguaProxy>();
            service.AddHttpClient<IQCFDIAguaProxy, QCFDIAguaProxy>();
            service.AddHttpClient<IQCedulaAguaProxy, QCedulaAguaProxy>();
            service.AddHttpClient<ICRespuestaAguaProxy, CRespuestaAguaProxy>();
            service.AddHttpClient<IQIncidenciaAguaProxy, QIncidenciaAguaProxy>();
            service.AddHttpClient<IQEntregableAguaProxy, QEntregableAguaProxy>();
            service.AddHttpClient<IQContratoAguaProxy, QContratoAguaProxy>();
            service.AddHttpClient<IQSContratoAguaProxy, QSContratoAguaProxy>();
            service.AddHttpClient<IQConvenioAguaProxy, QConvenioAguaProxy>();
            service.AddHttpClient<IQEContratoAguaProxy, QEContratoAguaProxy>();
            service.AddHttpClient<IQLCedulaAguaProxy, QLCedulaAguaProxy>();
            service.AddHttpClient<IQLEntregableAguaProxy, QLEntregableAguaProxy>();
            service.AddHttpClient<IQFirmanteAguaProxy, QFirmanteAguaProxy>();
            service.AddHttpClient<IQOficioAguaProxy , QOficioAguaProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesAguaCommands(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<ICFirmanteAguaProxy, CFirmanteAguaProxy>();
            service.AddHttpClient<ICCedulaAguaProxy, CCedulaAguaProxy>();
            service.AddHttpClient<ICRespuestaAguaProxy, CRespuestaAguaProxy>();
            service.AddHttpClient<ICRepositorioAguaProxy, CRepositorioAguaProxy>();
            service.AddHttpClient<ICCFDIAguaProxy, CCFDIAguaProxy>();
            service.AddHttpClient<ICIncidenciaAguaProxy, CIncidenciaAguaProxy>();
            service.AddHttpClient<ICContratoAguaProxy, CContratoAguaProxy>();
            service.AddHttpClient<ICSContratoAguaProxy, CSContratoAguaProxy>();
            service.AddHttpClient<ICConvenioAguaProxy, CConvenioAguaProxy>();
            service.AddHttpClient<ICEntregableAguaProxy, CEntregableAguaProxy>();
            service.AddHttpClient<ICEntregableAguaProxy, CEntregableAguaProxy>();
            service.AddHttpClient<ICEContratoAguaProxy, CEContratoAguaProxy>();
            service.AddHttpClient<ICLCedulaAguaProxy, CLCedulaAguaProxy>();
            service.AddHttpClient<ICLEntregableAguaProxy, CLEntregableAguaProxy>();
            service.AddHttpClient<ICOficioAguaProxy, COficioAguaProxy>();
            return service;
        }
    }
}
