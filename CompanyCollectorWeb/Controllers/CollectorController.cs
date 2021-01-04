using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyCollectorWeb.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Route("[controller]")]
    public class CollectorController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {

            var lst = new List<string>{"bmw", "abb", "VW"};

            return lst.ToArray();
        }
    }
}
