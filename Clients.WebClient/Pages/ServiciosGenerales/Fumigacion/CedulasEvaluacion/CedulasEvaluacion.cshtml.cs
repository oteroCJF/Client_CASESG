using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs;
using Api.Gateway.Models.CFDIs.ServiciosGenerales.DTOs;
using Api.Gateway.Models.Contratos.DTOs;
using Api.Gateway.Models.Estatus.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Fumigacion.CedulasEvaluacion;
using Api.Gateway.WebClient.Proxy.Fumigacion.Contratos;
using Api.Gateway.WebClient.Proxy.Fumigacion.Facturas;
using Api.Gateway.WebClient.Proxy.Fumigacion.Repositorios;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Fumigacion.CedulasEvaluacion
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CedulasEvaluacionModel : PageModel
    {
        private readonly IMesProxy _mes;
        private readonly ICTServicioProxy _servicios;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IFRepositorioProxy _repositorios;
        private readonly IModuloProxy _modulo;
        private readonly IFCFDIProxy _facturas;
        private readonly IFCedulaProxy _cedula;
        private readonly IEstatusCedulaProxy _estatusc;
        private readonly IPermisoProxy _permisos;
        private readonly IFContratoProxy _contrato;

        public int Submodulo { get; set; }
        public List<int> InmueblesUsuarios { get; set; } = new List<int>();
        public RepositorioDto Repositorio { get; set; } = new RepositorioDto();
        public ModuloDto Modulo { get; set; } = new ModuloDto();
        public CTServicioDto Servicio { get; set; } = new CTServicioDto();
        public ContratoDto Contrato { get; set; } = new ContratoDto();
        public List<PermisoUsuarioDto> Permisos { get; set; } = new List<PermisoUsuarioDto>();
        public List<CedulaEvaluacionDto> Cedulas { get; set; } = new List<CedulaEvaluacionDto>();
        public List<CFDIDto> Facturas { get; set; } = new List<CFDIDto>();
        public List<EstatusDto> FiltrosEstatus { get; set; } = new List<EstatusDto>();
        public List<InmuebleDto> FiltrosInmueble { get; set; } = new List<InmuebleDto>();

        public CedulasEvaluacionModel(IMesProxy mes, IInmuebleProxy inmuebles, IFRepositorioProxy repositorios, IModuloProxy modulo, ICTServicioProxy servicios,
                                      IFCFDIProxy facturas, IFCedulaProxy cedula, IEstatusCedulaProxy estatusc, IPermisoProxy permisos, IFContratoProxy contrato)
        {
            _mes = mes;
            _servicios = servicios;
            _inmuebles = inmuebles;
            _repositorios = repositorios;
            _contrato = contrato;
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
                Contrato = await _contrato.GetContratoByIdAsync(contrato);
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
