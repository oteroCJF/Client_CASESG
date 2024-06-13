using Api.Gateway.WebClient.Proxy.Microbiologicos.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.Microbiologicos.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Microbiologicos.Convenios.Commands;
using Api.Gateway.WebClient.Proxy.Microbiologicos.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.Microbiologicos.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.Microbiologicos.EntregablesContrato.Queries;
using Api.Gateway.WebClient.Proxy.Microbiologicos.ServiciosContrato.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Gateway.WebClient.Proxy.Microbiologicos.ServiciosContrato.Commands;

namespace Clients.WebClient.Config.Microbiologicos
{
    public static class StartUpConfiguration
    {
        public static IServiceCollection AddAppsettingBinding(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            return service;
        }

        public static IServiceCollection AddProxiesMicrobiologicosQueries(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<IQContratoMicrobiologicosProxy, QContratoMicrobiologicosProxy>();
            service.AddHttpClient<IQSContratoMicrobiologicosProxy, QSContratoMicrobiologicosProxy>();
            service.AddHttpClient<IQConvenioMicrobiologicosProxy, QConvenioMicrobiologicosProxy>();
            service.AddHttpClient<IQEContratoMicrobiologicosProxy, QEContratoMicrobiologicosProxy>();

            return service;
        }

        public static IServiceCollection AddProxiesMicrobiologicosCommands(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            service.AddHttpClient<ICContratoMicrobiologicosProxy, CContratoMicrobiologicosProxy>();
            service.AddHttpClient<ICSContratoMicrobiologicosProxy, CSContratoMicrobiologicosProxy>();
            service.AddHttpClient<ICConvenioMicrobiologicosProxy, CConvenioMicrobiologicosProxy>();
            service.AddHttpClient<ICEContratoMicrobiologicosProxy, CEContratoMicrobiologicosProxy>();

            return service;
        }
    }
}
