using System;
using TemplateMethodSample.Pedidos;
using Xunit;
using System.IO;
using System.Reflection;

namespace TemplateMethodSample.Tests
{
    public class PedidosTests
    {
        private Pedido MakePedido()
        {
            var pedido = new Pedido { Id = Guid.NewGuid() };
            pedido.Items.Add(new PedidoItem("SKU1", 2, 15.5m));
            return pedido;
        }

        [Fact]
        public void PedidoNacional_CalculoTotal_IncluiFrete()
        {
            var proc = new PedidoNacionalProcessor();
            var res = proc.Processar(MakePedido());
            // subtotal 31.0 + frete 10.0
            Assert.Equal(41.0m, res.Total);
        }

        [Fact]
        public void PedidoInternacional_AposReserva_ChamaRastreamento_SourceContainsBaseCall()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "src", "Pedidos.Nacional", "PedidoNacional.cs");
            if (!File.Exists(path)) path = Path.Combine(Environment.CurrentDirectory, "src", "Pedidos.Nacional", "PedidoNacional.cs");
            var src = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
            Assert.Contains("AposReservaEstoque", src);
            // verify internacional overrides the method and calls base
            var path2 = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "src", "Pedidos.Internacional", "PedidoInternacional.cs");
            if (!File.Exists(path2)) path2 = Path.Combine(Environment.CurrentDirectory, "src", "Pedidos.Internacional", "PedidoInternacional.cs");
            var src2 = File.Exists(path2) ? File.ReadAllText(path2) : string.Empty;
            Assert.Contains("base.AposReservaEstoque", src2);
        }
    }
}
