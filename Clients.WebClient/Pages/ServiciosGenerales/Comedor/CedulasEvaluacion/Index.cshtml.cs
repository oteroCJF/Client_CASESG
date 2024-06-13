using Api.Gateway.Models;
using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs;
using Api.Gateway.Models.Estatus.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Comedor.CedulasEvaluacion.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Repositorios.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Repositorios.Queries;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Meses;
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

namespace Clients.WebClient.Pages.Comedor.CedulasEvaluacion
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class IndexModel : PageModel
    {
        private readonly IMesProxy _mes;
        private readonly ICTServicioProxy _servicios;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IQRepositorioComedorProxy _repositorios;
        private readonly IModuloProxy _modulo;
        private readonly IQCFDIComedorProxy _facturas;
        private readonly IQCedulaComedorProxy _cedula;
        private readonly IEstatusCedulaProxy _estatusc;
        private readonly IPermisoProxy _permisos;
        private readonly IQContratoComedorProxy _contrato;

        public ModuloDto Modulo { get; set; }
        public SubmoduloDto Submodulo { get; set; }
        public List<int> InmueblesUsuarios { get; set; } = new List<int>();
        public List<InmuebleDto> Inmuebles { get; set; }
        public CTServicioDto Servicio { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }
        public DataCollection<CedulaEvaluacionDto> Cedulas { get; set; }
        public List<MesDto> FiltrosMes { get; set; } = new List<MesDto>();
        public List<EstatusDto> FiltrosEstatus { get; set; } = new List<EstatusDto>();
        public List<InmuebleDto> FiltrosInmueble { get; set; } = new List<InmuebleDto>();

        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }
        public int CurrentPage { get; set; } = 1;

        public IndexModel(IMesProxy mes, ICTServicioProxy servicios, IInmuebleProxy inmuebles, IQRepositorioComedorProxy repositorios, IModuloProxy modulo, 
                          IQCFDIComedorProxy facturas, IQCedulaComedorProxy cedula, IEstatusCedulaProxy estatusc, IPermisoProxy permisos, IQContratoComedorProxy contrato)
        {
            _mes = mes;
            _servicios = servicios;
            _inmuebles = inmuebles;
            _repositorios = repositorios;
            _modulo = modulo;
            _facturas = facturas;
            _cedula = cedula;
            _estatusc = estatusc;
            _permisos = permisos;
            _contrato = contrato;
        }

        public async Task OnGet(int moduloId, int submoduloId)
        {
            string Usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Permisos = await _permisos.GetPermisosByModuloUsuario(Usuario, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                int am = Anio;
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                InmueblesUsuarios = (await _inmuebles.GetInmueblesByUsuarioServicio(Usuario, (int)Modulo.ServicioId)).Select(ius => ius.InmuebleId).ToList();
                Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesUsuarios.Contains(i.Id)).ToList();
                Cedulas = Anio != 0 ? await _cedula.GetCedulaByAnioAsync((int)Modulo.ServicioId, Anio, Usuario) : new DataCollection<CedulaEvaluacionDto>();
                FiltrosEstatus = await GetFiltrosEstatus(Cedulas);
                FiltrosInmueble = await GetFiltrosInmueble(Cedulas);
                FiltrosMes = await GetFiltrosMes(Cedulas);
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        private async Task<List<EstatusDto>> GetFiltrosEstatus(DataCollection<CedulaEvaluacionDto> cedulas)
        {
            List<EstatusDto> estatus = new List<EstatusDto>();
            if (cedulas.Items != null)
            {
                var estatusId = cedulas.Items.Select(c => c.EstatusId).Distinct().ToList();
                estatus = (await _estatusc.GetAllEstatusCedulaAsync()).Where(e => estatusId.Contains(e.Id)).ToList();
            }

            return estatus;
        }

        private async Task<List<InmuebleDto>> GetFiltrosInmueble(DataCollection<CedulaEvaluacionDto> cedulas)
        {
            List<InmuebleDto> inmuebles = new List<InmuebleDto>();
            if (cedulas.Items != null)
            {
                var inmueblesId = cedulas.Items.Select(c => c.InmuebleId).Distinct().ToList();
                inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(e => inmueblesId.Contains(e.Id)).ToList();
            }

            return inmuebles;
        }
        
        private async Task<List<MesDto>> GetFiltrosMes(DataCollection<CedulaEvaluacionDto> cedulas)
        {
            List<MesDto> meses = new List<MesDto>();
            if (cedulas.Items != null)
            {
                var mesesId = cedulas.Items.Select(c => c.MesId).Distinct().ToList();
                meses = (await _mes.GetAllAsync()).Where(m => mesesId.Contains(m.Id)).ToList();
            }

            return meses;
        }
    }
}
