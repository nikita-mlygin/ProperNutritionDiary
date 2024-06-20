FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Diary/ProperNutritionDiary.DiaryApi/ProperNutritionDiary.DiaryApi.csproj", "src/Diary/ProperNutritionDiary.DiaryApi/"]
RUN dotnet restore "src/Diary/ProperNutritionDiary.DiaryApi/ProperNutritionDiary.DiaryApi.csproj"
COPY src src
WORKDIR "/src/src/Diary/ProperNutritionDiary.DiaryApi"
RUN dotnet build "ProperNutritionDiary.DiaryApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ProperNutritionDiary.DiaryApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProperNutritionDiary.DiaryApi.dll"]
