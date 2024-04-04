FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5063

ENV ASPNETCORE_URLS=http://+:5063

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/User/ProperNutritionDiary.User.Api/ProperNutritionDiary.User.Api.csproj", "src/User/ProperNutritionDiary.User.Api/"]
RUN dotnet restore "src/User/ProperNutritionDiary.User.Api/ProperNutritionDiary.User.Api.csproj"
COPY . .
WORKDIR "/src/src/User/ProperNutritionDiary.User.Api"
RUN dotnet build "ProperNutritionDiary.User.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ProperNutritionDiary.User.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProperNutritionDiary.User.Api.dll"]
