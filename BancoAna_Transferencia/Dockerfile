# Stage base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os .csproj
COPY ["BancoAna_Transferencia/BancoAna_Transferencia.csproj", "BancoAna_Transferencia/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "BancoAna_Transferencia/BancoAna_Transferencia.csproj"

# Copiar tudo
COPY . .

# Build
WORKDIR /src/BancoAna_Transferencia
RUN dotnet build -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BancoAna_Transferencia.dll"]
