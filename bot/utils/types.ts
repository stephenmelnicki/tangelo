export interface Command {
  id: string;
  name: string;
  description: string;
}

export interface User {
  id: string;
  username: string;
  discriminator: string;
}

export interface Guild {
  id: string;
  name: string;
}

export interface Usage {
  id: string;
  commandId: string;
  userId: string;
  guildId: string;
}

export interface UsageMessage {
  commandName: string;
  commandDescription: string;
  userId: string;
  username: string;
  userDiscriminator: string;
  guildId: string;
  guildName: string;
}
