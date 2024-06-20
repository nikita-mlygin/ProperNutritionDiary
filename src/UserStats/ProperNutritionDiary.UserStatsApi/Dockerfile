FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/UserStats/ProperNutritionDiary.UserStatsApi/ProperNutritionDiary.UserStatsApi.csproj", "src/UserStats/ProperNutritionDiary.UserStatsApi/"]
RUN dotnet restore "src/UserStats/ProperNutritionDiary.UserStatsApi/ProperNutritionDiary.UserStatsApi.csproj"
COPY src src
WORKDIR "/src/src/UserStats/ProperNutritionDiary.UserStatsApi"
RUN dotnet build "ProperNutritionDiary.UserStatsApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ProperNutritionDiary.UserStatsApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProperNutritionDiary.UserStatsApi.dll"]
