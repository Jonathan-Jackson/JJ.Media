#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Discord/Discord.Console/Discord.Console.csproj", "src/Services/Discord/Discord.Console/"]
COPY ["src/Services/Discord/Discord.Core/Discord.Core.csproj", "src/Services/Discord/Discord.Core/"]
COPY ["src/Libs/JJ.Framework/JJ.Framework.csproj", "src/Libs/JJ.Framework/"]
COPY ["src/Services/MediaInfo/MediaInfo.API.Client/MediaInfo.API.Client.csproj", "src/Services/MediaInfo/MediaInfo.API.Client/"]
RUN dotnet restore "src/Services/Discord/Discord.Console/Discord.Console.csproj"
COPY . .
WORKDIR "/src/src/Services/Discord/Discord.Console"
RUN dotnet build "Discord.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Discord.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discord.Console.dll"]