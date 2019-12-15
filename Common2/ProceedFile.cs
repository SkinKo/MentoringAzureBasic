using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Common2
{
    public class FileProcessor
    {
        private readonly string _storageConnectionString;

        public FileProcessor(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
        }

        public async Task<bool> Proceed(Stream stream, string fileName, Dictionary<string, string> headers, string contentType)
        {
            var connectionString = _storageConnectionString;

            if (stream.Length > 0)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

                var blobCient = storageAccount.CreateCloudBlobClient();
                var queueClient = storageAccount.CreateCloudQueueClient();

                var blobContainer = blobCient.GetContainerReference("uploadfiles");
                await blobContainer.CreateIfNotExistsAsync();

                var blobName = fileName + DateTime.Now.Ticks;
                var blob = blobContainer.GetBlockBlobReference(blobName);

                var queue = queueClient.GetQueueReference("uploadfiles");
                await queue.CreateIfNotExistsAsync();

                await blob.UploadFromStreamAsync(stream, stream.Length);
                blob.Properties.ContentType = contentType;

                var message = new FileUploadMessage
                {
                    FileName = fileName,
                    BlobName = blobName,
                    Metadata = new Dictionary<string, string>()
                };

                foreach (var header in headers)
                {
                    message.Metadata.Add(header.Key, header.Value);
                }

                var json = JsonConvert.SerializeObject(message, Formatting.Indented);

                CloudQueueMessage queueMessage = new CloudQueueMessage(json);

                await queue.AddMessageAsync(queueMessage, new TimeSpan(1, 0, 0), TimeSpan.Zero, new QueueRequestOptions(), new OperationContext());
            }

            return true;
        }
    }
}
