﻿namespace Proligence.PowerShell.Core.Tenants.Cmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    public abstract class AlterTenantCmdletBase : OrchardCmdlet
    {
        private IShellSettingsManager shellSettingsManager;

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "TenantObject", ValueFromPipeline = true)]
        public ShellSettings Tenant { get; set; }

        protected abstract bool AllowAlterDefaultTenant { get; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.shellSettingsManager = this.OrchardDrive.ComponentContext.Resolve<IShellSettingsManager>();
        }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "Default")
            {
                this.AlterTenant(this.Name);
            }
            else if (this.ParameterSetName == "TenantObject")
            {
                this.AlterTenant(this.Tenant.Name);
            }
        }

        protected abstract string GetActionName();
        protected abstract bool PerformAction(ShellSettings tenant);

        private void AlterTenant(string name)
        {
            if (this.ShouldProcess("Tenant: " + name, this.GetActionName()))
            {
                try
                {
                    ShellSettings tenant = this.shellSettingsManager.LoadSettings()
                        .FirstOrDefault(x => x.Name == name);

                    if (tenant == null)
                    {
                        this.WriteError(Error.FailedToFindTenant(name));
                        return;
                    }

                    if (!this.AllowAlterDefaultTenant)
                    {
                        if (tenant.Name == ShellSettings.DefaultName)
                        {
                            this.WriteError(Error.CannotAlterDefaultTenant());
                            return;
                        }
                    }

                    if (this.PerformAction(tenant))
                    {
                        this.shellSettingsManager.SaveSettings(tenant);
                    }
                }
                catch (Exception ex)
                {
                    this.WriteError(Error.NotSpecified(ex, "AlterTenantFailed", name));
                }
            }
        }
    }
}