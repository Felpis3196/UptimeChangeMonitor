# 🥇 Uptime & Change Monitor

Um sistema distribuído que monitora se sites estão online e detecta mudanças no conteúdo, usando workers assíncronos, filas e dashboard em tempo real.

## 🎯 Problema que Resolve

Empresas precisam saber:
- Se o site caiu
- Se alguém mudou conteúdo importante (preço, texto, banner, termos)
- Quando isso aconteceu

## 🏗️ Arquitetura

```
┌─────────────┐
│  Frontend   │  (TypeScript)
│ Dashboard   │
└─────▲───────┘
      │
┌─────┴───────┐
│ API (.NET)  │  CRUD + Auth + Regras
└─────▲───────┘
      │
┌─────┴──────────┐
│ RabbitMQ       │  Fila de Jobs
└─────▲──────────┘
      │
┌─────┴──────────┐
│ Workers        │  (Docker / C#)
│ - Uptime       │
│ - ChangeDetect │
└─────▲──────────┘
      │
┌─────┴──────────┐
│ PostgreSQL     │
└───────────────┘
```

## 🔧 Tecnologias

- .NET 8
- RabbitMQ
- Docker / Docker Compose
- PostgreSQL
- React/Next.js (Frontend)

## 📁 Estrutura do Projeto

```
UptimeChangeMonitor/
├── src/
│   ├── UptimeChangeMonitor.API/          # Web API
│   │   ├── Controllers/                  # Endpoints REST
│   │   ├── Extensions/                   # Extension Methods
│   │   ├── Middleware/                   # Custom Middleware
│   │   └── Program.cs
│   │
│   ├── UptimeChangeMonitor.Domain/       # Entidades e Domínio
│   │   ├── Entities/                     # Entidades do domínio
│   │   ├── Enums/                        # Enumerações
│   │   └── ValueObjects/                 # Value Objects
│   │
│   ├── UptimeChangeMonitor.Application/  # Lógica de Negócio
│   │   ├── DTOs/                         # Data Transfer Objects
│   │   ├── Interfaces/                   # Contratos/Interfaces
│   │   ├── Services/                     # Serviços de Aplicação
│   │   ├── Mappings/                     # AutoMapper Profiles
│   │   └── Validators/                   # FluentValidation
│   │
│   ├── UptimeChangeMonitor.Infrastructure/ # Infraestrutura
│   │   ├── Data/                         # DbContext, Migrations
│   │   ├── Repositories/                 # Implementação de Repositórios
│   │   ├── Messaging/                    # RabbitMQ Consumers/Producers
│   │   └── Configurations/               # Entity Framework Configurations
│   │
│   └── UptimeChangeMonitor.Workers/      # Background Workers
│       ├── UptimeWorker/                 # Worker de Uptime Check
│       ├── ChangeDetectionWorker/        # Worker de Change Detection
│       └── Shared/                       # Código compartilhado entre workers
│
├── docker-compose.yml                    # PostgreSQL + RabbitMQ
└── UptimeChangeMonitor.sln              # Solution File
```

## 🚀 Como Executar

### Pré-requisitos

- .NET 8 SDK
- Docker Desktop

### Passos

1. Clone o repositório
2. Execute os serviços de infraestrutura:
   ```bash
   docker-compose up -d
   ```
3. Execute as migrations (quando implementadas)
4. Execute a API:
   ```bash
   cd src/UptimeChangeMonitor.API
   dotnet run
   ```
5. Execute os Workers:
   ```bash
   cd src/UptimeChangeMonitor.Workers
   dotnet run
   ```

## 📝 MVP

- ✅ Monitorar URL
- ✅ Verificar se está online
- ✅ Detectar mudança simples
- ✅ Dashboard básico

## 🔄 Fluxo de Execução

1. Usuário cadastra um site
2. API cria um job na fila
3. Worker consome o job
4. Faz HTTP request
5. Compara com estado anterior
6. Salva resultado
7. Front mostra histórico

## 📈 Evoluções Futuras

- Alertas por e-mail
- Webhook
- Retry inteligente
- Rate limit
- Scheduler
- Multi-tenant

## 📄 Licença

MIT
