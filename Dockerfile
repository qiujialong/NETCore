#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM docker.io/microsoft/dotnet:latest 
WORKDIR /app
EXPOSE 80

FROM docker.io/microsoft/dotnet:latest
WORKDIR /src
COPY ["Docker.csproj", "./"]
RUN dotnet restore "Docker.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Docker.csproj" -c Release -o /app

RUN dotnet publish "Docker.csproj" -c Release -o /app

WORKDIR /app
ENTRYPOINT ["dotnet", "Docker.dll"]