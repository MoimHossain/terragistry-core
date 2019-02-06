

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Terragistry.Web
{
    [Route("api/[controller]")]
    public class ModulesController : Controller
    {
        // List Modules
        [HttpGet]
        public async Task<TerraModuleCollection> ListModulesAsync()
        {
            return await ListAllModulesAsync();
        }

        // List Modules
        [HttpGet]
        [Route("{namespace}")]
        public async Task<TerraModuleCollection> ListModulesByNamespaceAsync(string @namespace)
        {

            return await ListAllModulesAsync();
        }

        // Searhc modules
        [HttpGet]
        [Route("search")]
        public async Task<TerraModuleCollection> ListModulesBySearchAsync(string @namespace)
        {
            return await ListAllModulesAsync();
        }

        // List Available Versions for a Specific Module
        // <base_url>/:namespace/:name/:provider/versions
        [HttpGet]
        [Route("{namespace}/{name}/{provider}/versions")]
        public async Task<TerraModuleVersionCollection> ListAvailableVersionsOfModuleAsync(
            string @namespace, string name, string provider)
        {
            return await ListAllVersionOfModuleAsync(@namespace, name, provider);
        }

        // Download Source Code for a Specific Module Version
        // <base_url>/:namespace/:name/:provider/:version/download
        // Sample response
        //      HTTP/1.1 204 No Content
        //      Content-Length: 0
        //      x-terraform-get: https://api.github.com/repos/hashicorp/terraform-aws-consul/tarball/v0.0.1//*?archive=tar.gz
        [HttpGet]
        [Route("{namespace}/{name}/{provider}/{version}/download")]
        public async Task<IActionResult> GetDownloadEndpointAsync(
            string @namespace, string name, string provider, string version)
        {
            var hostUrl = $"{Request.Scheme}://{Request.Host}";
            this.Response.Headers.Add("x-terraform-get", $"{hostUrl}/api/modules/{@namespace}/{name}/{provider}/{version}/module_content.zip");
            return await Task.FromResult(new NoContentResult());
        }

        // Actual download
        [HttpGet]
        [Route("{namespace}/{name}/{provider}/{version}/module_content.zip")]
        public async Task<IActionResult> GetModuleContentAsync(
            string @namespace, string name, string provider, string version)
        {
            //var contentType = "application/octet-stream";
            var contentType = "application/zip";
            //var contentType = "application/x-gzip";

            //var fileName = "app-service.tar.gz";
            var fileName = "app-service.zip";

            return await Task.FromResult(File(fileName, contentType, fileName));
        }

        private async Task<TerraModuleVersionCollection> ListAllVersionOfModuleAsync(
            string @namespace, string name, string provider)
        {
            return await Task.FromResult(new TerraModuleVersionCollection
            {
                Modules = new List<TerraModuleVersionInfo>
                {
                    new TerraModuleVersionInfo
                    {
                        Source = "hashicorp/consul/aws",
                        Versions = new List<TerraModuleVersion>
                        {
                            new TerraModuleVersion
                            {
                                Root = new TerraModuleVersionRoot
                                {
                                    Providers = new List<TerraProvider>
                                    {
                                        new TerraProvider
                                        {
                                            Name = "template"
                                        }
                                    },
                                    Dependencies = new List<string>
                                    {
                                    }
                                },
                                Submodules = new List<TerraSubmodule>
                                {
                                    
                                },
                                Version = "0.0.1"
                            }
                        }
                    }
                }
            });
        }

        private async static Task<TerraModuleCollection> ListAllModulesAsync()
        {
            return await Task.FromResult(new TerraModuleCollection
            {
                Meta = new TerraMetaData
                {
                    Current_offset = 0,
                    Limit = 1,
                    Next_offset = -1,
                    Next_url = string.Empty
                },
                Modules = new List<TerraModule>
                {
                    new TerraModule
                    {
                        Id = "GoogleCloudPlatform/lb-http/google/1.0.4",
                        Owner = string.Empty,
                        Namespace = "GoogleCloudPlatform",
                        Name = "lb-http",
                        Version = "1.0.4",
                        Provider = "google",
                        Description = "Modular Global HTTP Load Balancer for GCE using forwarding rules.",
                        Downloads = 212,
                        Published_at = DateTime.Now.Subtract(new TimeSpan(22, 0, 0,0)),
                        Source = "https://github.com/GoogleCloudPlatform/terraform-google-lb-http",
                        Verified = true
                    }
                }
            });
        }
    }
}
