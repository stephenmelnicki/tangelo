import lume from "lume/mod.ts";
import jsx from "lume/plugins/jsx_preact.ts";
import postcss from "lume/plugins/postcss.ts";

import tw from "tailwindcss";
import tailwindConfig from "./tailwind.config.ts";

const site = lume();

site.copy("static", ".");
site.copy("server.ts");

site.use(jsx());
site.use(postcss({ plugins: [tw(tailwindConfig)] }));

site.ignore("README.md");

export default site;
