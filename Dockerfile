#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build 
ADD http://ftp.us.debian.org/debian/pool/main/c/ca-certificates/ca-certificates_20210119_all.deb .
RUN dpkg -i ca-certificates_20210119_all.deb
RUN apt-get update && apt-get install -y ca-certificates && update-ca-certificates && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["ReCheckDemo_Core.csproj", ""]
RUN dotnet restore "./ReCheckDemo_Core.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ReCheckDemo_Core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReCheckDemo_Core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReCheckDemo_Core.dll"]