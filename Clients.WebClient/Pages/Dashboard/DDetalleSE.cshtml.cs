using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Dashboard.Cedulas;
using Api.Gateway.Models.Estatus.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Dashboards;
using Api.Gateway.WebClient.Proxy.Estatus;
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

namespace Clients.WebClient.Pages.Dashboard
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DDetalleSE : PageModel
    {
        private readonly ICTServicioProxy _servicios;
        private readonly IEstatusCedulaProxy _estatusc;
        private readonly IDashboardMesProxy _dashboard;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IMesProxy _meses;
        private readonly IModuloProxy _modulos;
        private readonly IPermisoProxy _permisos;

        public List<CedulaDto> Detalle { get; set; } = new List<CedulaDto>();
        public CTServicioDto Servicio { get; set; } = new CTServicioDto();
        public EstatusDto Estatus { get; set; } = new EstatusDto();
        public List<MesDto> Mes { get; set; } = new List<MesDto>();

        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }
        public int EstatusId { get; set; }
        public int ServicioId  { get; set; }

        public DDetalleSE(ICTServicioProxy servicios, IDashboardMesProxy dashboard, IInmuebleProxy inmuebles, IMesProxy meses,
                          IModuloProxy modulos, IPermisoProxy permisos, IEstatusCedulaProxy estatusc)
        {
            _servicios = servicios;
            _estatusc = estatusc;
            _dashboard = dashboard;
            _inmuebles = inmuebles;
            _modulos = modulos;
            _permisos = permisos;
            _meses = meses;
        }

        public async Task OnGet(int servicio, int anio, int estatus)
        {
            string usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Servicio = await _servicios.GetServicioByIdAsync(servicio);
            Estatus = await _estatusc.GetECByIdAsync(estatus);
            Anio = Anio == 0 ? DateTime.Now.Year : Anio;
            Detalle = await _dashboard.GetDDetalleServicios(Anio, usuario, estatus, Servicio);
        }
    }
}
