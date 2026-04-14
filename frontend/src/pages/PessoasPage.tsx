import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { pessoaApi } from "../api";
import type { Pessoa } from "../types";
import type { CrudPageProps } from "./page-types";

export function PessoasPage({ reloadKey, onDataChanged }: CrudPageProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [nomePessoa, setNomePessoa] = useState("");
  const [idadePessoa, setIdadePessoa] = useState<number>(18);
  const [pessoaEditando, setPessoaEditando] = useState<Pessoa | null>(null);

  useEffect(() => {
    // "Escuta o sininho": se reloadKey mudar, recarrega a lista.
    void carregarPessoas();
  }, [reloadKey]);

  async function carregarPessoas(): Promise<void> {
    setLoading(true);
    setError(null);

    try {
      // Busca a lista atual de pessoas no backend.
      const dados = await pessoaApi.list();
      setPessoas(dados);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar pessoas.");
    } finally {
      setLoading(false);
    }
  }

  async function salvarPessoa(event: FormEvent): Promise<void> {
    event.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const payload = { nome: nomePessoa, idade: Number(idadePessoa) };

      // Se existe pessoa em edição, faz update; se não, cria novo registro.
      if (pessoaEditando) {
        await pessoaApi.update(pessoaEditando.id, payload);
      } else {
        await pessoaApi.create(payload);
      }

      setNomePessoa("");
      setIdadePessoa(18);
      setPessoaEditando(null);
      await carregarPessoas();
      // "Toca o sininho": avisa a App que teve mudança de dados.
      onDataChanged();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Falha ao salvar pessoa.");
    } finally {
      setLoading(false);
    }
  }

  async function excluirPessoa(id: number): Promise<void> {
    setLoading(true);
    setError(null);

    try {
      // Exclui pessoa pelo id.
      await pessoaApi.remove(id);

      if (pessoaEditando?.id === id) {
        setPessoaEditando(null);
        setNomePessoa("");
        setIdadePessoa(18);
      }

      await carregarPessoas();
      // "Toca o sininho": informa que os dados mudaram após exclusão.
      onDataChanged();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Falha ao excluir pessoa.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <section>
      {error && <div className="alert alert-danger">{error}</div>}
      {loading && <div className="alert alert-info">Processando...</div>}

      <form className="row g-3 mb-4" onSubmit={salvarPessoa}>
        {/* Formulário simples de pessoa com nome e idade. */}
        <div className="col-md-7"><label className="form-label">Nome</label><input className="form-control" value={nomePessoa} onChange={(e) => setNomePessoa(e.target.value)} required maxLength={200} /></div>
        <div className="col-md-3"><label className="form-label">Idade</label><input type="number" className="form-control" min={0} value={idadePessoa} onChange={(e) => setIdadePessoa(Number(e.target.value))} required /></div>
        <div className="col-md-2 d-flex align-items-end"><button className="btn btn-primary w-100" type="submit">{pessoaEditando ? "Atualizar" : "Salvar"}</button></div>
      </form>

      <div className="table-responsive">
        <table className="table table-striped table-sm">
          <thead><tr><th>ID</th><th>Nome</th><th>Idade</th><th>Ações</th></tr></thead>
          <tbody>
            {pessoas.map((pessoa) => (
              <tr key={pessoa.id}>
                <td>{pessoa.id}</td><td>{pessoa.nome}</td><td>{pessoa.idade}</td>
                <td className="d-flex gap-2">
                  <button className="btn btn-outline-secondary btn-sm" onClick={() => { setPessoaEditando(pessoa); setNomePessoa(pessoa.nome); setIdadePessoa(pessoa.idade); }}>Editar</button>
                  <button className="btn btn-outline-danger btn-sm" onClick={() => void excluirPessoa(pessoa.id)}>Excluir</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
