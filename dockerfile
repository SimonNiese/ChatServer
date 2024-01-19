FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /ChatServer

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /ChatServer
COPY --from=build-env /ChatServer/out .
ENTRYPOINT ["dotnet", "ChatServer.dll"]
EXPOSE 8080/tcp