using Api.Gateway.Models.Inmuebles.DTOs.Inmuebles;
using Api.Gateway.WebClient.Proxy.Inmuebles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.WebClient.Pages.Inmuebles
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IInmuebleProxy _inmuebles;

        public List<InmuebleDto> Inmuebles { get; set; }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public IndexModel(
           ILogger<IndexModel> logger,
           IInmuebleProxy limpiezaProxy
       )
        {
            _inmuebles = limpiezaProxy;
            _logger = logger;
        }
        public async Task OnGet()
        {
            Inmuebles = await _inmuebles.GetAllInmueblesAsync();
        }
    }
}
