#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

# BEGIN MODIFICATION - Node is needed for development (but not production)
RUN apt-get update; apt-get install --assume-yes curl
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -
RUN apt-get install --assume-yes nodejs
# END MODIFICATION

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DataExportPlatform/DataExportPlatform.csproj", "DataExportPlatform/"]
RUN dotnet restore "DataExportPlatform/DataExportPlatform.csproj"
COPY . .
WORKDIR "/src/DataExportPlatform"
RUN dotnet build "DataExportPlatform.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataExportPlatform.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataExportPlatform.dll"]