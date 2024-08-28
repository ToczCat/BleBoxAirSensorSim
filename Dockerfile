# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN --mount=type=secret,id=token \
    dotnet nuget add source https://nuget.pkg.github.com/DariuszGarbarz/index.json --name="github" --username $(cat /run/secrets/token) --valid-authentication-types basic --store-password-in-clear-text --password $(cat /run/secrets/token)

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BleBoxAirSensorSim/BleBoxAirSensorSim.csproj", "BleBoxAirSensorSim/"]
RUN dotnet restore "./BleBoxAirSensorSim/BleBoxAirSensorSim.csproj"
COPY . .
WORKDIR "/src/BleBoxAirSensorSim"
RUN dotnet build "./BleBoxAirSensorSim.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BleBoxAirSensorSim.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BleBoxAirSensorSim.dll"]