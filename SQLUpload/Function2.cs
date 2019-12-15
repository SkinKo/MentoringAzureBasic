using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SQLUpload
{
    public static class Function2
    {
        private static string GetKeyVaultEndpoint() => "https://keyvaultsk1985.vault.azure.net/";

        [FunctionName("Function2")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var connectionString = (await keyVaultClient.GetSecretAsync(GetKeyVaultEndpoint(), "AdventureWorksConnectionString")).Value;

            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "fileName", true) == 0).Value;
            string nodeId = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0).Value;
            string guid = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "guid", true) == 0).Value;

            if (name == null && nodeId == null && guid == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a fileName, id or guid on the query string");
            }

            FileRepository repository = new FileRepository(connectionString);
            var fileInfo = repository.GetFile(name, nodeId, guid);

            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(new MemoryStream(fileInfo.File));
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileInfo.FileName
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/" + fileInfo.Extension.Trim('.'));

            return response;
        }
    }
}
