using Api.Gateway.WebClient.Proxy.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Clients.Services;
using Clients.WebClient.Config;
using Clients.WebClient.Config.Comedor;
using Clients.WebClient.Config.Mensajeria;
using Clients.WebClient.Config.BMuebles;
using Clients.WebClient.Config.Microbiologicos;
using Clients.WebClient.Config.Transporte;
using Clients.WebClient.Config.Celular;
using Clients.WebClient.Config.Convencional;
using Clients.WebClient.Config.Agua;

namespace Client.WebClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Debug Compilation
            services.AddRazorPages().AddRazorRuntimeCompilation();
            // Proxies
            services.AddSingleton(new ApiGatewayUrl(Configuration.GetValue<string>("ApiGatewayUrl")));
            services.AddHttpContextAccessor();

            services.AddProxiesDashboards(Configuration);
            services.AddProxiesServices(Configuration);
            services.AddProxiesCatalogos(Configuration);
            services.AddProxiesEstatus(Configuration);
            services.AddProxiesAEElectrica(Configuration);
            services.AddProxiesLimpieza(Configuration);
            services.AddProxiesMensajeriaCommands(Configuration);
            services.AddProxiesMensajeriaQueries(Configuration);
            services.AddProxiesFumigacion(Configuration);
            
            services.AddProxiesBMueblesQueries(Configuration);
            services.AddProxiesBMueblesCommands(Configuration);

            services.AddProxiesComedorQueries(Configuration);
            services.AddProxiesComedorCommands(Configuration);

            services.AddProxiesTransporteQueries(Configuration);
            services.AddProxiesTransporteCommands(Configuration);

            services.AddProxiesMicrobiologicosQueries(Configuration);
            services.AddProxiesMicrobiologicosCommands(Configuration);
            
            services.AddProxiesConvencionalQueries(Configuration);
            services.AddProxiesConvencionalCommands(Configuration);
            
            services.AddProxiesCelularQueries(Configuration);
            services.AddProxiesCelularCommands(Configuration);
            
            services.AddProxiesAguaQueries(Configuration);
            services.AddProxiesAguaCommands(Configuration);

            // Razor Pages & MVC
            services.AddRazorPages(o => o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));
            services.AddControllers();

            // Add Cookie Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie();

            services.AddTransient<PermisosServicios>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
