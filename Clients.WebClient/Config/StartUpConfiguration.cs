using Api.Gateway.WebClient.Proxy.Catalogos.CTEntregables;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Limpieza.CedulaEvaluacion;
using Api.Gateway.WebClient.Proxy.Limpieza.Contratos;
using Api.Gateway.WebClient.Proxy.Limpieza.Convenios;
using Api.Gateway.WebClient.Proxy.Limpieza.Entregables;
using Api.Gateway.WebClient.Proxy.Limpieza.entregablesContrato;
using Api.Gateway.WebClient.Proxy.Limpieza.Facturas;
using Api.Gateway.WebClient.Proxy.Limpieza.Firmantes;
using Api.Gateway.WebClient.Proxy.Limpieza.Flujo;
using Api.Gateway.WebClient.Proxy.Limpieza.Historiales;
using Api.Gateway.WebClient.Proxy.Limpieza.Incidencias;
using Api.Gateway.WebClient.Proxy.Limpieza.ServiciosContrato;
using Api.Gateway.WebClient.Proxy.Limpieza.Variables;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.Entregables;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.CFDIs;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.SolicitudesPago;
using Api.Gateway.WebClient.Proxy.Usuarios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.LogSolicitudes;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.LogEntregables;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServiciosContratos;
using Api.Gateway.WebClient.Proxy.Catalogos.CTParametros;
using Api.Gateway.WebClient.Proxy.Catalogos.CTIncidencias;
using Api.Gateway.WebClient.Proxy.Dashboards;
using Api.Gateway.WebClient.Proxy.Fumigacion.Facturas;
using Api.Gateway.WebClient.Proxy.Fumigacion.CedulasEvaluacion;
using Api.Gateway.WebClient.Proxy.Fumigacion.CedulaEvaluacion;
using Api.Gateway.WebClient.Proxy.Fumigacion.Contratos;
using Api.Gateway.WebClient.Proxy.Fumigacion.ServiciosContrato;
using Api.Gateway.WebClient.Proxy.Fumigacion.Convenios;
using Api.Gateway.WebClient.Proxy.Fumigacion.entregablesContrato;
using Api.Gateway.WebClient.Proxy.Fumigacion.Firmantes;
using Api.Gateway.WebClient.Proxy.Fumigacion.Historiales;
using Api.Gateway.WebClient.Proxy.Fumigacion.Flujo;
using Api.Gateway.WebClient.Proxy.Fumigacion.Variables;
using Api.Gateway.WebClient.Proxy.Fumigacion.Entregables;
using Api.Gateway.WebClient.Proxy.Fumigacion.Incidencias;
using Api.Gateway.WebClient.Proxy.Catalogos.CTIndemnizaciones;
using Api.Gateway.WebClient.Proxy.Fumigacion.Oficios;
using Api.Gateway.WebClient.Proxy.Inmuebles.Direcciones;
using Api.Gateway.WebClient.Proxy.Limpieza.Oficios;
using Api.Gateway.WebClient.Proxy.Catalogos.CTActividadesContrato;
using Api.Gateway.WebClient.Proxy.Catalogos.CTDestino;
using Api.Gateway.WebClient.Proxy.Limpieza.Repositorios;
using Api.Gateway.WebClient.Proxy.Fumigacion.Repositorios;
using Api.Gateway.WebClient.Proxy.Catalogos.CTDIasInhabiles;

namespace Clients.WebClient.Config
{
    public static class StartUpConfiguration
    {
        public static IServiceCollection AddAppsettingBinding(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            return service;
        }

        public static IServiceCollection AddProxiesDashboards(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            service.AddHttpClient<IDashboardProxy, DashboardProxy>();
            service.AddHttpClient<IDashboardMesProxy, DashboardMesProxy>();
            service.AddHttpClient<IDFinancierosProxy, DFinancierosProxy>();
            service.AddHttpClient<IFDetalleServicioProxy, FDetalleServicioProxy>();

            return service;
        }
        
        public static IServiceCollection AddProxiesCatalogos(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            service.AddHttpClient<ICTEntregableProxy, CTEntregableProxy>();
            service.AddHttpClient<ICTIncidenciaProxy, CTIncidenciaProxy>();
            service.AddHttpClient<ICTILimpiezaProxy, CTILimpiezaProxy>();
            service.AddHttpClient<ICTIComedorProxy, CTIComedorProxy>();
            service.AddHttpClient<ICTIAguaProxy, CTIAguaProxy>();
            service.AddHttpClient<ICTServicioProxy, CTServicioProxy>();
            service.AddHttpClient<ICTServicioContratoProxy, CTServicioContratoProxy>();
            service.AddHttpClient<ICTParametroProxy, CTParametroProxy>();
            service.AddHttpClient<ICTIndemnizacionProxy, CTIndemnizacionProxy>();
            service.AddHttpClient<ICTIFumigacionProxy, CTIFumigacionProxy>();
            service.AddHttpClient<ICTActividadContratoProxy, CTActividadContratoProxy>();
            service.AddHttpClient<ICTDestinoProxy, CTDestinoProxy>();
            service.AddHttpClient<ICTDiasInhabilesProxy, CTDiasInhabilesProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesEstatus(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            service.AddHttpClient<IEstatusCedulaProxy, EstatusCedulaProxy>();
            service.AddHttpClient<IEstatusFacturaProxy, EstatusFacturaProxy>();
            service.AddHttpClient<IEstatusEntregableProxy, EstatusEntregableProxy>();
            service.AddHttpClient<IEstatusOficioProxy, EstatusOficioProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesAEElectrica(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            service.AddHttpClient<IAEESolicitudPagoProxy, AEESolicitudPagoProxy>();
            service.AddHttpClient<IAEECFDIProxy, AEECFDIProxy>();
            service.AddHttpClient<IAEEEntregableProxy, AEEEntregableProxy>();
            service.AddHttpClient<IAEELogSolicitudProxy, AEELogSolicitudProxy>();
            service.AddHttpClient<IAEELogEntregableProxy, AEELogEntregableProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesServices(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<IInmuebleProxy, InmuebleProxy>();
            service.AddHttpClient<IDireccionProxy, DireccionProxy>();
            service.AddHttpClient<IMesProxy, MesProxy>();
            service.AddHttpClient<IUsuarioProxy, UsuarioProxy>();
            service.AddHttpClient<IPermisoProxy, PermisoProxy>();
            service.AddHttpClient<IModuloProxy, ModuloProxy>();

            return service;
        }
        
        public static IServiceCollection AddProxiesLimpieza(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            //Servicio de Limpieza
            service.AddHttpClient<ILRepositorioProxy, LRepositorioProxy>();
            service.AddHttpClient<ILCFDIProxy, LCFDIProxy>();
            service.AddHttpClient<ILCedulaProxy, LCedulaProxy>();
            service.AddHttpClient<ILRespuestaProxy, LRespuestaProxy>();
            service.AddHttpClient<ILIncidenciaProxy, LIncidenciaProxy>();
            service.AddHttpClient<ILEntregableProxy, LEntregableProxy>();
            service.AddHttpClient<ILFlujoProxy, LFlujoProxy>();
            service.AddHttpClient<ILLogCedulaProxy, LLCedulaProxy>();
            service.AddHttpClient<ILLogEntregableProxy, LLEntregableProxy>();
            service.AddHttpClient<ILFirmanteProxy, LFirmanteProxy>();
            service.AddHttpClient<ILContratoProxy, LContratoProxy>();
            service.AddHttpClient<ILServicioContratoProxy, LSContratoProxy>();
            service.AddHttpClient<ILConvenioProxy, LConvenioProxy>();
            service.AddHttpClient<ILEContratoProxy, LEContratoProxy>();
            service.AddHttpClient<ILOficioProxy, LOficioProxy>();
            service.AddHttpClient<ILParametroProxy, LParametroProxy>();

            return service;
        }
        
        public static IServiceCollection AddProxiesFumigacion(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<IFRepositorioProxy, FRepositorioProxy>();
            service.AddHttpClient<IFCFDIProxy, FCFDIProxy>();
            service.AddHttpClient<IFCedulaProxy, FCedulaProxy>();
            service.AddHttpClient<IFRespuestasProxy, FRespuestasProxy>();
            service.AddHttpClient<IFIncidenciaProxy, FIncidenciaProxy>();
            service.AddHttpClient<IFEntregableProxy, FEntregableProxy>();
            service.AddHttpClient<IFContratoProxy, FContratoProxy>();
            service.AddHttpClient<IFServicioContratoProxy, FServicioContratoProxy>();
            service.AddHttpClient<IFConvenioProxy, FConvenioProxy>();
            service.AddHttpClient<IFEContratoProxy, FEContratoProxy>();
            service.AddHttpClient<IFParametroProxy, FParametroProxy>();
            service.AddHttpClient<IFFlujoProxy, FFlujoProxy>();
            service.AddHttpClient<IFLCedulaProxy, FLCedulaProxy>();
            service.AddHttpClient<IFLEntregableProxy, FLEntregableProxy>();
            service.AddHttpClient<IFFirmanteProxy, FFirmanteProxy>();
            service.AddHttpClient<IFOficioProxy, FOficioProxy>();

            return service;
        }
    }
}
