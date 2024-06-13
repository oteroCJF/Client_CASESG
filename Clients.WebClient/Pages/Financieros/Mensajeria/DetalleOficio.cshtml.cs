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
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Mensajeria.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Oficios.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Oficios.Queries;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clients.WebClient.Pages.Financieros.Mensajeria
{
    public class DetalleOficioModel : PageModel
    {
        private readonly ICTServicioProxy _servicios;
        private readonly IFDetalleServicioProxy _detalle;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IMesProxy _meses;
        private readonly IModuloProxy _modulos;
        private readonly IPermisoProxy _permisos;
        private readonly IQOficioMensajeriaProxy _oficiosQuery;
        private readonly ICOficioMensajeriaProxy _oficiosCommand;
        private readonly IQCFDIMensajeriaProxy _cfdis;
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

        public DetalleOficioModel(ICTServicioProxy servicios, IFDetalleServicioProxy detalle, IInmuebleProxy inmuebles, IMesProxy meses,
                        IModuloProxy modulos, IPermisoProxy permisos, IQOficioMensajeriaProxy oficiosQuery, IQCFDIMensajeriaProxy cfdis, 
                        IEstatusOficioProxy flujo, IEstatusFacturaProxy efacturas, ICOficioMensajeriaProxy oficiosCommand)
        {
            _servicios = servicios;
            _detalle = detalle;
            _inmuebles = inmuebles;
            _efacturas = efacturas;
            _modulos = modulos;
            _oficiosQuery = oficiosQuery;
            _oficiosCommand = oficiosCommand;
            _permisos = permisos;
            _meses = meses;
            _cfdis = cfdis;
            _flujo = flujo;
        }


        public async Task OnGet(int moduloId, int servicioId, int anio, int oficioId)
        {
            string usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Permisos = (await _permisos.GetPermisosByUsuario(usuario)).Select(p => p.ModuloId).ToList();
            Modulos = (await _modulos.GetAllModulosAsync()).Where(m => Permisos.Contains(m.Id)).Select(m => m.ServicioId).ToList();
            Modulo = await _modulos.GetModuloByIdAsync(moduloId);
            Servicio = await _servicios.GetServicioByIdAsync(servicioId);
            Anio = anio;
            FacturasP = await _oficiosQuery.GetFacturasNCPendientes(oficioId);
            Oficio = await _oficiosQuery.GetOficioById(oficioId);
            Flujo = await _flujo.GetFlujoByOficio(servicioId, (int)Oficio.EstatusId);
        }

        public async Task<IActionResult> OnPostCreateDTOficio([FromBody] List<DetalleOficioCreateCommand> dtOficio)
        {
            var detalle = await _oficiosCommand.CreateDetalleOficio(dtOficio);
            return StatusCode(200);
        }
        
        public async Task<IActionResult> OnPostDeleteDTOficio([FromBody] DetalleOficioDeleteCommand dtOficio)
        {
            var detalle = await _oficiosCommand.DeleteDetalleOficio(dtOficio);
            return StatusCode(200);
        }
        
        public async Task<IActionResult> OnPostCorregirOficio([FromBody] CorregirOficioCommand oficio)
        {
            var detalle = await _oficiosCommand.CorregirOficio(oficio);
            return StatusCode(200);
        }
        
        public async Task<IActionResult> OnPostPagarOficio([FromBody] PagarOficioCommand oficio)
        {
            var detalle = await _oficiosCommand.PagarOficio(oficio);
            return StatusCode(200);
        }
        
        public async Task<IActionResult> OnPostCancelarOficio([FromBody] CancelarOficioCommand oficio)
        {
            var detalle = await _oficiosCommand.CancelarOficio(oficio);
            return StatusCode(200);
        }
        
        public async Task<IActionResult> OnPostEDGPPTOficio([FromBody] EDGPPTOficioCommand oficio)
        {
            var totalFNP = await GetFacturasNoPendientes(oficio.Id);
            if (totalFNP == 0) {
                var detalle = await _oficiosCommand.EDGPPTOficio(oficio);
                return StatusCode(200);
            }

            return BadRequest();
        }

        public async Task<int> GetFacturasNoPendientes(int oficio)
        {
            var facturasP = (await _oficiosQuery.GetOficioById(oficio)).CFDIs;
            var estatus = (await _efacturas.GetAllEstatusFacturasAsync()).Single(ef => ef.Abreviacion.Equals("Pendiente")).Id;
            var facturasNP = facturasP.Where(p => p.EFactura.Id != estatus).ToList();

            return facturasNP.Count();
        }

    }
}
