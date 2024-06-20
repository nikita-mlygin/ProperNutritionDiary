import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    https: {
      key: "./.cert/private-key.pem",
      cert: "./.cert/public-cert.pem",
    },
    host: '0.0.0.0', // Ensure the server is accessible externally
    port: 5173,
    strictPort: true,
    watch: {
      usePolling: true, // Use polling to watch for changes (required for Docker)
    },
  },
  plugins: [react()],
});
