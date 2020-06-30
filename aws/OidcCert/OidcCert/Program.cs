using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace OidcCert
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var endpoint = new Uri($"https://oidc.eks.ap-northeast-1.amazonaws.com:443");
            using (var handler = new CustomHttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback += (HttpRequestMessage msg, X509Certificate2 cert, X509Chain chain, SslPolicyErrors pollyError) =>
                {
                    Console.WriteLine($"PollyError: {pollyError}, Cert: {cert.FriendlyName}, {cert.Subject}, {cert.Thumbprint.ToLower()}");
                    //chain.ChainElements.Dump();
                    var validcert = chain.ChainElements.Cast<X509ChainElement>()
                    .Select(item =>
                    {
                        Console.WriteLine($"{item.Certificate.FriendlyName}, {item.Certificate.Subject}, {item.Certificate.Thumbprint.ToLower()}, {item.Certificate.NotAfter}");
                        return item;
                    })
                    .Where(x => x.Certificate.Extensions.Cast<X509Extension>().Any(x => x.Critical))
                    .Reverse() // check chain last
                    .First();

                    Console.WriteLine($"Result Cert: {validcert.Certificate.FriendlyName}, {validcert.Certificate.Subject}, {validcert.Certificate.Thumbprint.ToLower()}, {validcert.Certificate.NotAfter}");
                    return chain.Build(cert);
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    using (HttpResponseMessage response = await client.GetAsync(endpoint))
                    {
                        // do nothing.
                    }
                }
            }
        }
    }

    public class CustomHttpClientHandler : HttpClientHandler
    {
        public CustomHttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = VerifyServerCertificate;
        }

        private bool VerifyServerCertificate(
          HttpRequestMessage message,
          X509Certificate certificate,
          X509Chain chain,
          SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                var isValid = chain.Build((X509Certificate2)certificate);

                return isValid;
            }

            // In all other cases, return false.
            return false;
        }
    }
}
