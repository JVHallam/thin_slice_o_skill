using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace practise_area.Controllers
{
    public class Details{
        public string Name;
    }

    [ApiController]
    [Route("[controller]")]
    public class MyController : ControllerBase
    {
        public MyController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return new OkObjectResult("Dingus");
        }

        [HttpPost]
        public IActionResult Post([FromBody] Details details){
            var str = JsonConvert.SerializeObject( details );
            return new OkObjectResult($"response - {str}");
        }
    }
}
