FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/gateway/api.gateway/api.gateway.csproj", "src/gateway/api.gateway/"]
RUN dotnet restore "src/gateway/api.gateway/api.gateway.csproj"
COPY . .
WORKDIR "/src/src/gateway/api.gateway"
RUN dotnet build "api.gateway.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "api.gateway.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.gateway.dll"]
