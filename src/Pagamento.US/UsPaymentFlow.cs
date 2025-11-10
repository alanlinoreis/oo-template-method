
using System;
using TemplateMethodSample.Pedidos;

namespace TemplateMethodSample.Pagamento
{
    public class UsPaymentFlow : PaymentFlow
    {
        public UsPaymentFlow(string moeda, string localidade) : base(moeda, localidade) { }

        protected override decimal CalcularImpostos(Pedido p) => CalcularSubtotal(p) * 0.08m;

        protected override string FormatarRecibo(ResultadoPagamento resultado) => $"Receipt-US - Success:{resultado.Sucesso}";

        protected override void AntesDeRegistrar(Pedido p, decimal subtotal, decimal impostos)
        {
            base.AntesDeRegistrar(p, subtotal, impostos);
            // logging adicional
        }
    }
}
