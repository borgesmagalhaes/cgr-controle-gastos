import { useEffect, useState } from "react";
import { dominioApi } from "../api";
import type { DominioItem } from "../types";
import type { CrudPageProps } from "./page-types";

// Tela de consulta simples para listar os tipos de transação disponíveis no banco.
export function TiposTransacaoPage({ reloadKey }: CrudPageProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [tipos, setTipos] = useState<DominioItem[]>([]);

  useEffect(() => {
    // "Escuta o sininho": se reloadKey mudou, recarrega tipos.
    void carregarTipos();
  }, [reloadKey]);

  async function carregarTipos(): Promise<void> {
    setLoading(true);
    setError(null);

    try {
      // Tipos vêm da tabela de domínio do banco.
      const listaTipos = await dominioApi.tiposTransacao();
      setTipos(listaTipos);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar tipos de transação.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <section>
      {error && <div className="alert alert-danger">{error}</div>}
      {loading && <div className="alert alert-info">Carregando...</div>}

      <div className="table-responsive">
        <table className="table table-striped table-sm">
          <thead>
            <tr>
              <th>ID</th>
              <th>Descrição</th>
            </tr>
          </thead>
          <tbody>
            {tipos.map((item) => (
              <tr key={item.id}>
                <td>{item.id}</td>
                <td>{item.descricao}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
