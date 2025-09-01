# Dockerfile para Transfers API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/Transfers/AnaBank.Transfers.API/AnaBank.Transfers.API.csproj", "src/Transfers/AnaBank.Transfers.API/"]
COPY ["src/Transfers/AnaBank.Transfers.Application/AnaBank.Transfers.Application.csproj", "src/Transfers/AnaBank.Transfers.Application/"]
COPY ["src/Transfers/AnaBank.Transfers.Domain/AnaBank.Transfers.Domain.csproj", "src/Transfers/AnaBank.Transfers.Domain/"]
COPY ["src/Transfers/AnaBank.Transfers.Infrastructure/AnaBank.Transfers.Infrastructure.csproj", "src/Transfers/AnaBank.Transfers.Infrastructure/"]
COPY ["src/BuildingBlocks/AnaBank.BuildingBlocks.Web/AnaBank.BuildingBlocks.Web.csproj", "src/BuildingBlocks/AnaBank.BuildingBlocks.Web/"]
COPY ["src/BuildingBlocks/AnaBank.BuildingBlocks.Data/AnaBank.BuildingBlocks.Data.csproj", "src/BuildingBlocks/AnaBank.BuildingBlocks.Data/"]

# Restore das dependências
RUN dotnet restore "src/Transfers/AnaBank.Transfers.API/AnaBank.Transfers.API.csproj"

# Copiar todo o código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/src/Transfers/AnaBank.Transfers.API"
RUN dotnet build "AnaBank.Transfers.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AnaBank.Transfers.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copiar scripts SQL
COPY Scripts/ /app/Scripts/

ENTRYPOINT ["dotnet", "AnaBank.Transfers.API.dll"]