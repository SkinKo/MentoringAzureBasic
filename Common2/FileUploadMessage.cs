using System;
using System.Collections.Generic;

namespace Common2
{
    [Serializable]
    public class FileUploadMessage
    {
        public string FileName { get; set; }
        public string BlobName { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
