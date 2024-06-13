using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Gateway.Models.BMuebles.Solicitudes.DTOs;
using Api.Gateway.Models.Catalogos.DTOs.Parametros;
using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Catalogos.DTOs.ServiciosContratos;
using Api.Gateway.Models.Contratos.DTOs;
using Api.Gateway.Models.Estatus.DTOs.EstatusCedulas;
using Api.Gateway.Models.Inmuebles.Commands.Direcciones;
using Api.Gateway.Models.Inmuebles.DTOs.Direcciones;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.Models.Usuarios.DTOs;
using Api.Gateway.WebClient.Proxy.BMuebles.Contratos;
using Api.Gateway.WebClient.Proxy.BMuebles.Contratos.Queries;
using Api.Gateway.WebClient.Proxy.BMuebles.Solicitudes;
using Api.Gateway.WebClient.Proxy.BMuebles.Solicitudes.Commands;
using Api.Gateway.WebClient.Proxy.BMuebles.Solicitudes.Queries;
using Api.Gateway.WebClient.Proxy.Catalogos.CTParametros;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServiciosContratos;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Inmuebles.Direcciones;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Api.Gateway.WebClient.Proxy.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clients.WebClient.Pages.ServiciosGenerales.BMuebles.Solicitudes
{
    public class DetalleSolicitudModel : PageModel
    {
        private readonly IModuloProxy _modulo;
        private readonly ICTServicioProxy _servicios;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IEstatusCedulaProxy _estatusc;

        private readonly IQBMSolicitudProxy _solicitudesQuery;
        private readonly ICBMSolicitudProxy _solicitudesCommand;
        
        private readonly IQBMContratoProxy _contratos;
        private readonly IPermisoProxy _permisos;
        private readonly IMesProxy _meses;
        private readonly ICTServicioContratoProxy _serviciosc;
        private readonly IDireccionProxy _direcciones;
        private readonly ICTParametroProxy _parametros;
        private readonly IUsuarioProxy _usuarios;

        public ModuloDto Modulo { get; set; } = new ModuloDto();
        public SubmoduloDto Submodulo { get; set; } = new SubmoduloDto();
        public List<int> InmueblesUsuarios { get; set; } = new List<int>();
        public List<MesDto> Meses { get; set; } = new List<MesDto>();
        public List<InmuebleDto> Inmuebles { get; set; } = new List<InmuebleDto>();
        public InmuebleDto Sede { get; set; } = new InmuebleDto();
        public CTServicioDto Servicio { get; set; } = new CTServicioDto();
        public List<CTParametroDto> Camiones { get; set; } = new List<CTParametroDto>();
        public List<PermisoUsuarioDto> Permisos { get; set; } = new List<PermisoUsuarioDto>();
        public ContratoDto Contrato { get; set; } = new ContratoDto();
        public List<CTServicioContratoDto> ServiciosContrato { get; set; } = new List<CTServicioContratoDto>();
        public List<DireccionDto> Direcciones { get; set; } = new List<DireccionDto>();
        public List<UsuarioDto> Usuarios { get; set; } = new List<UsuarioDto>();
        public List<FlujoServicioDto> Flujo { get; set; } = new List<FlujoServicioDto>();
        public SolicitudDto Solicitud { get; set; } = new SolicitudDto();
        public string Supervision { get; set; }

        public DetalleSolicitudModel(IModuloProxy modulo, ICTServicioProxy servicios, IInmuebleProxy inmuebles, IQBMSolicitudProxy solicitudesQuery,
                                   ICBMSolicitudProxy solicitudesCommand, IPermisoProxy permisos, IQBMContratoProxy contratos, ICTServicioContratoProxy serviciosc, 
                                   IMesProxy meses, IDireccionProxy direcciones, ICTParametroProxy parametros, IUsuarioProxy usuarios, 
                                   IEstatusCedulaProxy estatusc)
        {
            _modulo = modulo;
            _servicios = servicios;
            _contratos = contratos;
            _inmuebles = inmuebles;
            _direcciones = direcciones;
            _estatusc= estatusc;
            _meses = meses;
            _solicitudesQuery = solicitudesQuery;
            _solicitudesCommand = solicitudesCommand;
            _permisos = permisos;
            _serviciosc = serviciosc;
            _parametros = parametros;
            _usuarios = usuarios;
        }

        public async Task OnGet(int moduloId, int submoduloId, int solicitud)
        {
            string Usuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Permisos = await _permisos.GetPermisosByModuloUsuario(Usuario, moduloId);
            Contrato = await _contratos.GetContratoActivo();
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Crear")).Count() != 0)
            {
                if (Contrato.Activo)
                {
                    Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                    Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                    Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                    Solicitud = await _solicitudesQuery.GetSolicitudById(solicitud);
                    InmueblesUsuarios = (await _inmuebles.GetInmueblesByUsuarioServicio(Usuario, (int)Modulo.ServicioId)).Select(ius => ius.InmuebleId).ToList();
                    Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesUsuarios.Contains(i.Id)).ToList();
                    ServiciosContrato = await _serviciosc.GetServiciosByServicioAsync((int)Modulo.ServicioId);
                    Camiones = await _parametros.GetParametroByTipo("Unidad");
                    Supervision = (await _inmuebles.GetInmueblesByServicio((int)Modulo.ServicioId)).SingleOrDefault(i => i.InmuebleId == Solicitud.InmuebleId).GrupoSupervision;
                    Flujo = await _estatusc.GetFlujoByServicio((int)Modulo.ServicioId, Solicitud.EstatusId, Supervision);
                    Meses = await _meses.GetAllAsync();
                    Sede = await _inmuebles.GetSedeByUsuario(Usuario);
                    Direcciones = await _direcciones.GetAllDirecciones();
                    Usuarios = await _usuarios.GetAllAsync();
                }
                else
                {
                    Response.Redirect("/error/notContract");
                }
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }

        public async Task<IActionResult> OnPostCreateDireccion([FromBody] CreateDireccionCommand create)
        {
            var direccion = await _direcciones.CreateDireccion(create);
            return new JsonResult(direccion);
        }

        public async Task<IActionResult> OnPutUpdateDireccion([FromBody] UpdateDireccionCommand update)
        {
            var direccion = await _direcciones.UpdateDireccion(update);
            return new JsonResult(direccion);
        }
        
        public async Task<IActionResult> OnGetDireccionId(int id)
        {
            var direccion = await _direcciones.GetDireccionById(id);
            return new JsonResult(direccion);
        }
    }
}
