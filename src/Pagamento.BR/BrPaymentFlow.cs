
using System;
using TemplateMethodSample.Pedidos;

namespace TemplateMethodSample.Pagamento
{
    public sealed class BrPaymentFlow : PaymentFlow
    {
        public BrPaymentFlow(string moeda, string localidade) : base(moeda, localidade) { }

        protected override decimal CalcularImpostos(Pedido p) => CalcularSubtotal(p) * 0.10m;

        protected override string FormatarRecibo(ResultadoPagamento resultado) => $"Recibo-BR - Sucesso:{resultado.Sucesso}";
    }
}
