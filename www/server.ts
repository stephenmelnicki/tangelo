import Server from "lume/core/server.ts";

export const server = new Server({ root: "." });

server.start();

console.log("Listening on http://localhost:8000");
