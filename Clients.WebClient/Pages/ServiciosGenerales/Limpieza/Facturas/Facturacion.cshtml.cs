using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Limpieza.CedulaEvaluacion;
using Api.Gateway.WebClient.Proxy.Limpieza.Facturas;
using Api.Gateway.WebClient.Proxy.Limpieza.Repositorios;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Limpieza.Facturas
{
    public class FacturacionModel : PageModel
    {
        private readonly IMesProxy _mes;
        private readonly IInmuebleProxy _inmuebles;
        private readonly ILRepositorioProxy _repositorios;
        private readonly ILCFDIProxy _cfdi;
        private readonly ILCedulaProxy _cedula;
        private readonly IEstatusCedulaProxy _estatusc;
        private readonly IModuloProxy _modulo;
        private readonly IPermisoProxy _permisos;
        private readonly ICTServicioProxy _servicios;

        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }
        public SubmoduloDto Submodulo { get; set; }
        public CTServicioDto Servicio { get; set; }
        public List<int> InmueblesServicio { get; set; }
        public List<InmuebleDto> Inmuebles { get; set; }
        public RepositorioDto Repositorio { get; set; }
        public MesDto Mes { get; set; }
        public ModuloDto Modulo { get; set; }
        public List<PermisoUsuarioDto> Permisos { get; set; }

        public FacturacionModel(IMesProxy mes, IInmuebleProxy inmuebles, ILRepositorioProxy repositorios, ILCFDIProxy cfdi, ILCedulaProxy cedula, 
                                IEstatusCedulaProxy estatusc, IModuloProxy modulo, IPermisoProxy permisos, ICTServicioProxy servicios)
        {
            _mes = mes;
            _inmuebles = inmuebles;
            _repositorios = repositorios;
            _cfdi = cfdi;
            _cedula = cedula;
            _estatusc = estatusc;
            _modulo = modulo;
            _permisos = permisos;
            _servicios = servicios;
        }

        public async Task OnGet(int moduloId, int submoduloId, int facturacion, int mes)
        {
            Permisos = await _permisos.GetPermisosByModuloUsuario(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                Repositorio = await _repositorios.GetRepositorioById(facturacion);
                InmueblesServicio = (await _inmuebles.GetInmueblesByServicio((int)Modulo.ServicioId)).Select(s => s.InmuebleId).ToList();
                Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesServicio.Contains(i.Id)).ToList();
                Mes = await _mes.GetAsync(mes);

                foreach (var inm in Inmuebles)
                {
                    var cedula = await _cedula.GetCedulaByInmuebleAnioMesAsync(inm.Id, Repositorio.Anio, mes);
                    inm.Facturas = await _cfdi.GetFacturasByInmueble(inm.Id, facturacion);
                    inm.Cedula.RequiereNC = cedula.Id != 0 ? (await _cedula.GetTotalPDAsync(cedula.Id)) > 0 ? true : false : false;
                    inm.Cedula.EstatusCedula = cedula.Id != 0 ? (await _estatusc.GetECByIdAsync(cedula.EstatusId)).Nombre : "Sin Iniciar";
                }
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }
    }
}
