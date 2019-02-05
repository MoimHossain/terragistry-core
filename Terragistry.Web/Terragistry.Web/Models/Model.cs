

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Terragistry.Web
{
    #region Terraform Module definitions
    public class TerraMetaData
    {
        public int Limit { get; set; }
        public int Current_offset { get; set; }
        public int Next_offset { get; set; }
        public string Next_url { get; set; }
    }

    public class TerraModule
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Provider { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public DateTime Published_at { get; set; }
        public int Downloads { get; set; }
        public bool Verified { get; set; }
    }

    public class TerraModuleCollection
    {
        public TerraMetaData Meta { get; set; }
        public List<TerraModule> Modules { get; set; }
    }
    #endregion


    #region Terraform Module versions
    public class TerraProvider
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public class TerraSubmodule
    {
        public string Path { get; set; }
        public List<TerraProvider> Providers { get; set; }
        public List<string> Dependencies { get; set; }
    }


    public class TerraModuleVersionRoot
    {
        public List<string> Dependencies { get; set; }
        public List<TerraProvider> Providers { get; set; }
    }

    public class TerraModuleVersion
    {
        public string Version { get; set; }
        public List<TerraSubmodule> Submodules { get; set; }
        public TerraModuleVersionRoot Root { get; set; }
    }

    public class TerraModuleVersionInfo
    {
        public string Source { get; set; }
        public List<TerraModuleVersion> Versions { get; set; }
    }

    public class TerraModuleVersionCollection
    {
        public List<TerraModuleVersionInfo> Modules { get; set; }
    }
    #endregion
}
