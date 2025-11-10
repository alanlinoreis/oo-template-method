
using System;
using System.Collections.Generic;

namespace TemplateMethodSample.Pedidos
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public List<PedidoItem> Items { get; } = new();
    }

    public class PedidoItem
    {
        public string Sku { get; }
        public int Quantidade { get; }
        public decimal Preco { get; }
        public PedidoItem(string sku,int q, decimal preco) { Sku=sku;Quantidade=q;Preco=preco; }
    }

    public class ResultadoProcessamento
    {
        public decimal Total { get; set; }
        public string Confirmacao { get; set; } = "";
    }

    public abstract class PedidoProcessor
    {
        public ResultadoProcessamento Processar(Pedido p)
        {
            ValidarItens(p);
            ReservarEstoque(p);
            var frete = CalcularFrete(p);
            var total = CalcularTotal(p, frete);
            PersistirPedido(p, total);
            var resultado = new ResultadoProcessamento{ Total = total };
            resultado.Confirmacao = GerarConfirmacao(resultado);
            return resultado;
        }

        protected virtual void ValidarItens(Pedido p) { /* no-op */ }
        protected virtual void ReservarEstoque(Pedido p) { /* no-op */ }

        // hooks
        protected abstract decimal CalcularFrete(Pedido p);
        protected virtual decimal CalcularTotal(Pedido p, decimal frete)
        {
            decimal subtotal = 0;
            foreach(var it in p.Items) subtotal += it.Preco * it.Quantidade;
            return subtotal + frete;
        }
        protected virtual void PersistirPedido(Pedido p, decimal total) { /* no-op */ }

        protected abstract string GerarConfirmacao(ResultadoProcessamento resultado);

        protected virtual void AposReservaEstoque(Pedido p) { /* no-op */ }
    }
}
