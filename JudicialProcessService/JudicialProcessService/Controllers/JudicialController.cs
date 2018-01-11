using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace JudicialProcessService.Controllers
{

    [RoutePrefix("api/judicial")]
    public class JudicialController : ApiController
    {
        [HttpGet, Filters.JudicialProcessAuthorization]
        public async Task<IHttpActionResult> Search(int IdType, string IdentificationNumber, string Token)
        {
            return Ok(await JudicialProcess_Core.Controller.JudicialProcess_Core.Search(IdType, "", IdentificationNumber, Token));
        }
    }
}
