using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CompanyCollectorWeb.HubConfig;
using CompanyCollectorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CompanyCollectorWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyCollectorController : ControllerBase
    {
        private readonly IHubContext<CompanyHub> _hubContext;

        public CompanyCollectorController(IHubContext<CompanyHub> hub)
        {
            _hubContext = hub;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            Companies companies = new Companies(s =>
            {
                Debug.WriteLine(s);
            });
            return await companies.GetCompanies(CompanyArrived, 50); //todo: add link
        }

        [HttpPost]
        [Route("deliverypoint")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> CompanyArrived(CompanyModel companyModel)
        {
            await _hubContext.Clients.All.SendAsync("CompanyMessageReceived", companyModel);

            return StatusCode(200);
        }
    }
}
