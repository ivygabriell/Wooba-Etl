# Wooba ETL

Ferramenta de linha de comando em .NET 10 que lê um arquivo CSV de clientes, corrige os problemas comuns de dados reais e grava o resultado em um banco SQLite em memória.

---

## O que a ferramenta faz

O processo é dividido em três etapas independentes:

**Leitura** -> lê o arquivo CSV e transporta cada linha como dado bruto, sem interpretar nada.

**Tratamento** —> remove espaços sobrando, aceita datas em três formatos diferentes, valida os campos, descarta linhas inválidas e elimina clientes duplicados. Cada descarte é registrado com o motivo.

**Gravação** —> insere os clientes válidos no SQLite através de comandos SQL escritos à mão.

---

## Pré-requisitos

- .NET SDK 10.0 ou superior

Verifique com:

```bash
dotnet --version
```

---

## Como rodar

```bash
git clone <url-do-repositorio>
cd WoobaEtl
dotnet run
```

As dependências são restauradas automaticamente pelo `dotnet run`. Se preferir fazer isso explicitamente:

```bash
dotnet restore
```
---

## Testando as quatro operações de banco

Ao iniciar, o programa executa o ETL completo e em seguida abre um modo interativo. O banco é em memória, então a conexão precisa permanecer aberta por isso as operações são feitas dentro da mesma sessão.

### 1. Inserir

Acontece automaticamente ao rodar `dotnet run`. Os clientes válidos são inseridos e o resumo é exibido:

```
Resumo:
  Total lido:       15
  Total inserido:   9
  Total descartado: 6
```

### 2. Consultar

```
> list
```

Lista todos os clientes gravados no banco.

### 3. Atualizar

Altera a cidade de um cliente a partir do e-mail:

```
> update joao.alves@gmail.com Goiânia
```

Confirme o resultado com `list`.

### 4. Excluir

Remove um cliente a partir do e-mail:

```
> delete joao.alves@gmail.com
```

Confirme com `list` a contagem diminui e o cliente desaparece da listagem.

### Encerrar

```
> exit
```

### Sessão de exemplo

```
> list
9 cliente(s):
  João Pedro Alves | joao.alves@gmail.com | Anápolis/GO
  ...

> update joao.alves@gmail.com Goiânia
Cliente joao.alves@gmail.com atualizado para a cidade Goiânia.

> list
9 cliente(s):
  João Pedro Alves | joao.alves@gmail.com | Goiânia/GO
  ...

> delete joao.alves@gmail.com
Cliente joao.alves@gmail.com excluido.

> list
8 cliente(s):
  ...

> exit
```

---

## Regras de tratamento

| Situação | Comportamento |
|---|---|
| Espaços no início ou fim dos campos | Removidos |
| Data em `dd/MM/yyyy`, `yyyy-MM-dd` ou `dd-MM-yyyy` | Convertida e padronizada |
| Data em formato desconhecido ou inexistente | Linha descartada |
| Nome vazio | Linha descartada |
| E-mail sem `@` ou com `@` repetido | Linha descartada |
| E-mail repetido (ignorando espaços e maiúsculas) | Linha descartada como duplicata |
| Data de nascimento no futuro | Linha descartada |

Todas as linhas descartadas são exibidas com o número da linha e o motivo.

---

## Estrutura do projeto

```
wooba-etl/
├── data/
│   └── customer_lot_a.csv
└── src/
    ├── Program.cs
    ├── Domain/
    │   ├── Customer.cs
    │   └── RawCustomer.cs
    ├── Application/
    │   ├── ICustomerReader.cs
    │   ├── ICustomerProcessor.cs
    │   ├── ICustomerRepository.cs
    │   ├── CustomerProcessor.cs
    │   ├── ProcessingResult.cs
    │   ├── DiscardedRow.cs
    │   └── EtlService.cs
    └── Infrastructure/
        ├── CsvCustomerReader.cs
        └── SqliteCustomerRepository.cs
```

### Responsabilidade de cada camada

| Camada | Pergunta que responde | Conteúdo |
|---|---|---|
| `Domain` | O que o negócio é | Entidades e regras de um cliente isolado |
| `Application` | O que o sistema faz | Contratos das três etapas e orquestração |
| `Infrastructure` | Como ele faz | Acesso a arquivo e banco de dados |

As dependências apontam sempre para dentro: `Program` → `Infrastructure` → `Application` → `Domain`. O domínio não conhece nenhuma camada externa.


---

## Tecnologias

- .NET 10
- Microsoft.Data.Sqlite
- Microsoft.Extensions.DependencyInjection
