﻿namespace Proligence.PowerShell.Core.Commands.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Commands;
    using Proligence.PowerShell.Core.Commands.Items;
    using Proligence.PowerShell.Core.Common.Items;
    using Proligence.PowerShell.Core.Utilities;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    
    /// <summary>
    /// Implements a VFS node which groups <see cref="CommandNode"/> nodes for a single Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Invoke-OrchardCommand")]
    public class CommandsNode : ContainerNode 
    {
        public CommandsNode(IPowerShellVfs vfs)
            : base(vfs, "Commands")
        {
            this.Item = new CollectionItem(this) 
            {
                Name = "Commands",
                Description = "Contains all legacy Orchard commands available in the current tenant."
            };
        }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() 
        {
            string tenantName = this.GetCurrentTenantName();
            if (tenantName == null) 
            {
                return new VfsNode[0];
            }

            return this.GetCommands(tenantName).Select(
                command => new CommandNode(this.Vfs, command));
        }

        private IEnumerable<OrchardCommand> GetCommands(string tenant)
        {
            return this.UsingWorkContextScope(
                tenant,
                scope =>
                    {
                        var commandManager = scope.Resolve<ICommandManager>();
                        IEnumerable<CommandDescriptor> commandDescriptors = commandManager.GetCommandDescriptors();
                        IEnumerable<OrchardCommand> commands = commandDescriptors.Select(
                            command => new OrchardCommand
                            {
                                CommandName = command.Name,
                                HelpText = command.HelpText,
                                TenantName = tenant
                            });

                        return commands.ToArray();
                    });
        }
    }
}