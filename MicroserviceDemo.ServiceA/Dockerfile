FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MicroserviceDemo.ServiceA/MicroserviceDemo.ServiceA.csproj", "MicroserviceDemo.ServiceA/"]
COPY ["MicroserviceDemo.BuildingBlock/MicroserviceDemo.BuildingBlock.csproj", "MicroserviceDemo.BuildingBlock/"]
RUN dotnet restore "MicroserviceDemo.ServiceA/MicroserviceDemo.ServiceA.csproj"
COPY . .
WORKDIR "/src/MicroserviceDemo.ServiceA"
RUN dotnet build "MicroserviceDemo.ServiceA.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MicroserviceDemo.ServiceA.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD [ "dotnet dev-certs https --trust" ]
ENTRYPOINT ["dotnet", "MicroserviceDemo.ServiceA.dll", "--environment=Docker"]
