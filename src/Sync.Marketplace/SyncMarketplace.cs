
using System;

namespace TemplateMethodSample.Sync
{
    public class SyncMarketplaceFlow : SyncOrquestrador
    {
        protected override DataSet ColetarBruto(Scope escopo)
        {
            var ds = new DataSet();
            ds.Rows.Add(new System.Collections.Generic.Dictionary<string,string>{{"sku","MKT1"},{"nome","ItemMKT"}});
            return ds;
        }

        protected override string GerarRelatorio(SyncStatus status) => $"MKT Sync - Updated:{status.Updated} Inserted:{status.Inserted}";

        protected override void PosAplicacao(SyncStatus status)
        {
            base.PosAplicacao(status);
            // exemplo: enviar métricas
        }
    }
}
