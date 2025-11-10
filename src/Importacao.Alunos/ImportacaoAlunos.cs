
using System;
using System.Collections.Generic;

namespace TemplateMethodSample.Importacao
{
    public class ImportacaoAlunos : ImportadorOrquestrador
    {
        protected override List<string> ValidarRegistro(Registro r)
        {
            var erros = new List<string>();
            r.Campos.TryGetValue("nome", out var nome);
            if (string.IsNullOrWhiteSpace(nome)) erros.Add("Aluno sem nome");
            return erros;
        }

        protected override void PosConsolidacao(Relatorio rel)
        {
            base.PosConsolidacao(rel);
            // Exemplo de total por categoria
            rel.TotaisPorCategoria["Alunos"] = Math.Max(0, 1);
        }
    }
    
}
