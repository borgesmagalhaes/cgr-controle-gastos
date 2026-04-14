import { useEffect, useMemo, useState } from "react";
import type { FormEvent } from "react";
import { categoriaApi, dominioApi, pessoaApi, transacaoApi } from "../api";
import type { Categoria, DominioItem, Pessoa, Transacao } from "../types";
import type { CrudPageProps } from "./page-types";

export function TransacoesPage({ reloadKey, onDataChanged }: CrudPageProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [tiposTransacao, setTiposTransacao] = useState<DominioItem[]>([]);
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);

  const [descricaoTransacao, setDescricaoTransacao] = useState("");
  const [valorTransacao, setValorTransacao] = useState<number>(0);
  const [valorTransacaoTexto, setValorTransacaoTexto] = useState("R$ 0,00");
  const [tipoTransacao, setTipoTransacao] = useState("");
  const [idPessoaTransacao, setIdPessoaTransacao] = useState<number | "">("");
  const [idCategoriaTransacao, setIdCategoriaTransacao] = useState<number | "">("");

  // Formata valores no padrão brasileiro de moeda (Real).
  function formatarMoeda(valor: number): string {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(valor);
  }

  // Converte texto digitado em moeda para número e texto formatado.
  function normalizarValorEntrada(textoDigitado: string): { valor: number; texto: string } {
    const apenasDigitos = textoDigitado.replace(/\D/g, "");
    const valor = apenasDigitos ? Number(apenasDigitos) / 100 : 0;

    return {
      valor,
      texto: formatarMoeda(valor),
    };
  }

  const pessoaSelecionada = useMemo(() => {
    // Quando ainda não existe pessoa escolhida, retornamos null para evitar regras indevidas.
    if (idPessoaTransacao === "") {
      return null;
    }

    return pessoas.find((p) => p.id === idPessoaTransacao) ?? null;
  }, [pessoas, idPessoaTransacao]);
  const menorDeIdadeSelecionado = Boolean(pessoaSelecionada && pessoaSelecionada.idade < 18);

  const categoriasCompativeis = useMemo(() => {
    return categorias.filter((categoria) => categoria.finalidade === "ambas" || categoria.finalidade === tipoTransacao);
  }, [categorias, tipoTransacao]);

  useEffect(() => {
    // "Escuta o sininho": reloadKey mudou, recarrega os dados da tela.
    void carregarTransacoes();
  }, [reloadKey]);

  useEffect(() => {
    // Sempre que trocar o tipo, filtramos categorias compatíveis no front para facilitar a escolha.
    if (idCategoriaTransacao !== "" && !categoriasCompativeis.some((categoria) => categoria.id === idCategoriaTransacao)) {
      setIdCategoriaTransacao("");
    }
  }, [categoriasCompativeis, idCategoriaTransacao]);

  useEffect(() => {
    // Regra visual: menor de idade não pode manter "receita" selecionado no campo tipo.
    if (menorDeIdadeSelecionado && tipoTransacao === "receita") {
      setTipoTransacao("despesa");
    }
  }, [menorDeIdadeSelecionado, tipoTransacao]);

  useEffect(() => {
    if (tipoTransacao && !tiposTransacao.some((tipo) => tipo.descricao === tipoTransacao)) {
      setTipoTransacao("");
    }
  }, [tiposTransacao, tipoTransacao]);

  async function carregarTransacoes(): Promise<void> {
    setLoading(true);
    setError(null);

    try {
      const [listaPessoas, listaCategorias, listaTransacoes] = await Promise.all([
        pessoaApi.list(),
        categoriaApi.list(),
        transacaoApi.list(),
      ]);
      const listaTipos = await dominioApi.tiposTransacao();

      setPessoas(listaPessoas);
      setCategorias(listaCategorias);
      setTransacoes(listaTransacoes);
      setTiposTransacao(listaTipos);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar transações.");
    } finally {
      setLoading(false);
    }
  }

  async function salvarTransacao(event: FormEvent): Promise<void> {
    event.preventDefault();
    setLoading(true);
    setError(null);

    try {
      // Validações simples para orientar o usuário antes de chamar a API.
      if (idPessoaTransacao === "") {
        throw new Error("Selecione a pessoa.");
      }

      if (!tipoTransacao) {
        throw new Error("Selecione o tipo.");
      }

      if (idCategoriaTransacao === "") {
        throw new Error("Selecione a categoria.");
      }

      if (menorDeIdadeSelecionado && tipoTransacao === "receita") {
        throw new Error("Menor de idade pode registrar apenas despesa.");
      }

      if (valorTransacao <= 0) {
        throw new Error("Informe um valor maior que zero.");
      }

      await transacaoApi.create({
        descricao: descricaoTransacao,
        valor: Number(valorTransacao),
        tipo: tipoTransacao,
        idCategoria: Number(idCategoriaTransacao),
        idPessoa: Number(idPessoaTransacao),
      });

      setDescricaoTransacao("");
      setValorTransacao(0);
      setValorTransacaoTexto("R$ 0,00");
      // Após salvar, voltamos os combos para placeholder para deixar claro que é um novo cadastro.
      setTipoTransacao("");
      setIdPessoaTransacao("");
      setIdCategoriaTransacao("");
      await carregarTransacoes();
      // "Toca o sininho": avisa a App que uma nova transação mudou os totais.
      onDataChanged();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Falha ao salvar transação.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <section>
      {error && <div className="alert alert-danger">{error}</div>}
      {loading && <div className="alert alert-info">Processando...</div>}

      <form className="row g-3 mb-4" onSubmit={salvarTransacao}>
        <div className="col-12 col-lg-7">
          <label className="form-label">Descrição</label>
          <input
            className="form-control"
            value={descricaoTransacao}
            onChange={(e) => setDescricaoTransacao(e.target.value)}
            required
            maxLength={400}
          />
        </div>

        <div className="col-12 col-sm-6 col-lg-2">
          <label className="form-label">Valor</label>
          <input
            type="text"
            inputMode="numeric"
            className="form-control"
            value={valorTransacaoTexto}
            onChange={(e) => {
              const resultado = normalizarValorEntrada(e.target.value);
              setValorTransacao(resultado.valor);
              setValorTransacaoTexto(resultado.texto);
            }}
            required
          />
        </div>

        <div className="col-12 col-sm-6 col-lg-3">
          <label className="form-label">Pessoa</label>
          <select
            className="form-select"
            value={idPessoaTransacao}
            onChange={(e) => setIdPessoaTransacao(e.target.value === "" ? "" : Number(e.target.value))}
            required
          >
            <option value="">Selecione a pessoa</option>
            {pessoas.map((pessoa) => (
              <option key={pessoa.id} value={pessoa.id}>
                {pessoa.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-12 col-sm-6 col-lg-3">
          <label className="form-label">Tipo</label>
          <select
            className="form-select"
            value={tipoTransacao}
            onChange={(e) => setTipoTransacao(e.target.value)}
            disabled={tiposTransacao.length === 0}
            required
          >
            <option value="">Selecione o tipo</option>
            {tiposTransacao.map((tipo) => (
              <option
                key={tipo.id}
                value={tipo.descricao}
                disabled={tipo.descricao === "receita" && Boolean(pessoaSelecionada && pessoaSelecionada.idade < 18)}
              >
                {tipo.descricao}
              </option>
            ))}
          </select>
        </div>

        <div className="col-12 col-sm-6 col-lg-5">
          <label className="form-label">Categoria</label>
          <select
            className="form-select"
            value={idCategoriaTransacao}
            onChange={(e) => setIdCategoriaTransacao(e.target.value === "" ? "" : Number(e.target.value))}
            required
          >
            <option value="">Selecione a categoria</option>
            {categoriasCompativeis.map((categoria) => (
              <option key={categoria.id} value={categoria.id}>
                {categoria.descricao}
              </option>
            ))}
          </select>
        </div>

        <div className="col-12 col-lg-4 d-grid align-self-end">
          <button className="btn btn-primary" type="submit">
            Salvar transação
          </button>
        </div>
      </form>
      {menorDeIdadeSelecionado && <div className="alert alert-warning">Para usuários menores de 18 anos, apenas despesas serão aceitas.</div>}
      {tiposTransacao.length === 0 && <div className="alert alert-warning">Tipos de transação não carregados. Verifique os domínios no banco.</div>}

      <div className="table-responsive">
        <table className="table table-striped table-sm">
          <thead><tr><th>ID</th><th>Descrição</th><th>Valor</th><th>Tipo</th><th>Categoria</th><th>Pessoa</th></tr></thead>
          <tbody>{transacoes.map((transacao) => <tr key={transacao.id}><td>{transacao.id}</td><td>{transacao.descricao}</td><td>{formatarMoeda(transacao.valor)}</td><td>{transacao.tipo}</td><td>{transacao.categoria}</td><td>{transacao.pessoa}</td></tr>)}</tbody>
        </table>
      </div>
    </section>
  );
}
