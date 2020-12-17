using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Mediator;
using Brimborium.Latrans.Utility;

using DemoWebApp.ActivityModel.ConfigurationActivity;

using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoWebApp.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase {
        private readonly IMediatorAccess _MedaitorAccess;

        public ConfigurationController(IMediatorAccess medaitorAccess) {
            this._MedaitorAccess = medaitorAccess;
        }

        // GET: api/<ConfigurationController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get() {
#if false
            try {
                var medaitorClient = this._MedaitorAccess.GetMedaitorClient();
                var arguments = new GetConfigurationRequest();
                var ctxt = medaitorClient.CreateContextByRequest(arguments);
                await medaitorClient.ExecuteAsync(ctxt, null, this.HttpContext.RequestAborted);
                return ctxt.ReturnAsActionResult<IEnumerable<string>>();
            } catch (System.Exception error) {
                return this.StatusCode(500, error.Message);
            }
#endif
            try {
                var request = new GetConfigurationRequest();
                using var client = this._MedaitorAccess.GetMedaitorClient();
                using var connected = await client.ConnectAsync(request, this.HttpContext.RequestAborted);
                var response = await connected.WaitForAsync(null, this.HttpContext.RequestAborted);
                return response.ConvertResponseToActionResult<GetConfigurationResponse, IEnumerable<string>>((r) => r.Result);
            } catch (System.Exception error) {
                return this.StatusCode(500, error.Message);
            }
        }

        // GET api/<ConfigurationController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id) {
            /*
            try {
                //using var p = new RequestResponsePair<GetConfigurationRequest, GetConfigurationResponse>();
                //var response2 = await p.ExecuteAsync(
                //    new GetConfigurationRequest(),
                //    this._MedaitorAccess,
                //    null,
                //    this.HttpContext.RequestAborted
                //    );
                var request = new GetConfigurationRequest();
                var medaitorAccess = this._MedaitorAccess;
                using var client = medaitorAccess.GetMedaitorClient();
                using var connected = await client.ConnectAsync(request, this.HttpContext.RequestAborted);
                var response = await connected.WaitForAsync(null, this.HttpContext.RequestAborted);
                return response.ConvertResponseToActionResult<GetConfigurationResponse, string>((r) => r.Result?.FirstOrDefault());
            } catch (System.Exception error) {
                return this.StatusCode(500, error.Message);
            }
            */
            var request = new GetConfigurationRequest();
            return await RequestResponseHelper<GetConfigurationRequest, GetConfigurationResponse>.
                ExecuteToActionResultAsync<string>(
                    this._MedaitorAccess,
                    request,
                    (r) => r.Result?.FirstOrDefault(),
                    null,
                    this.HttpContext.RequestAborted
                );
        }



        // POST api/<ConfigurationController>
        [HttpPost]
        public void Post([FromBody] string value) {
        }

        // PUT api/<ConfigurationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/<ConfigurationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}
