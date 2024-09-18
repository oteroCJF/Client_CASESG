using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Gateway.Models.Catalogos.DTOs.Entregables;
using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Contratos.DTOs;
using Api.Gateway.Models.Dashboard.Financieros;
using Api.Gateway.Models.Entregables.ServiciosGenerales.Commands.Cedulas;
using Api.Gateway.Models.Estatus.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Oficios.Commands;
using Api.Gateway.Models.Oficios.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTEntregables;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Dashboards;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Api.Gateway.WebClient.Proxy.Agua.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.Agua.Entregables.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Entregables.Commands;
using Api.Gateway.WebClient.Proxy.Agua.Oficios.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Oficios.Commands;



namespace Clients.WebClient.Pages.Financieros.Agua
{
    public class DetalleServicioModel : PageModel
    {
        private readonly ICTServicioProxy _servicios;
        private readonly ICTEntregableProxy _ctentregables;
        private readonly IEstatusEntregableProxy _estatuse;
        private readonly IFDetalleServicioProxy _detalle;

        private readonly IQEntregableAguaProxy _entregablesQueries;
        private readonly ICEntregableAguaProxy _entregablesCommands;


        private readonly IInmuebleProxy _inmuebles;
        private readonly IMesProxy _meses;
        private readonly IModuloProxy _modulos;
        private readonly IPermisoProxy _permisos;

        private readonly IQOficioAguaProxy _oficiosQueries;
        private readonly ICOficioAguaProxy _oficiosCommands;

        private readonly IQContratoAguaProxy _contratosQueries;
        private readonly ICContratoAguaProxy _contratosCommands;

        public List<DetalleServicioDto> Detalle = new List<DetalleServicioDto>();

        public List<ContratoDto> Contratos = new List<ContratoDto>();
        public CTServicioDto Servicio { get; set; } = new CTServicioDto();
        public List<InmuebleDto> Inmuebles { get; set; } = new List<InmuebleDto>();
        public List<int> InmueblesServicio { get; set; } = new List<int>();
        public List<OficioDto> Oficios { get; set; } = new List<OficioDto>();
        public List<MesDto> Meses { get; set; } = new List<MesDto>();
        public List<int> Permisos { get; set; } = new List<int>();
        public ModuloDto Modulo { get; set; } = new ModuloDto();

        public List<CTEntregableDto> CTEntregables = new List<CTEntregableDto>();

        public List<EstatusDto> CTEstatus = new List<EstatusDto>();
        public List<int?> Modulos { get; set; } = new List<int?>();
        public int Anio { get; set; }

        public DetalleServicioModel(ICTServicioProxy servicios, ICTEntregableProxy ctentregables, IEstatusEntregableProxy estatuse,
                                    IFDetalleServicioProxy detalle, IQEntregableAguaProxy entregablesQ, ICEntregableAguaProxy entregablesC, IInmuebleProxy inmuebles, IMesProxy meses,
                                    IModuloProxy modulos, IPermisoProxy permisos, IQOficioAguaProxy oficiosQ, ICOficioAguaProxy oficiosC , IQContratoAguaProxy contratosQ, ICContratoAguaProxy contratosC)
        {
            _servicios = servicios;
            _ctentregables = ctentregables;
            _estatuse = estatuse;
            _detalle = detalle;
            _entregablesQueries = entregablesQ;
            _entregablesCommands = entregablesC;
            _inmuebles = inmuebles;
            _meses = meses;
            _modulos = modulos;
            _permisos = permisos;
            _oficiosQueries = oficiosQ;
            _oficiosCommands = oficiosC;
            _contratosQueries = contratosQ;
            _contratosCommands = contratosC;
        }

        public async Task OnGet(int moduloId, int servicioId, int anio)
        {
            string usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Modulo = await _modulos.GetModuloByIdAsync(moduloId);
            Permisos = (await _permisos.GetPermisosByUsuario(usuario)).Select(p => p.ModuloId).ToList();
            Modulos = (await _modulos.GetAllModulosAsync()).Where(m => Permisos.Contains(m.Id)).Select(m => m.ServicioId).ToList();
            Contratos = await _contratosQueries.GetAllAsync();
            Servicio = await _servicios.GetServicioByIdAsync(servicioId);
            Anio = Anio == 0 ? DateTime.Now.Year : Anio;
            Oficios = await _oficiosQueries.GetOficiosByAnio(anio);
            Meses = await _meses.GetAllAsync();
            InmueblesServicio = (await _inmuebles.GetInmueblesByServicio(servicioId)).Select(iu => iu.InmuebleId).ToList();
            Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesServicio.Contains(i.Id)).ToList();
            CTEntregables = await _ctentregables.GetEntregablesByServicioAsync(servicioId);
            CTEstatus = await _estatuse.GetAllEstatusEntregablesAsync();
            Detalle = await _detalle.GetFDetalleServicios(anio, usuario, Servicio);
        }

        public async Task<JsonResult> OnPostCreateOficio([FromForm] OficioCreateCommand oficio)
        {
            var result = await _oficiosCommands.CreateOficio(oficio);
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostDescargarEntregables([FromBody] DEntregablesCommand descarga)
        {
            var result = await _entregablesCommands.DescargarEntregables(descarga);
            byte[] fileBytes = System.IO.File.ReadAllBytes(result);
            string fileName = "Entregables_Agua" + DateTime.Now.ToString("dd-MM-yyyy") + ".zip";
            return File(fileBytes, "application/zip", fileName);
        }
    }
}
