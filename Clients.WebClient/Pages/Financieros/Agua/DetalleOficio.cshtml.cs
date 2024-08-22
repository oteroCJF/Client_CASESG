using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.CFDIs.ServiciosGenerales.DTOs;
using Api.Gateway.Models.Dashboard.Financieros;
using Api.Gateway.Models.Estatus.DTOs.EstatusOficios;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Oficios.Commands;
using Api.Gateway.Models.Oficios.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Dashboards;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Agua.CFDIs.Commands;
using Api.Gateway.WebClient.Proxy.Agua.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Oficios.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Oficios.Commands;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clients.WebClient.Pages.Financieros.Agua
{
    public class DetalleOficioModel : PageModel
    {
        private readonly ICTServicioProxy _servicios;
        private readonly IFDetalleServicioProxy _detalle;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IMesProxy _meses;
        private readonly IModuloProxy _modulos;
        private readonly IPermisoProxy _permisos;

        private readonly IQOficioAguaProxy _oficiosQueries;
        private readonly ICOficioAguaProxy _oficiosCommands;

        private readonly ICCFDIAguaProxy _cfdisCommands;
        private readonly IQCFDIAguaProxy _cfdisQueries;

        private readonly IEstatusOficioProxy _flujo;
        private readonly IEstatusFacturaProxy _efacturas;

        public List<DetalleServicioDto> Detalle = new List<DetalleServicioDto>();
        public CTServicioDto Servicio { get; set; } = new CTServicioDto();
        public List<InmuebleDto> Inmuebles { get; set; } = new List<InmuebleDto>();
        public List<MesDto> Meses { get; set; } = new List<MesDto>();
        public List<CFDIDto> FacturasP { get; set; } = new List<CFDIDto>();
        public List<int> Permisos { get; set; } = new List<int>();
        public List<int?> Modulos { get; set; } = new List<int?>();
        public ModuloDto Modulo { get; set; } = new ModuloDto();
        public OficioDto Oficio { get; set; } = new OficioDto();
        public List<FlujoOficiosDto> Flujo { get; set; } = new List<FlujoOficiosDto>();
        public int Anio { get; set; }

        public DetalleOficioModel(ICTServicioProxy servicios, IFDetalleServicioProxy detalle, IInmuebleProxy inmuebles,
                                  IMesProxy meses, IModuloProxy modulos, IPermisoProxy permisos, IQOficioAguaProxy oficiosQ,
                                   ICOficioAguaProxy oficiosC,
                                  IQCFDIAguaProxy cfdisQ, ICCFDIAguaProxy cfdisC, IEstatusOficioProxy flujo, IEstatusFacturaProxy efacturas)
        {
            _servicios = servicios;
            _detalle = detalle;
            _inmuebles = inmuebles;
            _meses = meses;
            _modulos = modulos;
            _permisos = permisos;
            _oficiosQueries = oficiosQ;
            _oficiosCommands = oficiosC;
            _cfdisQueries = cfdisQ;
            _cfdisCommands = cfdisC;
            _flujo = flujo;
            _efacturas = efacturas;
        }

        public async Task OnGet(int moduloId, int servicioId, int anio, int oficioId)
        {
            string usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Permisos = (await _permisos.GetPermisosByUsuario(usuario)).Select(p => p.ModuloId).ToList();
            Modulos = (await _modulos.GetAllModulosAsync()).Where(m => Permisos.Contains(m.Id)).Select(m => m.ServicioId).ToList();
            Modulo = await _modulos.GetModuloByIdAsync(moduloId);
            Servicio = await _servicios.GetServicioByIdAsync(servicioId);
            Anio = Anio == 0 ? DateTime.Now.Year : Anio;
            FacturasP = await _oficiosQueries.GetFacturasNCPendientes(oficioId);
            Oficio = await _oficiosQueries.GetOficioById(oficioId);
            Flujo = await _flujo.GetFlujoByOficio(servicioId, (int)Oficio.EstatusId);
        }

        public async Task<IActionResult> OnPostCreateDTOficio([FromBody] List<DetalleOficioCreateCommand> dtOficio)
        {
            var detalle = await _oficiosCommands.CreateDetalleOficio(dtOficio);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostDeleteDTOficio([FromBody] DetalleOficioDeleteCommand dtOficio)
        {
            var detalle = await _oficiosCommands.DeleteDetalleOficio(dtOficio);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostCorregirOficio([FromBody] CorregirOficioCommand oficio)
        {
            var detalle = await _oficiosCommands.CorregirOficio(oficio);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostPagarOficio([FromBody] PagarOficioCommand oficio)
        {
            var detalle = await _oficiosCommands.PagarOficio(oficio);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostCancelarOficio([FromBody] CancelarOficioCommand oficio)
        {
            var detalle = await _oficiosCommands.CancelarOficio(oficio);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostEDGPPTOficio([FromBody] EDGPPTOficioCommand oficio)
        {
            var totalFNP = await GetFacturasNoPendientes(oficio.Id);
            if (totalFNP == 0)
            {
                var detalle = await _oficiosCommands.EDGPPTOficio(oficio);
                return StatusCode(200);
            }

            return BadRequest();
        }

        public async Task<int> GetFacturasNoPendientes(int oficio)
        {
            var facturasP = (await _oficiosQueries.GetOficioById(oficio)).CFDIs;
            var estatus = (await _efacturas.GetAllEstatusFacturasAsync()).Single(ef => ef.Abreviacion.Equals("Pendiente")).Id;
            var facturasNP = facturasP.Where(p => p.EstatusId != estatus).ToList();

            return facturasNP.Count();
        }
    }
}

