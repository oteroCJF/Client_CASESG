using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Dashboard.Cedulas;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Dashboards;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class IndexModel : PageModel
    {
        private readonly ICTServicioProxy _servicios;
        private readonly IDashboardProxy _dashboard;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IMesProxy _meses;
        private readonly IModuloProxy _modulos;
        private readonly IPermisoProxy _permisos;

        public List<DashboardDto> Dashboard { get; set; } = new List<DashboardDto>();
        public List<CTServicioDto> Servicios { get; set; } = new List<CTServicioDto>();
        public List<InmuebleDto> Inmuebles { get; set; } = new List<InmuebleDto>();
        public List<MesDto> Meses { get; set; } = new List<MesDto>();
        public List<int> Permisos { get; set; } = new List<int>();
        public List<int?> Modulos { get; set; } = new List<int?>();

        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }

        public IndexModel(ICTServicioProxy servicios, IDashboardProxy dashboard, IInmuebleProxy inmuebles, IMesProxy meses,
                          IModuloProxy modulos, IPermisoProxy permisos)
        {
            _servicios = servicios;
            _dashboard = dashboard;
            _inmuebles = inmuebles;
            _modulos = modulos;
            _permisos = permisos;
            _meses = meses;
        }

        public async Task OnGet()
        {
            string usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Permisos = (await _permisos.GetPermisosByUsuario(usuario)).Select(p => p.ModuloId).ToList();
            Modulos = (await _modulos.GetAllModulosAsync()).Where(m => Permisos.Contains(m.Id)).Select(m => m.ServicioId).ToList();
            Servicios = (await _servicios.GetAllCatalogoServiciosAsync()).Where(s => Modulos.Contains(s.Id)).ToList();
            Anio = Anio == 0 ? DateTime.Now.Year : Anio;
            Dashboard = await _dashboard.GetDashboardsServicios(Anio, usuario, Servicios);
        }
    }
}
