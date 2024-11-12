import type { CacheType, Interaction, SlashCommandBuilder } from "discord.js";

import * as ping from "./ping.ts";

export interface CommandHandler {
  data: SlashCommandBuilder;
  execute: (interaction: Interaction<CacheType>) => Promise<void>;
}

export const commands = new Map<string, CommandHandler>([
  ["ping", ping as CommandHandler],
]);
