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
using Api.Gateway.WebClient.Proxy.Mensajeria.ServiciosContrato;
using Api.Gateway.WebClient.Proxy.Mensajeria.Variables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Gateway.WebClient.Proxy.Mensajeria.Respuestas.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Incidencias.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Incidencias.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.ServiciosContrato.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.SoportePago.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.SoportePago.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Oficios.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Oficios.Commands;

namespace Clients.WebClient.Config.Mensajeria
{
    public static class StartUpConfiguration
    {
        public static IServiceCollection AddAppsettingBinding(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            return service;
        }

        public static IServiceCollection AddProxiesMensajeriaQueries(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<IQRepositorioMensajeriaProxy, QRepositorioMensajeriaProxy>();
            service.AddHttpClient<IQCFDIMensajeriaProxy, QCFDIMensajeriaProxy>();
            service.AddHttpClient<IQCedulaMensajeriaProxy, QCedulaMensajeriaProxy>();
            service.AddHttpClient<ICRespuestaMensajeriaProxy, CRespuestaMensajeriaProxy>();
            service.AddHttpClient<IQIncidenciaMensajeriaProxy, QIncidenciaMensajeriaProxy>();
            service.AddHttpClient<IQEntregableMensajeriaProxy, QEntregableMensajeriaProxy>();
            service.AddHttpClient<IQContratoMensajeriaProxy, QContratoMensajeriaProxy>();
            service.AddHttpClient<IQSContratoMensajeriaProxy, QSContratoMensajeriaProxy>();
            service.AddHttpClient<IQConvenioMensajeriaProxy, QConvenioMensajeriaProxy>();
            service.AddHttpClient<IQEContratoMensajeriaProxy, QEContratoMensajeriaProxy>();
            service.AddHttpClient<IQFlujoMensajeriaProxy, QFlujoMensajeriaProxy>();
            service.AddHttpClient<IQLCedulaMensajeriaProxy, QLCedulaMensajeriaProxy>();
            service.AddHttpClient<IQLEntregableMensajeriaProxy, QLEntregableMensajeriaProxy>();
            service.AddHttpClient<IQFirmanteMensajeriaProxy, QFirmanteMensajeriaProxy>();
            service.AddHttpClient<IQSoportePagoMensajeriaProxy, QSoportePagoMensajeriaProxy>();
            service.AddHttpClient<IQOficioMensajeriaProxy , QOficioMensajeriaProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesMensajeriaCommands(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<ICFirmanteMensajeriaProxy, CFirmanteMensajeriaProxy>();
            service.AddHttpClient<ICCedulaMensajeriaProxy, CCedulaMensajeriaProxy>();
            service.AddHttpClient<ICRespuestaMensajeriaProxy, CRespuestaMensajeriaProxy>();
            service.AddHttpClient<ICRepositorioMensajeriaProxy, CRepositorioMensajeriaProxy>();
            service.AddHttpClient<ICCFDIMensajeriaProxy, CCFDIMensajeriaProxy>();
            service.AddHttpClient<ICIncidenciaMensajeriaProxy, CIncidenciaMensajeriaProxy>();
            service.AddHttpClient<ICContratoMensajeriaProxy, CContratoMensajeriaProxy>();
            service.AddHttpClient<ICSContratoMensajeriaProxy, CSContratoMensajeriaProxy>();
            service.AddHttpClient<ICConvenioMensajeriaProxy, CConvenioMensajeriaProxy>();
            service.AddHttpClient<ICEntregableMensajeriaProxy, CEntregableMensajeriaProxy>();
            service.AddHttpClient<ICEntregableMensajeriaProxy, CEntregableMensajeriaProxy>();
            service.AddHttpClient<ICEContratoMensajeriaProxy, CEContratoMensajeriaProxy>();
            service.AddHttpClient<ICLCedulaMensajeriaProxy, CLCedulaMensajeriaProxy>();
            service.AddHttpClient<ICLEntregableMensajeriaProxy, CLEntregableMensajeriaProxy>();
            service.AddHttpClient<ICSoportePagoMensajeriaProxy, CSoportePagoMensajeriaProxy>();
            service.AddHttpClient<ICOficioMensajeriaProxy, COficioMensajeriaProxy>();
            return service;
        }
    }
}
