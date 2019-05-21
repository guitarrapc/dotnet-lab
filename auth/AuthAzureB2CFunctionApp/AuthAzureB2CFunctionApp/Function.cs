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
using System;

namespace AuthAzureB2CFunctionApp
{
    public class Function : FunctionsStartup
    {
        private readonly IConfiguration configuration;
        private readonly AzureAdB2COptions azureAdB2COptions;

        public Function(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.azureAdB2COptions = new AzureAdB2COptions();
            configuration.GetSection(nameof(azureAdB2COptions)).Bind(azureAdB2COptions);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
        }

        [FunctionName("auth")]
        public IActionResult Auth(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation($"{nameof(Auth)}: C# HTTP trigger function processed a request.");
            req.HttpContext.Items.TryGetValue("MS_AzureFunctionsRequestID", out var requestId);
            var redirectUri = new Uri($"{req.Scheme}://{req.Host.Value}/{azureAdB2COptions.RedirectPathAuth}");
            var profile = azureAdB2COptions.SignUpSignInPolicyId;
            var url = $@"https://{azureAdB2COptions.Instance}/{azureAdB2COptions.Tenant}/{Uri.EscapeDataString(profile)}/oauth2/v2.0/authorize?client_id={azureAdB2COptions.ClientId}&redirect_uri={Uri.EscapeDataString(redirectUri.AbsoluteUri)}&response_type={Uri.EscapeDataString(azureAdB2COptions.ResponseType)}&scope=openid%20profile%20offline_access%20{Uri.EscapeDataString(azureAdB2COptions.ApiScopes)}&response_mode=form_post&nonce={Uri.EscapeDataString(context.InvocationId.ToString() + "_" + req.HttpContext.TraceIdentifier + "_" + requestId)}&x-client-SKU=ID_NETSTANDARD2_0&x-client-ver=5.3.0.0";
#if DEBUG
            LogParams(profile, redirectUri, req.HttpContext.TraceIdentifier, requestId, url, context, log);
#endif
            // redirect to AzureB2C directly
            return new RedirectResult(url);
        }

        [FunctionName("profile")]
        public IActionResult Profile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation($"{nameof(Profile)}: C# HTTP trigger function processed a request.");
            req.HttpContext.Items.TryGetValue("MS_AzureFunctionsRequestID", out var requestId);
            var redirectUri = new Uri($"{req.Scheme}://{req.Host.Value}/{azureAdB2COptions.RedirectPathProfile}");
            var profile = azureAdB2COptions.EditProfilePolicyId;
            var url = $@"https://{azureAdB2COptions.Instance}/{azureAdB2COptions.Tenant}/{Uri.EscapeDataString(profile)}/oauth2/v2.0/authorize?client_id={azureAdB2COptions.ClientId}&redirect_uri={Uri.EscapeDataString(redirectUri.AbsoluteUri)}&response_type={Uri.EscapeDataString(azureAdB2COptions.ResponseType)}&scope=openid%20profile%20offline_access%20{Uri.EscapeDataString(azureAdB2COptions.ApiScopes)}&response_mode=form_post&nonce={Uri.EscapeDataString(context.InvocationId.ToString() + "_" + req.HttpContext.TraceIdentifier + "_" + requestId)}&x-client-SKU=ID_NETSTANDARD2_0&x-client-ver=5.3.0.0";
#if DEBUG
            LogParams(profile, redirectUri, req.HttpContext.TraceIdentifier, requestId, url, context, log);
#endif
            // redirect to AzureB2C directly
            return new RedirectResult(url);
        }

        [FunctionName("resetpassword")]
        public IActionResult ResetPassword(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation($"{nameof(ResetPassword)}: C# HTTP trigger function processed a request.");
            req.HttpContext.Items.TryGetValue("MS_AzureFunctionsRequestID", out var requestId);
            var redirectUri = new Uri($"{req.Scheme}://{req.Host.Value}/{azureAdB2COptions.RedirectPathResetPassword}");
            var profile = azureAdB2COptions.ResetPasswordPolicyId;
            var url = $@"https://{azureAdB2COptions.Instance}/{azureAdB2COptions.Tenant}/{Uri.EscapeDataString(profile)}/oauth2/v2.0/authorize?client_id={azureAdB2COptions.ClientId}&redirect_uri={Uri.EscapeDataString(redirectUri.AbsoluteUri)}&response_type={Uri.EscapeDataString(azureAdB2COptions.ResponseType)}&scope=openid%20profile%20offline_access%20{Uri.EscapeDataString(azureAdB2COptions.ApiScopes)}&response_mode=form_post&nonce={Uri.EscapeDataString(context.InvocationId.ToString() + "_" + req.HttpContext.TraceIdentifier + "_" + requestId)}&x-client-SKU=ID_NETSTANDARD2_0&x-client-ver=5.3.0.0";
#if DEBUG
            LogParams(profile, redirectUri, req.HttpContext.TraceIdentifier, requestId, url, context, log);
#endif
            // redirect to AzureB2C directly
            return new RedirectResult(url);
        }

        [FunctionName("replyauth")]
        public async Task<IActionResult> ReplyAuth(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation($"{nameof(ReplyAuth)}: C# HTTP trigger function processed a request.");
            var redirectUri = new Uri($"{req.Scheme}://{req.Host.Value}/{azureAdB2COptions.RedirectPathAuth}");
#if DEBUG
            log.LogInformation($"Environment information");
            LogParams("", redirectUri, "", "", "", context, log);
#endif

            // get user info from `id_token`
            var idTokens = req.Form.Where(x => x.Key == "id_token").Select(x => x.Value).FirstOrDefault();
            if (!idTokens.Any())
                return new UnauthorizedResult();
            var payload = JwtPayload.Base64UrlDeserialize(idTokens.First().Split('.').Skip(1).FirstOrDefault());
            if (payload == null)
                return new BadRequestResult();
            var displayName = payload.Claims.FirstOrDefault(x => x.Type == "displayName")?.Value; // display name
            var signedInUserID = payload.Claims.FirstOrDefault(x => x.Type == "name")?.Value; // name
            var sub = payload.Claims.FirstOrDefault(x => x.Type == "sub")?.Value; // user id            
            var idp = payload.Claims.FirstOrDefault(x => x.Type == "idp")?.Value; // idp id
            bool.TryParse(payload.Claims.FirstOrDefault(x => x.Type == "newUser")?.Value, out bool isNewUser); // sign up only
            if (string.IsNullOrEmpty(signedInUserID))
            {
                return new UnauthorizedResult();
            }
            var id_validTo = payload.ValidTo;

            // setup username on front
            var name = string.IsNullOrEmpty(idp) ? displayName : signedInUserID;

            // OAuth authorization code is contains in `code`, short live 10min.
            // https://docs.microsoft.com/ja-jp/azure/active-directory-b2c/active-directory-b2c-reference-oauth-code
            var codes = req.Form.Where(x => x.Key == "code").FirstOrDefault().Value;
            if (!codes.Any())
            {
                return new UnauthorizedResult();
            }
            var token = await GetAuthenticationResultAsync(codes.First(), redirectUri);
            var expireOn = token.ExpiresOn;
            req.HttpContext.Response.Cookies.Append(".aadb2c_access_token", token.AccessToken);
            req.HttpContext.Response.Cookies.Append(".aadb2c_access_token_validuntil", expireOn.ToString("yyyy/MM/dd HH:mm:ss"));
            return new OkObjectResult(new
            {
                message = $"{(isNewUser ? $"successfully create account, welcome {name}!" : $"successfully login, welcome back {name}")}",
                name = name,
                idp = idp,
            });
        }

        [FunctionName("replyprofile")]
        public IActionResult ReplyProfile(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
    ExecutionContext context,
    ILogger log)
        {
            log.LogInformation($"{nameof(ReplyProfile)}: C# HTTP trigger function processed a request.");
            var redirectUri = new Uri($"{req.Scheme}://{req.Host.Value}/{azureAdB2COptions.RedirectPathProfile}");
#if DEBUG
            log.LogInformation($"Environment information");
            LogParams("", null, "", "", "", context, log);
#endif

            // get user info from `id_token`
            var idTokens = req.Form.Where(x => x.Key == "id_token").Select(x => x.Value).FirstOrDefault();
            if (!idTokens.Any())
                return new UnauthorizedResult();
            var payload = JwtPayload.Base64UrlDeserialize(idTokens.First().Split('.').Skip(1).FirstOrDefault());
            if (payload == null)
                return new BadRequestResult();
            var displayName = payload.Claims.FirstOrDefault(x => x.Type == "displayName")?.Value; // display name
            var signedInUserID = payload.Claims.FirstOrDefault(x => x.Type == "name")?.Value; // name
            var sub = payload.Claims.FirstOrDefault(x => x.Type == "sub")?.Value; // user id            
            var idp = payload.Claims.FirstOrDefault(x => x.Type == "idp")?.Value; // idp id
            var id_validTo = payload.ValidTo;

            return new OkObjectResult(new
            {
                message = "successfully edit profile."
            });
        }

        [FunctionName("replyresetpassword")]
        public IActionResult ReplyResetPassword(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
    ExecutionContext context,
    ILogger log)
        {
            log.LogInformation($"{nameof(ReplyResetPassword)}: C# HTTP trigger function processed a request.");
            var redirectUri = new Uri($"{req.Scheme}://{req.Host.Value}/{azureAdB2COptions.RedirectPathProfile}");
#if DEBUG
            log.LogInformation($"Environment information");
            LogParams("", null, "", "", "", context, log);
#endif

            // get user info from `id_token`
            var idTokens = req.Form.Where(x => x.Key == "id_token").Select(x => x.Value).FirstOrDefault();
            if (!idTokens.Any())
                return new UnauthorizedResult();
            var payload = JwtPayload.Base64UrlDeserialize(idTokens.First().Split('.').Skip(1).FirstOrDefault());
            if (payload == null)
                return new BadRequestResult();
            var displayName = payload.Claims.FirstOrDefault(x => x.Type == "displayName")?.Value; // display name
            var signedInUserID = payload.Claims.FirstOrDefault(x => x.Type == "name")?.Value; // name
            var sub = payload.Claims.FirstOrDefault(x => x.Type == "sub")?.Value; // user id            
            var idp = payload.Claims.FirstOrDefault(x => x.Type == "idp")?.Value; // idp id
            var id_validTo = payload.ValidTo;

            return new OkObjectResult(new
            {
                message = "successfully reset password."
            });
        }

        private async Task<AuthenticationResult> GetAuthenticationResultAsync(string code, Uri redirectUri)
        {            
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create($"{azureAdB2COptions.ClientId}")
                .WithRedirectUri(redirectUri.AbsoluteUri)
                .WithClientSecret(azureAdB2COptions.ClientSecret)
                .WithB2CAuthority(azureAdB2COptions.Authority)
                .Build();

            // make sure you cannot use Refresh Token with MSAL anymore.
            var scopes = azureAdB2COptions.ApiScopes.Split(' ');
            var auth = await app.AcquireTokenByAuthorizationCode(scopes, code).ExecuteAsync();
            return auth;
        }

        private void LogParams(string profile, Uri redirectUri, string traceIdentifier, object requestId, string url, ExecutionContext context, ILogger log)
        {
            log.LogInformation($"Environment information");
            var parameters = new[] {
                $"  {nameof(azureAdB2COptions.Instance)} = {azureAdB2COptions.Instance}",
                $"  {nameof(azureAdB2COptions.Tenant)} = {azureAdB2COptions.Tenant}",
                $"  {nameof(profile)} = {profile}",
                $"  {nameof(azureAdB2COptions.ClientId)} = {azureAdB2COptions.ClientId}",
                $"  {nameof(azureAdB2COptions.RedirectPathAuth)} = {azureAdB2COptions.RedirectPathAuth}",
                $"  {nameof(azureAdB2COptions.ResponseType)} = {azureAdB2COptions.ResponseType}",
                $"  {nameof(azureAdB2COptions.ApiScopes)} = {azureAdB2COptions.ApiScopes}",
                $"  redirectUri = {redirectUri?.AbsoluteUri}",
                $"  {nameof(context.InvocationId)} = {context.InvocationId}",
                $"  TraceIdentifier = {traceIdentifier}",
                $"  MS_AzureFunctionsRequestID = {requestId}",
                $"  url = {url}",
            };
            log.LogInformation(string.Join("\n", parameters));
        }
    }
}
