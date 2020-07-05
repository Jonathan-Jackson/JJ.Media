#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/MediaInfo/MediaInfo.API/MediaInfo.API.csproj", "src/Services/MediaInfo/MediaInfo.API/"]
COPY ["src/Services/MediaInfo/MediaInfo.Infrastructure/MediaInfo.Infrastructure.csproj", "src/Services/MediaInfo/MediaInfo.Infrastructure/"]
COPY ["src/Libs/JJ.Framework/JJ.Framework.csproj", "src/Libs/JJ.Framework/"]
COPY ["src/Services/MediaInfo/MediaInfo.Domain/MediaInfo.Domain.csproj", "src/Services/MediaInfo/MediaInfo.Domain/"]
RUN dotnet restore "src/Services/MediaInfo/MediaInfo.API/MediaInfo.API.csproj"
COPY . .
WORKDIR "/src/src/Services/MediaInfo/MediaInfo.API"
RUN dotnet build "MediaInfo.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediaInfo.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaInfo.API.dll"]