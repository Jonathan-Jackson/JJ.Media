#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Downloader/Downloader.Console/Downloader.Console.csproj", "src/Services/Downloader/Downloader.Console/"]
COPY ["src/Services/Downloader/Downloader.Core/Downloader.Core.csproj", "src/Services/Downloader/Downloader.Core/"]
COPY ["src/Libs/JJ.Framework/JJ.Framework.csproj", "src/Libs/JJ.Framework/"]
COPY ["src/Services/Storage/Storage.API.Client/Storage.API.Client.csproj", "src/Services/Storage/Storage.API.Client/"]
RUN dotnet restore "src/Services/Downloader/Downloader.Console/Downloader.Console.csproj"
COPY . .
WORKDIR "/src/src/Services/Downloader/Downloader.Console"
RUN dotnet build "Downloader.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Downloader.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Downloader.Console.dll"]