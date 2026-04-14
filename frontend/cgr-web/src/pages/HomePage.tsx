import { useEffect, useState } from "react";
import { consultaApi, pessoaApi, categoriaApi, transacaoApi } from "../api";
import type { ConsultaCategorias, ConsultaPessoas } from "../types";
import type { CrudPageProps } from "./page-types";

// Home mostra uma visao geral: contagens e consultas consolidadas.
export function HomePage({ reloadKey }: CrudPageProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [totalPessoas, setTotalPessoas] = useState(0);
  const [totalCategorias, setTotalCategorias] = useState(0);
  const [totalTransacoes, setTotalTransacoes] = useState(0);
  const [consultaPessoas, setConsultaPessoas] = useState<ConsultaPessoas | null>(null);
  const [consultaCategorias, setConsultaCategorias] = useState<ConsultaCategorias | null>(null);

  useEffect(() => {
    // "Escuta o sininho": toda mudança no sistema faz a Home atualizar os cartões e totais.
    void carregarHome();
  }, [reloadKey]);

  async function carregarHome(): Promise<void> {
    setLoading(true);
    setError(null);

    try {
      // Carrega tudo em paralelo para a Home ficar mais rápida.
      const [pessoas, categorias, transacoes, porPessoa, porCategoria] = await Promise.all([
        pessoaApi.list(),
        categoriaApi.list(),
        transacaoApi.list(),
        consultaApi.totaisPorPessoa(),
        consultaApi.totaisPorCategoria(),
      ]);

      setTotalPessoas(pessoas.length);
      setTotalCategorias(categorias.length);
      setTotalTransacoes(transacoes.length);
      setConsultaPessoas(porPessoa);
      setConsultaCategorias(porCategoria);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar Home.");
    } finally {
      setLoading(false);
    }
  }

  const totalReceitas = consultaPessoas?.totalGeral.totalReceitas ?? 0;
  const totalDespesas = consultaPessoas?.totalGeral.totalDespesas ?? 0;
  const saldoLiquido = consultaPessoas?.totalGeral.saldoLiquido ?? 0;

  // Formata valores monetários no padrão brasileiro (Real).
  function formatarMoeda(valor: number): string {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(valor);
  }

  return (
    <section className="d-grid gap-4">
      {error && <div className="alert alert-danger">{error}</div>}
      {loading && <div className="alert alert-info">Carregando...</div>}

      <div className="row g-3">
        {/* Cards de contagem geral para visão rápida. */}
        <div className="col-12 col-md-4"><div className="card"><div className="card-body"><h2 className="h6 text-secondary">Pessoas</h2><p className="h4 mb-0">{totalPessoas}</p></div></div></div>
        <div className="col-12 col-md-4"><div className="card"><div className="card-body"><h2 className="h6 text-secondary">Categorias</h2><p className="h4 mb-0">{totalCategorias}</p></div></div></div>
        <div className="col-12 col-md-4"><div className="card"><div className="card-body"><h2 className="h6 text-secondary">Transações</h2><p className="h4 mb-0">{totalTransacoes}</p></div></div></div>
      </div>

      <div className="row g-3">
        {/* Cards financeiros principais do sistema. */}
        <div className="col-12 col-md-4"><div className="card border-success-subtle"><div className="card-body"><h2 className="h6 text-success">Total Receitas</h2><p className="h5 mb-0">{formatarMoeda(totalReceitas)}</p></div></div></div>
        <div className="col-12 col-md-4"><div className="card border-danger-subtle"><div className="card-body"><h2 className="h6 text-danger">Total Despesas</h2><p className="h5 mb-0">{formatarMoeda(totalDespesas)}</p></div></div></div>
        <div className="col-12 col-md-4"><div className="card border-primary-subtle"><div className="card-body"><h2 className="h6 text-primary">Saldo Líquido</h2><p className="h5 mb-0">{formatarMoeda(saldoLiquido)}</p></div></div></div>
      </div>

      <div>
        <h2 className="h5">Totais por pessoa</h2>
        <div className="table-responsive">
          <table className="table table-bordered table-sm">
            <thead><tr><th>Pessoa</th><th>Receitas</th><th>Despesas</th><th>Saldo</th></tr></thead>
            <tbody>
              {consultaPessoas?.pessoas.map((linha) => (
                <tr key={linha.pessoaId}>
                  <td>{linha.nome}</td>
                  <td>{formatarMoeda(linha.totalReceitas)}</td>
                  <td>{formatarMoeda(linha.totalDespesas)}</td>
                  <td>{formatarMoeda(linha.saldo)}</td>
                </tr>
              ))}
            </tbody>
            <tfoot>
              <tr className="table-light fw-semibold">
                <td>Total geral</td>
                <td>{formatarMoeda(consultaPessoas?.totalGeral.totalReceitas ?? 0)}</td>
                <td>{formatarMoeda(consultaPessoas?.totalGeral.totalDespesas ?? 0)}</td>
                <td>{formatarMoeda(consultaPessoas?.totalGeral.saldoLiquido ?? 0)}</td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>

      <div>
        <h2 className="h5">Totais por categoria</h2>
        <div className="table-responsive">
          <table className="table table-bordered table-sm">
            <thead><tr><th>Categoria</th><th>Receitas</th><th>Despesas</th><th>Saldo</th></tr></thead>
            <tbody>
              {consultaCategorias?.categorias.map((linha) => (
                <tr key={linha.categoriaId}>
                  <td>{linha.descricao}</td>
                  <td>{formatarMoeda(linha.totalReceitas)}</td>
                  <td>{formatarMoeda(linha.totalDespesas)}</td>
                  <td>{formatarMoeda(linha.saldo)}</td>
                </tr>
              ))}
            </tbody>
            <tfoot>
              <tr className="table-light fw-semibold">
                <td>Total geral</td>
                <td>{formatarMoeda(consultaCategorias?.totalGeral.totalReceitas ?? 0)}</td>
                <td>{formatarMoeda(consultaCategorias?.totalGeral.totalDespesas ?? 0)}</td>
                <td>{formatarMoeda(consultaCategorias?.totalGeral.saldoLiquido ?? 0)}</td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    </section>
  );
}
