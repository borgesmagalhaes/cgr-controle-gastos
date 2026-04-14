import React from "react";
import ReactDOM from "react-dom/client";
import "bootstrap/dist/css/bootstrap.min.css";
import { App } from "./App";

// Ponto de entrada da aplicacao React.
// Em termos simples: daqui o React "assume" a <div id="root"> do index.html.
ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    {/* StrictMode ajuda a detectar problemas comuns durante desenvolvimento. */}
    <App />
  </React.StrictMode>,
);
