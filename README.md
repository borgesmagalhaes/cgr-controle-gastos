# Controle de Gastos Residenciais (CGR)

Projeto full stack para processo seletivo, com foco em regras de negócio, simplicidade e clareza.

## O que este sistema faz

- Cadastra pessoas
- Cadastra categorias
- Cadastra transações (receita e despesa)
- Mostra totais por pessoa
- Mostra totais por categoria
- Mostra contadores e resumo financeiro na Home

## Tecnologias usadas

- Back-end: C# + ASP.NET Core + Entity Framework Core
- Banco: SQLite "Para Persistência"
- Front-end: React + TypeScript + Vite
- Estilo: Bootstrap

## Estrutura do projeto

```text
backend/Cgr.Api
  Controllers/
  Dtos/
  Domain/Entities/
  Infrastructure/Data/
  Services/

frontend/cgr-web
  src/
    api.ts
    types.ts
    App.tsx
    main.tsx
    pages/
```

## Muito importante sobre `node_modules`

A pasta `node_modules` **não sobe para o Git** porque está no `.gitignore`.

Por isso, quando você baixar o projeto em outra máquina, precisa rodar `npm install` para recriar essa pasta.

## Como executar (jeito mais fácil)

### 1. Instalar dependências

Na raiz do projeto:

```bash
npm install
npm --prefix frontend/cgr-web install
```

### 2. Subir tudo com um comando

Ainda na raiz:

```bash
npm run dev
```

Esse comando sobe:

- Front-end: `http://localhost:5173`
- Back-end: `http://localhost:5101`

## Como executar separado (opcional) - Na verdade nem precisa, mas pode ser interessante

### Back-end

```bash
cd backend/Cgr.Api
dotnet restore
dotnet run
```

### Front-end

```bash
cd frontend/cgr-web
npm install
npm run dev
```

## Regras de negócio principais

- Pessoa:
  - nome obrigatório (máximo 200)
  - idade não pode ser negativa
- Categoria:
  - descrição obrigatória (máximo 400)
  - precisa informar uma finalidade válida (vinda da tabela `finalidade`)
- Transação:
  - descrição obrigatória (máximo 400)
  - valor positivo
  - `idTipo` deve existir em `tipo_transacao`
  - categoria precisa ser compatível com o tipo
- Ao excluir uma pessoa:
  - as transações dessa pessoa são excluídas junto (cascade)
- Menor de 18 anos:
  - no front, só permite despesa

- Observação: Para ficar mais organizado, criei duas tabelas de apoio no banco: tipo_transacao e finalidade. Pense nelas como “listas prontas” com opções corretas. Assim, o sistema usa essas opções para relacionar os dados e evita confusão, evita enums e etc...

## Endpoints principais

### Pessoas

- `GET /api/pessoas`
- `POST /api/pessoas`
- `PUT /api/pessoas/{id}`
- `DELETE /api/pessoas/{id}`

### Categorias

- `GET /api/categorias`
- `POST /api/categorias`

### Transações

- `GET /api/transacoes`
- `POST /api/transacoes`

### Consultas

- `GET /api/consultas/totais-por-pessoa`
- `GET /api/consultas/totais-por-categoria`

## Exemplos de payload

### Criar pessoa

```json
{
  "nome": "Ana Silva",
  "idade": 25
}
```

### Criar categoria

```json
{
  "descricao": "Mercado",
  "idFinalidade": 1
}
```

### Criar transação

```json
{
  "descricao": "Compra mensal",
  "valor": 250.5,
  "idTipo": 1,
  "idCategoria": 1,
  "idPessoa": 1
}
```

## Explicação simples do `reloadKey` (front)

Pense no `reloadKey` como um contador:

- Começa em `0`
- Quando salva/edita/exclui algo, soma `+1`
- As páginas “escutam” esse número
- Se mudar, elas recarregam os dados

É como um sininho:

- Mudou número = sininho tocou
- Página escutou = atualiza a tela

## Observações finais

- O código foi comentado em linguagem simples, pensado para leitura didática.
- O foco do projeto é regra de negócio e funcionamento, não design avançado.
