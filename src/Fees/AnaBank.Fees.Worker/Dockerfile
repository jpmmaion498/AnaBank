# Dockerfile para Fees Worker
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/Fees/AnaBank.Fees.Worker/AnaBank.Fees.Worker.csproj", "src/Fees/AnaBank.Fees.Worker/"]
COPY ["src/BuildingBlocks/AnaBank.BuildingBlocks.Data/AnaBank.BuildingBlocks.Data.csproj", "src/BuildingBlocks/AnaBank.BuildingBlocks.Data/"]

# Restore das dependências
RUN dotnet restore "src/Fees/AnaBank.Fees.Worker/AnaBank.Fees.Worker.csproj"

# Copiar todo o código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/src/Fees/AnaBank.Fees.Worker"
RUN dotnet build "AnaBank.Fees.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AnaBank.Fees.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copiar scripts SQL
COPY Scripts/ /app/Scripts/

ENTRYPOINT ["dotnet", "AnaBank.Fees.Worker.dll"]