using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Gateway.Models.BMuebles.Solicitudes.DTOs;
using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.BMuebles.Solicitudes;
using Api.Gateway.WebClient.Proxy.BMuebles.Solicitudes.Queries;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clients.WebClient.Pages.ServiciosGenerales.BMuebles.Solicitudes
{
    public class IndexModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly ICTServicioProxy _servicios;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IQBMSolicitudProxy _solicitudes;
        private readonly IPermisoProxy _permisos;

        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }
        public ModuloDto Modulo { get; set; } = new ModuloDto();
        public SubmoduloDto Submodulo { get; set; } = new SubmoduloDto();
        public List<int> InmueblesUsuarios { get; set; } = new List<int>();
        public List<InmuebleDto> Inmuebles { get; set; } = new List<InmuebleDto>();
        public CTServicioDto Servicio { get; set; } = new CTServicioDto();
        public List<PermisoUsuarioDto> Permisos { get; set; } = new List<PermisoUsuarioDto>();
        public List<SolicitudDto> Solicitudes { get; set; } = new List<SolicitudDto>();

        public IndexModel(IModuloProxy modulo, ICTServicioProxy servicios, IInmuebleProxy inmuebles, IQBMSolicitudProxy solicitudes, IPermisoProxy permisos)
        {
            _modulo = modulo;
            _servicios = servicios;
            _inmuebles = inmuebles;
            _solicitudes = solicitudes;
            _permisos = permisos;
        }

        public async Task OnGet(int moduloId, int submoduloId)
        {
            string Usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Permisos = await _permisos.GetPermisosByModuloUsuario(Usuario, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                InmueblesUsuarios = (await _inmuebles.GetInmueblesByUsuarioServicio(Usuario, (int)Modulo.ServicioId)).Select(ius => ius.InmuebleId).ToList();
                Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesUsuarios.Contains(i.Id)).ToList();
                Solicitudes = Anio != 0 ? await _solicitudes.GetAllSolicitudes() : new List<SolicitudDto>();
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }
    }
}
