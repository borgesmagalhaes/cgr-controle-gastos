import type {
  Categoria,
  ConsultaCategorias,
  ConsultaPessoas,
  DominioItem,
  Pessoa,
  Transacao,
} from "./types";

// URL base da API; permite override por variavel de ambiente do Vite.
const API_BASE_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5101";

// Utilitario central para chamadas HTTP com tratamento uniforme de erro.
async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      headers: {
        "Content-Type": "application/json",
        ...(init?.headers ?? {}),
      },
      ...init,
    });
  } catch {
    throw new Error("Não foi possível conectar à API. Verifique se o back-end está em execução e se o CORS está liberado para a URL do front.");
  }

  if (!response.ok) {
    // Tenta ler o erro no formato ProblemDetails da API.
    const payload = (await response.json().catch(() => null)) as { detail?: string; title?: string } | null;
    const fallback = `Erro ${response.status} ao chamar a API.`;
    throw new Error(payload?.detail ?? payload?.title ?? fallback);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

// API de pessoas.
export const pessoaApi = {
  // Lista pessoas para preencher tabela e combos.
  list: () => request<Pessoa[]>("/api/pessoas"),
  create: (body: { nome: string; idade: number }) =>
    request<Pessoa>("/api/pessoas", { method: "POST", body: JSON.stringify(body) }),
  update: (id: number, body: { nome: string; idade: number }) =>
    request<Pessoa>(`/api/pessoas/${id}`, { method: "PUT", body: JSON.stringify(body) }),
  remove: (id: number) => request<void>(`/api/pessoas/${id}`, { method: "DELETE" }),
};

// API de categorias.
export const categoriaApi = {
  // Lista categorias já com a finalidade textual para exibição.
  list: () => request<Categoria[]>("/api/categorias"),
  create: (body: { descricao: string; idFinalidade: number }) =>
    request<Categoria>("/api/categorias", { method: "POST", body: JSON.stringify(body) }),
};

// API de transacoes.
export const transacaoApi = {
  // Cria transação validando regras no backend (tipo, categoria e pessoa).
  list: () => request<Transacao[]>("/api/transacoes"),
  create: (body: {
    descricao: string;
    valor: number;
    tipo: string;
    idCategoria: number;
    idPessoa: number;
  }) => request<Transacao>("/api/transacoes", { method: "POST", body: JSON.stringify(body) }),
};

// API de consultas consolidadas.
export const consultaApi = {
  // Consulta consolidada para cards e tabelas da Home.
  totaisPorPessoa: () => request<ConsultaPessoas>("/api/consultas/totais-por-pessoa"),
  totaisPorCategoria: () => request<ConsultaCategorias>("/api/consultas/totais-por-categoria"),
};

// API de valores de domínio (finalidade e tipo de transação).
export const dominioApi = {
  finalidades: () => request<DominioItem[]>("/api/dominios/finalidades"),
  tiposTransacao: () => request<DominioItem[]>("/api/dominios/tipos-transacao"),
};
