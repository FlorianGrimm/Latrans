using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Medaitor.Controllers {

    //[Route("api/[controller]")]
    //[ApiController]
    public class ResultControllerBase : ControllerBase {
        public ResultControllerBase() {
        }

        // https://docs.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-5.0
        // GET api/<ConfigurationController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id) {
            await Task.Yield();
            return null;
        }
    }
}
