import { useEffect, useState } from "react";
import { dominioApi } from "../api";
import type { DominioItem } from "../types";
import type { CrudPageProps } from "./page-types";

// Tela de consulta simples para listar as finalidades disponíveis no banco.
export function FinalidadesPage({ reloadKey }: CrudPageProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [finalidades, setFinalidades] = useState<DominioItem[]>([]);

  useEffect(() => {
    // "Escuta o sininho": se reloadKey mudou, recarrega finalidades.
    void carregarFinalidades();
  }, [reloadKey]);

  async function carregarFinalidades(): Promise<void> {
    setLoading(true);
    setError(null);

    try {
      // Finalidades vêm da tabela de domínio do banco.
      const listaFinalidades = await dominioApi.finalidades();
      setFinalidades(listaFinalidades);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar finalidades.");
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
            {finalidades.map((item) => (
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
