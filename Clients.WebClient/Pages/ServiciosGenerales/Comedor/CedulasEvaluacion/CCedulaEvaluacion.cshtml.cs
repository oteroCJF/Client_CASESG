using Api.Gateway.Models.Catalogos.DTOs.Parametros;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs.Comedor;
using Api.Gateway.Models.Firmantes.DTOs;
using Api.Gateway.Models.Incidencias.Comedor.DTOs;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTParametros;
using Api.Gateway.WebClient.Proxy.Comedor.CedulasEvaluacion.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Firmantes;
using Api.Gateway.WebClient.Proxy.Comedor.Firmantes.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Incidencias;
using Api.Gateway.WebClient.Proxy.Comedor.Incidencias.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Repositorios.Commands;
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

namespace Clients.WebClient.Pages.Comedor.CedulasEvaluacion
{
    public class CCedulaEvaluacionModel : PageModel
    {
        private readonly IQCedulaComedorProxy _cedula;
        private readonly IQRepositorioComedorProxy _repositorios;
        private readonly IQCFDIComedorProxy _facturas;
        private readonly IQIncidenciaComedorProxy _incidencias;
        private readonly IQFirmanteComedorProxy _firmantes;
        private readonly ICTParametroProxy _ctParametro;

        public RepositorioDto Repositorio { get; set; }
        public CedulaComedorDto Cedula { get; set; }
        public List<FirmanteDto> Firmantes { get; set; }
        public List<CTParametroDto> Parametros{ get; set; }

        public CCedulaEvaluacionModel(IQCedulaComedorProxy cedula, IQRepositorioComedorProxy repositorios, IQCFDIComedorProxy facturas,
                                      IQIncidenciaComedorProxy incidencias, IQFirmanteComedorProxy firmantes, ICTParametroProxy ctParametro)
        {
            _cedula = cedula;
            _repositorios = repositorios;
            _facturas = facturas;
            _incidencias = incidencias;
            _firmantes = firmantes;
            _ctParametro = ctParametro;
        }

        public async Task<IActionResult> OnGet(int moduloId, int submoduloId, int facturacion, int inmueble, int cedula)
        {
            Repositorio = await _repositorios.GetRepositorioById(facturacion);
            Cedula = await _cedula.GetCedulaById(cedula);
            Firmantes = await _firmantes.GetFirmantesByInmueble(Cedula.InmuebleId);
            Parametros = await _ctParametro.GetParametroByTipo("Categoria");

            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\CedulasEvaluacion\\CedulaComedor2.rdlc";
            local.ReportPath = path;
            local.DataSources.Add(new ReportDataSource("Cuestionario", GeneraCuestionario(Cedula, Parametros)));
            
            local.DataSources.Add(new ReportDataSource("CedulaComedor", GeneraCedula(Cedula)));
            local.DataSources.Add(new ReportDataSource("Firmantes", GeneraFirmantes(Firmantes)));
            local.SetParameters(new[] { new ReportParameter("elaboro", Cedula.Usuario.NombreEmp + " " + Cedula.Usuario.PaternoEmp + " " + Cedula.Usuario.MaternoEmp + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        public DataTable GeneraCedula(CedulaComedorDto cedula)
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

        public DataTable GeneraCuestionario(CedulaComedorDto cedula, List<CTParametroDto> parametros)
        {
            DataTable dt = new DataTable();
            var Cuestionarios = Cedula.Cuestionario.OrderBy(c => c.Id);
            dt.Columns.Add("Consecutivo");
            dt.Columns.Add("Pregunta");
            dt.Columns.Add("Respuesta");
            dt.Columns.Add("Categoria");
            dt.Columns.Add("Incidencia");
            dt.Columns.Add("FechaIncidencia");
            dt.Columns.Add("FechaProgramada");
            dt.Columns.Add("UltimoDia");
            dt.Columns.Add("FechaRealizada");
            dt.Columns.Add("FechaInventario");
            dt.Columns.Add("FechaNotificacion");
            dt.Columns.Add("FechaAcordadaAdmin");
            dt.Columns.Add("FechaEntrega");
            dt.Columns.Add("FechaLimite");
            dt.Columns.Add("EntregaEnseres");
            dt.Columns.Add("HoraInicio");
            dt.Columns.Add("HoraReal");
            dt.Columns.Add("Cantidad");
            dt.Columns.Add("Observaciones");
            dt.Columns.Add("MFechaIncidencia");
            dt.Columns.Add("MFechaProgramada");
            dt.Columns.Add("MUltimoDia");
            dt.Columns.Add("MFechaRealizada");
            dt.Columns.Add("MFechaInventario");
            dt.Columns.Add("MFechaNotificacion");
            dt.Columns.Add("MFechaAcordadaAdmin");
            dt.Columns.Add("MFechaEntrega");
            dt.Columns.Add("MFechaLimite");
            dt.Columns.Add("MEntregaEnseres");
            dt.Columns.Add("MHoraInicio");
            dt.Columns.Add("MHoraReal");
            dt.Columns.Add("MCantidad");
            dt.Columns.Add("MObservaciones");
            dt.Columns.Add("TipoPregunta");

            DataRow row;

            foreach (var ct in Cuestionarios)
            {
                foreach (var rs in ct.Respuestas) 
                {
                    if (rs.iComedor.Count() == 0)
                    {
                        row = dt.NewRow();
                        row["Consecutivo"] = Convert.ToInt32(rs.cuestionario.Consecutivo);
                        row["Pregunta"] = rs.cuestionario.Pregunta;
                        row["Respuesta"] = (rs.Detalles.Equals("N/A") ? "La captura de incidencias no aplica para esta evaluación." : rs.Detalles.Equals("N/R") ?
                                            "No se realizó la fumigación en esta evaluación.": rs.ciComedor.RespuestaCedula);
                        row["Categoria"] = parametros.SingleOrDefault(p => p.Id == rs.cuestionario.CategoriaId).Nombre;
                        row["Incidencia"] = "";
                        row["FechaIncidencia"] = "1990-01-01";
                        row["FechaProgramada"] = "1990-01-01";
                        row["UltimoDia"] = "1990-01-01";
                        row["FechaRealizada"] = "1990-01-01";
                        row["FechaInventario"] = "1990-01-01";
                        row["FechaNotificacion"] = "1990-01-01";
                        row["FechaAcordadaAdmin"] = "1990-01-01";
                        row["FechaEntrega"] = "1990-01-01";
                        row["FechaLimite"] = "1990-01-01";
                        row["EntregaEnseres"] = false;
                        row["HoraInicio"] = "00:00:00.000";
                        row["HoraReal"] = "00:00:00.000";
                        row["Cantidad"] = 0;
                        row["Observaciones"] = "";
                        row["MFechaIncidencia"] = false;
                        row["MFechaProgramada"] = false;
                        row["MUltimoDia"] = false;
                        row["MFechaRealizada"] = false;
                        row["MFechaInventario"] = false;
                        row["MFechaNotificacion"] = false;
                        row["MFechaAcordadaAdmin"] = false;
                        row["MFechaEntrega"] = false;
                        row["MFechaLimite"] = false;
                        row["MEntregaEnseres"] = false;
                        row["MHoraInicio"] = false;
                        row["MHoraReal"] = false;
                        row["MCantidad"] = false;
                        row["MObservaciones"] = false;
                        row["TipoPregunta"] = "";
                        dt.Rows.Add(row);
                    }
                    else 
                    {
                        foreach (var ic in rs.iComedor)
                        {
                            if (ic.Pregunta == rs.Pregunta)
                            {
                                row = dt.NewRow();
                                row["Consecutivo"] = Convert.ToInt32(rs.cuestionario.Consecutivo);
                                row["Pregunta"] = rs.cuestionario.Pregunta;
                                row["Respuesta"] = rs.ciComedor.RespuestaCedula;
                                row["Categoria"] = parametros.SingleOrDefault(p => p.Id == rs.cuestionario.CategoriaId).Nombre;
                                row["Incidencia"] = ic.Incidencia.Valor;
                                row["FechaIncidencia"] = ic.FechaIncidencia.ToString("dd/MM/yyyy");
                                row["FechaProgramada"] = ic.FechaProgramada.ToString("dd/MM/yyyy");
                                row["UltimoDia"] = ic.UltimoDia.ToString("dd/MM/yyyy");
                                row["FechaRealizada"] = ic.FechaRealizada.ToString("dd/MM/yyyy");
                                row["FechaInventario"] = ic.FechaInventario.ToString("dd/MM/yyyy");
                                row["FechaNotificacion"] = ic.FechaNotificacion.ToString("dd/MM/yyyy");
                                row["FechaAcordadaAdmin"] = ic.FechaAcordadaAdmin.ToString("dd/MM/yyyy");
                                row["FechaEntrega"] = ic.FechaEntrega.ToString("dd/MM/yyyy");
                                row["FechaLimite"] = ic.FechaLimite.ToString("dd/MM/yyyy");
                                row["EntregaEnseres"] = ic.EntregaEnseres;
                                row["HoraInicio"] = ic.HoraInicio.Substring(0,8);
                                row["HoraReal"] = ic.HoraReal.Substring(0, 8);
                                row["Cantidad"] = ic.Cantidad;
                                row["MFechaIncidencia"] = ic.ciComedor.FechaIncidencia;
                                row["MFechaProgramada"] = ic.ciComedor.FechaProgramada;
                                row["MUltimoDia"] = ic.ciComedor.UltimoDia;
                                row["MFechaRealizada"] = ic.ciComedor.FechaRealizada;
                                row["MFechaInventario"] = ic.ciComedor.FechaInventario;
                                row["MFechaNotificacion"] = ic.ciComedor.FechaNotificacion;
                                row["MFechaAcordadaAdmin"] = ic.ciComedor.FechaAcordadaAdmin;
                                row["MFechaEntrega"] = ic.ciComedor.FechaEntrega;
                                row["MFechaLimite"] = ic.ciComedor.FechaLimite;
                                row["MEntregaEnseres"] = ic.ciComedor.EntregaEnseres;
                                row["MHoraInicio"] = ic.ciComedor.HoraInicio;
                                row["MHoraReal"] = ic.ciComedor.HoraReal;
                                row["MCantidad"] = ic.ciComedor.Cantidad;
                                row["MObservaciones"] = ic.ciComedor.Observaciones;
                                row["TipoPregunta"] = rs.cuestionario.Tipo;
                                if (ic.DIncidencias.Count() != 0)
                                {
                                    string detalle = "";
                                    foreach (var dtic in ic.DIncidencias)
                                    {
                                        detalle += "- "+dtic.DIncidencia.Nombre + "<br>";
                                    }
                                    row["Observaciones"] = "<b>Conceptos: </b><br>"+ detalle + "<br><b>Observaciones: </b><br>"+ic.Observaciones;
                                }
                                else
                                {
                                    row["Observaciones"] = ic.Observaciones;
                                }
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
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

            DataRow row = dt.NewRow();

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
