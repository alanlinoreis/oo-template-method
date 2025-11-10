using System;
using System.Reflection;
using TemplateMethodSample.Pagamento;
using TemplateMethodSample.Pedidos;
using Xunit;

namespace TemplateMethodSample.Tests
{
    public class PagamentoTests
    {
        private Pedido MakePedido()
        {
            var pedido = new Pedido { Id = Guid.NewGuid() };
            pedido.Items.Add(new PedidoItem("SKU1", 2, 15.5m));
            return pedido;
        }

        [Fact]
        public void BrPaymentFlow_ImpostosAplicados()
        {
            var br = new BrPaymentFlow("BRL","pt-BR");
            var metodo = typeof(BrPaymentFlow).GetMethod("CalcularImpostos", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(metodo);
            var impostos = (decimal)metodo.Invoke(br, new object[]{ MakePedido() });
            Assert.Equal(3.1m, impostos);
        }

        [Fact]
        public void UsPaymentFlow_AntesDeRegistrar_Chamado_StaticSourceCheck()
        {
            var path = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "src", "Pagamento.US", "UsPaymentFlow.cs");
            if (!System.IO.File.Exists(path)) path = System.IO.Path.Combine(Environment.CurrentDirectory, "src", "Pagamento.US", "UsPaymentFlow.cs");
            var src = System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path) : string.Empty;
            Assert.Contains("base.AntesDeRegistrar", src);
        }
    }
}
