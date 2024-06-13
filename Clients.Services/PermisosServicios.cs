using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.WebClient.Proxy.Permisos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.Services
{
    public class PermisosServicios
    {
        private readonly IPermisoProxy _permisos;

        public PermisosServicios(IPermisoProxy permisos)
        {
            _permisos = permisos;
        }

        public async Task<List<ModuloDto>> GetModulosByUsuario(string usuario)
        {
            var modulos = await _permisos.GetModulosByUsuario(usuario);

            return modulos;
        }
    }
}
