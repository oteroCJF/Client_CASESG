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
using Api.Gateway.WebClient.Proxy.Mensajeria.Contratos;
using Api.Gateway.WebClient.Proxy.Mensajeria.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Entregables;
using Api.Gateway.WebClient.Proxy.Mensajeria.Entregables.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Entregables.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Oficios;
using Api.Gateway.WebClient.Proxy.Mensajeria.Oficios.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Oficios.Queries;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clients.WebClient.Pages.Financieros.Mensajeria
{
    public class DetalleServicioModel : PageModel
    {
        private readonly ICTServicioProxy _servicios;
        private readonly ICTEntregableProxy _ctentregables;
        private readonly IEstatusEntregableProxy _estatuse;
        private readonly IFDetalleServicioProxy _detalle;
        private readonly IQEntregableMensajeriaProxy _entregablesQuery;
        private readonly ICEntregableMensajeriaProxy _entregablesCommand;

        private readonly IInmuebleProxy _inmuebles;
        private readonly IMesProxy _meses;
        private readonly IModuloProxy _modulos;
        private readonly IPermisoProxy _permisos;

        private readonly IQOficioMensajeriaProxy _oficiosQuery;
        private readonly ICOficioMensajeriaProxy _oficiosCommand;

        private readonly IQContratoMensajeriaProxy _contratosQuery;

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
                                    IFDetalleServicioProxy detalle, IQEntregableMensajeriaProxy entregablesQuery, 
                                    ICEntregableMensajeriaProxy entregablesCommand, IInmuebleProxy inmuebles, IMesProxy meses, 
                                    IModuloProxy modulos, IPermisoProxy permisos, IQOficioMensajeriaProxy oficiosQuery, 
                                    ICOficioMensajeriaProxy oficiosCommand, IQContratoMensajeriaProxy contratosQuery)
        {
            _servicios = servicios;
            _ctentregables = ctentregables;
            _estatuse = estatuse;
            _entregablesQuery = entregablesQuery;
            _entregablesCommand = entregablesCommand;
            _inmuebles = inmuebles;
            _meses = meses;
            _modulos = modulos;
            _permisos = permisos;
            _oficiosQuery = oficiosQuery;
            _oficiosCommand = oficiosCommand;
            _contratosQuery = contratosQuery;
            _detalle = detalle;
        }

        public async Task OnGet(int moduloId, int servicioId, int anio)
        {
            string usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Modulo = await _modulos.GetModuloByIdAsync(moduloId);
            Permisos = (await _permisos.GetPermisosByUsuario(usuario)).Select(p => p.ModuloId).ToList();
            Modulos = (await _modulos.GetAllModulosAsync()).Where(m => Permisos.Contains(m.Id)).Select(m => m.ServicioId).ToList();
            Contratos = await _contratosQuery.GetAllAsync();
            Servicio = await _servicios.GetServicioByIdAsync(servicioId);
            Anio = anio;
            Oficios = await _oficiosQuery.GetOficiosByAnio(anio);
            Meses = await _meses.GetAllAsync();
            InmueblesServicio = (await _inmuebles.GetInmueblesByServicio(servicioId)).Select(iu => iu.InmuebleId).ToList();
            Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesServicio.Contains(i.Id)).ToList();
            CTEntregables = await _ctentregables.GetEntregablesByServicioAsync(servicioId);
            CTEstatus = await _estatuse.GetAllEstatusEntregablesAsync();
            Detalle = await _detalle.GetFDetalleServicios(anio, usuario, Servicio);
        }

        public async Task<JsonResult> OnPostCreateOficio([FromForm] OficioCreateCommand oficio)
        {
            var result = await _oficiosCommand.CreateOficio(oficio);
            return new JsonResult(result);
        }
        
        public async Task<IActionResult> OnPostDescargarEntregables([FromBody] DEntregablesCommand descarga)
        {
            var result = await _entregablesCommand.DescargarEntregables(descarga);
            byte[] fileBytes = System.IO.File.ReadAllBytes(result);
            string fileName = "Entregables_Mensajeria" + DateTime.Now.ToString("dd-MM-yyyy") + ".zip";
            return File(fileBytes, "application/zip", fileName);
        }
    }
}
