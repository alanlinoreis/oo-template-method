
using System;

namespace TemplateMethodSample.Sync
{
    public sealed class SyncErpFlow : SyncOrquestrador
    {
        protected override DataSet ColetarBruto(Scope escopo)
        {
            var ds = new DataSet();
            ds.Rows.Add(new System.Collections.Generic.Dictionary<string,string>{{"sku","ERP1"},{"nome","ItemERP"}});
            return ds;
        }

        protected override string GerarRelatorio(SyncStatus status) => $"ERP Sync - Updated:{status.Updated} Inserted:{status.Inserted}";
    }
    
}
