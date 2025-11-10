# Solução: Exercícios Template Method (C# / .NET 9)

**Gerado:** 2025-11-10T11:34:31.868787Z  
**Estrutura exigida pelo enunciado**

/src
  Core/                      # tipos comuns (se precisar) e projeto de execução
  Importacao.Core/
  Importacao.Alunos/
  Importacao.Produtos/
  Pedidos.Core/
  Pedidos.Nacional/
  Pedidos.Internacional/
  Pagamento.Core/
  Pagamento.BR/
  Pagamento.US/
  Sync.Core/
  Sync.Erp/
  Sync.Marketplace/
/tests
  Importacao.Tests/
  Pedidos.Tests/
  Pagamento.Tests/
  Sync.Tests/

---

## Resumo do que implementei (comparado ao PDF)

Implementei 4 exercícios conforme o PDF usando o padrão **Template Method**. Para cada exercício criei:
- Uma classe base *orquestradora* (public) que define o fluxo fixo.
- Máximo de **3 ganchos** (protected abstract ou protected virtual) por orquestrador.
- Classes concretas que implementam os ganchos obrigatórios (abstract) e, quando aplicável, sobrescrevem ganchos opcionais (virtual) chamando `base.` quando apropriado.
- Uso de `sealed` em implementações que o enunciado sugeriu (ex.: `PedidoInternacionalProcessor`, `BrPaymentFlow`, `SyncErpFlow`).
- `README.md` detalhado, diagramas textuais, justificativas, checklist, plano de testes e o formato de feedback com rubrica, conforme o PDF.

A seguir há seções por exercício com diagramas textuais, justificativa `abstract` vs `virtual`, testes conceituais e checklist.

---

# 1) Validador de Importação (Importacao)

### Diagrama textual
ImportadorOrquestrador (base)
  ├─> LerFonte() (protected virtual)
  ├─> foreach Registro -> ValidarRegistro(Registro) (protected abstract)
  ├─> Consolidar(Relatorio) (protected virtual)
  └─> PosConsolidacao(Relatorio) (protected virtual)
Implementações:
  ├─ ImportacaoAlunos : ImportadorOrquestrador  (override ValidarRegistro, override PosConsolidacao)
  └─ ImportacaoProdutos : ImportadorOrquestrador (override ValidarRegistro)

### Justificativa (abstract vs virtual) - 2-4 linhas
`ValidarRegistro` é `abstract` porque a lógica de validação depende do tipo de dado (aluno/produto) e não existe uma implementação genérica segura; cada subclasse deve fornecer sua regra. `PosConsolidacao` é `virtual` porque é uma etapa opcional de pós-processamento (ex.: calcular totais por categoria) e a maioria das importações pode não precisar dela; fornecer um no-op evita obrigar todas as subclasses a implementá-la.

### Testes conceituais (cenários)
- Cenário válido: arquivos com registros completos → `Executar()` retorna Relatorio com `Erros.Count == 0` e `TotaisPorCategoria` apropriados.
- Cenário inválido: registro de aluno sem nome → `Executar()` retorna Relatorio com erro "Aluno sem nome".
- Verificar ordem: `LerFonte` → `ValidarRegistro` para cada registro → `Consolidar` → `PosConsolidacao` (pode usar um stub que marca chamadas em sequência).
- Verificar que `PosConsolidacao` de `ImportacaoAlunos` chama `base.PosConsolidacao(rel)` e acrescenta `TotaisPorCategoria`.

### Checklist curto (Importacao)
- [ ] Orquestrador público `Executar` existe e define fluxo fixo.
- [ ] Máximo 3 ganchos (aqui: LerFonte está como virtual de apoio; ganchos principais: ValidarRegistro, PosConsolidacao).
- [ ] `ValidarRegistro` é `protected abstract` e implementado nas concretas.
- [ ] `PosConsolidacao` é `protected virtual` com no-op por padrão.
- [ ] Não há `if(tipo)` no orquestrador.

---

# 2) Processador de Pedidos (Pedidos)

### Diagrama textual
PedidoProcessor (base)
  ├─> ValidarItens() (protected virtual)
  ├─> ReservarEstoque() (protected virtual)
  ├─> CalcularFrete(Pedido) (protected abstract)
  ├─> CalcularTotal(Pedido, frete) (protected virtual)
  ├─> PersistirPedido(Pedido, total) (protected virtual)
  └─> GerarConfirmacao(Resultado) (protected abstract)
Implementações:
  ├─ PedidoNacionalProcessor : PedidoProcessor (override CalcularFrete, GerarConfirmacao)
  └─ PedidoInternacionalProcessor : PedidoProcessor (sealed, override CalcularFrete, GerarConfirmacao, override AposReservaEstoque -> base.AposReservaEstoque)

### Justificativa (abstract vs virtual)
`CalcularFrete` e `GerarConfirmacao` são `abstract` pois variam de forma determinante entre nacional e internacional — não há uma regra padrão sensata. Hooks como `ValidarItens`, `ReservarEstoque` e `AposReservaEstoque` são `virtual` com implementação padrão (no-op) por conveniência, permitindo sobrescrita quando necessário.

### Testes conceituais (cenários)
- Cenário válido (nacional): Pedido com itens → `Processar` calcula subtotal + frete nacional (10) → `Resultado.Total` correto e `Confirmacao` formatada como nacional.
- Cenário internacional: frete maior (50) e `AposReservaEstoque` executa lógica extra (verificável via spy/stub).
- Ordem do fluxo: ValidarItens -> ReservarEstoque -> CalcularFrete -> CalcularTotal -> PersistirPedido -> GerarConfirmacao.
- Verificar `sealed` impede herança adicional (compilador bloqueia).

### Checklist curto (Pedidos)
- [ ] Orquestrador `Processar` existe e sem condicionais por tipo.
- [ ] Máximo 3 ganchos abstr/virtual principais.
- [ ] Implementações concretas chamam `base` quando estendem comportamento opcional.
- [ ] `PedidoInternacionalProcessor` marcado `sealed` conforme exemplo.

---

# 3) Pipeline de Pagamento (Pagamento)

### Diagrama textual
PaymentFlow (base)
  ├─> ValidarPedido() (protected virtual)
  ├─> CalcularSubtotal() (protected virtual)
  ├─> CalcularImpostos(Pedido) (protected abstract)
  ├─> AntesDeRegistrar(Pedido, subtotal, impostos) (protected virtual)
  ├─> RegistrarPagamento(...) (protected virtual)
  ├─> AposRegistrar(Resultado) (protected virtual)
  └─> FormatarRecibo(Resultado) (protected abstract)
Implementações:
  ├─ BrPaymentFlow : PaymentFlow (sealed, override CalcularImpostos, FormatarRecibo)
  └─ UsPaymentFlow : PaymentFlow (override CalcularImpostos, FormatarRecibo, override AntesDeRegistrar -> base.AntesDeRegistrar)

### Justificativa (abstract vs virtual)
`CalcularImpostos` e `FormatarRecibo` são `abstract` pois leis fiscais e formatos de recibo mudam por país; não existe implementação padrão. `AntesDeRegistrar` e `AposRegistrar` são `virtual` para permitir extensões (ex.: logging) sem obrigação.

### Testes conceituais (cenários)
- Cenário BR: subtotal calculado, impostos 10% do subtotal, `RegistrarPagamento` retorna sucesso e `Recibo` formatado com padrão BR.
- Cenário US: impostos 8%, `AntesDeRegistrar` é chamado (verificar via spy).
- Verificar ordem e que impostos afetam apenas a etapa apropriada.

### Checklist curto (Pagamento)
- [ ] Máximo 3 ganchos (abstract/virtual).
- [ ] `CalcularImpostos` e `FormatarRecibo` são abstract.
- [ ] `BrPaymentFlow` pode ser `sealed` conforme exemplo.

---

# 4) Sincronizador de Catálogo Multi-Fonte (Sync)

### Diagrama textual
SyncOrquestrador (base)
  ├─> ColetarBruto(Scope) (protected abstract)
  ├─> NormalizarEReconciliar(DataSet) (protected virtual)
  ├─> AplicarDiferencas(DataSet) (protected virtual)
  ├─> PosAplicacao(SyncStatus) (protected virtual)
  └─> GerarRelatorio(SyncStatus) (protected abstract)
Implementações:
  ├─ SyncErpFlow : SyncOrquestrador (sealed, override ColetarBruto, GerarRelatorio)
  └─ SyncMarketplaceFlow : SyncOrquestrador (override ColetarBruto, GerarRelatorio, override PosAplicacao -> base.PosAplicacao)

### Justificativa (abstract vs virtual)
`ColetarBruto` e `GerarRelatorio` são `abstract` porque dependem fortemente da fonte e do formato de relatório requerido; `PosAplicacao` é `virtual` para permitir envio de métricas/opcionalidades sem forçar todas as concretas.

### Testes conceituais (cenários)
- Cenário ERP: coleta retorna dataset com linhas; aplicar diferenças atualiza/inserta registros; relatorio reflete contagens.
- Cenário marketplace: `PosAplicacao` envia métricas (verificável via spy).
- Verificar que a ordem do pipeline é preservada.

### Checklist curto (Sync)
- [ ] Orquestrador `Executar` sem condicionais por tipo.
- [ ] Máximo 3 ganchos (abstract/virtual).
- [ ] `SyncErpFlow` pode ser `sealed` se não for estendido.

---

# Rubrica (soma 100 pontos) - conforme PDF

- Template Method correto: 30 pts
- Encapsulamento & Acesso: 20 pts
- Qualidade dos ganchos: 20 pts
- Reuso com base.X(): 15 pts
- Testabilidade & Clareza (README + testes): 15 pts

Penalizações:
- Uso de if/switch por tipo no orquestrador: -10 pts
- Mais de 3 ganchos: -5 pts por gancho extra
- Novos membros públicos nas derivadas: -5 pts por ocorrência
- Ausência de justificativas: -5 pts
- Ausência de checklist: -5 pts

### Formato do feedback (exigido)
Retornar feedback em 3 parágrafos contínuos:
1. `NOME_DO_ALUNO, sua nota nesta entrega é NOTA/100.` + pontos positivos.
2. (Se aplicável) Justificar por que não alcançou 100, citando arquivos/linhas e como ajustar.
3. Mensagem motivadora convidando à revisão e reavaliação.

Observação: o README inclui templates para validação usando a rubrica acima.

---

# Instruções para executar o projeto

No diretório `src/Core` execute:
```bash
dotnet run
```

Isso compilará e executará o `Program.cs` de demonstração. Os testes foram deixados como placeholders em `/tests` para que você adicione xUnit/NUnit conforme preferir.

---

# Checklist curto global (para submissão)
- [ ] Projeto compila com `dotnet run` (TargetFramework net9.0)
- [ ] README com diagramas, justificativas, checklist e rubrica conforme PDF
- [ ] Estrutura de pastas conforme enunciado
- [ ] Testes deixados como placeholders
- [ ] Nenhum `if(tipo)` no orquestrador; variação tratada por ganchos

---

Se quiser, eu incluo agora um projeto de testes xUnit básico (com 1-2 testes por exercício) para você só rodar `dotnet test`. Caso queira isso, responda "sim, inclua testes".