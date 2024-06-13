using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Gateway.WebClient.Proxy.Comedor.Repositorios.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Repsuestas.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.CedulasEvaluacion.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Entregables.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.ServiciosContrato.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.EntregablesContrato.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Flujo.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.LogCedulas.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Firmantes.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.LogEntregables.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.CedulasEvaluacion.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Repositorios.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.CFDIs.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.ServiciosContrato;
using Api.Gateway.WebClient.Proxy.Comedor.Convenios.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Entregables.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.LogCedulas.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.LogEntregables.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Firmantes.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.ServiciosContrato.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Repsuestas.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Incidencias.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Incidencias.Queries;

namespace Clients.WebClient.Config.Comedor
{
    public static class StartUpConfiguration
    {
        public static IServiceCollection AddAppsettingBinding(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            return service;
        }
        
        public static IServiceCollection AddProxiesComedorQueries(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<IQRepositorioComedorProxy, QRepositorioComedorProxy>();
            service.AddHttpClient<IQCFDIComedorProxy, QCFDIComedorProxy>();
            service.AddHttpClient<IQCedulaComedorProxy, QCedulaComedorProxy>();
            service.AddHttpClient<IQRespuestaComedorProxy, QRespuestaComedorProxy>();
            service.AddHttpClient<ICIncidenciaComedorProxy, CIncidenciaComedorProxy>();
            service.AddHttpClient<IQEntregableComedorProxy, QEntregableComedorProxy>();
            service.AddHttpClient<IQContratoComedorProxy, QContratoComedorProxy>();
            service.AddHttpClient<IQSContratoComedorProxy, QSContratoComedorProxy>();
            service.AddHttpClient<IQConvenioComedorProxy, QConvenioComedorProxy>();
            service.AddHttpClient<IQEContratoComedorProxy, QEContratoComedorProxy>();
            service.AddHttpClient<IQFlujoComedorProxy, QFlujoComedorProxy>();
            service.AddHttpClient<IQLCedulaComedorProxy, QLCedulaComedorProxy>();
            service.AddHttpClient<IQLEntregableComedorProxy, QLEntregableComedorProxy>();
            service.AddHttpClient<IQFirmanteComedorProxy, QFirmanteComedorProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesComedorCommands(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<ICFirmanteComedorProxy, CFirmanteComedorProxy>();
            service.AddHttpClient<ICCedulaComedorProxy, CCedulaComedorProxy>();
            service.AddHttpClient<ICRespuestaComedorProxy, CRespuestaComedorProxy>();
            service.AddHttpClient<ICRepositorioComedorProxy, CRepositorioComedorProxy>();
            service.AddHttpClient<ICCFDIComedorProxy, CCFDIComedorProxy>();
            service.AddHttpClient<ICContratoComedorProxy, CContratoComedorProxy>();
            service.AddHttpClient<ICSContratoComedorProxy, CSContratoComedorProxy>();
            service.AddHttpClient<ICConvenioComedorProxy, CConvenioComedorProxy>();
            service.AddHttpClient<ICEntregableComedorProxy, CEntregableComedorProxy>();
            service.AddHttpClient<ICEntregableComedorProxy, CEntregableComedorProxy>();
            service.AddHttpClient<ICEContratoComedorProxy, CEContratoComedorProxy>();
            service.AddHttpClient<ICLCedulaComedorProxy, CLCedulaComedorProxy>();
            service.AddHttpClient<IQIncidenciaComedorProxy, QIncidenciaComedorProxy>();
            service.AddHttpClient<ICLEntregableComedorProxy, CLEntregableComedorProxy>();

            return service;
        }

    }
}
