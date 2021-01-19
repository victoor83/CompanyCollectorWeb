using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CompanyCollectorWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyCollectorController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Companies companies = new Companies();

            var names = companies.GetCompanies(3);

            return names.ToArray();
        }
    }
}
