using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Contratos.DTOs;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Fumigacion.Contratos;
using Api.Gateway.WebClient.Proxy.Fumigacion.Repositorios;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Gateway.Models.Repositorios.Commands;

namespace Clients.WebClient.Pages.Fumigacion.Facturas
{
    public class IndexModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly IMesProxy _mes;
        private readonly IFContratoProxy _contratos;
        private readonly IFRepositorioProxy _repositorios;
        private readonly ICTServicioProxy _servicios;
        private readonly IPermisoProxy _permisos;

        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }
        public ModuloDto Modulo { get; set; }
        public SubmoduloDto Submodulo { get; set; }
        public List<InmuebleDto> Inmuebles { get; set; }
        public List<RepositorioDto> Repositorio { get; set; }
        public CTServicioDto Servicio { get; set; }
        public List<ContratoDto> Contratos { get; set; }
        public List<MesDto> Meses { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }

        public IndexModel(IModuloProxy modulo, IFRepositorioProxy RepositorioProxy, ICTServicioProxy servicios, IPermisoProxy permisos, 
                          IFContratoProxy contratos, IMesProxy mes)
        {
            _modulo = modulo;
            _repositorios = RepositorioProxy;
            _servicios = servicios;
            _permisos = permisos;
            _contratos = contratos;
            _mes = mes;

        }

        public async Task OnGet(int moduloId, int submoduloId)
        {
            Permisos = await _permisos.GetPermisosByModuloUsuario(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                Repositorio = Anio != 0 ? await _repositorios.GetAllRepositorios(Anio) : new List<RepositorioDto>();
                Contratos = await _contratos.GetAllAsync();
                Meses = await _mes.GetAllAsync();
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        public async Task<JsonResult> OnPostCreateRepositorio([FromBody] RepositorioCreateCommand facturacion)
        {
            var exists = (await _repositorios.GetAllRepositorios(facturacion.Anio))
                                            .SingleOrDefault(f => f.ContratoId == facturacion.ContratoId &&
                                                         f.Anio == facturacion.Anio &&
                                                         f.MesId == facturacion.MesId);

            var contrato = await _contratos.GetContratoByIdAsync(facturacion.ContratoId);

            var m = Convert.ToDateTime(contrato.InicioVigencia).Month;

            if (exists != null && exists.Id != 0)
            {
                return new JsonResult(null)
                {
                    StatusCode = 205
                };
            }
            else
            {
                if (facturacion.MesId < Convert.ToDateTime(contrato.InicioVigencia).Month &&
                    (facturacion.Anio == Convert.ToDateTime(contrato.InicioVigencia).Year || facturacion.Anio < Convert.ToDateTime(contrato.InicioVigencia).Year))
                {
                    return new JsonResult(null)
                    {
                        StatusCode = 208
                    };
                }
                else
                {
                    facturacion.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    int status = await _repositorios.CreateRepositorio(facturacion);
                    return new JsonResult(status);
                }
            }


        }
    }
}
