FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Product/ProperNutritionDiary.Product.Api/ProperNutritionDiary.Product.Api.csproj", "src/Product/ProperNutritionDiary.Product.Api/"]
RUN dotnet restore "src/Product/ProperNutritionDiary.Product.Api/ProperNutritionDiary.Product.Api.csproj"
COPY . .
WORKDIR "/src/src/Product/ProperNutritionDiary.Product.Api"
RUN dotnet build "ProperNutritionDiary.Product.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ProperNutritionDiary.Product.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProperNutritionDiary.Product.Api.dll"]
