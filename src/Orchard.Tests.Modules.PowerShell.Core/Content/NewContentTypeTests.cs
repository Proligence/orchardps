﻿namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class NewContentTypeTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public NewContentTypeTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldCreateContentType()
        {
            this.EnsureContentTypeDoesNotExist("Foo");
            this.powerShell.Execute("New-ContentType Foo");
            var table = this.powerShell.ExecuteTable("Get-ContentType Foo");
            Assert.Equal("Foo", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldCreateContentTypeWithCustomDisplayName()
        {
            this.EnsureContentTypeDoesNotExist("Foo");
            this.powerShell.Execute("New-ContentType Foo -DisplayName 'My Foo'");
            var table = this.powerShell.ExecuteTable("Get-ContentType Foo");
            Assert.Equal("Foo", table.Rows.Single()[0]);
            Assert.Equal("My Foo", table.Rows.Single()[1]);
        }

        [Fact, Integration]
        public void ShouldCreateNewContentTypeWithCustomStereotype()
        {
            this.EnsureContentTypeDoesNotExist("Foo");
            this.powerShell.Execute("New-ContentType Foo -Stereotype Bar");
            var output = this.powerShell.Execute("(Get-ContentType Foo).Settings['Stereotype']");
            Assert.Equal("Bar", output);
        }

        [Theory, Integration]
        [InlineData("Creatable", "ContentTypeSettings.Creatable")]
        [InlineData("Listable", "ContentTypeSettings.Listable")]
        [InlineData("Draftable", "ContentTypeSettings.Draftable")]
        [InlineData("Securable", "ContentTypeSettings.Securable")]
        public void ShouldCreateContentTypeWithStandardSettings(string switchName, string settingName)
        {
            this.EnsureContentTypeDoesNotExist("Foo");
            this.powerShell.Execute("New-ContentType Foo -" + switchName);
            var output = this.powerShell.Execute("(Get-ContentType Foo).Settings['" + settingName + "']");
            Assert.Equal("True", output);
        }

        [Fact, Integration]
        public void ShouldCreateNewContentTypeWithContentParts()
        {
            this.EnsureContentTypeDoesNotExist("Foo");
            this.powerShell.Execute("New-ContentType Foo -Parts ('CommonPart', 'TitlePart')");
            var table = this.powerShell.ExecuteTable("Get-ContentType Foo");
            Assert.Equal("Foo", table.Rows.Single()[0]);
            Assert.Equal("CommonPart, TitlePart", table.Rows.Single()[2]);
        }

        [Fact, Integration]
        public void ShouldCreateContentTypeWithCustomSettings()
        {
            this.EnsureContentTypeDoesNotExist("Foo");
            this.powerShell.Execute("New-ContentType Foo -Settings @{'Bar'='Bar value'; 'Baz'='Baz value'}");
            var table = this.powerShell.ExecuteTable("(Get-ContentType Foo).Settings");
            Assert.Equal(2, table.Rows.Count);
            Assert.Equal("Bar", table[0, "Key"]);
            Assert.Equal("Bar value", table[0, "Value"]);
            Assert.Equal("Baz", table[1, "Key"]);
            Assert.Equal("Baz value", table[1, "Value"]);
        }

        private void EnsureContentTypeDoesNotExist(string name)
        {
            var output = this.powerShell.Execute("Get-ContentType " + name);
            if (!string.IsNullOrEmpty(output))
            {
                this.powerShell.Execute("Remove-ContentType " + name);
                this.powerShell.ConsoleConnection.Reset();
                Assert.Empty(this.powerShell.Execute("Get-ContentType " + name));
            }
        }
    }
}