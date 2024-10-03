using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Globalization;
using Spire.Doc;
using System;
using Api.Gateway.Models.Contratos.DTOs;
using Api.Gateway.Models.CFDIs.ServiciosGenerales.DTOs;
using Api.Gateway.Models.Entregables.ServiciosGenerales.DTOs.Cedulas;
using Api.Gateway.Models.Firmantes.DTOs;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.Commands.Respuestas;
using Api.Gateway.Models.Entregables.ServiciosGenerales.Commands.Cedulas;
using Api.Gateway.Models.LogsCedulas.Commands;
using Api.Gateway.Models.LogEntregables.Commands;
using Api.Gateway.WebClient.Proxy.Catalogos.CTEntregables;
using Api.Gateway.WebClient.Proxy.Mensajeria.Variables;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Incidencias.Mensajeria.DTOs;
using Api.Gateway.Models.Incidencias.Mensajeria.Commands;
using Api.Gateway.WebClient.Proxy.Catalogos.CTIncidencias;
using Api.Gateway.Models.Estatus.DTOs.EstatusCedulas;
using Api.Gateway.Models.Catalogos.DTOs.Incidencias;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.Commands.CedulasEvaluacion;
using Api.Gateway.Models.Estatus.DTOs.EstatusEntregables;
using Api.Gateway.Models.Inmuebles.DTOs.InmueblesUS;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs.Mensajeria;
using Spire.Xls;
using System.Data;
using Api.Gateway.Models.Firmantes.Commands;
using Api.Gateway.WebClient.Proxy.Usuarios;
using Api.Gateway.Models.Usuarios.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTIndemnizaciones;
using Api.Gateway.Models.Catalogos.DTOs.Indemnizaciones;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.WebClient.Proxy.Mensajeria.Flujo.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.CedulasEvaluacion.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Entregables.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Respuestas.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Incidencias.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.CedulasEvaluacion.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Entregables.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Incidencias.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.SoportePago.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Firmantes.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.Firmantes.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.Repositorios.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.LogCedulas.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.LogEntregables.Queries;
using Api.Gateway.WebClient.Proxy.Mensajeria.LogCedulas.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.LogEntregables.Commands;
using Api.Gateway.WebClient.Proxy.Mensajeria.SoportePago.Commands;
using Api.Gateway.Models.Entregables.ServiciosGenerales.Commands.Cedulas.Update;
using Api.Gateway.Models.Estatus.DTOs.EntregablesEstatus;

namespace Clients.WebClient.Pages.Mensajeria.CedulasEvaluacion
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DetalleModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly IPermisoProxy _permisos;
        private readonly ICTEntregableProxy _centregable;
        private readonly ICTIncidenciaProxy _cincidencias;
        private readonly IEstatusCedulaProxy _estatusc;
        private readonly IEstatusEntregableProxy _estatuse;
        private readonly IQFlujoMensajeriaProxy _flujo;
        private readonly IQCFDIMensajeriaProxy _facturasQuery;
        private readonly IInmuebleProxy _inmuebles;
        
        private readonly IQCedulaMensajeriaProxy _cedulaQuery;
        private readonly ICCedulaMensajeriaProxy _cedulaCommand;
        
        private readonly IQEntregableMensajeriaProxy _entregablesQuery;
        private readonly ICEntregableMensajeriaProxy _entregablesCommand;
        
        private readonly ICRespuestaMensajeriaProxy _respuestas;
        
        private readonly IQIncidenciaMensajeriaProxy _incidenciasQuery;
        private readonly ICIncidenciaMensajeriaProxy _incidenciasCommand;

        private readonly IQSoportePagoMensajeriaProxy _soporteQuery;
        private readonly ICSoportePagoMensajeriaProxy _soporteCommand;

        private readonly IQFirmanteMensajeriaProxy _firmantesQuery;
        private readonly ICFirmanteMensajeriaProxy _firmantesCommand;

        private readonly IQRepositorioMensajeriaProxy _repositoriosQuery;

        private readonly IUsuarioProxy _usuarios;

        private readonly IQLCedulaMensajeriaProxy _logCQuery;
        private readonly ICLCedulaMensajeriaProxy _logCCommand;

        private readonly IQLEntregableMensajeriaProxy _logEQuery;
        private readonly ICLEntregableMensajeriaProxy _logECommand;

        private readonly ICTIndemnizacionProxy _indemnizacion;


        public SubmoduloDto Submodulo { get; set; }
        public ModuloDto Modulo { get; set; }
        public int MesId { get; set; }
        public int FacturacionId { get; set; }
        public int InmuebleId { get; set; }
        public IFormFile XML { get; set; }
        public MesDto Mes { get; set; } = new MesDto();
        public InmuebleDto Inmueble { get; set; } = new InmuebleDto();
        public RepositorioDto Repositorio { get; set; } = new RepositorioDto();
        public ContratoDto Contrato { get; set; } = new ContratoDto();
        public List<MIncidenciaDto> Incidencias { get; set; } = new List<MIncidenciaDto>();
        public List<MSoportePagoDto> Soporte { get; set; } = new List<MSoportePagoDto>();
        public List<CFDIDto> Facturas { get; set; } = new List<CFDIDto>();
        public List<MConfiguracionIncidenciaDto> ConfiguracionIncidencias { get; set; } = new List<MConfiguracionIncidenciaDto>();
        public CedulaMensajeriaDto Cedula { get; set; } = new CedulaMensajeriaDto();
        public List<PermisoUsuarioDto> Permisos { get; set; } = new List<PermisoUsuarioDto>();
        public List<CTIncidenciaDto> CTIncidencias { get; set; } = new List<CTIncidenciaDto>();
        public List<EntregableEstatusDto> EntregablesEstatus { get; set; } = new List<EntregableEstatusDto>();
        public InmuebleUSDto InmueblesUsuarios { get; set; } = new InmuebleUSDto();
        public List<int> IUFirmantes { get; set; } = new List<int>();
        public List<InmuebleDto> InmueblesFirmantes { get; set; } = new List<InmuebleDto>();
        public List<FlujoServicioDto> Flujo { get; set; } = new List<FlujoServicioDto>();
        public List<FlujoEntregableDto> FlujoEntregables { get; set; } = new List<FlujoEntregableDto>();
        public List<FirmanteDto> Firmantes { get; set; } = new List<FirmanteDto>();
        public List<UsuarioDto> Usuarios { get; set; } = new List<UsuarioDto>();
        public List<CTIndemnizacionDto> Indemnizaciones { get; set; } = new List<CTIndemnizacionDto>();       
        public string Supervision { get; set; }
        public int RevertirEstatus { get; set; }

        public DetalleModel(IModuloProxy modulo, IPermisoProxy permisos, ICTEntregableProxy centregable, ICTIncidenciaProxy cincidencias, 
                            IEstatusCedulaProxy estatusc, IEstatusEntregableProxy estatuse, IQFlujoMensajeriaProxy flujo, 
                            IQCFDIMensajeriaProxy facturasQuery, IInmuebleProxy inmuebles, 
                            IQCedulaMensajeriaProxy cedulaQuery, ICCedulaMensajeriaProxy cedulaCommand, 
                            IQEntregableMensajeriaProxy entregablesQuery, ICEntregableMensajeriaProxy entregablesCommand, 
                            ICRespuestaMensajeriaProxy respuestas, IQIncidenciaMensajeriaProxy incidenciasQuery, 
                            ICIncidenciaMensajeriaProxy incidenciasCommand, IQSoportePagoMensajeriaProxy soporteQuery, 
                            ICSoportePagoMensajeriaProxy soporteCommand, IQFirmanteMensajeriaProxy firmantesQuery, 
                            ICFirmanteMensajeriaProxy firmantesCommand, IQRepositorioMensajeriaProxy repositoriosQuery, 
                            IUsuarioProxy usuarios, IQLCedulaMensajeriaProxy logCQuery, ICLCedulaMensajeriaProxy logCCommand, 
                            IQLEntregableMensajeriaProxy logEQuery, ICLEntregableMensajeriaProxy logECommand, ICTIndemnizacionProxy indemnizacion)
        {
            _modulo = modulo;
            _permisos = permisos;
            _centregable = centregable;
            _cincidencias = cincidencias;
            _estatusc = estatusc;
            _estatuse = estatuse;
            _flujo = flujo;
            _facturasQuery = facturasQuery;
            _inmuebles = inmuebles;
            _cedulaQuery = cedulaQuery;
            _cedulaCommand = cedulaCommand;
            _entregablesQuery = entregablesQuery;
            _entregablesCommand = entregablesCommand;
            _respuestas = respuestas;
            _incidenciasQuery = incidenciasQuery;
            _incidenciasCommand = incidenciasCommand;
            _soporteQuery = soporteQuery;
            _soporteCommand = soporteCommand;
            _firmantesQuery = firmantesQuery;
            _firmantesCommand = firmantesCommand;
            _repositoriosQuery = repositoriosQuery;
            _usuarios = usuarios;
            _logCQuery = logCQuery;
            _logCCommand = logCCommand;
            _logEQuery = logEQuery;
            _logECommand = logECommand;
            _indemnizacion = indemnizacion;
        }

        public async Task OnGet(int moduloId, int submoduloId, int inmueble, int cedula)
        {
            var usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Permisos = await _permisos.GetPermisosByModuloUsuario(usuario, moduloId);
            Modulo = await _modulo.GetModuloByIdAsync(moduloId);
            Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
            InmueblesUsuarios = (await _inmuebles.GetInmueblesByUsuarioServicio(User.FindFirst(ClaimTypes.NameIdentifier).Value, (int)Modulo.ServicioId)).SingleOrDefault(iu => iu.InmuebleId == inmueble);
            Cedula = await _cedulaQuery.GetCedulaById(cedula);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0 && InmueblesUsuarios != null && InmueblesUsuarios.InmuebleId == inmueble)
            {
                CTIncidencias = await _cincidencias.GetIncidenciasByServicio((int)Modulo.ServicioId);
                Repositorio = await _repositoriosQuery.GetRepositorioByAMC(Cedula.Anio, Cedula.MesId, Cedula.ContratoId);
                Facturas = await _facturasQuery.GetFacturasByInmueble(inmueble, Repositorio.Id);
                Inmueble = await _inmuebles.GetInmuebleById(inmueble);
                ConfiguracionIncidencias = await _incidenciasQuery.GetConfiguracionIncidencias();
                Supervision = (await _inmuebles.GetInmueblesByServicio((int)Modulo.ServicioId)).SingleOrDefault(i => i.InmuebleId == inmueble).GrupoSupervision;
                Flujo = await _estatusc.GetFlujoByServicio((int)Modulo.ServicioId, Cedula.EstatusId, Supervision);
                FlujoEntregables = await _estatuse.GetFlujoByEntregablesSE((int)Modulo.ServicioId, Cedula.EstatusId);
                Soporte = await _soporteQuery.GetGuiasPendientes(cedula);
                IUFirmantes = (await _inmuebles.GetInmueblesByUsuarioServicio(usuario, (int)Modulo.ServicioId)).Select(ius => ius.InmuebleId).ToList();
                InmueblesFirmantes = (await _inmuebles.GetAllInmueblesAsync()).Where(i => IUFirmantes.Contains(i.Id)).ToList();
                Usuarios = await _usuarios.GetUsuariosByServicio(usuario, (int)Modulo.ServicioId);
                RevertirEstatus = await GetRevertirEstatus(cedula);
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        /********************************************* Incidencias *************************************/
        //Obtiene Incidencias por Pregunta y Cédula
        public async Task<JsonResult> OnGetIncidencias(int cedula, int pregunta)
        {
            Incidencias = await _incidenciasQuery.GetIncidenciasByPreguntaAndCedula(cedula, pregunta);
            return new JsonResult(Incidencias);
        }

        //Agregar Incidencia
        public async Task<IActionResult> OnPostCrearIncidencia([FromForm] MIncidenciaCreateCommand create)
        {
            await _incidenciasCommand.CreateIncidencia(create);
            return StatusCode(200);
        }
        //Agregar Incidencia Excel
        public async Task<JsonResult> OnPostCrearIncidenciaExcel([FromForm] MIncidenciaExcelCreateCommand create)
        {
            var incidencias = await _incidenciasCommand.CreateIncidenciaExcel(create);
            var proceed = incidencias.Where(i => !i.Observaciones.Equals("SI")).Count();
            if (proceed > 0)
            {
                return new JsonResult(new { incidencias = incidencias, status = 205 });
            }

            return new JsonResult(new { incidencias = incidencias, status = 200 });
        }
        
        public async Task<IActionResult> OnPostReporteErrorIE([FromBody] List<MIncidenciaDto> incidencias)
        {
            Workbook workbook = new Workbook();
            workbook.Worksheets.Clear();
            var sheet = workbook.Worksheets.Add("Sheet1");
            sheet.Range["A1:H1"].Style.Font.Size = 11;
            sheet.Range["A1:H1"].Style.Font.IsBold = true; //set the font bold
            sheet.Range["A1:H1"].ColumnWidth = 20;
            sheet.Range["A1:H1"].Style.Font.FontName = "Calibri";
            DataTable dt = CreateTable(incidencias);
            sheet.InsertDataTable(dt, true, 1, 1);

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                workbook.SaveToStream(ms1, Spire.Xls.FileFormat.Version2016);
                toArray = ms1.ToArray();
            }

            return File(toArray, "application/vnd.ms-excel", "Reporte de Carga Masiva.xlsx");
        }

        private static DataTable CreateTable(List<MIncidenciaDto> incidencias)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Fecha Programada", typeof(DateTime));
            dt.Columns.Add("Fecha de Entrega", typeof(DateTime));
            dt.Columns.Add("Numero de Guia", typeof(string));
            dt.Columns.Add("Codigo de Rastreo", typeof(string));
            dt.Columns.Add("Tipo de Servicio", typeof(string));
            dt.Columns.Add("Sobrepeso", typeof(string));
            dt.Columns.Add("Acuse", typeof(string));
            dt.Columns.Add("Observaciones", typeof(string));
            foreach (var inc in incidencias)
            {
                dt.Rows.Add(
                                Convert.ToDateTime(inc.FechaProgramada).ToString("dd/MM/yyyy"),
                                Convert.ToDateTime(inc.FechaEntrega).ToString("dd/MM/yyyy"),
                                inc.NumeroGuia,
                                inc.CodigoRastreo,
                                inc.TipoServicio,
                                inc.Sobrepeso,
                                inc.Acuse,
                                inc.Observaciones
                           );
            }
            return dt;
        }

        //Editar Incidencia
        public async Task<IActionResult> OnPutActualizarIncidencia([FromForm] MIncidenciaUpdateCommand update)
        {
            await _incidenciasCommand.UpdateIncidencia(update);
            return StatusCode(200);
        }

        //Eliminar Incidencia
        public async Task<JsonResult> OnPostEliminarIncidencias([FromBody] MIncidenciaDeleteCommand delete)
        {
            int incidencias = await _incidenciasCommand.DeleteIncidencias(delete);
            return new JsonResult(incidencias);
        }
        
        public async Task<JsonResult> OnPostEliminarIncidencia([FromBody] MIncidenciaDeleteCommand delete)
        {
            int incidencias = await _incidenciasCommand.DeleteIncidencia(delete);
            return new JsonResult(incidencias);
        }

        public async Task<JsonResult> OnGetIndemnizacionByIncidencia(int incidencia)
        {
            var indemnizaciones = await _indemnizacion.GetIndemnizacionByIncidencia(incidencia);

            return new JsonResult(indemnizaciones);
        }
        /********************************************* Incidencias *************************************/

        /************************* Soporte Pago*****************************************/
        public async Task<JsonResult> OnPostActualizaSoportePago([FromForm] MSoportePagoUpdateCommand soporte)
        {
            var soportes = await _soporteCommand.ActualizaIncidenciaExcel(soporte);
            return new JsonResult(soportes);
        }
        /************************* Soporte Pago*****************************************/

        /********************************************* Respuestas *************************************/

        public async Task<IActionResult> OnPutActualizarRespuestas([FromBody] List<RespuestasUpdateCommand> respuestas)
        {
            await _respuestas.UpdateRespuestas(respuestas);
            return StatusCode(200);
        }

        /********************************************* Respuestas *************************************/

        /********************************************* Entregables *************************************/
        public async Task<IActionResult> OnPutActualizarEntregables([FromForm] EntregableCommandUpdate entregable)
        {
            entregable.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _entregablesCommand.UpdateEntregable(entregable);
            return StatusCode(200);
        }
        
        public async Task<IActionResult> OnPutValidarEntregables([FromForm] EntregableCommandUpdate entregable)
        {
            entregable.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _entregablesCommand.ValidarEntregables(entregable);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPutAutorizarEntregables([FromForm] EEntregableUpdateCommand entregable)
        {
            await _entregablesCommand.AUpdateEntregable(entregable);
             return StatusCode(200);
        }

        public async Task<IActionResult> OnPostCreateHistorialEntregables([FromBody] LogEntregableCreateCommand historial)
        {
            await _logECommand.CreateHistorial(historial);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnGetVisualizarEntregable(int cAnio, string cMes, string cFolio, string cArchivo, string tipo)
        {
            string path = await _entregablesQuery.VisualizarEntregables(cAnio, cMes, cFolio, cArchivo, tipo);
            Stream stream = System.IO.File.Open(path, FileMode.Open);
            return File(stream, "application/pdf");
        }

        /********************************************* Entregables*************************************/

        /**************************** Actualización de Estatus ***********************************/
        public async Task<IActionResult> OnPostCreateHistorial([FromBody] LogCedulaCreateCommand historial)
        {
            await _logCCommand.CreateHistorial(historial);
            return StatusCode(200);
        }

        public async Task<JsonResult> OnPutUpdateCedula([FromBody] CedulaEvaluacionUpdateCommand cedula)
        {
            var result = await _cedulaCommand.UpdateCedula(cedula);
            return new JsonResult(result);
        }

        public async Task<JsonResult> OnPostAutorizarCAE([FromBody] CedulaMensajeriaDto cedula)
        {
            List<EntregableDto> entregables = await _entregablesQuery.GetEntregablesByCedula(cedula.Id);

            foreach (var en in entregables)
            { 
                en.estatus = await _estatuse.GetEEByIdAsync(en.EstatusId);
            }

            int eRevision = entregables.Where(e => e.estatus.Nombre.Equals("En Revisión") && !e.FechaEliminacion.HasValue).Count();

            return new JsonResult(eRevision);
        }

        public async Task<JsonResult> OnPostTramiteRechazado([FromBody] CedulaMensajeriaDto cedula)
        {
            List<EntregableDto> entregables = await _entregablesQuery.GetEntregablesByCedula(cedula.Id);

            foreach (var en in entregables)
            { 
                en.estatus = await _estatuse.GetEEByIdAsync(en.EstatusId);
            }

            int eRevision = entregables.Where(e => e.estatus.Nombre.Equals("En Revisión") && !e.FechaEliminacion.HasValue).Count();

            return new JsonResult(eRevision);
        }

        public async Task<JsonResult> OnPostTramitarPago([FromBody] CedulaMensajeriaDto cedula)
        {
            List<EntregableDto> entregables = await _entregablesQuery.GetEntregablesByCedula(cedula.Id);

            foreach (var en in entregables)
            {
                en.estatus = await _estatuse.GetEEByIdAsync(en.EstatusId);
                en.tipoEntregable = await _centregable.GetEntregableByIdAsync(en.EntregableId);
            }

            int eRevision = entregables.Where(e => (e.estatus.Nombre.Equals("Sin Iniciar") || e.estatus.Nombre.Equals("En Revisión")) && !e.FechaEliminacion.HasValue).Count();
            bool memorandum = true;
            bool cedulaf = true;
            bool acta = true;

            List<EntregableDto> memorandums = entregables.Where(e => e.tipoEntregable.Abreviacion.Equals("Memorandum") && !e.FechaEliminacion.HasValue).ToList();
            List<EntregableDto> actas= entregables.Where(e => e.tipoEntregable.Abreviacion.Equals("ActaERF") && !e.FechaEliminacion.HasValue).ToList();
            List<EntregableDto> cedulasf = entregables.Where(e => e.tipoEntregable.Abreviacion.Equals("Cedula_Firmada") && !e.FechaEliminacion.HasValue).ToList();

            foreach (var m in memorandums)
            {
                if(m.Validado == false || m.Validado == null)
                {
                    memorandum = false;
                }
            }

            foreach (var a in actas)
            {
                if (a.Validado == false || a.Validado == null)
                {
                    acta = false;
                }
            }

            foreach (var c in cedulasf)
            {
                if (c.Validado == false || c.Validado == null)
                {
                    cedulaf = false;
                }
            }

            if (eRevision > 0)
            {
                return new JsonResult(206);
            }
            else if(memorandum == false || acta == false || cedulaf == false)
            {
                return new JsonResult(207);
            }
            else
            {
                return new JsonResult(200);
            }
        }

        public async Task<IActionResult> OnGetActaEntregaRecepcion(int nfacturacion, int ninmueble, int cedulaId, string supervision)
        {
            Repositorio = await _repositoriosQuery.GetRepositorioById(nfacturacion);
            Facturas = await _facturasQuery.GetFacturasByInmueble(ninmueble, nfacturacion);
            Firmantes = await _firmantesQuery.GetFirmantesByInmueble(ninmueble);
            Incidencias = await _incidenciasQuery.GetIncidenciasByCedula(cedulaId);
            string reviso = "";
            string superviso = "";
            if(Firmantes != null)
            {
                reviso = Firmantes.Single(f => f.Tipo.Equals("Reviso")).Usuario.NombreEmp + " " + Firmantes.Single(f => f.Tipo.Equals("Reviso")).Usuario.PaternoEmp + " " + Firmantes.Single(f => f.Tipo.Equals("Reviso")).Usuario.MaternoEmp;
                superviso = Firmantes.Single(f => f.Tipo.Equals("Superviso")).Usuario.NombreEmp + " " + Firmantes.Single(f => f.Tipo.Equals("Superviso")).Usuario.PaternoEmp + " " + Firmantes.Single(f => f.Tipo.Equals("Superviso")).Usuario.MaternoEmp;
            }
            Document document = new Document();
            var path = @"E:\Plantillas\Acta ER\Acta Entrega - Recepción 2022 Mensajeria.docx";
            var cedula = await _cedulaQuery.GetCedulaById(cedulaId);
            document.LoadFromFile(path);

            Section tablas = document.AddSection();

            document.Replace("|Ciudad|", cedula.Inmueble.Estado, false, true);
            if (cedula.Inmueble.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + cedula.Inmueble.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + cedula.Inmueble.Estado, false, true);
                document.Replace("|Estado|", "en " + cedula.Inmueble.Estado, false, true);
            }

            document.Replace("|EncabezadoInmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.Inmueble.Descripcion), false, true);
            document.Replace("|AdministracionInmueble|", cedula.Inmueble.Nombre.ToUpper(), false, true);
            document.Replace("|Inmueble|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cedula.Inmueble.Nombre.ToLower()), false, true);
            document.Replace("|InmuebleEvaluado|", cedula.Inmueble.Nombre, false, true);
            document.Replace("|Contrato|", Repositorio.Contrato.NoContrato, false, true);
            document.Replace("|Puesto|", cedula.Inmueble.DescripcionAdministrador, false, true);
            document.Replace("|DomicilioInmueble|", cedula.Inmueble.Direccion, false, true);
            document.Replace("|Administrador|", cedula.Inmueble.Administrador, false, true);
            document.Replace("|Reviso|", (!reviso.Equals("") ? Firmantes.Single(f => f.Tipo.Equals("Reviso")).Escolaridad+" "+ reviso : "Sin Firmante"), false, true);
            document.Replace("|Superviso|", (!superviso.Equals("") ? Firmantes.Single(f => f.Tipo.Equals("Superviso")).Escolaridad+" "+ superviso : "Sin Firmante"), false, true);
            document.Replace("|PuestoFirma|", cedula.Inmueble.DescripcionAdministrador, false, true);
            string usuario = cedula.Usuario.NombreEmp + " " + cedula.Usuario.PaternoEmp + " " + cedula.Usuario.MaternoEmp;
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(usuario.ToLower()), false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes.Nombre+" "+cedula.Anio, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Empresa|", Repositorio.Contrato.Empresa, false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|PeriodoInicial|", Repositorio.Contrato.InicioVigencia.Day + " de " + Repositorio.Contrato.InicioVigencia.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + Repositorio.Contrato.InicioVigencia.Year, false, true);
            document.Replace("|PeriodoFinal|", Repositorio.Contrato.FinVigencia.Day + " de " + Repositorio.Contrato.FinVigencia.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + Repositorio.Contrato.FinVigencia.Year, false, true);
            document.Replace("|Coordinacion|", (supervision.Equals("CAE") ? "COORDINACIÓN DE ADMINISTRACIÓN DE EDIFICIOS" : "COORDINACIÓN DE ADMINISTRACIÓN REGIONAL"), false, true);


            if (Incidencias.Count() > 0)
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura " +
                    "Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y " +
                    "evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del " +
                    "Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios " +
                    "generales.", false, true);
            }

            string fTotal = "";
            string fIva = "";
            string fCantidad = "";
            string fTimbrado = "";
            string fFolios = "";
            
            string ncTotal = "";
            string ncIva = "";
            string ncCantidad = "";
            string ncTimbrado = "";
            string ncFolios = "";

            int c = 0;

            foreach(var fc in Facturas)
            {
                c++;
                if (fc.Tipo.Equals("Factura"))
                {
                    fTotal += c + ".- " + String.Format("{0:C}", fc.Total) + "\n";
                    fIva += c + ".- " + fc.IVA + "\n";
                    fTimbrado += c + ".- " + fc.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                    fFolios += c + ".- " + fc.Serie + fc.Folio + "\n";
                    foreach (var cn in fc.ConceptosFactura)
                    {
                        fCantidad += c + ".- " +(cn.Descripcion.Contains("SOBREPESO") ? (decimal)cn.Cantidad : (int)cn.Cantidad)+" "+cn.Descripcion + "\n";
                    }
                }
                else 
                {
                    ncTotal += c + ".- " + String.Format("{0:C}", fc.Total) + "\n";
                    ncIva += c + ".- " + fc.IVA + "\n";
                    ncTimbrado += c + ".- " + fc.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                    ncFolios += c + ".- " + fc.Serie + fc.Folio + "\n";
                    foreach (var cn in fc.ConceptosFactura)
                    {
                        ncCantidad += c + ".- " +(cn.Descripcion.Contains("SOBREPESO") ? (decimal)cn.Cantidad : (int)cn.Cantidad)+" "+cn.Descripcion + "\n";
                    }
                }
            }

            document.Replace("|ImporteIVA|", fTotal, false, true);
            document.Replace("|CantidadServicios|", fCantidad, false, true);
            document.Replace("|FechaTimbrado|", fTimbrado, false, true);
            document.Replace("|FolioFactura|", fFolios, false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", ncTotal, false, true);
            document.Replace("|CantidadNota|", ncCantidad, false, true);
            document.Replace("|TimbradoNota|", ncTimbrado, false, true);
            document.Replace("|FolioNota|", ncFolios, false, true);

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }

            return File(toArray, "application/ms-word", "Acta_Entrega_Recepcion_cedula_Recepcion.docx");
        }

        public async Task<IActionResult> OnGetVisualizarFactura(int cAnio, string cMes, string cFolio, string tipo, string cInmueble, string cArchivo)
        {
            string path = await _facturasQuery.VisualizarFactura(cAnio, cMes, cFolio, tipo, cInmueble, cArchivo);
            Stream stream = System.IO.File.Open(path, FileMode.Open);
            return File(stream, "application/pdf");
        }

        public async Task<IActionResult> OnGetVisualizarActas(int cAnio, string cMes, string cFolio, string tipo, string tipoArchivo, string cArchivo)
        {
            string path = await _incidenciasQuery.VisualizarActas(cAnio, cMes, cFolio, tipo, tipoArchivo, cArchivo);
            Stream stream = System.IO.File.Open(path, FileMode.Open);
            return File(stream, "application/pdf");
        }

        public async Task<IActionResult> OnGetPlantillaIncidencias()
        {
            string fileName = @"E:\Plantillas\Plantilla Incidencias.xlsx";
            byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Plantilla_Incidencias.xlsx");
        }

        public async Task<int> GetRevertirEstatus(int cedulaId)
        {
            var cedula = await _cedulaQuery.GetCedulaById(cedulaId);
            var log = cedula.logs.Where(h => h.EstatusId != cedula.EstatusId).ToList();

            var estatus = 0;

            if(log.Count() != 0)
            {
                estatus = log.First().EstatusId;
            }

            return estatus;
        }

        /******************************************* Firmantes *********************************************/
        public async Task<JsonResult> OnGetRevisarFirmantes(int inmueble)
        {
            var firmante = await _firmantesQuery.GetFirmantesByInmueble(inmueble);
            if (firmante.Count() != 0 && 
                firmante.Single(f => f.Tipo.Equals("Reviso")).Id != 0 && firmante.Single(f => f.Tipo.Equals("Superviso")).Id != 0)
            {
                return new JsonResult(firmante);
            }
            return new JsonResult(null);
        }

        public async Task<JsonResult> OnPostCreateFirmante([FromBody] FirmanteCreateCommand firmante)
        {
            var firmantes = await _firmantesCommand.CreateFirmantes(firmante);
            if(firmantes != null)
            {
                return new JsonResult(firmantes);
            }
            return new JsonResult(null);
        }
        /******************************************* Firmantes *********************************************/
    }
}
