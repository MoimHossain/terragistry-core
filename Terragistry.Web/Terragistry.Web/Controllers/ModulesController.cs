

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Terragistry.Web.Data;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Terragistry.Web
{
    [Route("api/[controller]")]
    public class ModulesController : Controller
    {
        private readonly ModuleRepository repository;

        public ModulesController(ModuleRepository repository)
        {
            this.repository = repository;
        }

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
            //var contentType = "application/x-gzip";
            //var fileName = "app-service.tar.gz";

            var contentType = "application/zip";
            var fileName = $"{name}.zip";

            var bytes = await this.repository.DownloadContentAsync($"{@namespace}/{name}/{provider}/{version}");

            if(bytes == null )
            {
                return NotFound();
            }
            return await Task.FromResult(File(bytes, contentType, fileName));
        }

        private async Task<TerraModuleVersionCollection> ListAllVersionOfModuleAsync(
            string @namespace, string name, string provider)
        {
            var source = $"{@namespace}/{name}/{provider}".ToLowerInvariant() ;
            var allModules = await this.repository.GetAllModulesAsync();

            var filteredModules = allModules.Modules
                .Where(m => m.Id.ToLowerInvariant().StartsWith(source))
                .OrderByDescending(m=> m.Version);

            var versions = new List<TerraModuleVersion>();
            foreach(var item in filteredModules)
            {
                versions.Add(new TerraModuleVersion
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
                    Version = item.Version
                });
            }

            return await Task.FromResult(new TerraModuleVersionCollection
            {
                Modules = new List<TerraModuleVersionInfo>
                {
                    new TerraModuleVersionInfo
                    {
                        Source = source,
                        Versions = versions
                    }
                }
            });
        }

        private async Task<TerraModuleCollection> ListAllModulesAsync()
        {
            return await repository.GetAllModulesAsync();            
        }
    }
}
