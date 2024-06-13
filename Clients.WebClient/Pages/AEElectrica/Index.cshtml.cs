using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.Models.SolicitudesPago.Commands;
using Api.Gateway.Models.SolicitudesPago.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.SolicitudesPago;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.AEElectrica
{
    public class IndexModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly IInmuebleProxy _inmuebles;
        private readonly ICTServicioProxy _servicios;
        private readonly IPermisoProxy _permisos;
        private readonly IMesProxy _mes;
        private readonly IAEESolicitudPagoProxy _solicitudes;

        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }
        public ModuloDto Modulo { get; set; } = new ModuloDto();
        public SubmoduloDto Submodulo { get; set; } = new SubmoduloDto();
        public CTServicioDto Servicio { get; set; } = new CTServicioDto();
        public List<SolicitudPagoDto> SolicitudesPago { get; set; } = new List<SolicitudPagoDto>();
        public List<int> InmueblesUsuarios { get; set; } = new List<int>();
        public List<MesDto> Meses { get; set; } = new List<MesDto>();
        public List<InmuebleDto> Inmuebles { get; set; } = new List<InmuebleDto>();
        public List<PermisoUsuarioDto> Permisos { get; set; } = new List<PermisoUsuarioDto>();

        public IndexModel(IModuloProxy modulo, ICTServicioProxy servicios, IPermisoProxy permisos, IAEESolicitudPagoProxy solicitudes, 
                          IInmuebleProxy inmuebles, IMesProxy mes)
        {
            _modulo = modulo;
            _mes = mes;
            _servicios = servicios;
            _permisos = permisos;
            _inmuebles = inmuebles;
            _solicitudes = solicitudes;
        }

        public async Task OnGet(int moduloId, int submoduloId)
        {
            Permisos = await _permisos.GetPermisosByModuloUsuario(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Meses = await _mes.GetAllAsync();
                InmueblesUsuarios = (await _inmuebles.GetInmueblesByUsuarioServicio(User.FindFirst(ClaimTypes.NameIdentifier).Value, (int)Modulo.ServicioId)).Select(ius => ius.InmuebleId).ToList();
                Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesUsuarios.Contains(i.Id)).ToList();
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                SolicitudesPago = Anio != 0 ? await _solicitudes.GetSolicitudesPagoByAnio(Anio):new List<SolicitudPagoDto>();
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        public async Task<JsonResult> OnPostCreateRepositorio([FromBody] SolicitudPagoCreateCommand solicitud)
        {
            solicitud.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int status = await _solicitudes.CreateSolicitud(solicitud);
            return new JsonResult(status);
        }
    }
}
