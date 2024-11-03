# See: https://devblogs.microsoft.com/dotnet/improving-multiplatform-container-support/
# (1) The build environment
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# See: https://docs.docker.com/engine/reference/builder/#automatic-platform-args-in-the-global-scope
ARG TARGETARCH
ARG BUILDPLATFORM

# Install Node JS
RUN apt-get -y update \
  && apt-get install -y curl \
  && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
  && apt-get install -y nodejs \
  && apt-get clean \
  && echo 'node verions:' $(node -v) \
  && echo 'npm version:' $(npm -v) \
  && echo 'dotnet version:' $(dotnet --version)

# Copy everything
COPY . .
# Install npm dependencies and compile tailwind styles
RUN npm install
RUN npm run tailwind
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN npm run tailwind
RUN dotnet publish -c Release --no-restore -o /app

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app .

ENV IS_CONTAINER=true
ENV ASPNETCORE_ENVIRONMENT=Production

USER $APP_UID
ENTRYPOINT [ "dotnet", "Tangelo.Web.dll" ]
