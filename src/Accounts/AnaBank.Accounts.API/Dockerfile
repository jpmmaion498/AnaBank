# Dockerfile para Accounts API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/Accounts/AnaBank.Accounts.API/AnaBank.Accounts.API.csproj", "src/Accounts/AnaBank.Accounts.API/"]
COPY ["src/Accounts/AnaBank.Accounts.Application/AnaBank.Accounts.Application.csproj", "src/Accounts/AnaBank.Accounts.Application/"]
COPY ["src/Accounts/AnaBank.Accounts.Domain/AnaBank.Accounts.Domain.csproj", "src/Accounts/AnaBank.Accounts.Domain/"]
COPY ["src/Accounts/AnaBank.Accounts.Infrastructure/AnaBank.Accounts.Infrastructure.csproj", "src/Accounts/AnaBank.Accounts.Infrastructure/"]
COPY ["src/BuildingBlocks/AnaBank.BuildingBlocks.Web/AnaBank.BuildingBlocks.Web.csproj", "src/BuildingBlocks/AnaBank.BuildingBlocks.Web/"]
COPY ["src/BuildingBlocks/AnaBank.BuildingBlocks.Data/AnaBank.BuildingBlocks.Data.csproj", "src/BuildingBlocks/AnaBank.BuildingBlocks.Data/"]

# Restore das dependências
RUN dotnet restore "src/Accounts/AnaBank.Accounts.API/AnaBank.Accounts.API.csproj"

# Copiar todo o código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/src/Accounts/AnaBank.Accounts.API"
RUN dotnet build "AnaBank.Accounts.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AnaBank.Accounts.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copiar scripts SQL
COPY Scripts/ /app/Scripts/

ENTRYPOINT ["dotnet", "AnaBank.Accounts.API.dll"]