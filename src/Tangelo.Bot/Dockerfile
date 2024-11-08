# See: https://devblogs.microsoft.com/dotnet/improving-multiplatform-container-support/
# (1) The build environment
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# See: https://docs.docker.com/engine/reference/builder/#automatic-platform-args-in-the-global-scope
ARG TARGETARCH
ARG BUILDPLATFORM

# Copy everything
COPY . .
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release --no-restore -o /app

# build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app
COPY --from=build /app .

ENV IS_CONTAINER=true
ENV ASPNETCORE_ENVIRONMENT=Production

USER $APP_UID
ENTRYPOINT [ "dotnet", "Tangelo.Bot.dll" ]
