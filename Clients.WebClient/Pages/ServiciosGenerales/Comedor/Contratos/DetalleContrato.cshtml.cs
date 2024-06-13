using Api.Gateway.Models.Catalogos.DTOs.Parametros;
using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Catalogos.DTOs.ServiciosContratos;
using Api.Gateway.Models.Contratos.Commands;
using Api.Gateway.Models.Contratos.Commands.ServicioContrato;
using Api.Gateway.Models.Contratos.DTOs;
using Api.Gateway.Models.Convenios.Commands;
using Api.Gateway.Models.Convenios.DTOs;
using Api.Gateway.Models.Entregables.ServiciosGenerales.Commands.Contratos;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTEntregables;
using Api.Gateway.WebClient.Proxy.Catalogos.CTParametros;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServiciosContratos;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Api.Gateway.WebClient.Proxy.Comedor.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Convenios.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.EntregablesContrato.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.ServiciosContrato.Commands;
using Api.Gateway.WebClient.Proxy.Comedor.ServiciosContrato.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Comedor.Contratos
{
    public class DetalleContratoModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly ICTServicioProxy _servicios;
        
        private readonly IQContratoComedorProxy _contratosQuery;
        private readonly ICContratoComedorProxy _contratosCommand;

        private readonly IQConvenioComedorProxy _conveniosQuery;
        private readonly ICConvenioComedorProxy _conveniosCommand;

        private readonly IQSContratoComedorProxy _scontratoQuery;
        private readonly ICSContratoComedorProxy _scontratoCommand;

        private readonly IQEContratoComedorProxy _entregablesQuery;
        private readonly ICEContratoComedorProxy _entregablesCommand;

        private readonly IPermisoProxy _permisos;
        private readonly ICTServicioContratoProxy _scontratos;
        private readonly ICTParametroProxy _parametros;
        private readonly ICTEntregableProxy _centregables;

        public ModuloDto Modulo { get; set; }
        public SubmoduloDto Submodulo { get; set; }
        public CTServicioDto Servicio { get; set; }
        public ContratoDto Contratos { get; set; }
        public List<ConvenioDto> Convenios { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }
        public List<CTServicioContratoDto> Servicios { get; set; }
        public List<CTParametroDto> Rubros { get; set; }

        public DetalleContratoModel(IModuloProxy modulo, ICTServicioProxy servicios, IQContratoComedorProxy contratosQuery, 
                                    ICContratoComedorProxy contratosCommand, IQConvenioComedorProxy conveniosQuery, 
                                    ICConvenioComedorProxy conveniosCommand, IQSContratoComedorProxy scontratoQuery, 
                                    ICSContratoComedorProxy scontratoCommand, IQEContratoComedorProxy entregablesQuery, 
                                    ICEContratoComedorProxy entregablesCommand, IPermisoProxy permisos, 
                                    ICTServicioContratoProxy scontratos, ICTParametroProxy parametros, ICTEntregableProxy centregables)
        {
            _modulo = modulo;
            _servicios = servicios;
            _contratosQuery = contratosQuery;
            _contratosCommand = contratosCommand;
            _conveniosQuery = conveniosQuery;
            _conveniosCommand = conveniosCommand;
            _scontratoQuery = scontratoQuery;
            _scontratoCommand = scontratoCommand;
            _entregablesQuery = entregablesQuery;
            _entregablesCommand = entregablesCommand;
            _permisos = permisos;
            _scontratos = scontratos;
            _parametros = parametros;
            _centregables = centregables;
        }

        public async Task OnGet(int moduloId, int submoduloId, int contrato)
        {
            Permisos = await _permisos.GetPermisosByModuloUsuario(User.FindFirst(ClaimTypes.NameIdentifier).Value, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                Contratos = await _contratosQuery.GetContratoByIdAsync(contrato);
                Convenios = await _conveniosQuery.GetConveniosByContrato(contrato);
                Servicios = await _scontratos.GetServiciosByServicioAsync((int)Modulo.ServicioId);
                Rubros = await _parametros.GetParametroByTabla("Convenio");
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        public async Task<JsonResult> OnPostActualizarContrato([FromBody] ContratoUpdateCommand contrato)
        {
            contrato.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int status = await _contratosCommand.UpdateContrato(contrato);
            return new JsonResult(status);
        }

        public async Task<JsonResult> OnPostCrearConvenio([FromBody] ConvenioCreateCommand convenio)
        {
            convenio.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int status = await _conveniosCommand.CreateConvenio(convenio);
            return new JsonResult(status);
        }

        public async Task<JsonResult> OnPostActualizarConvenio([FromBody] ConvenioUpdateCommand convenio)
        {
            convenio.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int status = await _conveniosCommand.UpdateConvenio(convenio);
            return new JsonResult(status);
        }
        
        public async Task<JsonResult> OnPutActualizarEntregable([FromForm] EntregableContratoUpdateCommand entregable)
        {
            entregable.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            entregable.Contrato = (await _contratosQuery.GetContratoByIdAsync(entregable.ContratoId)).NoContrato;
            entregable.Convenio = entregable.ConvenioId != 0 ? (await _conveniosQuery.GetConvenioByIdAsync(entregable.ConvenioId)).NoConvenio : "";
            entregable.TipoEntregable = (await _centregables.GetEntregableByIdAsync(entregable.EntregableId)).Nombre;
            int status = await _entregablesCommand.UpdateEntregable(entregable);
            return new JsonResult(status);
        }

        public async Task<IActionResult> OnGetVisualizarEntregable(string ncontrato, string tipoEntregable, string archivo, string convenio)
        {
            string path = "";
            var regex = "[^0-9A-Za-z_ ]";
            ncontrato = Regex.Replace(ncontrato, regex, "_");
            if (convenio != null && !convenio.Equals(""))
            {
                convenio = Regex.Replace(convenio, regex, "_");
                path = await _entregablesQuery.VisualizarEntregablesConv(ncontrato, convenio, tipoEntregable, archivo);
            }
            else
            {
                path = await _entregablesQuery.VisualizarEntregablesCont(ncontrato, tipoEntregable, archivo);
            }
            Stream stream = System.IO.File.Open(path, FileMode.Open);
            return File(stream, "application/pdf");
        }

        public async Task<IActionResult> OnPostCrearSContrato([FromBody] ServicioContratoCreateCommand scontrato)
        {
            await _scontratoCommand.CreateServicioContrato(scontrato);
            return StatusCode(200);
        }
        
        public async Task<IActionResult> OnPutEditarSContrato([FromBody] ServicioContratoUpdateCommand scontrato)
        {
            await _scontratoCommand.UpdateServicioContrato(scontrato);
            return StatusCode(200);
        }
    }
}
