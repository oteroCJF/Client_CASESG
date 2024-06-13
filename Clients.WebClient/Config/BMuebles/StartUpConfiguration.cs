using Api.Gateway.WebClient.Proxy.Mensajeria.CedulasEvaluacion.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.CedulasEvaluacion.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.CFDIs.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Convenios.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Entregables.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Entregables.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.EntregablesContrato.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Firmantes.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Firmantes.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Flujo.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.LogCedulas.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.LogCedulas.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.LogEntregables.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.LogEntregables.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Repositorios.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Repositorios.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.ServiciosContrato.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Gateway.WebClient.Proxy.Mensajeria.Respuestas.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Incidencias.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Incidencias.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.ServiciosContrato.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.SoportePago.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.SoportePago.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.EntregablesContrato.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.Solicitudes.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.Convenios.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.Solicitudes.Commands;

namespace Clients.WebClient.Config.BMuebles
{
    public static class StartUpConfiguration
    {
        public static IServiceCollection AddAppsettingBinding(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            return service;
        }

        public static IServiceCollection AddProxiesBMueblesQueries(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<IQBMContratoProxy, QBMContratoProxy>();
            //service.AddHttpClient<IFServicioContratoProxy, FServicioContratoProxy>();
            service.AddHttpClient<IQBMConvenioProxy, QBMConvenioProxy>();
            service.AddHttpClient<IQBMEContratoProxy, QBMEContratoProxy>();
            service.AddHttpClient<IQBMSolicitudProxy, QBMSolicitudProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesBMueblesCommands(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<ICBMContratoProxy, CBMContratoProxy>();
            //service.AddHttpClient<IFServicioContratoProxy, FServicioContratoProxy>();
            service.AddHttpClient<ICBMConvenioProxy, CBMConvenioProxy>();
            service.AddHttpClient<ICBMEContratoProxy, CBMEContratoProxy>();
            service.AddHttpClient<ICBMSolicitudProxy, CBMSolicitudProxy>();
            return service;
        }
    }
}
