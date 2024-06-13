using Api.Gateway.Models.Catalogos.DTOs.Servicios;
using Api.Gateway.Models.Repositorios.DTOs;
using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.Models.Meses.DTOs;
using Api.Gateway.Models.Modulos.DTOs;
using Api.Gateway.Models.Permisos.DTOs;
using Api.Gateway.WebClient.Proxy.Catalogos.CTServicios;
using Api.Gateway.WebClient.Proxy.Estatus;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Api.Gateway.WebClient.Proxy.Meses;
using Api.Gateway.WebClient.Proxy.Modulos;
using Api.Gateway.WebClient.Proxy.Permisos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Gateway.WebClient.Proxy.Comedor.CFDIs.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.CedulasEvaluacion.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Repositorios.Queries;
using Api.Gateway.WebClient.Proxy.Comedor.Repositorios.Commands;

namespace Clients.WebClient.Pages.Comedor.Facturas
{
    public class FacturacionModel : PageModel
    {
        private readonly IMesProxy _mes;
        private readonly IInmuebleProxy _inmuebles;
        private readonly IQRepositorioComedorProxy _repositoriosQuery;
        private readonly IQCFDIComedorProxy _cfdi;
        private readonly IQCedulaComedorProxy _cedula;
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

        public FacturacionModel(IMesProxy mes, IInmuebleProxy inmuebles, IQRepositorioComedorProxy repositoriosQuery, 
                                IQCFDIComedorProxy cfdi, IQCedulaComedorProxy cedula, IEstatusCedulaProxy estatusc, 
                                IModuloProxy modulo, IPermisoProxy permisos, ICTServicioProxy servicios)
        {
            _mes = mes;
            _inmuebles = inmuebles;
            _repositoriosQuery = repositoriosQuery;
            _cfdi = cfdi;
            _cedula = cedula;
            _estatusc = estatusc;
            _modulo = modulo;
            _permisos = permisos;
            _servicios = servicios;
        }

        public async Task OnGet(int moduloId, int submoduloId, int repositorio, int mes)
        {
            string usuario = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            Permisos = await _permisos.GetPermisosByModuloUsuario(usuario, moduloId);
            if (Permisos.Where(p => p.Permiso.Nombre.Equals("Ver")).Count() != 0)
            {
                Modulo = await _modulo.GetModuloByIdAsync(moduloId);
                Submodulo = await _modulo.GetSubmoduloByIdAsync(submoduloId);
                Servicio = await _servicios.GetServicioByIdAsync((int)Modulo.ServicioId);
                Repositorio = await _repositoriosQuery.GetRepositorioById(repositorio);
                InmueblesServicio = (await _inmuebles.GetInmueblesByServicio((int)Modulo.ServicioId)).Select(s => s.InmuebleId).ToList();
                Inmuebles = (await _inmuebles.GetAllInmueblesAsync()).Where(i => InmueblesServicio.Contains(i.Id)).ToList();
                Mes = await _mes.GetAsync(mes);
                var cedulas = await _cedula.GetCedulaByAnioMes(Servicio.Id, Repositorio.Anio, mes, Repositorio.ContratoId, usuario);
                var facturas = await _cfdi.GetAllFacturasByRepositorio(repositorio);

                foreach (var i in Inmuebles)
                {
                    i.Facturas = facturas.Where(f => f.InmuebleId == i.Id).ToList();
                    if (cedulas.HasItems)
                    {
                        i.Cedula.EstatusCedula = cedulas.Items.Where(c => c.InmuebleId == i.Id).Count() != 0 ? cedulas.Items.Single(c => c.InmuebleId == i.Id).Estatus : "";
                        i.Cedula.RequiereNC = cedulas.Items.Where(c => c.InmuebleId == i.Id).Count() != 0 ? (bool)cedulas.Items.Single(c => c.InmuebleId == i.Id).RequiereNC : false;
                    }
                }
            }
            else
            {
                Response.Redirect("/error/denegado");
            }
        }
    }
}
