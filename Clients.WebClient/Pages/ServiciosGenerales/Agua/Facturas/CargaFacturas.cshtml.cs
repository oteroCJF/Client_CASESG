using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.CFDIs.ServiciosGenerales.Commands;
using Api.Gateway.Models.CFDIs.ServiciosGenerales.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Agua.CFDIs.Commands;
using Api.Gateway.WebClient.Proxy.Agua.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Repositorios.Commands;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spire.Xls;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Agua.Facturas
{
    public class CargaFacturasModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IQRepositorioAguaProxy _repositorios;
        private readonly IQCFDIAguaProxy _facturasQuery;
        private readonly ICCFDIAguaProxy _facturasCommand;
        private readonly ICTServicioProxy _servicios;
        private readonly IPermisoProxy _permisos;

        public int Anio { get; set; }
        public ModuloDto Modulo { get; set; }
        public SubmoduloDto Submodulo { get; set; }
        public List<int> InmueblesServicio { get; set; } = new List<int>();
        public List<InmuebleDto> Inmuebles { get; set; }
        public RepositorioDto Repositorio { get; set; }
        public CTServicioDto Servicio { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }

        public CargaFacturasModel(IModuloProxy modulo, IInmuebleProxy inmuebles, IQRepositorioAguaProxy repositorios, 
                                  IQCFDIAguaProxy facturasQuery, ICCFDIAguaProxy facturasCommand, ICTServicioProxy servicios, 
                                  IPermisoProxy permisos)
        {
            _modulo = modulo;
            _inmuebles = inmuebles;
            _repositorios = repositorios;
            _facturasQuery = facturasQuery;
            _facturasCommand = facturasCommand;
            _servicios = servicios;
            _permisos = permisos;
        }

        public async Task OnGet(int moduloId, int submoduloId, int facturacion)
        {
            Permisos = await _permisos.GetPermisosByModuloUsuario(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                InmueblesServicio = (await _inmuebles.GetInmueblesByServicio((int)Modulo.ServicioId)).Select(i => i.InmuebleId).ToList();
                Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesServicio.Contains(i.Id)).ToList();
                Repositorio = await _repositorios.GetRepositorioById(facturacion);
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        public async Task<IActionResult> OnPostFacturas(CFDICommand facturas, ICollection<IFormFile> files)
        {
            int statusFactura = 0;

            if (!facturas.File.ContentType.Contains("pdf"))
            {
                CFDICreateCommand factura = new CFDICreateCommand();
                factura.Anio = facturas.Anio;
                factura.InmuebleId = facturas.InmuebleId;
                factura.Inmueble = facturas.Inmueble;
                factura.RepositorioId = facturas.RepositorioId;
                factura.Mes = facturas.Mes;
                factura.UsuarioId = facturas.UsuarioId;
                factura.TipoFacturacion = facturas.TipoFacturacion;
                factura.XML = files.FirstOrDefault();
                statusFactura = (await _facturasCommand.CreateFactura(factura)).EstatusId;
            }
            else if (facturas.File.ContentType.Contains("pdf"))
            {
                CFDIUpdateCommand factura = new CFDIUpdateCommand();
                factura.Anio = facturas.Anio;
                factura.Mes = facturas.Mes;
                factura.Inmueble = facturas.Inmueble;
                factura.PDF = files.FirstOrDefault();
                statusFactura = (await _facturasCommand.UpdateFactura(factura)).EstatusId;
            }
            else
            {
                return BadRequest();
            }

            return StatusCode(statusFactura);
        }

        public async Task<IActionResult> OnGetReporteFacturas(int facturacionId)
        {
            Workbook workbook = new Workbook();
            workbook.Worksheets.Clear();
            var sheet = workbook.Worksheets.Add("Sheet1");
            sheet.Range["A1:I20000"].Style.Font.Size = 10;
            sheet.Range["A1:I1"].Style.Font.IsBold = true; //set the font bold
            sheet.Range["A1:I1"].ColumnWidth = 20; 
            sheet.Range["A1:I1"].Style.Font.FontName = "Calibri"; 
            List< HistorialMFDto> historial = await _facturasQuery.GetHistorialMFByFacturacion(facturacionId);
            DataTable dt = CreateTable(historial);
            sheet.InsertDataTable(dt, true, 1, 1);

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                workbook.SaveToStream(ms1, FileFormat.Version2016);
                toArray = ms1.ToArray();
            }

            return File(toArray, "application/vnd.ms-excel", "Reporte de Carga Masiva.xlsx");
        }

        private static DataTable CreateTable(List<HistorialMFDto> historial)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("A�o", typeof(int));
            dt.Columns.Add("Mes", typeof(string));
            dt.Columns.Add("Inmueble", typeof(string));
            dt.Columns.Add("Elabor�", typeof(string));
            dt.Columns.Add("TipoArchivo", typeof(string));
            dt.Columns.Add("ArchivoXML", typeof(string));
            dt.Columns.Add("ArchivoPDF", typeof(string));
            dt.Columns.Add("Observaciones", typeof(string));
            dt.Columns.Add("FechaCreacion", typeof(string));
            foreach (var hs in historial)
            {
                dt.Rows.Add(hs.Anio, hs.Mes, hs.Inmueble, hs.Usuario, hs.TipoArchivo, hs.ArchivoXML, hs.ArchivoPDF, hs.Observaciones, hs.FechaCreacion);
            }
            return dt;
        }
    }
}
