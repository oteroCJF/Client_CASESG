using Api.Gateway.Models.Inmuebles.Commands;
using Api.Gateway.Models.Inmuebles.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Inmuebles.DTOs.InmueblesServicio;
using Api.Gateway.Models.Inmuebles.DTOs.InmueblesUS;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.Commands;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.Models.Usuarios.DTOs;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Api.Gateway.WebClient.Proxy.Usuarios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Usuarios
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DetalleModel : PageModel
    {
        private readonly IUsuarioProxy _usuarios;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IModuloProxy _modulo;
        private readonly IPermisoProxy _permisos;
        public int Anio { get; set; }
        public ModuloDto Modulo { get; set; }
        public List<ModuloDto> Modulos { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }
        public UsuarioDto Usuario { get; set; }
        public List<InmuebleDto> Inmuebles { get; set; }
        public List<InmuebleUSDto> InmueblesUsuario { get; set; }
        public List<InmuebleServicioDto> InmueblesServicio { get; set; }

        public DetalleModel(IUsuarioProxy usuarios, IModuloProxy modulo, IPermisoProxy permisos, IInmuebleProxy inmuebles)
        {
            _usuarios = usuarios;
            _modulo = modulo;
            _permisos = permisos;
            _inmuebles = inmuebles;
        }
        public async Task OnGet(int moduloId, string usuarioId)
        {
            Permisos = await _permisos.GetPermisosByModuloUsuario(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Permisos = await _permisos.GetPermisosByUsuario(usuarioId);
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Modulos = await _modulo.GetAllModulosAsync();
                Usuario = await _usuarios.GetUsuarioById(usuarioId);
                InmueblesUsuario = await _inmuebles.GetInmueblesByUsuario(usuarioId);
                InmueblesServicio = await _inmuebles.GetAllInmueblesServicio();
                Inmuebles = await _inmuebles.GetAllInmueblesAsync();
            }
            else
            {
                Response.Redirect("/error/denegado");
            }            
        }

        public async Task<IActionResult> OnDeleteBorrarPermisos(string usuario)
        {
            await _permisos.DeletePermisos(usuario);
            return this.StatusCode(200);
        }

        public async Task<IActionResult> OnPostCrearPermisos([FromBody] List<PermisoCreateCommand> permisos)
        {
            await _permisos.CreatePermisos(permisos);
            return this.StatusCode(200);
        }

        public async Task<IActionResult> OnDeleteBorrarInmueblesUS(string usuario)
        {
            await _inmuebles.DeleteInmuebleUS(usuario);
            return this.StatusCode(200);
        }

        public async Task<IActionResult> OnPostCrearInmueblesUS([FromBody] List<CreateCommandInmuebleUS> inmuebles)
        {
            await _inmuebles.CreateInmuebleUS(inmuebles);
            return this.StatusCode(200);
        }
    }
}
