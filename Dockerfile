#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Docker.csproj", ""]
RUN dotnet restore "./Docker.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Docker.csproj" -c Release -o /app

RUN dotnet publish "Docker.csproj" -c Release -o /app

WORKDIR /app
ENTRYPOINT ["dotnet", "Docker.dll"]