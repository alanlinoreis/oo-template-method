
using System;

namespace TemplateMethodSample.Pedidos
{
    public class PedidoNacionalProcessor : PedidoProcessor
    {
        protected override decimal CalcularFrete(Pedido p) => 10m;

        protected override string GerarConfirmacao(ResultadoProcessamento resultado)
            => $"CONF_NACIONAL:{resultado.Total:C}";
    }
}
