
using System;
using System.Collections.Generic;

namespace TemplateMethodSample.Sync
{
    public record Scope(string Name);
    public class DataSet
    {
        public List<Dictionary<string,string>> Rows { get; } = new();
    }

    public class SyncStatus
    {
        public int Updated { get; set; }
        public int Inserted { get; set; }
    }

    public abstract class SyncOrquestrador
    {
        public string Executar(string scope)
        {
            var sc = new Scope(scope);
            var bruto = ColetarBruto(sc);
            var reconciliado = NormalizarEReconciliar(bruto);
            var status = AplicarDiferencas(reconciliado);
            PosAplicacao(status);
            return GerarRelatorio(status);
        }

        protected virtual DataSet NormalizarEReconciliar(DataSet bruto) => bruto;

        protected virtual SyncStatus AplicarDiferencas(DataSet ds) => new SyncStatus { Updated = 1, Inserted = 1 };

        // hooks
        protected abstract DataSet ColetarBruto(Scope escopo);
        protected abstract string GerarRelatorio(SyncStatus status);
        protected virtual void PosAplicacao(SyncStatus status) { }
    }
}
