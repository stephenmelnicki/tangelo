import type { Interaction } from "discord.js";

import { commands } from "../commands/mod.ts";
import { log } from "../utils/logger.ts";

export async function interactionCreateListener(
  interaction: Interaction,
): Promise<void> {
  if (!interaction.isChatInputCommand()) return;

  const command = commands.get(interaction.commandName);
  if (!command) {
    log.info(`Command not found: ${interaction.commandName}`);
    return;
  }

  try {
    log.info(`Executing command: ${interaction.commandName}`);
    await command.execute(interaction);
  } catch (err: unknown) {
    log.error("Command execution failed.", err);

    if (interaction.replied || interaction.deferred) {
      await interaction.followUp({
        content: "Uh oh! Something went wrong. Please try again.",
        ephemeral: true,
      });
    } else {
      await interaction.reply({
        content: "Uh oh! Something went wrong. Please try again.",
        ephemeral: true,
      });
    }
  }
}
