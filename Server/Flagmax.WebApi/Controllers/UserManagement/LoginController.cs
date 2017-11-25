using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Security;
using Microsoft.AspNet.Identity;

namespace Flagmax.WebApi.Controllers.UserManagement
{
    public class LoginController : ApiController
    {
        [AllowAnonymous]
        public HttpResponseMessage Post(LoginModel model)
        {
            if (String.IsNullOrEmpty(model.Password))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid credentials");
            }

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, model.Login));
            var id = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignIn(id);

            return Request.CreateResponse();
        }

        public class LoginModel
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }
    }
}
