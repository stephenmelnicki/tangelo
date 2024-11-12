export const layout = "layout.tsx";
export const url = "/";

export default function () {
  return (
    <main role="main" class="flex flex-col justify-center items-center">
      <img class="h-72 w-72" src="/logo.svg" alt="Tangelo Bot logo" />
      <a
        class="px-6 py-2.5 whitespace-nowrap font-medium rounded-md border-2 border-black shadow-sm"
        href="https://discord.com/oauth2/authorize?client_id=1297010441548988581&permissions=0&integration_type=0&scope=bot+applications.commands"
      >
        Add to Discord
      </a>
    </main>
  );
}
