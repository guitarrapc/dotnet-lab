using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;

namespace AuthAzureB2CFunctionApp
{
    public class Function : FunctionsStartup
    {
        private readonly IConfiguration configuration;

        public Function(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
        }

        [FunctionName("auth")]
        public async Task<IActionResult> Auth(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(Auth)}: C# HTTP trigger function processed a request.");
            //var url = $"{azureAdB2cAuth.Instance}/{azureAdB2COptions.Tenant}/oauth2/v2.0/authorize?p={Uri.EscapeDataString(azureAdB2COptions.SignUpSignInPolicyId)}&response_type=token&state=&client_id={azureAdB2COptions.ClientId}&scope={Uri.EscapeDataString(azureAdB2cAuth.Scope)}&redirect_uri={Uri.EscapeDataString(azureAdB2cAuth.CallbackUrl)}";
            var hoge = configuration.GetValue<string>("hoge");
            // TODO: GEn Nounce from Session or distributed random session
            return new RedirectResult(@"https://{AzureAdB2cAuth.Instance}/{AzureAdB2cAuth.Tenant}/{AzureAdB2cAuth.SignUpSignInPolicyId}/oauth2/v2.0/authorize?client_id={AzureAdB2cAuth.ClientId}&redirect_uri={Url.DataEncode(AzureAdB2cAuth.RedirectUri)}&response_type=code%20id_token&scope=openid%20profile%20offline_access%2{Uri.DataEncode(AzureAdB2cAuth.ApiScopes)}&response_mode=form_post&nonce={Session}&x-client-SKU=ID_NETSTANDARD2_0&x-client-ver=5.3.0.0");
        }

        [FunctionName("reply")]
        public async Task<IActionResult> Reply(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation($"{nameof(Reply)}: C# HTTP trigger function processed a request.");

            // get user info from `id_token`
            var idToken = req.Form.Where(x => x.Key == "id_token").FirstOrDefault().Value;
            var payload = JwtPayload.Base64UrlDeserialize(idToken.First().Split('.').Skip(1).First());
            var signedInUserID = payload.Claims.Where(x => x.Type == "name").First().Value;
            if (string.IsNullOrEmpty(signedInUserID))
            {
                return new UnauthorizedResult();
            }

            // OAuth authorization code is contains in `code`
            var code = req.Form.Where(x => x.Key == "code").FirstOrDefault().Value;
            var token = await GetAuthenticationResultAsync(code.First(), signedInUserID);
            return new OkObjectResult(token.AccessToken);
        }

        private async Task<AuthenticationResult> GetAuthenticationResultAsync(string code, string signedInUserID)
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create("{AzureAdB2cAuth.ClientId}")
                .WithRedirectUri("{AzureAdB2cAuth.RedirectUri}")
                .WithClientSecret("{AzureAdB2cAuth.ClientSecret}")
                .WithB2CAuthority("{AzureAdB2cAuth.AzureAdB2CInstance}/{AzureAdB2cAuth.Tenant}/{AzureAdB2cAuth.SignUpSignInPolicyId}/v2.0")
                .Build();

            // TODO: split scope string to array
            var auth = await app.AcquireTokenByAuthorizationCode(new[] { "{AzureAdB2cAuth.ApiScopes}" }, code).ExecuteAsync();
            return auth;
        }

        public class AzureAdB2cAuth
        {
            public string AzureAdB2CInstance { get; set; }
            public string ClientId { get; set; }
            public string Tenant { get; set; }
            public string SignUpSignInPolicyId { get; set; }
            public string ResetPasswordPolicyId { get; set; }
            public string EditProfilePolicyId { get; set; }
            public string RedirectUri { get; set; }
            public string ClientSecret { get; set; }
            public string ApiUrl { get; set; }
            public string ApiScopes { get; set; }
            public string Instance { get; set; }
        }
    }

}
