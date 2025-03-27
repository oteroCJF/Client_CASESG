using Api.Gateway.Models.Catalogos.DTOs.Parametros;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs.Agua;
using Api.Gateway.Models.Firmantes.DTOs;
using Api.Gateway.Models.Incidencias.Agua.DTOs;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.WebClient.Proxy.Agua.CedulasEvaluacion.Queries;
using Api.Gateway.WebClient.Proxy.Agua.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Firmantes.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Incidencias.Queries;
using Api.Gateway.WebClient.Proxy.Agua.Repositorios.Commands;
using Api.Gateway.WebClient.Proxy.Catalogos.CTParametros;
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

namespace Clients.WebClient.Pages.Agua.CedulasEvaluacion
{
    public class MCedulaEvaluacionModel : PageModel
    {
        private readonly IQCedulaAguaProxy _cedula;
        private readonly IQRepositorioAguaProxy _repositorios;
        private readonly IQCFDIAguaProxy _facturas;
        private readonly IQIncidenciaAguaProxy _incidencias;
        private readonly ICTParametroProxy _ctParametro;
        private readonly IQFirmanteAguaProxy _firmantes;

        public RepositorioDto Repositorio { get; set; }
        public CedulaAguaDto Cedula { get; set; }
        public List<FirmanteDto> Firmantes { get; set; }
        public List<CTParametroDto> Parametros { get; set; }

        public MCedulaEvaluacionModel(IQCedulaAguaProxy cedula, IQRepositorioAguaProxy repositorios, IQCFDIAguaProxy facturas,
                                      IQIncidenciaAguaProxy incidencias, IQFirmanteAguaProxy firmantes, ICTParametroProxy ctParametro)
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
            //if (Cedula.Penalizacion == Convert.ToDecimal(0.00) && Cedula.Calificacion == Convert.ToDecimal(10.0))
            //{
            //    path = Directory.GetCurrentDirectory() + "\\CedulasEvaluacion\\CedulaAgua.rdlc";
            //}
            //else
            //{
            //    path = Directory.GetCurrentDirectory() + "\\CedulasEvaluacion\\CedulaAgua_SinIncidencia.rdlc";
            //}
            var path = Directory.GetCurrentDirectory() + "\\CedulasEvaluacion\\CedulaEvaluacionAgua.rdlc";
            local.ReportPath = path;
            local.DataSources.Add(new ReportDataSource("Cuestionario", GeneraCuestionario(Cedula, Parametros)));

            local.DataSources.Add(new ReportDataSource("CedulaComedor", GeneraCedula(Cedula)));
            local.DataSources.Add(new ReportDataSource("Firmantes", GeneraFirmantes(Firmantes)));
            local.SetParameters(new[] { new ReportParameter("elaboro", Cedula.Usuario.NombreEmp + " " + Cedula.Usuario.PaternoEmp + " " + Cedula.Usuario.MaternoEmp + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        public DataTable GeneraCedula(CedulaAguaDto cedula)
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

        public DataTable GeneraCuestionario(CedulaAguaDto cedula, List<CTParametroDto> parametros)
        {
            DataTable dt = new DataTable();
            var Cuestionarios = Cedula.respuestas;
            dt.Columns.Add("Consecutivo");
            dt.Columns.Add("Pregunta");
            dt.Columns.Add("Respuesta");
            dt.Columns.Add("Incidencia");
            dt.Columns.Add("DIncidencia");
            dt.Columns.Add("FechaProgramada");
            dt.Columns.Add("FechaEntrega");
            dt.Columns.Add("HoraProgramada");
            dt.Columns.Add("HoraRealizada");
            dt.Columns.Add("Cantidad");
            dt.Columns.Add("Observaciones");
            dt.Columns.Add("MTipo");
            dt.Columns.Add("MFechaProgramada");
            dt.Columns.Add("MFechaEntrega");
            dt.Columns.Add("MHoraProgramada");
            dt.Columns.Add("MHoraRealizada");
            dt.Columns.Add("MCantidad");
            dt.Columns.Add("MObservaciones");

            DataRow row;

            foreach (var rs in Cuestionarios)
            {
                if (rs.iAgua.Count() == 0)
                {
                    row = dt.NewRow();
                    row["Consecutivo"] = Convert.ToInt32(rs.cuestionario.Consecutivo);
                    row["Pregunta"] = rs.cuestionario.Pregunta;
                    row["Respuesta"] = rs.ciAgua.RespuestaCedula;
                    row["Incidencia"] = "";
                    row["DIncidencia"] = "";
                    row["FechaProgramada"] = "1990-01-01";
                    row["FechaEntrega"] = "1990-01-01";
                    row["HoraProgramada"] = "00:00:00.000";
                    row["HoraRealizada"] = "00:00:00.000";
                    row["Cantidad"] = 0;
                    row["Observaciones"] = "";
                    row["MFechaProgramada"] = false;
                    row["MFechaEntrega"] = false;
                    row["MHoraProgramada"] = false;
                    row["MHoraRealizada"] = false;
                    row["MCantidad"] = false;
                    row["MObservaciones"] = false;
                    dt.Rows.Add(row);
                }
                else
                {
                    foreach (var ic in rs.iAgua)
                    {
                        if (ic.Pregunta == rs.Pregunta)
                        {
                            row = dt.NewRow();
                            row["Consecutivo"] = Convert.ToInt32(rs.cuestionario.Consecutivo);
                            row["Pregunta"] = rs.cuestionario.Pregunta;
                            row["Respuesta"] = rs.ciAgua.RespuestaCedula;
                            row["Incidencia"] = ic.Incidencia.Valor;
                            row["DIncidencia"] = ic.DIncidencia.Nombre;
                            row["FechaProgramada"] = ic.FechaProgramada.ToString("dd/MM/yyyy");
                            row["FechaEntrega"] = ic.FechaEntrega.ToString("dd/MM/yyyy");
                            row["HoraProgramada"] = ic.HoraProgramada;
                            row["HoraRealizada"] = ic.HoraRealizada;
                            row["Cantidad"] = ic.Cantidad;
                            row["Observaciones"] = ic.Observaciones;
                            row["MTipo"] = ic.ciAgua.Tipo;
                            row["MFechaProgramada"] = ic.ciAgua.FechaProgramada;
                            row["MFechaEntrega"] = ic.ciAgua.FechaEntrega;
                            row["MHoraProgramada"] = ic.ciAgua.HoraProgramada;
                            row["MHoraRealizada"] = ic.ciAgua.HoraRealizada;
                            row["MCantidad"] = ic.ciAgua.Cantidad;
                            row["MObservaciones"] = ic.ciAgua.Observaciones;
                            dt.Rows.Add(row);
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
