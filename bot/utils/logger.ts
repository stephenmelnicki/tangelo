import { ConsoleHandler, formatters, getLogger, setup } from "@std/log";

setup({
  handlers: {
    default: new ConsoleHandler("DEBUG", {
      formatter: formatters.jsonFormatter,
    }),
  },
});

export const log = getLogger();
