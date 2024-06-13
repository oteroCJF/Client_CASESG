using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs;
using Api.Gateway.Models.CFDIs.ServiciosGenerales.DTOs;
using Api.Gateway.Models.Estatus.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Limpieza.CedulaEvaluacion;
using Api.Gateway.WebClient.Proxy.Limpieza.Contratos;
using Api.Gateway.WebClient.Proxy.Limpieza.Facturas;
using Api.Gateway.WebClient.Proxy.Limpieza.Repositorios;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Limpieza.CedulasEvaluacion
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CedulasEvaluacionModel : PageModel
    {
        private readonly IMesProxy _mes;
        private readonly ICTServicioProxy _servicios;
        private readonly IInmuebleProxy _inmuebles;
        private readonly ILRepositorioProxy _repositorios;
        private readonly IModuloProxy _modulo;
        private readonly ILCFDIProxy _facturas;
        private readonly ILCedulaProxy _cedula;
        private readonly IEstatusCedulaProxy _estatusc;
        private readonly IPermisoProxy _permisos;
        private readonly ILContratoProxy _contrato;

        public int Submodulo { get; set; }
        public List<int> InmueblesUsuarios { get; set; } = new List<int>();
        public RepositorioDto Repositorio { get; set; }
        public ModuloDto Modulo { get; set; }
        public CTServicioDto Servicio { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }
        public List<CedulaEvaluacionDto> Cedulas { get; set; }
        public List<CFDIDto> Facturas { get; set; }
        public List<EstatusDto> FiltrosEstatus { get; set; } = new List<EstatusDto>();
        public List<InmuebleDto> FiltrosInmueble { get; set; } = new List<InmuebleDto>();

        public CedulasEvaluacionModel(IMesProxy mes, IInmuebleProxy inmuebles, ILRepositorioProxy repositorios, IModuloProxy modulo, ICTServicioProxy servicios,
                                      ILCFDIProxy facturas, ILCedulaProxy cedula, IEstatusCedulaProxy estatusc, IPermisoProxy permisos,
                                      ILContratoProxy contrato)
        {
            _mes = mes;
            _servicios = servicios;
            _inmuebles = inmuebles;
            _contrato = contrato;
            _repositorios = repositorios;
            _modulo = modulo;
            _facturas = facturas;
            _cedula = cedula;
            _estatusc = estatusc;
            _permisos = permisos;
        }

        public async Task OnGet(int moduloId, int submoduloId, int anio, int mes, int contrato)
        {
            string usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Permisos = await _permisos.GetPermisosByModuloUsuario(usuario, moduloId);
            Modulo = await _modulo.GetModuloByIdAsync(moduloId);
            Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Submodulo = submoduloId;
                Repositorio = (await _repositorios.GetAllRepositorios(anio)).SingleOrDefault(f => f.MesId == mes && f.ContratoId == contrato);
                Cedulas = await _cedula.GetCedulaByAnioMesAsync((int)Modulo.ServicioId, anio, mes, contrato, usuario);
                FiltrosEstatus = await GetFiltrosEstatus(Cedulas);
                FiltrosInmueble = await GetFiltrosInmueble(Cedulas);
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        private async Task<List<EstatusDto>> GetFiltrosEstatus(List<CedulaEvaluacionDto> cedulas)
        {
            var estatusId = cedulas.Select(c => c.EstatusId).Distinct().ToList();
            List<EstatusDto> estatus = (await _estatusc.GetAllEstatusCedulaAsync()).Where(e => estatusId.Contains(e.Id)).ToList();

            return estatus;
        }

        private async Task<List<InmuebleDto>> GetFiltrosInmueble(List<CedulaEvaluacionDto> cedulas)
        {
            var inmueblesId = cedulas.Select(c => c.InmuebleId).Distinct().ToList();
            List<InmuebleDto> inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(e => inmueblesId.Contains(e.Id)).ToList();

            return inmuebles;
        }
    }
}
