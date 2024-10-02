using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Gateway.Models;
using Api.Gateway.Models.Catalogos.DTOs.Entregables;
using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs.Mensajeria;
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
using Api.Gateway.WebClient.Proxy.Comedor.CedulasEvaluacion.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Contratos;
using Api.Gateway.WebClient.Proxy.Comedor.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Entregables;
using Api.Gateway.WebClient.Proxy.Comedor.Entregables.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Entregables.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Oficios;
using Api.Gateway.WebClient.Proxy.Comedor.Oficios.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Oficios.Queries;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;

namespace Clients.WebClient.Pages.Financieros.Comedor
{
    public class ReporteModel : PageModel
    {
        private readonly ICTServicioProxy _servicios;
        private readonly IModuloProxy _modulos;
        private readonly IPermisoProxy _permisos;

        private readonly IQContratoComedorProxy _contratosQuery;
        private readonly IQCedulaComedorProxy _cedulas;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IMesProxy _meses;


        public List<ContratoDto> Contratos = new List<ContratoDto>();
        public CTServicioDto Servicio { get; set; } = new CTServicioDto();
        public List<InmuebleDto> Inmuebles { get; set; } = new List<InmuebleDto>();
        public List<MesDto> Meses { get; set; } = new List<MesDto>();
        public List<int> Permisos { get; set; } = new List<int>();
        public ModuloDto Modulo { get; set; } = new ModuloDto();

        public List<EstatusDto> CTEstatus = new List<EstatusDto>();
        public List<int?> Modulos { get; set; } = new List<int?>();

        public ReporteModel(ICTServicioProxy servicios, IModuloProxy modulos, IPermisoProxy permisos, IQContratoComedorProxy contratosQuery, IQCedulaComedorProxy cedulas, IInmuebleProxy inmuebles, IMesProxy meses)
        {
            _servicios = servicios;
            _modulos = modulos;
            _permisos = permisos;
            _contratosQuery = contratosQuery;
            _cedulas = cedulas;
            _inmuebles = inmuebles;
            _meses = meses;
        }

        public async Task<IActionResult> OnGet(int moduloId, int servicioId, int anio, int mesId)
        {
            DataCollection<CedulaEvaluacionDto> cedulas = await _cedulas.GetReportePAT(anio, mesId);
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\CedulasEvaluacion\\ReportePAT.rdlc";
            var mes = (await _meses.GetAllAsync()).Single(m => m.Id == mesId).Nombre;
            var table = await GeneraReporte(cedulas, servicioId);
            local.ReportPath = path;
            local.DataSources.Add(new ReportDataSource("ReportePAT", table));
            local.SetParameters(new[] { new ReportParameter("mes", mes.ToString()) });
            local.SetParameters(new[] { new ReportParameter("anio", anio.ToString()) });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        public async Task<DataTable> GeneraReporte(DataCollection<CedulaEvaluacionDto> cedulas, int servicio)
        {
            var servicios = await _servicios.GetAllCatalogoServiciosAsync();
            var contratos = await _contratosQuery.GetAllAsync();
            DataTable dt = new DataTable();
            dt.Columns.Add("No");
            dt.Columns.Add("Servicio");
            dt.Columns.Add("Inmueble");
            dt.Columns.Add("Folio");
            dt.Columns.Add("Mes");
            dt.Columns.Add("Anio");
            dt.Columns.Add("Empresa");

            DataRow row;
            int c = 0;

            foreach (var cd in cedulas.Items)
            {
                c++;
                row = dt.NewRow();
                row["No"] = c;
                row["Servicio"] = servicios.Single(s => s.Id == servicio).Nombre;
                row["Inmueble"] = cd.Inmueble;
                row["Folio"] = cd.Folio;
                row["Anio"] = cd.Anio;
                row["Mes"] = cd.Mes;
                row["Empresa"] = contratos.Single(c => c.Id == cd.ContratoId).Empresa;
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}

