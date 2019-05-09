using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AwsSecretStoreAspNetCore
{
    /// <summary>
    /// Add AWS Secrets as a Configuration Manager
    /// </summary>
    public static class AwsSecretsConfigurationBuilderExtensions
    {
        public static IWebHostBuilder AddAwsSecrets(this IWebHostBuilder hostBuilder)
            => hostBuilder.AddAwsSecrets(null, null, credentials: null);

        public static IWebHostBuilder AddAwsSecrets(this IWebHostBuilder hostBuilder, string region)
            => hostBuilder.AddAwsSecrets(null, region, credentials: null);

        public static IWebHostBuilder AddAwsSecrets(this IWebHostBuilder hostBuilder, string prefix, string region, string profile)
        {
            var chain = new Amazon.Runtime.CredentialManagement.CredentialProfileStoreChain();
            if (chain.TryGetAWSCredentials(profile, out var credentials))
            {
                return hostBuilder.AddAwsSecrets(prefix, region, credentials);
            }
            return hostBuilder;
        }

        public static IWebHostBuilder AddAwsSecrets(this IWebHostBuilder hostBuilder, string prefix, string region, AWSCredentials credentials)
        {
            return hostBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var endpoint = !string.IsNullOrWhiteSpace(region) ? RegionEndpoint.GetBySystemName(region) : null;
                configBuilder.AddAwsSecrets(hostingContext, prefix, endpoint, credentials);
            });
        }

        private static IConfigurationBuilder AddAwsSecrets(this IConfigurationBuilder configurationBuilder, WebHostBuilderContext hostingContext, string prefix, RegionEndpoint endpoint, AWSCredentials credentials)
        {
            // build partially
            var partialConfig = configurationBuilder.Build();
            var settings = new AwsSecretsManagerSettings();
            partialConfig.GetSection(nameof(AwsSecretsManagerSettings)).Bind(settings);

            // Filter which secret to load
            var allowedPrefixes = settings.SecretGroups
                .Select(x => $"{prefix}{x}")
                .ToArray();

            configurationBuilder.AddSecretsManager(region: endpoint, credentials: credentials, configurator: opts =>
            {
                opts.SecretFilter = entry => HasPrefix(allowedPrefixes, entry);
                opts.KeyGenerator = (entry, key) => GenerateKey(allowedPrefixes, key);
            });
            return configurationBuilder;
        }

        /// <summary>
        /// Only load entries that start with any of the allowed prefixes
        /// </summary>
        /// <param name="allowedPrefixes"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static bool HasPrefix(IEnumerable<string> allowedPrefixes, SecretListEntry entry)
            => allowedPrefixes.Any(prefix => entry.Name.StartsWith(prefix));

        /// <summary>
        /// Strip the prefix and replace '__' with ':'
        /// </summary>
        /// <param name="prefixes"></param>
        /// <param name="secretValue"></param>
        /// <returns></returns>
        private static string GenerateKey(IEnumerable<string> prefixes, string secretValue)
        {
            // don't use '/' in your environment or secretgroup name.
            var prefix = prefixes.First(secretValue.StartsWith);

            // Strip the prefix
            var s = secretValue.Substring(prefix.Length + 1);
            return s;
        }

        private class AwsSecretsManagerSettings
        {
            /// <summary>
            /// The allowed secret groups, e.g. Shared or MyAppsSecrets
            /// </summary>
            public string[] SecretGroups { get; set; } = Array.Empty<string>();
        }
    }
}