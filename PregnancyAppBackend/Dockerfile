FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PregnancyAppBackend/PregnancyAppBackend.csproj", "PregnancyAppBackend/"]
RUN dotnet restore "PregnancyAppBackend/PregnancyAppBackend.csproj"
COPY . .
WORKDIR "/src/PregnancyAppBackend"
RUN dotnet build "PregnancyAppBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PregnancyAppBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PregnancyAppBackend.dll"]
