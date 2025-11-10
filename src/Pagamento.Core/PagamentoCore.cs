
using System;
using TemplateMethodSample.Pedidos;

namespace TemplateMethodSample.Pagamento
{
    public class ResultadoPagamento
    {
        public bool Sucesso { get; set; }
        public string Recibo { get; set; } = "";
    }

    public abstract class PaymentFlow
    {
        protected string Moeda { get; }
        protected string Localidade { get; }

        protected PaymentFlow(string moeda, string localidade)
        {
            Moeda = moeda ?? throw new ArgumentNullException(nameof(moeda));
            Localidade = localidade ?? throw new ArgumentNullException(nameof(localidade));
        }

        public ResultadoPagamento Processar(Pedido p)
        {
            ValidarPedido(p);
            var subtotal = CalcularSubtotal(p);
            var impostos = CalcularImpostos(p);
            AntesDeRegistrar(p, subtotal, impostos);
            var resultado = RegistrarPagamento(p, subtotal, impostos);
            AposRegistrar(resultado);
            resultado.Recibo = FormatarRecibo(resultado);
            return resultado;
        }

        protected virtual void ValidarPedido(Pedido p) { }

        protected virtual decimal CalcularSubtotal(Pedido p)
        {
            decimal s = 0;
            foreach(var it in p.Items) s += it.Preco * it.Quantidade;
            return s;
        }

        // hooks
        protected abstract decimal CalcularImpostos(Pedido p);
        protected abstract string FormatarRecibo(ResultadoPagamento resultado);
        protected virtual void AntesDeRegistrar(Pedido p, decimal subtotal, decimal impostos) { }
        protected virtual void AposRegistrar(ResultadoPagamento resultado) { }
        protected virtual ResultadoPagamento RegistrarPagamento(Pedido p, decimal subtotal, decimal impostos)
        {
            return new ResultadoPagamento{ Sucesso = true };
        }
    }
}
