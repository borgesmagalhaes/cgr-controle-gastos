import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// Configuracao de build e dev server do frontend React.
export default defineConfig({
  plugins: [react()],
});
