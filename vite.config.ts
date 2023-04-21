import react from "@vitejs/plugin-react-swc";
import path from "path";
import { defineConfig, splitVendorChunkPlugin } from "vite";
const pkg = require("./package.json");

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), splitVendorChunkPlugin()],
  define: {
    "import.meta.env.APP_VERSION": JSON.stringify(pkg.version),
  },
  build: {
    outDir: "wwwroot",
    rollupOptions: {
      output: {
        manualChunks: {
          react: ["react", "react-router-dom", "react-dom"],
          chartjs: ["chart.js", "chartjs-plugin-annotation"],
          highlight: ["rehype-highlight", "remark-gfm"],
        },
      },
    },
  },
  server: {
    port: 3000,
    proxy: {
      '/api' : {
        target: 'http://localhost:5251',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, '/api')
      }
    } 
  },
  resolve: {
    conditions: ["development", "browser"],
    alias: [
      { find: "@app/charts", replacement: path.resolve(__dirname, "./webapp/pkg/charts") },
      { find: "@app/primitives", replacement: path.resolve(__dirname, "./webapp/pkg/primitives") },
      { find: "@app/theme", replacement: path.resolve(__dirname, "./webapp/pkg/theme") },
      { find: "@app/navigation", replacement: path.resolve(__dirname, "./webapp/pkg/navigation") },
      { find: "@app/apps", replacement: path.resolve(__dirname, "./webapp/pkg/apps") },
      { find: "@app/auth", replacement: path.resolve(__dirname, "./webapp/pkg/auth") },
      { find: "@app/widgets", replacement: path.resolve(__dirname, "./webapp/pkg/widgets") },
      { find: "@app", replacement: path.resolve(__dirname, "./webapp/src") },
    ],
  },
});