using System;
using TemplateMethodSample.Importacao;
using TemplateMethodSample.Pedidos;
using TemplateMethodSample.Pagamento;
using TemplateMethodSample.Sync;

class Program
{
    static void Main()
    {
        Console.WriteLine("Template Method Sample - Execução de exemplo");

        var alunosImport = new ImportacaoAlunos();
        var relAlunos = alunosImport.Executar("dados-alunos.csv");
        Console.WriteLine($"ImportaçãoAlunos -> Erros: {relAlunos.Erros.Count} TotaisPorCategoria: {relAlunos.TotaisPorCategoria.Count}");

        var produtosImport = new ImportacaoProdutos();
        var relProdutos = produtosImport.Executar("dados-produtos.csv");
        Console.WriteLine($"ImportacaoProdutos -> Erros: {relProdutos.Erros.Count}");

        var pedido = new Pedido { Id = Guid.NewGuid() };
        pedido.Items.Add(new PedidoItem("SKU1", 2, 15.5m));

        var nac = new PedidoNacionalProcessor();
        var resNac = nac.Processar(pedido);
        Console.WriteLine($"PedidoNacional Total={resNac.Total} Conf={resNac.Confirmacao}");

        var intl = new PedidoInternacionalProcessor();
        var resInt = intl.Processar(pedido);
        Console.WriteLine($"PedidoInternacional Total={resInt.Total} Conf={resInt.Confirmacao}");

        var pay = new BrPaymentFlow("BRL","pt-BR");
        var payRes = pay.Processar(pedido);
        Console.WriteLine($"Pagamento BR Sucesso={payRes.Sucesso} Recibo={payRes.Recibo}");

        var sync = new SyncErpFlow();
        var report = sync.Executar("erp");
        Console.WriteLine(report);

        Console.WriteLine("Execução concluída.");
    }
}