import { useState } from "react";
import { HomePage } from "./pages/HomePage";
import { PessoasPage } from "./pages/PessoasPage";
import { CategoriasPage } from "./pages/CategoriasPage";
import { TransacoesPage } from "./pages/TransacoesPage";
import { FinalidadesPage } from "./pages/FinalidadesPage";
import { TiposTransacaoPage } from "./pages/TiposTransacaoPage";

type ViewKey = "home" | "pessoas" | "categorias" | "transacoes" | "finalidades" | "tipos-transacao";

type NavItem = {
  key: ViewKey;
  label: string;
};

const NAV_ITEMS: NavItem[] = [
  { key: "home", label: "Home" },
  { key: "pessoas", label: "Pessoas" },
  { key: "categorias", label: "Categorias" },
  { key: "transacoes", label: "Transações" },
  { key: "finalidades", label: "Finalidades" },
  { key: "tipos-transacao", label: "Tipos de Transação" },
];

// App principal: mantém apenas navegação e composição das páginas.
// Ideia para leitura leiga:
// esta tela "não faz regra de negócio", ela só controla qual página aparece.
export function App() {
  const [view, setView] = useState<ViewKey>("home");

  // Sempre que algum cadastro muda, incrementamos essa chave para forçar recarga das telas.
  const [reloadKey, setReloadKey] = useState(0);

  function notifyDataChanged(): void {
    // Aqui o "contador-sininho" toca: sempre soma 1 para avisar as páginas.
    setReloadKey((current) => current + 1);
  }

  function renderCurrentPage() {
    // Cada if escolhe qual página será exibida no corpo da aplicação.
    if (view === "home") {
      // reloadKey: quando muda, a Home recarrega os dados.
      // onDataChanged: callback para a página avisar que algo mudou.
      return <HomePage reloadKey={reloadKey} onDataChanged={notifyDataChanged} />;
    }

    if (view === "pessoas") {
      // Mesma ideia: passa o contador e a função de aviso para a página.
      return <PessoasPage reloadKey={reloadKey} onDataChanged={notifyDataChanged} />;
    }

    if (view === "categorias") {
      // Quando a página chamar onDataChanged, o contador sobe e tudo pode recarregar.
      return <CategoriasPage reloadKey={reloadKey} onDataChanged={notifyDataChanged} />;
    }

    if (view === "transacoes") {
      return <TransacoesPage reloadKey={reloadKey} onDataChanged={notifyDataChanged} />;
    }

    if (view === "finalidades") {
      return <FinalidadesPage reloadKey={reloadKey} onDataChanged={notifyDataChanged} />;
    }

    return <TiposTransacaoPage reloadKey={reloadKey} onDataChanged={notifyDataChanged} />;
  }

  return (
    <>
      {/* Barra superior simples com título do sistema e botão para recarregar dados. */}
      <nav className="navbar navbar-expand-lg bg-body-tertiary border-bottom">
        <div className="container">
          <span className="navbar-brand mb-0 h1">CGR</span>
          <button className="btn btn-outline-primary btn-sm" onClick={notifyDataChanged}>
            Atualizar dados
          </button>
        </div>
      </nav>

      <main className="container py-4">
        {/* Menu de navegação por abas (pills), para o usuário trocar de tela. */}
        <ul className="nav nav-pills mb-4">
          {NAV_ITEMS.map((item) => (
            <li key={item.key} className="nav-item">
              <button
                type="button"
                className={`nav-link ${view === item.key ? "active" : ""}`}
                onClick={() => setView(item.key)}
              >
                {item.label}
              </button>
            </li>
          ))}
        </ul>

        <h2 className="h4 mb-3">{NAV_ITEMS.find((item) => item.key === view)?.label}</h2>

        {renderCurrentPage()}
      </main>
    </>
  );
}
