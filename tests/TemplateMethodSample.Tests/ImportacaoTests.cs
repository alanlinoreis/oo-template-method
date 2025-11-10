using System;
using System.Collections.Generic;
using TemplateMethodSample.Importacao;
using Xunit;

namespace TemplateMethodSample.Tests
{
    public class ImportacaoTests
    {
        [Fact]
        public void ImportacaoAlunos_RegistroSemNome_GeraErro()
        {
            var import = new ImportacaoAlunos();
            var rel = import.Executar("dummy");
            Assert.Contains(rel.Erros, e => e.Contains("Aluno sem nome"));
        }

        [Fact]
        public void ImportacaoProdutos_RegistroValido_SemErros()
        {
            // Subclass to provide a clean source with valid records
            var import = new TestImportacaoProdutos();
            var rel = import.Executar("dummy");
            Assert.Empty(rel.Erros);
        }

        private class TestImportacaoProdutos : ImportacaoProdutos
        {
            protected override List<Registro> LerFonte(string caminho)
            {
                return new List<Registro> {
                    new Registro(new Dictionary<string,string>{{"nome","Produto1"}}),
                    new Registro(new Dictionary<string,string>{{"nome","Produto2"}})
                };
            }
        }
    }
}
