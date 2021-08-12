using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services.Interfaces;

namespace api.Controllers
{
    [Route("")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public TestController(ICustomerService customerService){
            _customerService = customerService;
        }

        [HttpGet]
        public IActionResult Get(){
            return new OkObjectResult("Dingus");
        }

        [HttpPost]
        public IActionResult Post(Customer customer){
            int id = _customerService.Save(customer);
            return new CreatedResult("", id.ToString());
        }

        [HttpGet]
        [Route("/{id}")]
        public IActionResult Get(int id){
            var response = _customerService.Retrieve(id);
            return new OkObjectResult( response );
        }
    }
}
