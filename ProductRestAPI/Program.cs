using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions;

namespace ProductRestAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((ctx, builder) =>
                 {
                     var keyVaultEndpoint = GetKeyVaultEndpoint();
                     if (!string.IsNullOrEmpty(keyVaultEndpoint))
                     {
                         var azureServiceTokenProvider = new AzureServiceTokenProvider();
                         var keyVaultClient = new KeyVaultClient(
                             new KeyVaultClient.AuthenticationCallback(
                                 azureServiceTokenProvider.KeyVaultTokenCallback));
                         builder.AddAzureKeyVault(
                             keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                     }
                 })
                .UseStartup<Startup>()
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));


        private static string GetKeyVaultEndpoint() => "https://keyvaultsk1985.vault.azure.net/";
    }
}

