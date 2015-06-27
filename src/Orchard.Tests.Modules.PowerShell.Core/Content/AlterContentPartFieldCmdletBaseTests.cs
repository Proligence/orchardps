﻿namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class AlterContentPartFieldCmdletBaseTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public AlterContentPartFieldCmdletBaseTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldAddAndRemoveContentFieldByName()
        {
            // Repeat the test two times to add/remove content field, and then to remove/add content field
            for (int i = 0; i < 2; i++)
            {
                if (this.HasContentField("TitlePart", "BooleanField"))
                {
                    this.powerShell.Execute("Remove-ContentField TitlePart BooleanField");
                    Assert.False(this.HasContentField("TitlePart", "BooleanField"));
                }
                else
                {
                    this.powerShell.Execute("Add-ContentField TitlePart BooleanField");
                    Assert.True(this.HasContentField("TitlePart", "BooleanField"));
                }
            }
        }

        [Fact, Integration]
        public void ShouldAddAndRemoveContentFieldByContentPartObject()
        {
            // Repeat the test two times to add/remove content field, and then to remove/add content field
            for (int i = 0; i < 2; i++)
            {
                if (this.HasContentField("TitlePart", "BooleanField"))
                {
                    this.powerShell.Execute("Get-ContentPartDefinition TitlePart | Remove-ContentField -ContentField BooleanField");
                    Assert.False(this.HasContentField("TitlePart", "BooleanField"));
                }
                else
                {
                    this.powerShell.Execute("Get-ContentPartDefinition TitlePart | Add-ContentField -ContentField BooleanField");
                    Assert.True(this.HasContentField("TitlePart", "BooleanField"));
                }
            }
        }

        private bool HasContentField(string contentPart, string contentField)
        {
            this.powerShell.ConsoleConnection.Reset();
            var output = this.powerShell.Execute("(Get-ContentPartDefinition " + contentPart + ").Fields");
            this.powerShell.ConsoleConnection.Reset();

            if (!string.IsNullOrEmpty(output))
            {
                var table = PsTable.Parse(output);
                var fields = table.Rows.Select(r => r[0]).ToArray();
                return fields.Any(p => p == contentField);
            }

            return false;
        }
    }
}