import {
  type ChatInputCommandInteraction,
  SlashCommandBuilder,
} from "discord.js";

import { log } from "../utils/logger.ts";
import { kv } from "../utils/db.ts";
import type { UsageMessage } from "../utils/types.ts";

export const data = new SlashCommandBuilder()
  .setName("ping")
  .setDescription("Replies with Pong!");

export async function execute(interaction: ChatInputCommandInteraction) {
  log.info("Ping command executing");

  await interaction.reply({ content: "Pong!", ephemeral: true });

  const message: UsageMessage = {
    commandName: data.name,
    commandDescription: data.description,
    userId: interaction.user.id,
    username: interaction.user.username,
    userDiscriminator: interaction.user.discriminator,
    guildId: interaction.guild!.id,
    guildName: interaction.guild!.name,
  };

  await kv.enqueue(message);
}
