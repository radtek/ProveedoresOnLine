using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace RegisterService.Filters
{
    public class JudicialProcessAuthorizationAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContent)
        {
            IEnumerable<string> values;
            if (actionContent.Request.Headers.TryGetValues("Authorization", out values))
            {
                bool tokenResult = Register_Core.Controller.Register_Core.IsAuthorized(values?.First());
                if (tokenResult == false)
                {
                    actionContent.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
            }
            else
                actionContent.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

        }
    }
}