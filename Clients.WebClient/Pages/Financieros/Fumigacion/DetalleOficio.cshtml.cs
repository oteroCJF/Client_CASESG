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
using Api.Gateway.WebClient.Proxy.Fumigacion.Facturas;
using Api.Gateway.WebClient.Proxy.Fumigacion.Oficios;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clients.WebClient.Pages.Financieros.Fumigacion
{
    public class DetalleOficioModel : PageModel
    {
        private readonly ICTServicioProxy _servicios;
        private readonly IFDetalleServicioProxy _detalle;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IMesProxy _meses;
        private readonly IModuloProxy _modulos;
        private readonly IPermisoProxy _permisos;
        private readonly IFOficioProxy _oficios;
        private readonly IFCFDIProxy _cfdis;
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
                                  IMesProxy meses, IModuloProxy modulos, IPermisoProxy permisos, IFOficioProxy oficios, 
                                  IFCFDIProxy cfdis, IEstatusOficioProxy flujo, IEstatusFacturaProxy efacturas)
        {
            _servicios = servicios;
            _detalle = detalle;
            _inmuebles = inmuebles;
            _meses = meses;
            _modulos = modulos;
            _permisos = permisos;
            _oficios = oficios;
            _cfdis = cfdis;
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
            FacturasP = await _oficios.GetFacturasNCPendientes(oficioId);
            Oficio = await _oficios.GetOficioById(oficioId);
            Flujo = await _flujo.GetFlujoByOficio(servicioId, (int)Oficio.EstatusId);
        }

        public async Task<IActionResult> OnPostCreateDTOficio([FromBody] List<DetalleOficioCreateCommand> dtOficio)
        {
            var detalle = await _oficios.CreateDetalleOficio(dtOficio);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostDeleteDTOficio([FromBody] DetalleOficioDeleteCommand dtOficio)
        {
            var detalle = await _oficios.DeleteDetalleOficio(dtOficio);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostCorregirOficio([FromBody] CorregirOficioCommand oficio)
        {
            var detalle = await _oficios.CorregirOficio(oficio);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostPagarOficio([FromBody] PagarOficioCommand oficio)
        {
            var detalle = await _oficios.PagarOficio(oficio);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostCancelarOficio([FromBody] CancelarOficioCommand oficio)
        {
            var detalle = await _oficios.CancelarOficio(oficio);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostEDGPPTOficio([FromBody] EDGPPTOficioCommand oficio)
        {
            var totalFNP = await GetFacturasNoPendientes(oficio.Id);
            if (totalFNP == 0)
            {
                var detalle = await _oficios.EDGPPTOficio(oficio);
                return StatusCode(200);
            }

            return BadRequest();
        }

        public async Task<int> GetFacturasNoPendientes(int oficio)
        {
            var facturasP = (await _oficios.GetOficioById(oficio)).CFDIs;
            var estatus = (await _efacturas.GetAllEstatusFacturasAsync()).Single(ef => ef.Abreviacion.Equals("Pendiente")).Id;
            var facturasNP = facturasP.Where(p => p.EstatusId != estatus).ToList();

            return facturasNP.Count();
        }
    }
}
