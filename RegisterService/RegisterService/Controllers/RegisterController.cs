using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace RegisterService.Controllers
{
    [RoutePrefix("api/register")]
    public class RegisterController : ApiController
    {
        [HttpGet, Filters.JudicialProcessAuthorization]
        public async Task<IHttpActionResult> Search(int IdType, string IdentificationNumber, string Token)
        {
            return Ok(await Register_Core.Controller.Register_Core.Search(IdType, IdentificationNumber, Token));
        }
    }
}
