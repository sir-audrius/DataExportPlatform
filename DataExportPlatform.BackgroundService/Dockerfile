#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DataExportPlatform.BackgroundService/DataExportPlatform.BackgroundService.csproj", "DataExportPlatform.BackgroundService/"]
COPY ["DataExportPlatform.Shared/DataExportPlatform.Shared.csproj", "DataExportPlatform.Shared/"]
RUN dotnet restore "DataExportPlatform.BackgroundService/DataExportPlatform.BackgroundService.csproj"
COPY . .
WORKDIR "/src/DataExportPlatform.BackgroundService"
RUN dotnet build "DataExportPlatform.BackgroundService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataExportPlatform.BackgroundService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataExportPlatform.BackgroundService.dll"]