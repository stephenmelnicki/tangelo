import "@std/dotenv/load";

import { Client, Events, GatewayIntentBits } from "discord.js";

import { readyListener } from "./events/ready.ts";
import { interactionCreateListener } from "./events/interactionCreate.ts";
import { log } from "./utils/logger.ts";
import { kv } from "./utils/db.ts";

export const discordClient: Client = new Client({
  intents: [GatewayIntentBits.Guilds],
});

discordClient.on(Events.ClientReady, readyListener);
discordClient.on(Events.InteractionCreate, interactionCreateListener);

discordClient.login(Deno.env.get("DISCORD_TOKEN"));

Deno.addSignalListener("SIGINT", () => {
  log.critical("Received SIGINT. Exiting...");

  discordClient.destroy();
  kv.close();
  Deno.exit();
});
