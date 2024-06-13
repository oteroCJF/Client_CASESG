using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs.Mensajeria;
using Api.Gateway.Models.Firmantes.DTOs;
using Api.Gateway.Models.Incidencias.Mensajeria.DTOs;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.WebClient.Proxy.Mensajeria.CedulasEvaluacion.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Firmantes.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Incidencias.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Repositorios.Commands;
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

namespace Clients.WebClient.Pages.Mensajeria.CedulasEvaluacion
{
    public class MCedulaEvaluacionModel : PageModel
    {
        private readonly IQCedulaMensajeriaProxy _cedula;
        private readonly IQRepositorioMensajeriaProxy _repositorios;
        private readonly IQCFDIMensajeriaProxy _facturas;
        private readonly IQIncidenciaMensajeriaProxy _incidencias;
        private readonly IQFirmanteMensajeriaProxy _firmantes;

        public RepositorioDto Repositorio { get; set; }
        public CedulaMensajeriaDto Cedula { get; set; }
        public List<FirmanteDto> Firmantes { get; set; }

        public MCedulaEvaluacionModel(IQCedulaMensajeriaProxy cedula, IQRepositorioMensajeriaProxy repositorios, IQCFDIMensajeriaProxy facturas,
                                      IQIncidenciaMensajeriaProxy incidencias, IQFirmanteMensajeriaProxy firmantes)
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
            var path = Directory.GetCurrentDirectory() + "\\CedulasEvaluacion\\CedulaMensajeria.rdlc";
            local.ReportPath = path;
            List<MRespuestaDto> respuestas = (List<MRespuestaDto>)Cedula.respuestas;
            List<MIncidenciaDto> incidencias;
            for (var i = 0; i < respuestas.Count(); i++)
            {
                if (respuestas[i].ciMensajeria.Respuesta == respuestas[i].Respuesta)
                {
                    incidencias = await _incidencias.GetIncidenciasByPreguntaAndCedula(Cedula.Id, respuestas[i].Pregunta);

                    local.SetParameters(new[] { new ReportParameter("pregunta" + respuestas[i].Pregunta, respuestas[i].cuestionario.Pregunta) });

                    if (respuestas[i].cuestionario.Incidencias)
                    {
                        local.DataSources.Add(new ReportDataSource("IncidenciasP" + respuestas[i].Pregunta, GeneraIncidencias(incidencias)));
                        if (respuestas[i].Detalles.Equals("N/A"))
                        {
                            local.SetParameters(new[] { new ReportParameter("respuesta" + respuestas[i].Pregunta, "No aplica") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("respuesta" + respuestas[i].Pregunta, respuestas[i].ciMensajeria.RespuestaCedula) });
                        }
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaMensajeria", GeneraCedula(Cedula)));
            local.DataSources.Add(new ReportDataSource("Firmantes", GeneraFirmantes(Firmantes)));
            local.SetParameters(new[] { new ReportParameter("elaboro", Cedula.Usuario.NombreEmp + " " + Cedula.Usuario.PaternoEmp + " " + Cedula.Usuario.MaternoEmp + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        public DataTable GeneraCedula(CedulaMensajeriaDto cedula)
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
        
        public DataTable GeneraIncidencias(List<MIncidenciaDto> incidencias)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Tipo");
            dt.Columns.Add("FechaProgramada");
            dt.Columns.Add("FechaEntrega");
            dt.Columns.Add("NumeroGuia");
            dt.Columns.Add("CodigoRastreo");
            dt.Columns.Add("Acuse");
            dt.Columns.Add("TotalAcuses");
            dt.Columns.Add("Sobrepeso");
            dt.Columns.Add("TipoServicio");
            dt.Columns.Add("Observaciones");


            DataRow row;

            for (var i = 0; i < incidencias.Count(); i++)
            {
                row = dt.NewRow();
                row["Tipo"] = incidencias[i].Incidencia.Valor;
                row["FechaProgramada"] = Convert.ToDateTime(incidencias[i].FechaProgramada).ToString("dd/MM/yyyy");
                row["FechaEntrega"] = Convert.ToDateTime(incidencias[i].FechaEntrega).ToString("dd/MM/yyyy");
                row["NumeroGuia"] = incidencias[i].NumeroGuia;
                row["CodigoRastreo"] = incidencias[i].CodigoRastreo;
                row["Sobrepeso"] = incidencias[i].Sobrepeso;
                row["Acuse"] = incidencias[i].Acuse;
                row["TipoServicio"] = incidencias[i].TipoServicio;
                row["TotalAcuses"] = incidencias[i].TotalAcuses;
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
