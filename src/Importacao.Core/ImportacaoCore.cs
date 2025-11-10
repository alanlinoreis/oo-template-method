
using System;
using System.Collections.Generic;

namespace TemplateMethodSample.Importacao
{
    public record Registro(Dictionary<string,string> Campos);

    public class Relatorio
    {
        public List<string> Erros { get; } = new();
        public Dictionary<string,int> TotaisPorCategoria { get; } = new();
        public void AddErro(string e) => Erros.Add(e);
    }

    public abstract class ImportadorOrquestrador
    {
        public Relatorio Executar(string caminho)
        {
            var rel = new Relatorio();
            var registros = LerFonte(caminho);
            foreach(var r in registros)
            {
                var erros = ValidarRegistro(r);
                foreach(var e in erros) rel.AddErro(e);
            }
            Consolidar(rel);
            PosConsolidacao(rel);
            return rel;
        }

        protected virtual List<Registro> LerFonte(string caminho)
        {
            // Simulação: dois registros
            return new List<Registro>{
                new Registro(new Dictionary<string,string>{{"tipo","aluno"},{"nome","Ana"}}),
                new Registro(new Dictionary<string,string>{{"tipo","aluno"},{"nome",""}})
            };
        }

        protected virtual void Consolidar(Relatorio rel)
        {
            // default: nenhuma ação além de manter erros
        }

        // Hooks (<=3)
        protected abstract List<string> ValidarRegistro(Registro r);
        protected virtual void PosConsolidacao(Relatorio rel) { /* no-op */ }
    }
}
