
using System;

namespace TemplateMethodSample.Pedidos
{
    public sealed class PedidoInternacionalProcessor : PedidoProcessor
    {
        protected override decimal CalcularFrete(Pedido p) => 50m;

        protected override string GerarConfirmacao(ResultadoProcessamento resultado)
            => $"CONF_INTERNACIONAL:{resultado.Total:C}";

        protected override void AposReservaEstoque(Pedido p)
        {
            base.AposReservaEstoque(p);
            // Simula operação adicional (rastreamento)
        }
    }
}
