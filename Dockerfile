#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Discord/Discord.API/Discord.API.csproj", "src/Services/Discord/Discord.API/"]
COPY ["src/Services/Storage/Storage.API.Client/Storage.API.Client.csproj", "src/Services/Storage/Storage.API.Client/"]
COPY ["src/Libs/JJ.Framework/JJ.Framework.csproj", "src/Libs/JJ.Framework/"]
COPY ["src/Libs/JJ.HostedService/JJ.HostedService.csproj", "src/Libs/JJ.HostedService/"]
COPY ["src/Services/MediaInfo/MediaInfo.API.Client/MediaInfo.API.Client.csproj", "src/Services/MediaInfo/MediaInfo.API.Client/"]
RUN dotnet restore "src/Services/Discord/Discord.API/Discord.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Discord/Discord.API"
RUN dotnet build "Discord.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Discord.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discord.API.dll"]

ENV JJNetDiscordToken dfadsfa
ENV ViewerDomain sdfasfd
