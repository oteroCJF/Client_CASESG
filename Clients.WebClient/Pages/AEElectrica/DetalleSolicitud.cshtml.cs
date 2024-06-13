using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.CFDIs.ServiciosBasicos.Commands;
using Api.Gateway.Models.Entregables.ServiciosBasicos.Commands;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Inmuebles.DTOs.InmueblesUS;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.Models.SolicitudesPago.Commands;
using Api.Gateway.Models.SolicitudesPago.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.CFDIs;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.Entregables;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.LogEntregables;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.LogSolicitudes;
using Api.Gateway.WebClient.Proxy.ServiciosBasicos.AEElectrica.SolicitudesPago;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.AEElectrica
{
    public class DetalleSolicitudModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly IInmuebleProxy _inmuebles;
        private readonly ICTServicioProxy _servicios;
        private readonly IPermisoProxy _permisos;
        private readonly IAEECFDIProxy _cfdi;
        private readonly IAEESolicitudPagoProxy _solicitudes;
        private readonly IAEEEntregableProxy _entregable;
        private readonly IAEELogSolicitudProxy _logSolicitud;
        private readonly IAEELogEntregableProxy _logEntregable;

        public ModuloDto Modulo { get; set; } = new ModuloDto();
        public SubmoduloDto Submodulo { get; set; } = new SubmoduloDto();
        public CTServicioDto Servicio { get; set; } = new CTServicioDto();
        public SolicitudPagoDto SolicitudesPago { get; set; } = new SolicitudPagoDto();
        public List<InmuebleDto> Inmuebles { get; set; } = new List<InmuebleDto>();
        public List<int> InmueblesUsuarios { get; set; } = new List<int>();
        public List<PermisoUsuarioDto> Permisos { get; set; } = new List<PermisoUsuarioDto>();

        public DetalleSolicitudModel(IModuloProxy modulo, ICTServicioProxy servicios, IPermisoProxy permisos, IAEESolicitudPagoProxy solicitudes,
                           IAEECFDIProxy cfdi, IAEEEntregableProxy entregable, IInmuebleProxy inmuebles, IAEELogSolicitudProxy logSolicitud,
                           IAEELogEntregableProxy logEntregable)
        {
            _modulo = modulo;
            _servicios = servicios;
            _cfdi = cfdi;
            _inmuebles = inmuebles;
            _permisos = permisos;
            _entregable = entregable;
            _solicitudes = solicitudes;
            _logSolicitud = logSolicitud;
            _logEntregable = logEntregable;
        }

        public async Task OnGet(int moduloId, int submoduloId, int solicitud)
        {
            Modulo = await _modulo.GetModuloByIdAsync(moduloId);
            Permisos = await _permisos.GetPermisosByModuloUsuario(User.FindFirst(ClaimTypes.NameIdentifier).Value, moduloId);
            InmueblesUsuarios = (await _inmuebles.GetInmueblesByUsuarioServicio(User.FindFirst(ClaimTypes.NameIdentifier).Value, (int)Modulo.ServicioId)).Select(ius => ius.InmuebleId).ToList();
            Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesUsuarios.Contains(i.Id)).ToList();
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0 && Inmuebles.Count() > 0)
            {
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                SolicitudesPago = await _solicitudes.GetSolicitudPagoById(solicitud);
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        public async Task<IActionResult> OnPostCargarFactura([FromForm]CFDISBCommand facturas)
        {
            if (facturas != null)
            {
                CFDISBCreateCommand factura = new CFDISBCreateCommand();
                factura.Anio = facturas.Anio;
                factura.SolicitudId = facturas.SolicitudId;
                factura.Mes = facturas.Mes;
                factura.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                factura.XML = facturas.FileXML;
                factura.PDF = facturas.FilePDF;
                int statusFactura = await _cfdi.CreateFactura(factura);
                return StatusCode(statusFactura);
            }
            else
            {
                return BadRequest();
            }
        }
        
        public async Task<IActionResult> OnPutEliminarFactura([FromBody] CFDISBDeleteCommand factura)
        {
            if (factura != null)
            {
                int statusFactura = await _cfdi.DeleteFactura(factura);
                return StatusCode(statusFactura);
            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> OnPutActualizarEntregable([FromForm] EntregableSBUpdateCommand entregable)
        {
            int status = await _entregable.UpdateEntregable(entregable);
            return StatusCode(status);
        }

        public async Task<IActionResult> OnGetVisualizarEntregable(int solicitud, string tipo, string archivo)
        {
            string path = await _entregable.VisualizarEntregables(solicitud, tipo, archivo);
            Stream stream = System.IO.File.Open(path, FileMode.Open);
            return File(stream, "application/pdf");
        }

        public async Task<IActionResult> OnPutUpdateCedula([FromBody] SolicitudPagoUpdateCommand solicitud)
        {
            solicitud.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _solicitudes.UpdateSolicitud(solicitud);
            return StatusCode(200);
        }
        
        public async Task<IActionResult> OnPutSeguimientoEntregable([FromBody] AEntregableSBUpdateCommand entregable)
        {
            int status = await _entregable.SeguimientoEntregable(entregable);
            return StatusCode(status);
        }
    }
}
