FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release
COPY ["MicroserviceDemo.ServiceB/MicroserviceDemo.ServiceB.csproj", "MicroserviceDemo.ServiceB/"]
COPY ["MicroserviceDemo.BuildingBlock/MicroserviceDemo.BuildingBlock.csproj", "MicroserviceDemo.BuildingBlock/"]
RUN dotnet restore "MicroserviceDemo.ServiceB/MicroserviceDemo.ServiceB.csproj"
COPY . .
WORKDIR "/src/MicroserviceDemo.ServiceB"
RUN dotnet build "MicroserviceDemo.ServiceB.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MicroserviceDemo.ServiceB.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD [ "dotnet dev-certs https --trust" ]
ENTRYPOINT ["dotnet", "MicroserviceDemo.ServiceB.dll", "--environment=Docker"]
