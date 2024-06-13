using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs.Limpieza;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Limpieza.CedulaEvaluacion;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Limpieza.CedulasEvaluacion
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class IndexModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly ICTServicioProxy _servicios;
        private readonly IInmuebleProxy _inmuebles;
        private readonly ILCedulaProxy _cedula;
        private readonly IPermisoProxy _permisos;

        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }
        public ModuloDto Modulo { get; set; }
        public SubmoduloDto Submodulo { get; set; }
        public List<int> InmueblesUsuarios { get; set; } = new List<int>();
        public List<InmuebleDto> Inmuebles { get; set; }
        public CTServicioDto Servicio { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }
        public List<CedulaLimpiezaDto> Cedulas { get; set; }


        public IndexModel(IModuloProxy modulo, IInmuebleProxy inmuebles, ICTServicioProxy servicios,
                          IPermisoProxy permisos, ILCedulaProxy cedula)
        {
            _modulo = modulo;
            _inmuebles = inmuebles;
            _servicios = servicios;
            _permisos = permisos;
            _cedula = cedula;

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
                Cedulas = Anio != 0 ? await _cedula.GetCedulaByAnioAsync((int)Modulo.ServicioId, Anio, Usuario) : new List<CedulaLimpiezaDto>();
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }
    }
}
