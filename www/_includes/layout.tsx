export default function Layout(props: Lume.Data) {
  return (
    <html lang="en">
      <head>
        <meta charset="utf-8" />
        <meta
          name="viewport"
          content="width=device-width, initial-scale=1.0"
        />
        <title>
          Tangelo Bot - Clowns hate tangelos. Messes with their equilibrium.
        </title>
        <meta
          name="description"
          content="Clowns hate tangelos. Messes with their equilibrium."
        />

        <link
          rel="stylesheet"
          type="text/css"
          href="/styles.css"
        />
      </head>
      <body class="flex flex-col px-4 sm:px-6 mx-auto min-h-screen max-w-screen-md">
        <props.comp.Header />
        {props.children}
        <props.comp.Footer />
      </body>
    </html>
  );
}
