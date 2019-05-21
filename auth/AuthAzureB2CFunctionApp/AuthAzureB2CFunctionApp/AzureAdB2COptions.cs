using System;
using System.Collections.Generic;
using System.Text;

namespace AuthAzureB2CFunctionApp
{
    public class AzureAdB2COptions
    {
        public AzureAdB2COptions() { }

        /// <summary>
        /// https://your_domain.b2clogin.com/tfp
        /// </summary>
        public string AzureAdB2CInstance { get; set; }
        /// <summary>
        /// client-id-you-have-issued-on-b2c-application
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// your_domain.onmicrosoft.com
        /// </summary>
        public string Tenant { get; set; }
        /// <summary>
        /// B2C_1_susi
        /// </summary>
        public string SignUpSignInPolicyId { get; set; }
        /// <summary>
        /// B2C_1_signin
        /// </summary>
        public string SignInPolicyId { get; set; }
        /// <summary>
        /// B2C_1_signup
        /// </summary>
        public string SignUpPolicyId { get; set; }
        /// <summary>
        /// B2C_1_reset
        /// </summary>
        public string ResetPasswordPolicyId { get; set; }
        /// <summary>
        /// B2C_1_edit
        /// </summary>
        public string EditProfilePolicyId { get; set; }
        /// <summary>
        /// path of your reply url, in `https://hogemoge.azurefunctions.com/api/reply` case, `api/reply`.
        /// </summary>
        public string RedirectPathAuth { get; set; }
        /// <summary>
        /// path of your reply url, in `https://hogemoge.azurefunctions.com/api/reply` case, `api/reply`.
        /// </summary>
        public string RedirectPathProfile { get; set; }
        /// <summary>
        /// path of your reply url, in `https://hogemoge.azurefunctions.com/api/reply` case, `api/reply`.
        /// </summary>
        public string RedirectPathResetPassword { get; set; }
        /// <summary>
        /// Client Secret you have issued on B2c Application for OAuth.
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// https://your_domain.onmicrosoft.com/your_api/user_impersonation https://your_domain.onmicrosoft.com/your_api/api:scope
        /// </summary>
        /// <remarks>
        /// Omit "openid profile offline_acccess" as it will put default.
        /// </remarks>
        public string ApiScopes { get; set; }
        /// <summary>
        /// yourdomain.b2clogin.com
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// code id_token
        /// </summary>
        public string ResponseType { get; set; }
        public string DefaultPolicy => SignUpSignInPolicyId;
        public string Authority => $"{AzureAdB2CInstance}/{Tenant}/{DefaultPolicy}/v2.0";
    }
}
