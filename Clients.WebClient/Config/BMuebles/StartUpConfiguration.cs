using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using Api.Gateway.WebClient.Proxy.BMuebles.Repositorios.Queries;
//using Api.Gateway.WebClient.Proxy.BMuebles.Repsuestas.Commands;
//using Api.Gateway.WebClient.Proxy.BMuebles.CedulasEvaluacion.Queries;
//using Api.Gateway.WebClient.Proxy.BMuebles.CFDIs.Queries;
//using Api.Gateway.WebClient.Proxy.BMuebles.Entregables.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.Contratos.Queries;
//using Api.Gateway.WebClient.Proxy.BMuebles.ServiciosContrato.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.EntregablesContrato.Queries;
//using Api.Gateway.WebClient.Proxy.BMuebles.Flujo.Queries;
//using Api.Gateway.WebClient.Proxy.BMuebles.LogCedulas.Queries;
//using Api.Gateway.WebClient.Proxy.BMuebles.LogEntregables.Queries;
//using Api.Gateway.WebClient.Proxy.BMuebles.CedulasEvaluacion.Commands;
//using Api.Gateway.WebClient.Proxy.BMuebles.Repositorios.Commands;
//using Api.Gateway.WebClient.Proxy.BMuebles.CFDIs.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.Contratos.Commands;
//using Api.Gateway.WebClient.Proxy.BMuebles.ServiciosContrato;
using Api.Gateway.WebClient.Proxy.BMuebles.Convenios.Commands;
//using Api.Gateway.WebClient.Proxy.BMuebles.Entregables.Commands;
//using Api.Gateway.WebClient.Proxy.BMuebles.LogCedulas.Commands;
//using Api.Gateway.WebClient.Proxy.BMuebles.LogEntregables.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.Firmantes.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.Firmantes.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.Solicitudes.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.Solicitudes.Commands;
using Api.Gateway.WebClient.Proxy.Fumigacion.ServiciosContrato;
//using Api.Gateway.WebClient.Proxy.BMuebles.ServiciosContrato.Commands;
//using Api.Gateway.WebClient.Proxy.BMuebles.Repsuestas.Queries;
//using Api.Gateway.WebClient.Proxy.BMuebles.Incidencias.Commands;
//using Api.Gateway.WebClient.Proxy.BMuebles.Incidencias.Queries;
//using Api.Gateway.WebClient.Proxy.BMuebles.Oficios.Commands;
//using Api.Gateway.WebClient.Proxy.BMuebles.Oficios.Queries;

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
            service.AddHttpClient<IFServicioContratoProxy, FServicioContratoProxy>();
            service.AddHttpClient<IQBMConvenioProxy, QBMConvenioProxy>();
            service.AddHttpClient<IQBMEContratoProxy, QBMEContratoProxy>();
            service.AddHttpClient<IQBMSolicitudProxy, QBMSolicitudProxy>();

            service.AddHttpClient<IQFirmanteBMueblesProxy, QFirmanteBMueblesProxy>();


            return service;
        }

        public static IServiceCollection AddProxiesBMueblesCommands(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<ICFirmanteBMueblesProxy, CFirmanteBMueblesProxy>();

            service.AddHttpClient<ICBMContratoProxy, CBMContratoProxy>();
            service.AddHttpClient<IFServicioContratoProxy, FServicioContratoProxy>();
            service.AddHttpClient<ICBMConvenioProxy, CBMConvenioProxy>();
            service.AddHttpClient<ICBMEContratoProxy, CBMEContratoProxy>();
            service.AddHttpClient<ICBMSolicitudProxy, CBMSolicitudProxy>();
            return service;
        }
    }
}
