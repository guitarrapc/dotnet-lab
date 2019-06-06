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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
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

        [FunctionName("accessToken")]
        public async Task<IActionResult> AccessToken(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
    ExecutionContext context,
    ILogger log)
        {
            log.LogInformation($"{nameof(AccessToken)}: C# HTTP trigger function processed a request.");
            var redirectUri = new Uri($"{req.Scheme}://{req.Host.Value}/{azureAdB2COptions.RedirectPathAuth}");
            req.HttpContext.Items.TryGetValue("MS_AzureFunctionsRequestID", out var requestId);
#if DEBUG
            log.LogInformation($"Environment information");
            LogParams("", redirectUri, "", "", "", context, log);
#endif

            using (var reader = new StreamReader(req.Body))
            {
                var body = await reader.ReadToEndAsync();
                var authReply = JsonConvert.DeserializeObject<AuthReplyBody>(body);

                // error action
                if (authReply.HasError())
                    return new BadRequestObjectResult(new
                    {
                        Message = authReply.error,
                        Description = authReply.error_description,
                        NextAction = "please reauthorize again.",
                    });
                if (authReply.IsMissingCode() || authReply.IsMissingIdToken())
                    return new BadRequestObjectResult(new { error = "missing id_token, code." });
                if (authReply.PayLoad == null)
                    return new BadRequestObjectResult(new { error = "invalid id_token detected." });

                // get user info from `id_token`
                var response = authReply.GetAuthResponse();
                if (string.IsNullOrEmpty(response.SignedInUserID))
                    return new BadRequestObjectResult(new { error = "invalid id_token detected. Missing name property." });

                // get access token
                var token = await authReply.GetAuthenticationResultAsync(redirectUri, azureAdB2COptions);
                if (string.IsNullOrEmpty(token.AccessToken))
                    return new UnauthorizedResult();

                // set cookie
                var expireOn = token.ExpiresOn;
                req.HttpContext.Response.Cookies.Append(".aadb2c_access_token", token.AccessToken, new CookieOptions() { Expires = expireOn, Secure = true });
                req.HttpContext.Response.Cookies.Append(".aadb2c_id_token", token.IdToken, new CookieOptions() { Expires = expireOn, Secure = true });

                return new OkObjectResult(new
                {
                    message = response.Message,
                    name = response.Name,
                    email = response.Email,
                    idp = response.Idp,
                    canResetPassword = response.CanResetPassword,
#if DEBUG
                    AuthorizationHeader = $"Bearer {token.AccessToken}",
#endif
                });
            }
        }

        [FunctionName("profile")]
        public IActionResult Profile(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
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

            var id_token = req.Form.Where(x => x.Key == "id_token").Select(x => x.Value).FirstOrDefault();
            var code = req.Form.Where(x => x.Key == "code").Select(x => x.Value).FirstOrDefault();
            var authReply = new AuthReplyBody(id_token, code)
            {
                error = req.Form.Where(x => x.Key == "error").Select(x => x.Value).FirstOrDefault(),
                error_description = req.Form.Where(x => x.Key == "error_description").Select(x => x.Value).FirstOrDefault(),
            };

            // error check
            if (authReply.HasError())
                return new BadRequestObjectResult(new
                {
                    Message = authReply.error,
                    Description = authReply.error_description,
                    NextAction = "please reauthorize again.",
                });
            if (authReply.IsMissingCode() || authReply.IsMissingIdToken())
                return new BadRequestObjectResult(new { error = "missing id_token, code." });
            if (authReply.PayLoad == null)
                return new BadRequestObjectResult(new { error = "invalid id_token detected." });

            // get user info from `id_token`
            var response = authReply.GetAuthResponse();
            if (string.IsNullOrEmpty(response.SignedInUserID))
                return new UnauthorizedResult();

            // get access token
            var token = await authReply.GetAuthenticationResultAsync(redirectUri, azureAdB2COptions);
            if (token.AccessToken == null)
                return new UnauthorizedResult();

            // set cookie
            var expireOn = token.ExpiresOn;
            req.HttpContext.Response.Cookies.Append(".aadb2c_access_token", token.AccessToken, new CookieOptions() { Expires = expireOn, Secure = true });
            req.HttpContext.Response.Cookies.Append(".aadb2c_id_token", token.IdToken, new CookieOptions() { Expires = expireOn, Secure = true });

            return new OkObjectResult(new
            {
                message = response.Message,
                name = response.Name,
                email = response.Email,
                idp = response.Idp,
                canResetPassword = response.CanResetPassword,
#if DEBUG
                AuthorizationHeader = $"Bearer {token.AccessToken}",
#endif
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

            return new OkObjectResult(new
            {
                message = "successfully reset password."
            });
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
