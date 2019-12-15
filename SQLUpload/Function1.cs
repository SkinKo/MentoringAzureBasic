using System;
using System.IO;
using Common2;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SQLUpload
{
    public static class Function1
    {
        private static string GetKeyVaultEndpoint() => "https://keyvaultsk1985.vault.azure.net/";

        [FunctionName("Function1")]
        public static async void Run([QueueTrigger("uploadfiles", Connection = "AzureQueue")]FileUploadMessage message,
            [Blob("uploadfiles/{BlobName}", FileAccess.Read)]Stream file, TraceWriter log, ExecutionContext context)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var connectionString = (await keyVaultClient.GetSecretAsync(GetKeyVaultEndpoint(), "AdventureWorksConnectionString")).Value;

            log.Info($"C# Queue trigger function processed: {message.FileName}");

            if (file != null)
            {
                byte[] fileContent = new byte[file.Length];
                await file.ReadAsync(fileContent, 0, (int)file.Length);

                FileRepository repository = new FileRepository(connectionString);
                repository.AddFile(message.FileName, message.Metadata, fileContent);
            }
        }
    }
}
