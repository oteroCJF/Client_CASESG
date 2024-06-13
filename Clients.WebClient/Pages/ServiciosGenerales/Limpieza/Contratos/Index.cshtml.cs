using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Contratos.Commands;
using Api.Gateway.Models.Contratos.DTOs;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Limpieza.Contratos;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Limpieza.Contratos
{
    public class IndexModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly ICTServicioProxy _servicios;
        private readonly ILContratoProxy _contratos;
        private readonly IPermisoProxy _permisos;

        public ModuloDto Modulo { get; set; }
        public SubmoduloDto Submodulo { get; set; }
        public CTServicioDto Servicio { get; set; }
        public List<ContratoDto> Contratos { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }
        public List<MesDto> Meses { get; set; }

        public IndexModel(IModuloProxy modulo, ICTServicioProxy servicios, ILContratoProxy contratos, IPermisoProxy permisos)
        {
            _modulo = modulo;
            _servicios = servicios;
            _contratos = contratos;
            _permisos = permisos;
        }

        public async Task OnGet(int moduloId, int submoduloId)
        {
            Permisos = await _permisos.GetPermisosByModuloUsuario(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                Contratos = await _contratos.GetAllAsync();
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        public async Task<JsonResult> OnPostCrearContrato([FromBody] ContratoCreateCommand contrato)
        {
            contrato.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int status = await _contratos.CreateContrato(contrato);
            return new JsonResult(status);
        }
    }
}
