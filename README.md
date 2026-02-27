# Sistema de Compra Programada de Ações — Itaú Corretora

Um sistema enterprise para gerenciar compras automáticas e recorrentes de ações, com distribuição proporcional entre clientes, cálculo de IR, rebalanceamento de carteira e análise de rentabilidade.

## Stack Tecnológico

- **Backend:** .NET 10 (C#)
- **API:** ASP.NET Core Web API (REST + Swagger/OpenAPI)
- **Arquitetura:** Clean Architecture + DDD + CQRS
- **Database:** MySQL 8.0
- **Messaging:** Apache Kafka
- **ORM:** Entity Framework Core + Pomelo MySql Driver
- **Testing:** xUnit + FluentAssertions + Moq
- **Logging:** Serilog (JSON estruturado)
- **Containerização:** Docker Compose (MySQL + Kafka + Zookeeper)

## Estrutura do Projeto

```
CompraProgramada/
├── src/
│   ├── CompraProgramada.Domain/          # Entidades, Value Objects, Interfaces
│   ├── CompraProgramada.Application/     # Use Cases, DTOs, Commands/Queries (CQRS)
│   ├── CompraProgramada.Infrastructure/  # EF Core, Repositórios, Kafka, Parsers
│   └── CompraProgramada.API/             # ASP.NET Core Web API, Controllers
├── tests/
│   └── CompraProgramada.Tests/           # Testes xUnit
├── cotacoes/                             # Arquivos COTAHIST B3
├── docker-compose.yml
├── CompraProgramada.sln
└── README.md
```

## Quick Start

```bash
# 1. Restaurar dependências
dotnet restore

# 2. Iniciar containers (MySQL + Kafka)
docker-compose up -d

# 3. Aplicar migrações EF Core
cd src/CompraProgramada.API && dotnet ef database update && cd ../../

# 4. Rodar API
cd src/CompraProgramada.API && dotnet run

# 5. Testes
dotnet test
```

API: `https://localhost:5001`
Swagger: `https://localhost:5001/swagger`

## Status: Fase 0 ✅ Scaffolding Criado

- ✅ Solução .NET 10 com 5 projetos
- ✅ Clean Architecture + DDD + CQRS configurado
- ✅ Docker Compose (MySQL + Kafka) pronto
- ✅ Pacotes NuGet instalados (MediatR, EF Core, Serilog, Kafka, etc.)
- ✅ Referências de projetos estabelecidas

Próximos: Fase 1 (Modelagem de dados e entidades)