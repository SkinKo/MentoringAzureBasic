using Common2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductRestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FilesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task Upload(IFormFile file)
        {
            var connectionString = _configuration["AzureStorageConnectionString"];

            if (file.Length > 0)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

                var blobCient = storageAccount.CreateCloudBlobClient();
                var queueClient = storageAccount.CreateCloudQueueClient();

                var blobContainer = blobCient.GetContainerReference("uploadfiles");
                await blobContainer.CreateIfNotExistsAsync();

                var blobName = file.FileName + DateTime.Now.Ticks;
                var blob = blobContainer.GetBlockBlobReference(blobName);

                var queue = queueClient.GetQueueReference("uploadfiles");
                await queue.CreateIfNotExistsAsync();

                using (var stream = file.OpenReadStream())
                {
                    await blob.UploadFromStreamAsync(stream, file.Length);
                    blob.Properties.ContentType = file.ContentType;
                }

                var message = new FileUploadMessage
                {
                    FileName = file.FileName,
                    BlobName = blobName,
                    Metadata = new Dictionary<string, string>()
                };

                foreach (var header in file.Headers)
                {
                    message.Metadata.Add(header.Key, header.Value);
                }

                var json = JsonConvert.SerializeObject(message, Formatting.Indented);

                CloudQueueMessage queueMessage = new CloudQueueMessage(json);

                await queue.AddMessageAsync(queueMessage, new TimeSpan(1, 0, 0), TimeSpan.Zero, new QueueRequestOptions(), new OperationContext());
            }
        }
    }
}