// Tipos de entrada/saida usados para conversar com a API do backend.
export type Pessoa = {
  id: number;
  nome: string;
  idade: number;
};

export type Categoria = {
  id: number;
  descricao: string;
  idFinalidade: number;
  finalidade: "despesa" | "receita" | "ambas";
};

export type Transacao = {
  id: number;
  descricao: string;
  valor: number;
  tipo: "despesa" | "receita";
  idCategoria: number;
  categoria: string;
  idPessoa: number;
  pessoa: string;
};

export type TotaisGerais = {
  totalReceitas: number;
  totalDespesas: number;
  saldoLiquido: number;
};

export type TotalPorPessoa = {
  pessoaId: number;
  nome: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
};

export type ConsultaPessoas = {
  pessoas: TotalPorPessoa[];
  totalGeral: TotaisGerais;
};

export type TotalPorCategoria = {
  categoriaId: number;
  descricao: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
};

export type ConsultaCategorias = {
  categorias: TotalPorCategoria[];
  totalGeral: TotaisGerais;
};

export type DominioItem = {
  id: number;
  descricao: string;
};
