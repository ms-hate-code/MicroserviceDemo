FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MicroserviceDemo.LoadBalancer/MicroserviceDemo.LoadBalancer.csproj", "MicroserviceDemo.LoadBalancer/"]
COPY ["MicroserviceDemo.BuildingBlock/MicroserviceDemo.BuildingBlock.csproj", "MicroserviceDemo.BuildingBlock/"]
RUN dotnet restore "MicroserviceDemo.LoadBalancer/MicroserviceDemo.LoadBalancer.csproj"
COPY . .
WORKDIR "/src/MicroserviceDemo.LoadBalancer"
RUN dotnet build "MicroserviceDemo.LoadBalancer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MicroserviceDemo.LoadBalancer.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroserviceDemo.LoadBalancer.dll", "--environment=Docker"]
