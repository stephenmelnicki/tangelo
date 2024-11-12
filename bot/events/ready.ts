import type { Client } from "discord.js";

import { commands } from "../commands/mod.ts";
import { log } from "../utils/logger.ts";

async function registerCommands(client: Client<true>) {
  log.info("Registering commands.");

  const body = Array.from(commands.values()).map((command) =>
    command.data.toJSON()
  );

  await client.application.commands.set(body);
  log.info("Done.");
}

export async function readyListener(client: Client<true>): Promise<void> {
  log.info(`Ready! Logged in as ${client.user.tag} (${client.user.id}).`);

  await registerCommands(client);
}
