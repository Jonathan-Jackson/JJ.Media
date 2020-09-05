#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Storage/Storage.Console/Storage.Console.csproj", "src/Services/Storage/Storage.Console/"]
COPY ["src/Services/Storage/Storage.Domain/Storage.Core.csproj", "src/Services/Storage/Storage.Domain/"]
COPY ["src/Services/MediaInfo/MediaInfo.API.Client/MediaInfo.API.Client.csproj", "src/Services/MediaInfo/MediaInfo.API.Client/"]
COPY ["src/Libs/JJ.Framework/JJ.Framework.csproj", "src/Libs/JJ.Framework/"]
RUN dotnet restore "src/Services/Storage/Storage.Console/Storage.Console.csproj"
COPY . .
WORKDIR "/src/src/Services/Storage/Storage.Console"
RUN dotnet build "Storage.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Storage.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Storage.Console.dll"]