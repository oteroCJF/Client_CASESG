using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.CFDIs.ServiciosGenerales.Commands;
using Api.Gateway.Models.CFDIs.ServiciosGenerales.DTOs;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Gateway.WebClient.Proxy.Mensajeria.Repositorios.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.CFDIs.Commands;

namespace Clients.WebClient.Pages.Mensajeria.Facturas
{
    public class DetalleFacturaModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly ICTServicioProxy _servicios;
        private readonly IMesProxy _mes;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IQRepositorioMensajeriaProxy _repositoriosQuery;
        private readonly IQCFDIMensajeriaProxy _facturasQuery;
        private readonly ICCFDIMensajeriaProxy _facturasCommand;
        private readonly IPermisoProxy _permisos;

        public int MesId { get; set; }
        public int FacturacionId { get; set; }
        public int InmuebleId { get; set; }
        public IFormFile XML { get; set; }
        public ModuloDto Modulo { get; set; }
        public SubmoduloDto Submodulo { get; set; }
        public CTServicioDto Servicio { get; set; }
        public MesDto Mes { get; set; }
        public InmuebleDto Inmueble { get; set; }
        public RepositorioDto Repositorio { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }
        public List<CFDIDto> Facturas { get; set; }

        public DetalleFacturaModel(IModuloProxy modulo, ICTServicioProxy servicios, IMesProxy mes, IInmuebleProxy inmuebles, 
                                   IQRepositorioMensajeriaProxy repositoriosQuery, IQCFDIMensajeriaProxy facturasQuery, 
                                   ICCFDIMensajeriaProxy facturasCommand, IPermisoProxy permisos)
        {
            _modulo = modulo;
            _servicios = servicios;
            _mes = mes;
            _inmuebles = inmuebles;
            _repositoriosQuery = repositoriosQuery;
            _facturasQuery = facturasQuery;
            _facturasCommand = facturasCommand;
            _permisos = permisos;
        }

        public async Task OnGet(int moduloId, int submoduloId, int inmueble, int facturacion)
        {
            Permisos = await _permisos.GetPermisosByModuloUsuario(User.FindFirst(ClaimTypes.NameIdentifier).Value, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                Repositorio = await _repositoriosQuery.GetRepositorioById(facturacion);
                Facturas = await _facturasQuery.GetFacturasByInmueble(inmueble, facturacion);
                Inmueble = await _inmuebles.GetInmuebleById(inmueble);
                Mes = await _mes.GetAsync(Repositorio.MesId);
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        public async Task<IActionResult> OnPostFacturas(CFDICreateCommand facturas)
        {

            if (facturas.XML != null && facturas.PDF != null)
            {
                CFDICreateCommand factura = new CFDICreateCommand();
                factura.Anio = facturas.Anio;
                factura.InmuebleId = facturas.InmuebleId;
                factura.Inmueble = facturas.Inmueble;
                factura.RepositorioId = facturas.RepositorioId;
                factura.Mes = facturas.Mes;
                factura.UsuarioId = facturas.UsuarioId;
                factura.TipoFacturacion = facturas.TipoFacturacion;
                factura.XML = facturas.XML;
                factura.PDF = facturas.PDF;
                var result= await _facturasCommand.CreateFactura(factura);

                return new JsonResult(result);
            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> OnGetVisualizarFactura(int cAnio, string cMes, string cFolio, string tipo, string cInmueble, string cArchivo)
        {
            string path = await _facturasQuery.VisualizarFactura(cAnio, cMes, cFolio, tipo, cInmueble, cArchivo);
            Stream stream = System.IO.File.Open(path, FileMode.Open);
            return File(stream, "application/pdf");
        }
        
        public async Task<IActionResult> OnGetDescargarXML(int cAnio, string cMes, string cFolio, string tipo, string cInmueble, string cArchivo)
        {
            string path = await _facturasQuery.VisualizarFactura(cAnio, cMes, cFolio, tipo, cInmueble, cArchivo);
            if (path != "")
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, cArchivo);
            }
            return BadRequest();
        }
    }
}
