using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs.Limpieza;
using Api.Gateway.Models.Firmantes.DTOs;
using Api.Gateway.Models.Incidencias.Limpieza.DTOs;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.WebClient.Proxy.Limpieza.CedulaEvaluacion;
using Api.Gateway.WebClient.Proxy.Limpieza.Facturas;
using Api.Gateway.WebClient.Proxy.Limpieza.Firmantes;
using Api.Gateway.WebClient.Proxy.Limpieza.Incidencias;
using Api.Gateway.WebClient.Proxy.Limpieza.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Limpieza.CedulasEvaluacion
{
    public class LCedulaEvaluacionModel : PageModel
    {
        private readonly ILCedulaProxy _cedula;
        private readonly ILRepositorioProxy _repositorios;
        private readonly ILCFDIProxy _facturas;
        private readonly ILIncidenciaProxy _incidencias;
        private readonly ILFirmanteProxy _firmantes;

        public RepositorioDto Repositorio { get; set; }
        public CedulaLimpiezaDto Cedula { get; set; }
        public List<FirmanteDto> Firmantes { get; set; }

        public LCedulaEvaluacionModel(ILCedulaProxy cedula, ILRepositorioProxy repositorios, ILCFDIProxy facturas, ILIncidenciaProxy incidencias, 
                                             ILFirmanteProxy firmantes)
        {
            _cedula = cedula;
            _repositorios = repositorios;
            _facturas = facturas;
            _incidencias = incidencias;
            _firmantes = firmantes;
        }

        public async Task<IActionResult> OnGet(int moduloId, int submoduloId, int facturacion, int inmueble, int cedula)
        {
            Repositorio = await _repositorios.GetRepositorioById(facturacion);
            Cedula = await _cedula.GetCedulaById(cedula);
            Firmantes = await _firmantes.GetFirmantesByInmueble(Cedula.InmuebleId);

            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\CedulasEvaluacion\\CedulaLimpieza.rdlc";
            local.ReportPath = path;
            List<LRespuestaDto> respuestas = (List<LRespuestaDto>)Cedula.respuestas;
            List<LIncidenciaDto> incidencias;
            for (var i = 0; i < respuestas.Count(); i++)
            {
                if (respuestas[i].ciLimpieza.Respuesta == respuestas[i].Respuesta)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + respuestas[i].Pregunta, respuestas[i].Pregunta+".-"+respuestas[i].cuestionario.Pregunta) });

                    if (respuestas[i].cuestionario.Incidencias)
                    {
                        incidencias = (List<LIncidenciaDto>)respuestas[i].iLimpieza;
                        local.DataSources.Add(new ReportDataSource("IncidenciasP" + respuestas[i].Pregunta, GeneraIncidencias(incidencias)));
                        local.SetParameters(new[] { new ReportParameter("respuesta" + respuestas[i].Pregunta, respuestas[i].ciLimpieza.RespuestaCedula) });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaLimpieza", GeneraCedula(Cedula)));
            local.DataSources.Add(new ReportDataSource("Firmantes", GeneraFirmantes(Firmantes)));
            local.SetParameters(new[] { new ReportParameter("elaboro", Cedula.Usuario.NombreEmp + " " + Cedula.Usuario.PaternoEmp + " " + Cedula.Usuario.MaternoEmp + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        public DataTable GeneraCedula(CedulaLimpiezaDto cedula)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Administracion");
            dt.Columns.Add("Inmueble");
            dt.Columns.Add("Folio");
            dt.Columns.Add("Mes");
            dt.Columns.Add("Anio");
            dt.Columns.Add("Estatus");
            dt.Columns.Add("Calificacion");
            dt.Columns.Add("FechaCreacion");
            dt.Columns.Add("Elaboro");


            DataRow row = dt.NewRow();

            row["Id"] = cedula.Id;
            row["Administracion"] = cedula.Inmueble.Descripcion;
            row["Inmueble"] = cedula.Inmueble.Nombre;
            row["Folio"] = cedula.Folio;
            row["Mes"] = cedula.Mes.Nombre;
            row["Anio"] = cedula.Anio;
            row["Estatus"] = cedula.Estatus.Nombre;
            row["Calificacion"] = cedula.Calificacion;
            row["FechaCreacion"] = cedula.FechaCreacion;
            row["Elaboro"] = cedula.Usuario.NombreEmp + " " + cedula.Usuario.PaternoEmp + " " + cedula.Usuario.MaternoEmp;

            dt.Rows.Add(row);
            return dt;
        }
        
        public DataTable GeneraIncidencias(List<LIncidenciaDto> incidencias)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Tipo");
            dt.Columns.Add("Area");
            dt.Columns.Add("Inasistencias");
            dt.Columns.Add("Mes");
            dt.Columns.Add("FechaIncidencia");
            dt.Columns.Add("Observaciones");


            DataRow row;

            for (var i = 0; i < incidencias.Count(); i++)
            {
                row = dt.NewRow();
                row["Tipo"] = incidencias[i].Incidencia.Incidencia.Valor;
                row["Area"] = incidencias[i].Incidencia.Nombre;                
                row["Mes"] = incidencias[i].Mes.Nombre;                
                row["Inasistencias"] = incidencias[i].Inasistencias;                
                row["FechaIncidencia"] = Convert.ToDateTime(incidencias[i].FechaIncidencia).ToString("dd/MM/yyyy");
                row["Observaciones"] = incidencias[i].Observaciones;
                dt.Rows.Add(row);
            }
            return dt;
        }
        
        public DataTable GeneraFirmantes(List<FirmanteDto> firmantes)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Reviso");
            dt.Columns.Add("PuestoReviso");
            dt.Columns.Add("Superviso");
            dt.Columns.Add("PuestoSuperviso");
            dt.Columns.Add("Autoriza");
            dt.Columns.Add("PuestoAutoriza");

            var Reviso = firmantes.SingleOrDefault(f => f.Tipo.Equals("Reviso"));
            var Superviso = firmantes.SingleOrDefault(f => f.Tipo.Equals("Superviso"));

            DataRow row= dt.NewRow();

            row["Reviso"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.TextInfo.ToLower(Reviso.Escolaridad + " " + Reviso.Usuario.NombreEmp + " " + Reviso.Usuario.PaternoEmp + " " + Reviso.Usuario.MaternoEmp));
            row["PuestoReviso"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.TextInfo.ToLower(Reviso.Usuario.Puesto));
            row["Superviso"] = Superviso.Escolaridad + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.TextInfo.ToLower(Superviso.Usuario.NombreEmp + " " + Superviso.Usuario.PaternoEmp + " " + Superviso.Usuario.MaternoEmp));
            row["PuestoSuperviso"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.TextInfo.ToLower(Superviso.Usuario.Puesto));
            row["Autoriza"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.TextInfo.ToLower(Reviso.Inmueble.Administrador));
            row["PuestoAutoriza"] = Reviso.Inmueble.DescripcionAdministrador;
            dt.Rows.Add(row);

            return dt;
        }
    }
}
