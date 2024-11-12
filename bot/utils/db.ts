import { ulid } from "@std/ulid";
import { log } from "./logger.ts";
import type { Command, Guild, Usage, UsageMessage, User } from "./types.ts";

export const kv = await Deno.openKv(Deno.env.get("DATABASE_URL"));

kv.listenQueue(async (message: unknown) => {
  if (isUsageMessage(message)) {
    log.info("Received usage message", message);
    await trackUsage(message);
  } else {
    log.error("Unknown message received", message);
  }
});

function isUsageMessage(value: unknown): value is UsageMessage {
  return (
    (value as UsageMessage).commandName !== undefined &&
    typeof (value as UsageMessage).commandName === "string" &&
    (value as UsageMessage).commandDescription !== undefined &&
    typeof (value as UsageMessage).commandDescription === "string" &&
    (value as UsageMessage).userId !== undefined &&
    typeof (value as UsageMessage).userId === "string" &&
    (value as UsageMessage).username !== undefined &&
    typeof (value as UsageMessage).username === "string" &&
    (value as UsageMessage).userDiscriminator !== undefined &&
    typeof (value as UsageMessage).userDiscriminator === "string" &&
    (value as UsageMessage).guildId !== undefined &&
    typeof (value as UsageMessage).guildId === "string" &&
    (value as UsageMessage).guildName !== undefined &&
    typeof (value as UsageMessage).guildName === "string"
  );
}

async function trackUsage(message: UsageMessage): Promise<void> {
  let command = await getCommandByName(message.commandName);
  if (!command) {
    command = await createCommand({
      id: ulid(),
      name: message.commandName,
      description: message.commandDescription,
    });
  }

  let user = await getUser(message.userId);
  if (!user) {
    user = await createUser({
      id: message.userId,
      username: message.username,
      discriminator: message.userDiscriminator,
    });
  }

  let guild = await getGuild(message.guildId);
  if (!guild) {
    guild = await createGuild({
      id: message.guildId,
      name: message.guildName,
    });
  }

  await createUsage({
    id: ulid(),
    commandId: command.id,
    userId: message.userId,
    guildId: message.guildId,
  });
}

async function getCommandByName(name: string): Promise<Command | null> {
  const result = await kv.get<Command>(["commands_by_name", name]);
  return result.value;
}

async function createCommand(command: Command): Promise<Command> {
  const primaryKey = ["commands", command.id];
  const byNameKey = ["commands_by_name", command.name];

  const result = await kv.atomic()
    .check({ key: primaryKey, versionstamp: null })
    .check({ key: byNameKey, versionstamp: null })
    .set(primaryKey, command)
    .set(byNameKey, command)
    .commit();

  if (!result.ok) {
    throw new Error("Command with id or name already exists.");
  }

  return command;
}

async function getUser(id: string): Promise<User | null> {
  const result = await kv.get<User>(["users", id]);
  return result.value;
}

async function createUser(user: User): Promise<User> {
  const primaryKey = ["users", user.id];

  const result = await kv.atomic()
    .check({ key: primaryKey, versionstamp: null })
    .set(primaryKey, user)
    .commit();

  if (!result.ok) {
    throw new Error("User already exists.");
  }

  return user;
}

async function getGuild(id: string): Promise<Guild | null> {
  const result = await kv.get<Guild>(["guilds", id]);
  return result.value;
}

async function createGuild(guild: Guild): Promise<Guild> {
  const primaryKey = ["guilds", guild.id];

  const result = await kv.atomic()
    .check({ key: primaryKey, versionstamp: null })
    .set(primaryKey, guild)
    .commit();

  if (!result.ok) {
    throw new Error("Guild already exists.");
  }

  return guild;
}

async function createUsage(usage: Usage): Promise<void> {
  const result = await kv.atomic()
    .set(["usages", usage.id], usage)
    .set(["usages_by_command", usage.commandId], usage)
    .set(["usages_by_user", usage.userId], usage)
    .set(["usages_by_guild", usage.guildId], usage)
    .commit();

  if (!result.ok) {
    throw new Error("Failed to create usage");
  }
}
