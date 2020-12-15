using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Brimborium.Latrans.Medaitor;
using Brimborium.Latrans.Utility;

using DemoWebApp.ActivityModel.ConfigurationActivity;

using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IMedaitorAccess _MedaitorAccess;

        public ConfigurationController(IMedaitorAccess medaitorAccess) {
            this._MedaitorAccess = medaitorAccess;
        }

        // GET: api/<ConfigurationController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            try {
                var medaitorClient = this._MedaitorAccess.GetMedaitorClient();
                var arguments = new GetConfigurationRequest();
                var ctxt = medaitorClient.CreateContextByRequest(arguments);
                await medaitorClient.ExecuteAsync(ctxt, null, this.HttpContext.RequestAborted);
                return ctxt.ReturnAsActionResult<IEnumerable<string>>();
            } catch (System.Exception error) {
                return this.StatusCode(500, error.Message);
            }
        }

        // GET api/<ConfigurationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ConfigurationController>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<ConfigurationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<ConfigurationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
