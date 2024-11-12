import type { Config } from "tailwindcss";

export default {
  content: [
    "{_components,_includes,static}/**/*.{md,ts,tsx}",
    "*.{ts,tsx}",
  ],
} satisfies Config;
