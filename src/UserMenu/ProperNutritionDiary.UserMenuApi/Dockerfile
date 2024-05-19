FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/UserMenu/ProperNutritionDiary.UserMenuApi/ProperNutritionDiary.UserMenuApi.csproj", "src/UserMenu/ProperNutritionDiary.UserMenuApi/"]
RUN dotnet restore "src/UserMenu/ProperNutritionDiary.UserMenuApi/ProperNutritionDiary.UserMenuApi.csproj"
COPY . .
WORKDIR "/src/src/UserMenu/ProperNutritionDiary.UserMenuApi"
RUN dotnet build "ProperNutritionDiary.UserMenuApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ProperNutritionDiary.UserMenuApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProperNutritionDiary.UserMenuApi.dll"]
