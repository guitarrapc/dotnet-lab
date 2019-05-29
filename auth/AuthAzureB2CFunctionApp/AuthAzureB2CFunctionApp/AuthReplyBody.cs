using Microsoft.Identity.Client;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAzureB2CFunctionApp
{
    internal class AuthReplyBody
    {
        public AuthReplyBody(string id_token, string code)
        {
            this.id_token = id_token;
            this.code = code;
        }

        public string id_token { get; set; }
        public string code { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }

        public JwtPayload PayLoad
        {
            get
            {
                if (payload == null)
                {
                    payload = JwtPayload.Base64UrlDeserialize(id_token.Split('.').Skip(1).FirstOrDefault());
                }
                return payload;
            }
        }
        private JwtPayload payload = null;

        public bool HasError() => !string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(error_description);
        public bool IsMissingIdToken() => string.IsNullOrEmpty(id_token);
        public bool IsMissingCode() => string.IsNullOrEmpty(code);

        public async Task<AuthenticationResult> GetAuthenticationResultAsync(Uri redirectUri, AzureAdB2COptions options)
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create($"{options.ClientId}")
                .WithRedirectUri(redirectUri.AbsoluteUri)
                .WithClientSecret(options.ClientSecret)
                .WithB2CAuthority(options.Authority)
                .Build();

            // make sure you cannot use Refresh Token with MSAL anymore.
            var scopes = options.ApiScopes.Split(' ');

            // OAuth authorization code is contains in `code`, short live 10min.
            // https://docs.microsoft.com/ja-jp/azure/active-directory-b2c/active-directory-b2c-reference-oauth-code
            var auth = await app.AcquireTokenByAuthorizationCode(scopes, code).ExecuteAsync();
            return auth;
        }

        public AuthResponse GetAuthResponse()
        {
            var signedInUserID = GetPayloadValue("name"); // name
            var displayName = GetPayloadValue("displayName"); // display name
            var email = GetPayloadValue("emails"); // name
            var name = string.IsNullOrEmpty(displayName) ? signedInUserID : displayName;
            var sub = GetPayloadValue("sub"); // user id            
            var idp = GetPayloadValue("idp"); // idp id
            bool.TryParse(GetPayloadValue("newUser"), out bool isNewUser); // sign up only

            return new AuthResponse
            {
                SignedInUserID = signedInUserID,
                Message = $"{(isNewUser ? $"successfully create account, welcome {name}!" : $"successfully login, welcome back {name}")}",
                Name = name,
                Email = email,
                Idp = idp,
                CanResetPassword = string.IsNullOrEmpty(idp),
            };
        }
        private string GetPayloadValue(string type) => PayLoad.Claims.FirstOrDefault(x => x.Type == type)?.Value;

        public class AuthResponse
        {
            public string SignedInUserID { get; set; }
            public string Message { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Idp { get; set; }
            public bool CanResetPassword { get; set; }
            public string AuthorizationHeader { get; set; }
        }
    }
}
