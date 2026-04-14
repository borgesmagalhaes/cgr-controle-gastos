import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { categoriaApi, dominioApi } from "../api";
import type { Categoria, DominioItem } from "../types";
import type { CrudPageProps } from "./page-types";

export function CategoriasPage({ reloadKey, onDataChanged }: CrudPageProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [finalidades, setFinalidades] = useState<DominioItem[]>([]);
  const [descricaoCategoria, setDescricaoCategoria] = useState("");
  const [idFinalidadeCategoria, setIdFinalidadeCategoria] = useState<number | "">("");

  useEffect(() => {
    // "Escuta o sininho": quando reloadKey mudar, busca de novo.
    void carregarCategorias();
  }, [reloadKey]);

  async function carregarCategorias(): Promise<void> {
    setLoading(true);
    setError(null);

    try {
      // Carrega em paralelo: dados da tabela e valores de domínio para o combo.
      const [listaCategorias, listaFinalidades] = await Promise.all([
        categoriaApi.list(),
        dominioApi.finalidades(),
      ]);

      setCategorias(listaCategorias);
      setFinalidades(listaFinalidades);

    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar categorias.");
    } finally {
      setLoading(false);
    }
  }

  async function salvarCategoria(event: FormEvent): Promise<void> {
    event.preventDefault();
    setLoading(true);
    setError(null);

    try {
      // Mantemos essa validação no front para feedback rápido ao usuário.
      if (idFinalidadeCategoria === "") {
        throw new Error("Selecione a finalidade.");
      }

      await categoriaApi.create({
        descricao: descricaoCategoria,
        idFinalidade: idFinalidadeCategoria,
      });
      setDescricaoCategoria("");
      setIdFinalidadeCategoria("");
      await carregarCategorias();
      // "Toca o sininho": avisa que categoria foi alterada/criada.
      onDataChanged();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Falha ao salvar categoria.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <section>
      {error && <div className="alert alert-danger">{error}</div>}
      {loading && <div className="alert alert-info">Processando...</div>}

      <form className="row g-3 mb-4" onSubmit={salvarCategoria}>
        <div className="col-md-8"><label className="form-label">Descrição</label><input className="form-control" value={descricaoCategoria} onChange={(e) => setDescricaoCategoria(e.target.value)} required maxLength={400} /></div>
        <div className="col-md-2"><label className="form-label">Finalidade</label><select className="form-select" value={idFinalidadeCategoria} onChange={(e) => setIdFinalidadeCategoria(e.target.value === "" ? "" : Number(e.target.value))} disabled={finalidades.length === 0} required><option value="">Selecione</option>{finalidades.map((item) => <option key={item.id} value={item.id}>{item.descricao}</option>)}</select></div>
        <div className="col-md-2 d-flex align-items-end"><button className="btn btn-primary w-100" type="submit">Salvar</button></div>
      </form>
      {finalidades.length === 0 && <div className="alert alert-warning">Finalidades não carregadas. Cadastre os domínios na base.</div>}

      <div className="table-responsive">
        <table className="table table-striped table-sm">
          <thead><tr><th>ID</th><th>Descrição</th><th>Finalidade</th></tr></thead>
          <tbody>{categorias.map((categoria) => <tr key={categoria.id}><td>{categoria.id}</td><td>{categoria.descricao}</td><td>{categoria.finalidade}</td></tr>)}</tbody>
        </table>
      </div>
    </section>
  );
}
