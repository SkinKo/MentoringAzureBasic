using System;
using System.IO;
using Common2;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SQLUpload
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async void Run([QueueTrigger("uploadfiles", Connection = "AzureQueue")]FileUploadMessage message,
            [Blob("uploadfiles/{BlobName}", FileAccess.Read)]Stream file, TraceWriter log, ExecutionContext context)
        {
            log.Info($"C# Queue trigger function processed: {message.FileName}");

            var connectionString = System.Environment.GetEnvironmentVariable("AdventureWorksConnectionString", EnvironmentVariableTarget.Process);

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
