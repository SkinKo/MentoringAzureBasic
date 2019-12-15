using Common2;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace SQLUpload
{
    public static class Function3
    {
        private static string GetKeyVaultEndpoint() => "https://keyvaultsk1985.vault.azure.net/";

        [FunctionName("Function3")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var connectionString = (await keyVaultClient.GetSecretAsync(GetKeyVaultEndpoint(), "AzureQueue")).Value;

            log.Info("C# HTTP trigger function processed a request.");

            //var connectionString = Environment.GetEnvironmentVariable("AzureQueue", EnvironmentVariableTarget.Process);

            var provider = await req.Content.ReadAsMultipartAsync();
            foreach (HttpContent content in provider.Contents)
            {
                var stream = content.ReadAsStreamAsync().Result;
                stream.Position = 0;

                Dictionary<string, string> headers = new Dictionary<string, string>();
                foreach (var header in content.Headers)
                {
                    var value = string.Empty;
                    foreach (var item in header.Value)
                    {
                        value += item;
                    }

                    headers.Add(header.Key, value);
                }

                FileProcessor processor = new FileProcessor(connectionString);
                await processor.Proceed(stream, content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty), headers, content.Headers.ContentType.MediaType);
            }

            return req.CreateResponse(System.Net.HttpStatusCode.OK);
        }
    }
}
