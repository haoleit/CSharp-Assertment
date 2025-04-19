import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";
// https://vite.dev/config/
export default defineConfig({
  plugins: [tailwindcss(), react()],
  server: {
    host: "0.0.0.0", // để truy cập từ bên ngoài
    port: 5173,
    origin: "http://localhost:5173", // đây là giá trị cố định CORS sẽ nhận
  },
});
