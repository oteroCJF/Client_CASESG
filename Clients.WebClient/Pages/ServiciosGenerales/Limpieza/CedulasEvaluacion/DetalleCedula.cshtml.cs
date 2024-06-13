using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Limpieza.Entregables;
using Api.Gateway.WebClient.Proxy.Limpieza.Facturas;
using Api.Gateway.WebClient.Proxy.Limpieza.Firmantes;
using Api.Gateway.WebClient.Proxy.Limpieza.Flujo;
using Api.Gateway.WebClient.Proxy.Limpieza.Historiales;
using Api.Gateway.WebClient.Proxy.Limpieza.Incidencias;
using Api.Gateway.WebClient.Proxy.Limpieza.Variables;
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
using Api.Gateway.WebClient.Proxy.Limpieza.CedulaEvaluacion;
using Api.Gateway.Models.Contratos.DTOs;
using Api.Gateway.Models.CFDIs.ServiciosGenerales.DTOs;
using Api.Gateway.Models.Entregables.ServiciosGenerales.DTOs.Cedulas;
using Api.Gateway.Models.Firmantes.DTOs;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.Commands.Respuestas;
using Api.Gateway.Models.Entregables.ServiciosGenerales.Commands.Cedulas;
using Api.Gateway.Models.LogsCedulas.Commands;
using Api.Gateway.Models.LogEntregables.Commands;
using Api.Gateway.WebClient.Proxy.Catalogos.CTEntregables;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.Commands.CedulasEvaluacion;
using Api.Gateway.Models.CedulasEvaluacion.ServiciosGenerales.DTOs.Limpieza;
using Api.Gateway.WebClient.Proxy.Catalogos.CTIncidencias;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Usuarios;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Catalogos.DTOs.Incidencias;
using Api.Gateway.Models.Inmuebles.DTOs.InmueblesUS;
using Api.Gateway.Models.Estatus.DTOs.EstatusEntregables;
using Api.Gateway.Models.Estatus.DTOs.EstatusCedulas;
using Api.Gateway.Models.Usuarios.DTOs;
using Api.Gateway.Models.Firmantes.Commands;
using System.Data;
using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.Models.Incidencias.Limpieza.DTOs;
using Api.Gateway.Models.Incidencias.Limpieza.Commands;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Limpieza.Repositorios;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.Models.Estatus.DTOs.EntregablesEstatus;
using Api.Gateway.Models.Entregables.ServiciosGenerales.Commands.Cedulas.Update;

namespace Clients.WebClient.Pages.Limpieza.CedulasEvaluacion
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DetalleModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly IMesProxy _meses;
        private readonly IPermisoProxy _permisos;
        private readonly ICTServicioProxy _servicios;
        private readonly ICTEntregableProxy _centregable;
        private readonly ICTIncidenciaProxy _cincidencias;
        private readonly IEstatusCedulaProxy _estatusc;
        private readonly IEstatusEntregableProxy _estatuse;
        private readonly ILFlujoProxy _flujo;
        private readonly ILCFDIProxy _facturas;
        private readonly IInmuebleProxy _inmuebles;
        private readonly ILParametroProxy _variables;
        private readonly ILCedulaProxy _cedula;
        private readonly ILEntregableProxy _entregables;
        private readonly ILRespuestaProxy _respuestas;
        private readonly ILIncidenciaProxy _incidencias;
        private readonly ICTILimpiezaProxy _ilimpieza;
        private readonly ILFirmanteProxy _firmantes;
        private readonly ILRepositorioProxy _repositorios;
        private readonly IUsuarioProxy _usuarios;
        private readonly ILLogCedulaProxy _logs;
        private readonly ILLogEntregableProxy _logse;

        public SubmoduloDto Submodulo { get; set; } = new SubmoduloDto();
        public ModuloDto Modulo { get; set; } = new ModuloDto();
        public List<MesDto> Meses { get; set; } = new List<MesDto>();
        public int MesId { get; set; }
        public int RepositorioId { get; set; }
        public int InmuebleId { get; set; }
        public IFormFile XML { get; set; }
        public MesDto Mes { get; set; } = new MesDto();
        public CTServicioDto Servicio { get; set; } = new CTServicioDto();
        public InmuebleDto Inmueble { get; set; } = new InmuebleDto();
        public RepositorioDto Repositorio { get; set; } = new RepositorioDto();
        public ContratoDto Contrato { get; set; } = new ContratoDto();
        public List<LIncidenciaDto> Incidencias { get; set; } = new List<LIncidenciaDto>();
        public List<CFDIDto> Facturas { get; set; } = new List<CFDIDto>();
        public List<LConfiguracionIncidenciaDto> ConfiguracionIncidencias { get; set; } = new List<LConfiguracionIncidenciaDto>();
        public CedulaLimpiezaDto Cedula { get; set; } = new CedulaLimpiezaDto();
        public List<PermisoUsuarioDto> Permisos { get; set; } = new List<PermisoUsuarioDto>();
        public List<CTIncidenciaDto> CTIncidencias { get; set; } = new List<CTIncidenciaDto>();
        public List<CTILimpiezaDto> CTILimpieza { get; set; } = new List<CTILimpiezaDto>();
        public List<EntregableEstatusDto> EntregablesEstatus { get; set; } = new List<EntregableEstatusDto>();
        public InmuebleUSDto InmueblesUsuarios { get; set; } = new InmuebleUSDto();
        public List<int> IUFirmantes { get; set; } = new List<int>();
        public List<InmuebleDto> InmueblesFirmantes { get; set; } = new List<InmuebleDto>();
        public List<FlujoServicioDto> Flujo { get; set; } = new List<FlujoServicioDto>();
        public List<FlujoEntregableDto> FlujoEntregables { get; set; } = new List<FlujoEntregableDto>();
        public List<FirmanteDto> Firmantes { get; set; } = new List<FirmanteDto>();
        public List<UsuarioDto> Usuarios { get; set; } = new List<UsuarioDto>();
        public string Supervision { get; set; }
        public int RevertirEstatus { get; set; }

        public DetalleModel(IModuloProxy modulo, IPermisoProxy permisos, ICTEntregableProxy centregable, ICTIncidenciaProxy cincidencias, ICTServicioProxy servicios,
                            IEstatusCedulaProxy estatusc, IEstatusEntregableProxy estatuse, ILFlujoProxy flujo, ILCFDIProxy facturas, 
                            IInmuebleProxy inmuebles, ILParametroProxy variables, ILCedulaProxy cedula, ILEntregableProxy entregables, 
                            ILRespuestaProxy respuestas, ILIncidenciaProxy incidencias, ILFirmanteProxy firmantes, ILRepositorioProxy repositorios, 
                            IUsuarioProxy usuarios, ILLogCedulaProxy logs, ILLogEntregableProxy logse, IMesProxy meses, ICTILimpiezaProxy ilimpieza)
        {
            _modulo = modulo;
            _meses = meses;
            _servicios = servicios;
            _permisos = permisos;
            _centregable = centregable;
            _cincidencias = cincidencias;
            _ilimpieza = ilimpieza;
            _estatusc = estatusc;
            _estatuse = estatuse;
            _flujo = flujo;
            _facturas = facturas;
            _inmuebles = inmuebles;
            _variables = variables;
            _cedula = cedula;
            _entregables = entregables;
            _respuestas = respuestas;
            _incidencias = incidencias;
            _firmantes = firmantes;
            _repositorios = repositorios;
            _usuarios = usuarios;
            _logs = logs;
            _logse = logse;
        }

        public async Task OnGet(int moduloId, int submoduloId, int inmueble, int facturacion, int cedula)
        {
            var usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Permisos = await _permisos.GetPermisosByModuloUsuario(usuario, moduloId);
            Modulo = await _modulo.GetModuloByIdAsync(moduloId);
            Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
            Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
            InmueblesUsuarios = (await _inmuebles.GetInmueblesByUsuarioServicio(User.FindFirst(ClaimTypes.NameIdentifier).Value, (int)Modulo.ServicioId)).SingleOrDefault(iu => iu.InmuebleId == inmueble);
            Cedula = await _cedula.GetCedulaById(cedula);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0 && InmueblesUsuarios != null && InmueblesUsuarios.InmuebleId == inmueble)
            {
                Meses = await _meses.GetAllAsync();
                CTIncidencias = await _cincidencias.GetIncidenciasByServicio((int)Modulo.ServicioId);
                Repositorio = await _repositorios.GetRepositorioById(facturacion);
                Facturas = await _facturas.GetFacturasByInmueble(inmueble, facturacion);
                Inmueble = await _inmuebles.GetInmuebleById(inmueble);
                ConfiguracionIncidencias = await _incidencias.GetConfiguracionIncidencias();
                Supervision = (await _inmuebles.GetInmueblesByServicio((int)Modulo.ServicioId)).SingleOrDefault(i => i.InmuebleId == inmueble).GrupoSupervision;
                Flujo = await _estatusc.GetFlujoByServicio((int)Modulo.ServicioId, Cedula.EstatusId, Supervision);
                FlujoEntregables = await _estatuse.GetFlujoByEntregablesSE((int)Modulo.ServicioId, Cedula.EstatusId);
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

        public async Task<JsonResult> OnGetTiposIncidencia()
        {
            var variables = (await _variables.GetVariablesTipoIncidencia()).OrderBy(x => x.Abreviacion).Select(v => v.Abreviacion).Distinct().ToList();
            return new JsonResult(variables);
        }

        public async Task<JsonResult> OnGetAreaIncidencia(string tipo)
        {
            var variables = (await _variables.GetVariablesTipoIncidencia()).Where(v => v.Abreviacion.Equals(tipo)).OrderBy(x => x.Valor).Select(v => v.Valor).Distinct().ToList();
            return new JsonResult(variables);
        }

        //Obtiene Incidencias por Pregunta y C�dula
        public async Task<JsonResult> OnGetIncidencias(int cedula, int pregunta)
        {
            Incidencias = await _incidencias.GetIncidenciasByPreguntaAndCedula(cedula, pregunta);
            return new JsonResult(Incidencias);
        }

        //Agregar Incidencia
        public async Task<IActionResult> OnPostCrearIncidencia([FromBody] LIncidenciaCreateCommand create)
        {
            await _incidencias.CreateIncidencia(create);
            return StatusCode(200);
        }

        private static DataTable CreateTable(List<LIncidenciaDto> incidencias)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Fecha Incidencia", typeof(DateTime));
            dt.Columns.Add("Inasistencias", typeof(string));
            dt.Columns.Add("Observaciones", typeof(string));
            dt.Columns.Add("Mes Correspondiente", typeof(string));
            dt.Columns.Add("Incidencia", typeof(string));
            dt.Columns.Add("Tipo de Incidencia", typeof(string));
            foreach (var inc in incidencias)
            {
                dt.Rows.Add(
                                Convert.ToDateTime(inc.FechaIncidencia).ToString("dd/MM/yyyy"),
                                inc.Inasistencias,
                                inc.Observaciones,
                                inc.Mes.Nombre,
                                inc.Incidencia.Incidencia.Valor,
                                inc.Incidencia.Nombre
                           );
            }
            return dt;
        }

        /************************************ Incidencias ****************************************/

        public async Task<JsonResult> OnGetTiposByIncidencia(int incidencia)
        {
            var tipos = (await _ilimpieza.GetIncidenciasByTipo(incidencia)).Select(i => i.Tipo).Distinct().ToList();

            return new JsonResult(tipos);
        }
        
        public async Task<JsonResult> OnGetNombresByTipo(string tipo)
        {
            CTILimpieza = await _ilimpieza.GetNombresByTipo(tipo);

            return new JsonResult(CTILimpieza);
        }

        public async Task<IActionResult> OnPutActualizarIncidencia([FromBody] LIncidenciaUpdateCommand update)
        {
            await _incidencias.UpdateIncidencia(update);
            return StatusCode(200);
        }

        //Eliminar Incidencia
        public async Task<JsonResult> OnPostEliminarIncidencias([FromBody] LIncidenciaDeleteCommand delete)
        {
            int incidencias = await _incidencias.DeleteIncidencias(delete);
            return new JsonResult(incidencias);
        }

        public async Task<JsonResult> OnPostEliminarIncidencia([FromBody] LIncidenciaDeleteCommand delete)
        {
            int incidencias = await _incidencias.DeleteIncidencia(delete);
            return new JsonResult(incidencias);
        }
        /************************************ Incidencias ****************************************/

        public async Task<IActionResult> OnPutActualizarRespuestas([FromBody] List<RespuestasUpdateCommand> respuestas)
        {
            await _respuestas.UpdateRespuestas(respuestas);
            return StatusCode(200);
        }

        //Actualizar Entregables
        public async Task<IActionResult> OnPutActualizarEntregables([FromForm] EntregableCommandUpdate entregable)
        {
            entregable.EstatusId = (await _estatuse.GetAllEstatusEntregablesAsync()).SingleOrDefault(e => e.Nombre.Equals(entregable.Estatus)).Id;
            entregable.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _entregables.UpdateEntregable(entregable);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPutAutorizarEntregables([FromForm] EEntregableUpdateCommand entregable)
        {
            await _entregables.AUpdateEntregable(entregable);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPostCreateHistorialEntregables([FromBody] LogEntregableCreateCommand historial)
        {
            await _logse.CreateHistorial(historial);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnGetVisualizarEntregable(int cAnio, string cMes, string cFolio, string cArchivo, string tipo)
        {
            string path = await _entregables.VisualizarEntregables(cAnio, cMes, cFolio, cArchivo, tipo);
            Stream stream = System.IO.File.Open(path, FileMode.Open);
            return File(stream, "application/pdf");
        }

        /**************************** Actualizaci�n de Estatus ***********************************/
        public async Task<IActionResult> OnPostCreateHistorial([FromBody] LogCedulaCreateCommand historial)
        {
            await _logs.CreateHistorial(historial);
            return StatusCode(200);
        }

        public async Task<JsonResult> OnPutUpdateCedula([FromBody] CedulaEvaluacionUpdateCommand cedula)
        {
            var result = await _cedula.UpdateCedula(cedula);
            return new JsonResult(result);
        }

        public async Task<JsonResult> OnPutRechazarCedula([FromBody] CedulaEvaluacionUpdateCommand cedula)
        {
            var result = await _cedula.RechazarCedula(cedula);
            return new JsonResult(result);
        }

        public async Task<JsonResult> OnPostAutorizarCAE([FromBody] CedulaLimpiezaDto cedula)
        {
            List<EntregableDto> entregables = await _entregables.GetEntregablesByCedula(cedula.Id);

            foreach (var en in entregables)
            {
                en.estatus = await _estatuse.GetEEByIdAsync(en.EstatusId);
            }

            int eRevision = entregables.Where(e => e.estatus.Nombre.Equals("En Revisi�n") && !e.FechaEliminacion.HasValue).Count();

            return new JsonResult(eRevision);
        }

        public async Task<JsonResult> OnPostTramiteRechazado([FromBody] CedulaLimpiezaDto cedula)
        {
            List<EntregableDto> entregables = await _entregables.GetEntregablesByCedula(cedula.Id);

            foreach (var en in entregables)
            {
                en.estatus = await _estatuse.GetEEByIdAsync(en.EstatusId);
            }

            int eRevision = entregables.Where(e => e.estatus.Nombre.Equals("En Revisi�n") && !e.FechaEliminacion.HasValue).Count();

            return new JsonResult(eRevision);
        }

        public async Task<JsonResult> OnPostTramitarPago([FromBody] CedulaLimpiezaDto cedula)
        {
            List<EntregableDto> entregables = await _entregables.GetEntregablesByCedula(cedula.Id);

            foreach (var en in entregables)
            {
                en.estatus = await _estatusc.GetECByIdAsync(en.EstatusId);
                en.tipoEntregable = await _centregable.GetEntregableByIdAsync(en.EntregableId);
            }

            int eRevision = entregables.Where(e => e.estatus.Nombre.Equals("En Revisión") && !e.FechaEliminacion.HasValue).Count();
            bool memorandum = true;
            bool cedulaf = true;
            bool acta = true;

            List<EntregableDto> memorandums = entregables.Where(e => e.tipoEntregable.Abreviacion.Equals("Memorandum") && !e.FechaEliminacion.HasValue).ToList();
            List<EntregableDto> actas = entregables.Where(e => e.tipoEntregable.Abreviacion.Equals("ActaERF") && !e.FechaEliminacion.HasValue).ToList();
            List<EntregableDto> cedulasf = entregables.Where(e => e.tipoEntregable.Abreviacion.Equals("Cedula_Firmada") && !e.FechaEliminacion.HasValue).ToList();

            foreach (var m in memorandums)
            {
                if (m.Validado == false)
                {
                    memorandum = false;
                }
            }

            foreach (var a in actas)
            {
                if (a.Validado == false)
                {
                    acta = false;
                }
            }

            foreach (var c in cedulasf)
            {
                if (c.Validado == false)
                {
                    cedulaf = false;
                }
            }

            if (eRevision > 0)
            {
                return new JsonResult(206);
            }
            else if (memorandum == false || acta == false || cedulaf == false)
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
            Repositorio = await _repositorios.GetRepositorioById(nfacturacion);
            Facturas = await _facturas.GetFacturasByInmueble(ninmueble, nfacturacion);
            Firmantes = await _firmantes.GetFirmantesByInmueble(ninmueble);
            string reviso = "";
            string superviso = "";
            if (Firmantes != null)
            {
                reviso = Firmantes.Single(f => f.Tipo.Equals("Reviso")).Usuario.NombreEmp + " " + Firmantes.Single(f => f.Tipo.Equals("Reviso")).Usuario.PaternoEmp + " " + Firmantes.Single(f => f.Tipo.Equals("Reviso")).Usuario.MaternoEmp;
                superviso = Firmantes.Single(f => f.Tipo.Equals("Superviso")).Usuario.NombreEmp + " " + Firmantes.Single(f => f.Tipo.Equals("Reviso")).Usuario.PaternoEmp + " " + Firmantes.Single(f => f.Tipo.Equals("Reviso")).Usuario.MaternoEmp;
            }
            Document document = new Document();
            var path = @"E:\Plantillas\Acta ER\Acta Entrega - Recepción 2022 Limpieza.docx";
            var cedula = await _cedula.GetCedulaById(cedulaId);
            document.LoadFromFile(path);

            Section tablas = document.AddSection();

            document.Replace("|Ciudad|", cedula.Inmueble.Estado, false, true);
            if (cedula.Inmueble.Estado.Equals("Ciudad de M�xico"))
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
            document.Replace("|Reviso|", (!reviso.Equals("") ? Firmantes.Single(f => f.Tipo.Equals("Reviso")).Escolaridad + " " + reviso : "Sin Firmante"), false, true);
            document.Replace("|Superviso|", (!superviso.Equals("") ? Firmantes.Single(f => f.Tipo.Equals("Superviso")).Escolaridad + " " + superviso : "Sin Firmante"), false, true);
            document.Replace("|PuestoFirma|", cedula.Inmueble.DescripcionAdministrador, false, true);
            string usuario = cedula.Usuario.NombreEmp + " " + cedula.Usuario.PaternoEmp + " " + cedula.Usuario.MaternoEmp;
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(usuario.ToLower()), false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes.Nombre + " " + cedula.Anio, false, true);


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
            document.Replace("|Coordinacion|", ("COORDINACIÓN DE ADMINISTRACIÓN DE EDIFICIOS"), false, true);


            foreach (var i in cedula.respuestas)
            {
                if (i.iLimpieza.Count() > 0)
                {
                    document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura " +
                        "Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y " +
                        "evaluación de servicios generales.", false, true);
                    break;
                }
                else
                {
                    document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
                }
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

            foreach (var fc in Facturas)
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
                        fCantidad += c + ".- " + (cn.Descripcion.Contains("SOBREPESO") ? (decimal)cn.Cantidad : (int)cn.Cantidad) + " " + cn.Descripcion + "\n";
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
                        ncCantidad += c + ".- " + (cn.Descripcion.Contains("SOBREPESO") ? (decimal)cn.Cantidad : (int)cn.Cantidad) + " " + cn.Descripcion + "\n";
                    }
                }
            }

            document.Replace("|ImporteIVA|", fTotal, false, true);
            document.Replace("|CantidadServicios|", fCantidad, false, true);
            document.Replace("|FechaTimbrado|", fTimbrado, false, true);
            document.Replace("|FolioFactura|", fFolios, false, true);

            //Notas de Cr�dito
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
            string path = await _facturas.VisualizarFactura(cAnio, cMes, cFolio, tipo, cInmueble, cArchivo);
            Stream stream = System.IO.File.Open(path, FileMode.Open);
            return File(stream, "application/pdf");
        }

        public async Task<IActionResult> OnGetVisualizarActas(int cAnio, string cMes, string cFolio, string tipo, string tipoArchivo, string cArchivo)
        {
            string path = await _incidencias.VisualizarActas(cAnio, cMes, cFolio, tipo, tipoArchivo, cArchivo);
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
            var cedula = await _cedula.GetCedulaById(cedulaId);
            var log = cedula.logs.Where(h => h.EstatusId != cedula.EstatusId).ToList();

            var estatus = 0;

            if (log.Count() != 0)
            {
                estatus = log.First().EstatusId;
            }

            return estatus;
        }

        /******************************************* Firmantes *********************************************/
        public async Task<JsonResult> OnGetRevisarFirmantes(int inmueble)
        {
            var firmante = await _firmantes.GetFirmantesByInmueble(inmueble);
            if (firmante.Count() != 0 &&
                firmante.Single(f => f.Tipo.Equals("Reviso")).Id != 0 && firmante.Single(f => f.Tipo.Equals("Superviso")).Id != 0)
            {
                return new JsonResult(firmante);
            }
            return new JsonResult(null);
        }

        public async Task<JsonResult> OnPostCreateFirmante([FromBody] FirmanteCreateCommand firmante)
        {
            var firmantes = await _firmantes.CreateFirmantes(firmante);
            if (firmantes != null)
            {
                return new JsonResult(firmantes);
            }
            return new JsonResult(null);
        }

        public async Task<JsonResult> OnPutCedulaSolicitudRechazo([FromBody] CedulaSRUpdateCommand cedula)
        {
            var result = await _cedula.CedulaSolicitudRechazo(cedula);
            return new JsonResult(result);
        }

        public async Task<JsonResult> OnPutDenegarSolicitudRechazo([FromBody] CedulaSRUpdateCommand cedula)
        {
            var result = await _cedula.DenegarSolicitudRechazo(cedula);
            return new JsonResult(result);
        }
        /******************************************* Firmantes *********************************************/
    }
}
