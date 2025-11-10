using System;
using System.Collections.Generic;
using System.IO;
using TemplateMethodSample.Pagamento;
using TemplateMethodSample.Pedidos;
using Xunit;

namespace TemplateMethodSample.Tests
{
    public class PaymentFlowTests
    {
        private Pedido MakePedido()
        {
            var pedido = new Pedido { Id = Guid.NewGuid() };
            pedido.Items.Add(new PedidoItem("SKU1", 2, 15.5m));
            return pedido;
        }

        [Fact]
        public void Processar_BaseSequence_CallsHooksInCorrectOrder()
        {
            var steps = new List<string>();

            var flow = new TestPaymentFlow(steps);
            flow.Processar(MakePedido());

            var expected = new List<string>
            {
                "ValidarPedido",
                "CalcularSubtotal",
                "CalcularImpostos",
                "AntesDeRegistrar",
                "RegistrarPagamento",
                "AposRegistrar",
                "FormatarRecibo"
            };

            Assert.Equal(expected, steps);
        }

        [Fact]
        public void CalcularImpostos_AffectsOnlyImpostosParameter_PassedToHooks()
        {
            decimal capturedSubtotal = -1;
            decimal capturedImpostos = -1;

            var flow = new ImpostoCaptureFlow((s, i) => { capturedSubtotal = s; capturedImpostos = i; });
            flow.Processar(MakePedido());

            Assert.Equal(31.0m, capturedSubtotal); // 2 * 15.5
            Assert.Equal(3.1m, capturedImpostos); // 10% of subtotal as implemented
        }

        [Fact]
        public void FormatarRecibo_DiffersBetweenCountries()
        {
            var pedido = MakePedido();

            var br = new BrPaymentFlow("BRL", "pt-BR");
            var resBr = br.Processar(pedido);

            var us = new UsPaymentFlow("USD", "en-US");
            var resUs = us.Processar(pedido);

            Assert.NotEqual(resBr.Recibo, resUs.Recibo);
            Assert.Contains("Recibo-BR", resBr.Recibo);
            Assert.Contains("Receipt-US", resUs.Recibo);
        }

        [Fact]
        public void BrPaymentFlow_IsSealed_PreventsInheritance()
        {
            Assert.True(typeof(BrPaymentFlow).IsSealed);
        }

        [Fact]
        public void UsPaymentFlow_AntesDeRegistrar_CallsBaseMethod_StaticCheck()
        {
            // Verificação estática: garantir que a implementação chama base.AntesDeRegistrar
            var path = Path.Combine(System.Environment.CurrentDirectory, "..", "..", "..", "..", "src", "Pagamento.US", "UsPaymentFlow.cs");
            if (!File.Exists(path))
            {
                // fallback: try repository relative path
                path = Path.Combine(System.Environment.CurrentDirectory, "src", "Pagamento.US", "UsPaymentFlow.cs");
            }

            var src = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
            Assert.Contains("base.AntesDeRegistrar", src);
        }

        // Helper classes used only in tests
        private class TestPaymentFlow : PaymentFlow
        {
            private readonly List<string> _steps;
            public TestPaymentFlow(List<string> steps) : base("TST", "en-US") { _steps = steps; }

            protected override void ValidarPedido(Pedido p)
            {
                _steps.Add("ValidarPedido");
            }

            protected override decimal CalcularSubtotal(Pedido p)
            {
                _steps.Add("CalcularSubtotal");
                // use base calculation for realism
                return base.CalcularSubtotal(p);
            }

            protected override decimal CalcularImpostos(Pedido p)
            {
                _steps.Add("CalcularImpostos");
                return base.CalcularSubtotal(p) * 0.1m;
            }

            protected override void AntesDeRegistrar(Pedido p, decimal subtotal, decimal impostos)
            {
                _steps.Add("AntesDeRegistrar");
            }

            protected override ResultadoPagamento RegistrarPagamento(Pedido p, decimal subtotal, decimal impostos)
            {
                _steps.Add("RegistrarPagamento");
                return new ResultadoPagamento { Sucesso = true };
            }

            protected override void AposRegistrar(ResultadoPagamento resultado)
            {
                _steps.Add("AposRegistrar");
            }

            protected override string FormatarRecibo(ResultadoPagamento resultado)
            {
                _steps.Add("FormatarRecibo");
                return "OK";
            }
        }

        private class ImpostoCaptureFlow : PaymentFlow
        {
            private readonly Action<decimal, decimal> _capture;
            public ImpostoCaptureFlow(Action<decimal, decimal> capture) : base("TST", "pt-BR") { _capture = capture; }

            protected override decimal CalcularImpostos(Pedido p)
            {
                // compute as 10% of subtotal
                return base.CalcularSubtotal(p) * 0.1m;
            }

            protected override string FormatarRecibo(ResultadoPagamento resultado) => "r";

            protected override ResultadoPagamento RegistrarPagamento(Pedido p, decimal subtotal, decimal impostos)
            {
                _capture(subtotal, impostos);
                return new ResultadoPagamento { Sucesso = true };
            }
        }
    }
}
