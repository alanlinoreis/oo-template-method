using System;
using TemplateMethodSample.Sync;
using Xunit;

namespace TemplateMethodSample.Tests
{
    public class SyncTests
    {
        [Fact]
        public void SyncErpFlow_GeraRelatorio()
        {
            var sync = new SyncErpFlow();
            var report = sync.Executar("erp");
            Assert.Contains("ERP Sync", report);
        }

        [Fact]
        public void SyncMarketplace_PosAplicacao_Chamada_StaticSourceCheck()
        {
            var path = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "src", "Sync.Marketplace", "SyncMarketplace.cs");
            if (!System.IO.File.Exists(path)) path = System.IO.Path.Combine(Environment.CurrentDirectory, "src", "Sync.Marketplace", "SyncMarketplace.cs");
            var src = System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path) : string.Empty;
            Assert.Contains("PosAplicacao", src);
            Assert.Contains("base.PosAplicacao", src);
        }
    }
}
