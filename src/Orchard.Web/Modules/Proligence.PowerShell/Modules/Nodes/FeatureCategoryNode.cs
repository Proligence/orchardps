﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureCategoryNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Modules.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Environment.Extensions.Models;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which groups <see cref="FeatureNode"/> nodes of a single feature category.
    /// </summary>
    public class FeatureCategoryNode : ContainerNode
    {
        private readonly FeatureDescriptor[] features;

        public FeatureCategoryNode(IPowerShellVfs vfs, string categoryName, FeatureDescriptor[] features)
            : base(vfs, categoryName)
        {
            this.features = features;

            this.Item = new CollectionItem(this) 
            {
                Name = categoryName,
                Description = "Contains all features of the " + categoryName + " category."
            };
        }

        public override IEnumerable<VfsNode> GetVirtualNodes()
        {
            return this.features.Select(feature => new FeatureNode(this.Vfs, feature));
        }
    }
}