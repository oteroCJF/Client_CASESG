using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Firmantes.Commands;
using Api.Gateway.Models.Firmantes.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.Models.Usuarios.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Mensajeria.Firmantes.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Firmantes.Queries;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Api.Gateway.WebClient.Proxy.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Mensajeria.Firmantes
{
    public class IndexModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly ICTServicioProxy _servicios;
        private readonly IQFirmanteMensajeriaProxy _firmantesQuery;
        private readonly ICFirmanteMensajeriaProxy _firmantesCommand;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IUsuarioProxy _usuarios;
        private readonly IPermisoProxy _permisos;

        public ModuloDto Modulo { get; set; }
        public SubmoduloDto Submodulo { get; set; }
        public CTServicioDto Servicio { get; set; }
        public List<FirmanteDto> Firmantes { get; set; } = new List<FirmanteDto>();
        public List<PermisoUsuarioDto> Permisos { get; set; } = new List<PermisoUsuarioDto>();
        public List<MesDto> Meses { get; set; } = new List<MesDto>();
        public List<int> InmueblesUsuarios { get; set; } = new List<int>();
        public List<InmuebleDto> Inmuebles { get; set; } = new List<InmuebleDto>();
        public List<UsuarioDto> Usuarios { get; set; } = new List<UsuarioDto>();

        public IndexModel(IModuloProxy modulo, ICTServicioProxy servicios, IQFirmanteMensajeriaProxy firmantesQuery, ICFirmanteMensajeriaProxy firmantesCommand, 
                          IPermisoProxy permisos, IInmuebleProxy inmuebles, IUsuarioProxy usuarios)
        {
            _modulo = modulo;
            _servicios = servicios;
            _firmantesQuery = firmantesQuery;
            _firmantesCommand = firmantesCommand;
            _permisos = permisos;
            _inmuebles = inmuebles;
            _usuarios = usuarios;
        }

        public async Task OnGet(int moduloId, int submoduloId)
        {
            string usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Permisos = await _permisos.GetPermisosByModuloUsuario(usuario, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                InmueblesUsuarios = (await _inmuebles.GetInmueblesByUsuarioServicio(usuario, (int)Modulo.ServicioId)).Select(ius => ius.InmuebleId).ToList();
                Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesUsuarios.Contains(i.Id)).ToList();
                Usuarios = await _usuarios.GetUsuariosByServicio(usuario, Servicio.Id);
                Firmantes = (await _firmantesQuery.GetAllFirmantesAsync()).Where(f => InmueblesUsuarios.Contains(f.InmuebleId)).ToList();
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        public async Task<JsonResult> OnPutUpdateFirmante([FromBody] FirmanteUpdateCommand firmante)
        {
            var firmantes = await _firmantesCommand.UpdateFirmantes(firmante);
            return new JsonResult(firmantes);
        }
    }
}
