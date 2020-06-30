using System;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ServerCertificateChain
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpoint = "oidc.eks.ap-northeast-1.amazonaws.com";
            Console.WriteLine("------ Begin Https Request ----");
            await DownloadSslCertificateHttp(endpoint);

            Console.WriteLine("------ Begin Tcp Request ----");
            await DownloadSslCertificateTcp(endpoint);

            Console.WriteLine("------ Begin Socket Request ----");
            await DownloadSslCertificateSocket(endpoint);
        }

        private static X509ChainElement GetLastChainCertificate(X509Chain chain)
        {
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
            return validcert;
        }

        private static X509Certificate2 GetLastChainExtraStoreCertificate(X509Chain chain)
        {
            var chainFirst = chain.ChainElements.Cast<X509ChainElement>().First();
            var extraFirst = chain.ChainPolicy.ExtraStore.Cast<X509Certificate2>().First();
            var validCert = chainFirst.Certificate.Thumbprint == extraFirst.Thumbprint
                ? chain.ChainPolicy.ExtraStore.Cast<X509Certificate2>()
                    .Select(item =>
                    {
                        Console.WriteLine($"{item.FriendlyName}, {item.Subject}, {item.Thumbprint.ToLower()}, {item.NotAfter}");
                        return item;
                    })
                    .Reverse() // check chain last
                    .First()
                : chain.ChainPolicy.ExtraStore.Cast<X509Certificate2>()
                    .Select(item =>
                    {
                        Console.WriteLine($"{item.FriendlyName}, {item.Subject}, {item.Thumbprint.ToLower()}, {item.NotAfter}");
                        return item;
                    })
                    .First();

            Console.WriteLine($"Result Cert: {validCert.FriendlyName}, {validCert.Subject}, {validCert.Thumbprint.ToLower()}, {validCert.NotAfter}");
            return validCert;
        }

        public static async Task DownloadSslCertificateHttp(string endpoint)
        {
            var url = new Uri($"https://{endpoint}:443");
            using var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (HttpRequestMessage msg, X509Certificate2 cert, X509Chain chain, SslPolicyErrors pollyError) =>
            {
                Console.WriteLine($"{chain.ChainElements.Count} chains, PollyError: {pollyError}, Cert: {cert.FriendlyName}, {cert.Subject}, {cert.Thumbprint.ToLower()}");

                var validcert = GetLastChainCertificate(chain);
                var validExtraCert = GetLastChainExtraStoreCertificate(chain);
                return chain.Build(cert);
            };

            using HttpClient client = new HttpClient(handler);
            using HttpResponseMessage response = await client.GetAsync(url);
        }

        public static async Task<X509Certificate2> DownloadSslCertificateTcp(string endpoint)
        {
            using var client = new TcpClient();
            client.Connect(endpoint, 443);
            var ServerCertificateCustomValidationCallback = new RemoteCertificateValidationCallback((sender, cert, chain, pollyError) =>
            {
                var cert2 = (X509Certificate2)cert;
                Console.WriteLine($"{chain.ChainElements.Count} chains, PollyError: {pollyError}, Cert: {cert2.FriendlyName}, {cert2.Subject}, {cert2.Thumbprint.ToLower()}");

                var validcert = GetLastChainCertificate(chain);
                var validExtraCert = GetLastChainExtraStoreCertificate(chain);
                return true;
            });
            using var ssl = new SslStream(client.GetStream(), false, ServerCertificateCustomValidationCallback, null);
            await ssl.AuthenticateAsClientAsync(endpoint);
            var cert = new X509Certificate2(ssl.RemoteCertificate);
            return cert;
        }

        public static async Task<X509Certificate2> DownloadSslCertificateSocket(string endpoint)
        {
            using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(endpoint, 443);
            var ServerCertificateCustomValidationCallback = new RemoteCertificateValidationCallback((sender, cert, chain, pollyError) =>
            {
                var cert2 = (X509Certificate2)cert;
                Console.WriteLine($"{chain.ChainElements.Count} chains, PollyError: {pollyError}, Cert: {cert2.FriendlyName}, {cert2.Subject}, {cert2.Thumbprint.ToLower()}");

                var validcert = GetLastChainCertificate(chain);
                var validExtraCert = GetLastChainExtraStoreCertificate(chain);
                return true;
            });
            using var ssl = new SslStream(new NetworkStream(client), false, ServerCertificateCustomValidationCallback, null);
            await ssl.AuthenticateAsClientAsync(endpoint);
            var cert = new X509Certificate2(ssl.RemoteCertificate);
            return cert;
        }
    }
}
