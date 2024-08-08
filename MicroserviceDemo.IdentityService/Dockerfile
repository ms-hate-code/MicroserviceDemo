FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MicroserviceDemo.IdentityService/MicroserviceDemo.IdentityService.csproj", "MicroserviceDemo.IdentityService/"]
COPY ["MicroserviceDemo.BuildingBlock/MicroserviceDemo.BuildingBlock.csproj", "MicroserviceDemo.BuildingBlock/"]
RUN dotnet restore "MicroserviceDemo.IdentityService/MicroserviceDemo.IdentityService.csproj"
COPY . .
WORKDIR "/src/MicroserviceDemo.IdentityService"
RUN dotnet build "MicroserviceDemo.IdentityService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MicroserviceDemo.IdentityService.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroserviceDemo.IdentityService.dll", "--environment=Docker"]
