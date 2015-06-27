﻿namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class GetContentTypeTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public GetContentTypeTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldGetAllContentTypes()
        {
            var table = this.powerShell.ExecuteTable("Get-ContentType");            
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Site"));
        }

        [Theory, Integration]
        [InlineData("Get-ContentType Site")]
        [InlineData("Get-ContentType -Name Site")]
        public void ShouldContentTypesByName(string command)
        {
            var table = this.powerShell.ExecuteTable(command);
            Assert.Equal("Site", table.Rows.Single()[0]);
        }

        [Theory, Integration]
        [InlineData("Get-ContentType S*")]
        [InlineData("Get-ContentType -Name S*")]
        public void ShouldGetContentTypesByWildcardName(string command)
        {
            var table = this.powerShell.ExecuteTable(command);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Site"));
            Assert.True(table.Rows.All(r => r[0].StartsWith("S")));
        }

        [Fact, Integration]
        public void ShouldGetContentTypesFromSpecificTenant()
        {
            var table = this.powerShell.ExecuteTable("Get-ContentType -Tenant Default");
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Site"));
        }

        [Fact, Integration]
        public void ShouldGetContentPartsFromAllTenants()
        {
            var table = this.powerShell.ExecuteTable("Get-ContentType -AllTenants");
            Assert.True(table.Rows.Count(r => r[0] == "Site") > 0);
        }
    }
}