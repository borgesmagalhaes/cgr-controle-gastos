// Props padrao para paginas que precisam recarregar dados e avisar alteracoes.
export type CrudPageProps = {
  // Mudou esse número = página deve recarregar seus dados.
  reloadKey: number;
  // Callback para avisar a App que houve mudança em cadastro.
  onDataChanged: () => void;
};
