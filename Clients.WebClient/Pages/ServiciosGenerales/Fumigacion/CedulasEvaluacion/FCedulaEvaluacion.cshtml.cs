using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs.Fumigacion;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs.Fumigacion;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.Models.Firmantes.DTOs;
using Api.Gateway.Models.Incidencias.Fumigacion.DTOs;
using Api.Gateway.Models.Incidencias.Fumigacion.DTOs;
using Api.Gateway.WebClient.Proxy.Fumigacion.CedulasEvaluacion;
using Api.Gateway.WebClient.Proxy.Fumigacion.Repositorios;
using Api.Gateway.WebClient.Proxy.Fumigacion.Facturas;
using Api.Gateway.WebClient.Proxy.Fumigacion.Firmantes;
using Api.Gateway.WebClient.Proxy.Fumigacion.Incidencias;
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

namespace Clients.WebClient.Pages.Fumigacion.CedulasEvaluacion
{
    public class FCedulaEvaluacionModel : PageModel
    {
        private readonly IFCedulaProxy _cedula;
        private readonly IFRepositorioProxy _repositorios;
        private readonly IFCFDIProxy _facturas;
        private readonly IFIncidenciaProxy _incidencias;
        private readonly IFFirmanteProxy _firmantes;

        public RepositorioDto Repositorio { get; set; }
        public CedulaFumigacionDto Cedula { get; set; }
        public List<FirmanteDto> Firmantes { get; set; }

        public FCedulaEvaluacionModel(IFCedulaProxy cedula, IFRepositorioProxy facturacion, IFCFDIProxy facturas, 
                                      IFIncidenciaProxy incidencias, IFFirmanteProxy firmantes)
        {
            _cedula = cedula;
            _repositorios = facturacion;
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
            var path = Directory.GetCurrentDirectory() + "\\CedulasEvaluacion\\CedulaFumigacion.rdlc";
            local.ReportPath = path;
            List<FRespuestaDto> respuestas = (List<FRespuestaDto>)Cedula.respuestas;
            List<FIncidenciaDto> incidencias;
            for (var i = 0; i < respuestas.Count(); i++)
            {
                if (respuestas[i].ciFumigacion.Respuesta == respuestas[i].Respuesta)
                {
                    incidencias = (List<FIncidenciaDto>)respuestas[i].iFumigacion;
                    local.SetParameters(new[] { new ReportParameter("pregunta" + respuestas[i].Pregunta,(i+1)+".- "+respuestas[i].cuestionario.Pregunta) });

                    if (respuestas[i].cuestionario.Incidencias)
                    {
                        local.DataSources.Add(new ReportDataSource("IncidenciasP" + respuestas[i].Pregunta, GeneraIncidencias(incidencias)));
                        local.SetParameters(new[] { new ReportParameter("respuesta" + respuestas[i].Pregunta, respuestas[i].ciFumigacion.RespuestaCedula) });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("respuesta" + respuestas[i].Pregunta, respuestas[i].ciFumigacion.RespuestaCedula) });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaFumigacion", GeneraCedula(Cedula)));
            local.DataSources.Add(new ReportDataSource("Firmantes", GeneraFirmantes(Firmantes)));
            local.SetParameters(new[] { new ReportParameter("elaboro", Cedula.Usuario.NombreEmp + " " + Cedula.Usuario.PaternoEmp + " " + Cedula.Usuario.MaternoEmp + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        public DataTable GeneraCedula(CedulaFumigacionDto cedula)
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
        
        public DataTable GeneraIncidencias(List<FIncidenciaDto> incidencias)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Tipo");
            dt.Columns.Add("Servicio");
            dt.Columns.Add("TipoFauna");
            dt.Columns.Add("Fauna");
            dt.Columns.Add("FechaProgramada");
            dt.Columns.Add("FechaRealizada");
            dt.Columns.Add("FechaReaparicion");
            dt.Columns.Add("MesId");
            dt.Columns.Add("Mes");
            dt.Columns.Add("HoraProgramada");
            dt.Columns.Add("HoraRealizada");
            dt.Columns.Add("DiasAtraso");
            dt.Columns.Add("HorasAtraso");
            dt.Columns.Add("Observaciones");


            DataRow row;

            for (var i = 0; i < incidencias.Count(); i++)
            {
                row = dt.NewRow();
                row["Tipo"] = incidencias[i].Incidencia.Valor;
                row["Servicio"] = incidencias[i].DIncidencia.Nombre;
                row["TipoFauna"] = incidencias[i].DIncidencia.Tipo;
                row["Fauna"] = incidencias[i].DIncidencia.Nombre;
                row["FechaProgramada"] = Convert.ToDateTime(incidencias[i].FechaProgramada).ToString("dd/MM/yyyy");
                row["FechaRealizada"] = Convert.ToDateTime(incidencias[i].FechaRealizada).ToString("dd/MM/yyyy");
                row["FechaReaparicion"] = Convert.ToDateTime(incidencias[i].FechaReaparicion).ToString("dd/MM/yyyy");
                row["HoraProgramada"] = incidencias[i].HoraProgramada;
                row["HoraRealizada"] = incidencias[i].HoraRealizada;
                row["DiasAtraso"] = incidencias[i].DiasAtraso;
                row["HorasAtraso"] = incidencias[i].HorasAtraso;
                row["MesId"] = incidencias[i].MesId;
                row["Mes"] = incidencias[i].Mes.Nombre;
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
