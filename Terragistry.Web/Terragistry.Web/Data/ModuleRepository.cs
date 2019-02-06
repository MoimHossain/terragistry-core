

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Terragistry.Web.Data
{
    public class ModuleRepository
    {
        private readonly string accountName;
        private readonly string storageKey;
        private readonly CloudBlobClient blobClient;
        private readonly CloudStorageAccount storageAccount;

        public ModuleRepository()
        {
            accountName = Environment.GetEnvironmentVariable("terragistry-account");
            storageKey = Environment.GetEnvironmentVariable("terragistry-key");

            var credentials = new StorageCredentials(this.accountName, this.storageKey);
            storageAccount = new CloudStorageAccount(credentials, true);
            blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<TerraModuleCollection> GetAllModulesAsync()
        {
            var modules = new List<TerraModule>();
            var segments = await blobClient.ListContainersSegmentedAsync(default(BlobContinuationToken));

            foreach(var bc in segments.Results)
            {
                var blobSegs = await bc.ListBlobsSegmentedAsync(null, true, BlobListingDetails.All,
                    100, default(BlobContinuationToken), null, null);
                // Loop over items within the container and output the length and URI.
                foreach (var item in blobSegs.Results)
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        var blob = (CloudBlockBlob)item;
                        // azure/api-app/0.0.44176.zip
                        var parts = blob.Name.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var @namespace = blob.Container.Name;
                        var name = "Unknown";
                        var provider = "Unknown";
                        var version = "Unknown";

                        if (parts.Length == 3)
                        {
                            name = parts[1];
                            provider = parts[0];
                            version = $"{parts[2]}".ToLowerInvariant().Replace(".zip", string.Empty);
                        }

                        modules.Add(new TerraModule
                        {
                            Id = $"{@namespace}/{name}/{provider}/{version}",
                            Owner = string.Empty,
                            Namespace = @namespace,
                            Name = name,
                            Version = version,
                            Provider = provider,
                            Description = $"{@namespace}/{name}/{provider}/{version}",
                            Downloads = new Random().Next(10, 30),
                            Published_at = DateTime.Now.Subtract(new TimeSpan(22, 0, 0, 0)),
                            Source = $"https://github.com/{ @namespace }/{ name }/{ provider }/{ version }",
                            Verified = true
                        });
                    }
                }
            }

            return await Task.FromResult(new TerraModuleCollection
            {
                Meta = new TerraMetaData
                {
                    Current_offset = 0,
                    Limit = modules.Count,
                    Next_offset = -1,
                    Next_url = string.Empty
                },
                Modules = modules
            });
        }

        public async Task<byte[]> DownloadContentAsync(string blobId)
        {
            var modules = new List<TerraModule>();
            var segments = await blobClient.ListContainersSegmentedAsync(default(BlobContinuationToken));

            foreach (var bc in segments.Results)
            {
                var blobSegs = await bc.ListBlobsSegmentedAsync(null, true, BlobListingDetails.All,
                    100, default(BlobContinuationToken), null, null);
                // Loop over items within the container and output the length and URI.
                foreach (var item in blobSegs.Results)
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        var blob = (CloudBlockBlob)item;
                        // azure/api-app/0.0.44176.zip
                        var parts = blob.Name.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var @namespace = blob.Container.Name;
                        var name = "Unknown";
                        var provider = "Unknown";
                        var version = "Unknown";

                        if (parts.Length == 3)
                        {
                            name = parts[1];
                            provider = parts[0];
                            version = $"{parts[2]}".ToLowerInvariant().Replace(".zip", string.Empty);
                        }

                        var Id = $"{@namespace}/{name}/{provider}/{version}";
                        if(Id.Equals(blobId, StringComparison.OrdinalIgnoreCase))
                        {
                            using (var memSteram = new MemoryStream())
                            {
                                await blob.DownloadToStreamAsync(memSteram);

                                return memSteram.ToArray();
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
