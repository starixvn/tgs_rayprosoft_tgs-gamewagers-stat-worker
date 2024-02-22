#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameWagers.Stat.Worker/Tgs.GameWagers.Stat.Worker.csproj", "Tgs.GameWagers.Stat.Worker/"]
COPY ["Tgs.GameWagers.Stat.Worker.Core/Tgs.GameWagers.Stat.Worker.Core.csproj", "Tgs.GameWagers.Stat.Worker.Core/"]
COPY ["Tgs.GameWagers.Stat.Worker.Common/Tgs.GameWagers.Stat.Worker.Common.csproj", "Tgs.GameWagers.Stat.Worker.Common/"]
COPY ["Tgs.GameWagers.Stat.Worker.Repositories/Tgs.GameWagers.Stat.Worker.Repositories.csproj", "Tgs.GameWagers.Stat.Worker.Repositories/"]
RUN dotnet restore "./Tgs.GameWagers.Stat.Worker/./Tgs.GameWagers.Stat.Worker.csproj"
COPY . .
WORKDIR "/src/Tgs.GameWagers.Stat.Worker"
RUN dotnet build "./Tgs.GameWagers.Stat.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameWagers.Stat.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameWagers.Stat.Worker.dll"]