﻿namespace Proligence.PowerShell.Core.Modules.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Environment.Extensions;
    using Proligence.PowerShell.Core.Common.Items;
    using Proligence.PowerShell.Core.Utilities;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    
    /// <summary>
    /// Implements a VFS node which groups <see cref="ModuleNode"/> nodes for a single Orchard tenant.
    /// </summary>
    public class ModulesNode : ContainerNode
    {
        private readonly IExtensionManager extensions;

        public ModulesNode(IPowerShellVfs vfs, IExtensionManager extensions)
            : base(vfs, "Modules")
        {
            this.extensions = extensions;
            this.Item = new CollectionItem(this) 
            {
                Name = "Modules",
                Description = "Contains all modules available in the current tenant."
            };
        }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() 
        {
            string tenantName = this.GetCurrentTenantName();
            if (tenantName == null) 
            {
                return new VfsNode[0];
            }

            return this.extensions.AvailableExtensions().Select(
                module => new ModuleNode(this.Vfs, module));
        }
    }
}