using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthAzureB2CApiApplication.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AzureADB2COptions azureAdB2COptions;
        private readonly AzureAdB2cAuth azureAdB2cAuth;

        public AuthController(IOptions<AzureADB2COptions> b2cOptions, IOptions<AzureAdB2cAuth> auth)
        {
            azureAdB2COptions = b2cOptions.Value;
            azureAdB2cAuth = auth.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("SignUrl")]
        public ActionResult<string> SignInUrl() => GenerateSigninUrl();

        [AllowAnonymous]
        [HttpGet]
        [Route("RedirectSign")]
        public IActionResult RedirectSignIn() => Redirect(GenerateSigninUrl());

        [AllowAnonymous]
        [HttpPost]
        [Route("Show")]
        public ActionResult<string> Post(Hoge value)
        {
            return value.Foo;
        }
        public class Hoge {
            public string Foo { get; set; }
            public string Bar { get; set; }
        }

        private string GenerateSigninUrl()
        {
            // https://nsguitarrapctest.b2clogin.com/nsguitarrapctest.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_signupsignin1&response_type=token&state=&client_id=0e193895-6062-4be0-b750-228cba0fff10&scope=https%3A%2F%2Fnsguitarrapctest.onmicrosoft.com%2Fdemoapi%2Fuser_impersonation%20openid%20offline_access&redirect_uri=https%3A%2F%2Fgetpostman.com%2Fpostman
            // https://nsguitarrapctest.b2clogin.com/nsguitarrapctest.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_signupsignin1&response_type=token&state=&client_id=d759e265-e2c1-4367-ba3f-1151807af50d&scope=https%3A%2F%2Fnsguitarrapctest.onmicrosoft.com%2Fdemoapi%2Fuser_impersonation%20openid%20offline_access&redirect_uri=https%3A%2F%2Fjwt.ms
            var url = $"{azureAdB2cAuth.Instance}/{azureAdB2COptions.Domain}/oauth2/v2.0/authorize?p={Uri.EscapeDataString(azureAdB2COptions.SignUpSignInPolicyId)}&response_type=token&state=&client_id={azureAdB2COptions.ClientId}&scope={Uri.EscapeDataString(azureAdB2cAuth.Scope)}&redirect_uri={Uri.EscapeDataString(azureAdB2cAuth.CallbackUrl)}";
            return url;
        }
    }
}
