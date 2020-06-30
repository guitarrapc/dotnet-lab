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
            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback += (HttpRequestMessage msg, X509Certificate2 cert, X509Chain chain, SslPolicyErrors pollyError) =>
                {
                    Console.WriteLine($"{chain.ChainElements.Count} chains, PollyError: {pollyError}, Cert: {cert.FriendlyName}, {cert.Subject}, {cert.Thumbprint.ToLower()}");
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
}
