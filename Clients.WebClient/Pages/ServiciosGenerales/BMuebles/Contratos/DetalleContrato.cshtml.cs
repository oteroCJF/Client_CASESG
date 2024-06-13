using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Api.Gateway.Models.Catalogos.DTOs.Parametros;
using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Catalogos.DTOs.ServiciosContratos;
using Api.Gateway.Models.Contratos.Commands;
using Api.Gateway.Models.Contratos.DTOs;
using Api.Gateway.Models.Convenios.Commands;
using Api.Gateway.Models.Convenios.DTOs;
using Api.Gateway.Models.Entregables.ServiciosGenerales.Commands.Contratos;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.BMuebles.Contratos.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.Convenios.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.Convenios.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.EntregablesContrato.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.EntregablesContrato.Queries;
using Api.Gateway.WebClient.Proxy.Catalogos.CTEntregables;
using Api.Gateway.WebClient.Proxy.Catalogos.CTParametros;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServiciosContratos;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clients.WebClient.Pages.ServiciosGenerales.BMuebles.Contratos
{
    public class DetalleContratoModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly IPermisoProxy _permisos;
        private readonly ICTServicioProxy _servicios;
        private readonly ICTServicioContratoProxy _scontratos;
        private readonly ICTEntregableProxy _centregables;

        private readonly IQBMContratoProxy _contratosQuery;
        private readonly ICBMContratoProxy _contratosCommand;
        
        private readonly IQBMConvenioProxy _conveniosQuery;
        private readonly ICBMConvenioProxy _conveniosCommand;

        //private readonly IQServicioContratoProxy _scontrato;
        private readonly IQBMEContratoProxy _entregablesQuery;
        private readonly ICBMEContratoProxy _entregablesCommand;

        private readonly ICTParametroProxy _parametros;

        public ModuloDto Modulo { get; set; }
        public SubmoduloDto Submodulo { get; set; }
        public CTServicioDto Servicio { get; set; }
        public ContratoDto Contratos { get; set; }
        public List<ConvenioDto> Convenios { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }
        public List<CTServicioContratoDto> Servicios { get; set; }
        public List<CTParametroDto> Rubros { get; set; }

        public DetalleContratoModel(IModuloProxy modulo, IPermisoProxy permisos, ICTServicioProxy servicios, ICTServicioContratoProxy scontratos, 
                                    ICTEntregableProxy centregables, IQBMContratoProxy contratosQuery, ICBMContratoProxy contratosCommand, 
                                    IQBMConvenioProxy conveniosQuery, ICBMConvenioProxy conveniosCommand, IQBMEContratoProxy entregablesQuery, 
                                    ICBMEContratoProxy entregablesCommand, ICTParametroProxy parametros)
        {
            _modulo = modulo;
            _permisos = permisos;
            _servicios = servicios;
            _scontratos = scontratos;
            _centregables = centregables;
            _contratosQuery = contratosQuery;
            _contratosCommand = contratosCommand;
            _conveniosQuery = conveniosQuery;
            _conveniosCommand = conveniosCommand;
            _entregablesQuery = entregablesQuery;
            _entregablesCommand = entregablesCommand;
            _parametros = parametros;
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

        /*public async Task<IActionResult> OnPostCrearSContrato([FromBody] ServicioContratoCreateCommand scontrato)
        {
            await _scontrato.CreateServicioContrato(scontrato);
            return StatusCode(200);
        }

        public async Task<IActionResult> OnPutEditarSContrato([FromBody] ServicioContratoUpdateCommand scontrato)
        {
            await _scontrato.UpdateServicioContrato(scontrato);
            return StatusCode(200);
        }*/
    }
}
