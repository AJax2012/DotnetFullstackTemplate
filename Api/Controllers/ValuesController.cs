using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SourceName.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet()]
        public IActionResult Get()
        {
            return Ok(new List<string> { "value1", "value2" });
        }

        [HttpGet("{value:int}")]
        public IActionResult Get([FromRoute]int value)
        {
            return Ok($"value {value}");
        }
    }
}