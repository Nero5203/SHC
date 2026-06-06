import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [
    react(),
    {
      name: "root-index-fallback",
      configureServer(server) {
        server.middlewares.use((request, _response, next) => {
          if (request.url === "/") {
            request.url = "/index.html";
          }

          next();
        });
      }
    }
  ]
});
