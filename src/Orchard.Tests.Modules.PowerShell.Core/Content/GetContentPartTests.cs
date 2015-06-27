﻿namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class GetContentPartTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public GetContentPartTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Theory, Integration]
        [InlineData("Get-ContentPart Blog")]
        [InlineData("Get-ContentPart -ContentType Blog")]
        public void ShouldGetAllContentParts(string command)
        {
            var table = this.powerShell.ExecuteTable(command);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "BlogPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "CommonPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "TitlePart"));
        }

        [Theory, Integration]
        [InlineData("Get-ContentPart Blog BlogPart")]
        [InlineData("Get-ContentPart -ContentType Blog -Name BlogPart")]
        public void ShouldGetContentPartsByName(string command)
        {
            var table = this.powerShell.ExecuteTable(command);
            Assert.Equal("BlogPart", table.Rows.Single()[0]);
        }

        [Theory, Integration]
        [InlineData("Get-ContentPart Blog Bl*")]
        [InlineData("Get-ContentPart -ContentType Blog -Name Bl*")]
        public void ShouldGetContentPartsByWildcardName(string command)
        {
            var table = this.powerShell.ExecuteTable(command);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "BlogPart"));
            Assert.True(table.Rows.All(r => r[0].StartsWith("Bl")));
        }

        [Fact, Integration]
        public void ShouldGetContentPartsFromContentTypeObject()
        {
            var table = this.powerShell.ExecuteTable("Get-ContentType Blog | Get-ContentPart");
            Assert.Equal(1, table.Rows.Count(r => r[0] == "BlogPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "CommonPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "TitlePart"));
        }

        [Fact, Integration]
        public void ShouldGetContentPartsFromSpecificTenant()
        {
            var table = this.powerShell.ExecuteTable("Get-ContentPart Blog -Tenant Default");
            Assert.Equal(1, table.Rows.Count(r => r[0] == "BlogPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "CommonPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "TitlePart"));
        }

        [Fact, Integration]
        public void ShouldGetContentPartsFromAllTenants()
        {
            var table = this.powerShell.ExecuteTable("Get-ContentPart Blog -AllTenants");
            Assert.True(table.Rows.Count(r => r[0] == "BlogPart") > 0);
            Assert.True(table.Rows.Count(r => r[0] == "CommonPart") > 0);
            Assert.True(table.Rows.Count(r => r[0] == "TitlePart") > 0);
        }
    }
}