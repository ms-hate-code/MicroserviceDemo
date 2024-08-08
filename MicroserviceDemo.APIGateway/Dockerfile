FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MicroserviceDemo.APIGateway/MicroserviceDemo.APIGateway.csproj", "MicroserviceDemo.APIGateway/"]
COPY ["MicroserviceDemo.BuildingBlock/MicroserviceDemo.BuildingBlock.csproj", "MicroserviceDemo.BuildingBlock/"]
RUN dotnet restore "MicroserviceDemo.APIGateway/MicroserviceDemo.APIGateway.csproj"
COPY . .
WORKDIR "/src/MicroserviceDemo.APIGateway"
RUN dotnet build "MicroserviceDemo.APIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MicroserviceDemo.APIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroserviceDemo.APIGateway.dll", "--environment=Docker"]
