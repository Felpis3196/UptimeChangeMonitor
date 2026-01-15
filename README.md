# Uptime & Change Monitor

Sistema distribuído para monitoramento de disponibilidade de sites e detecção de mudanças no conteúdo, utilizando workers assíncronos, filas de mensagens e dashboard em tempo real.

## Funcionalidades

- Monitoramento de disponibilidade (uptime) de sites
- Detecção de mudanças no conteúdo das páginas
- Análise detalhada de alterações (caracteres, palavras, linhas, elementos HTML)
- Dashboard em tempo real com gráficos e estatísticas
- Histórico completo de verificações e mudanças

## Arquitetura

```
┌─────────────┐
│  Frontend   │  Next.js + TypeScript + React
│ Dashboard   │
└─────▲───────┘
      │
┌─────┴───────┐
│ API (.NET)  │  ASP.NET Core 8
└─────▲───────┘
      │
┌─────┴──────────┐
│ RabbitMQ       │  Fila de Jobs
└─────▲──────────┘
      │
┌─────┴──────────┐
│ Workers        │  Background Workers (.NET 8)
│ - Uptime       │
│ - ChangeDetect │
└─────▲──────────┘
      │
┌─────┴──────────┐
│ PostgreSQL     │  Banco de Dados
└───────────────┘
```

## Tecnologias

**Backend:**
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- RabbitMQ

**Frontend:**
- Next.js 14
- TypeScript
- React
- Tailwind CSS
- Recharts

**Infraestrutura:**
- Docker / Docker Compose

## Estrutura do Projeto

```
UptimeChangeMonitor/
├── src/
│   ├── UptimeChangeMonitor.API/          # Web API
│   ├── UptimeChangeMonitor.Domain/        # Entidades e Domínio
│   ├── UptimeChangeMonitor.Application/   # Lógica de Negócio
│   ├── UptimeChangeMonitor.Infrastructure/# Infraestrutura
│   └── UptimeChangeMonitor.Workers/      # Background Workers
├── frontend/                               # Frontend Next.js
├── docker-compose.yml                     # Orquestração de serviços
└── UptimeChangeMonitor.sln               # Solution File
```

## Como Executar

### Pré-requisitos

- .NET 8 SDK
- Docker Desktop
- Node.js 18+ (para desenvolvimento do frontend)

### Executando com Docker Compose

1. Clone o repositório:
   ```bash
   git clone <repository-url>
   cd UptimeChangeMonitor
   ```

2. Execute todos os serviços:
   ```bash
   docker-compose up -d --build
   ```

   Isso irá iniciar:
   - PostgreSQL (porta 5432)
   - RabbitMQ (porta 5672, Management UI: 15672)
   - API (porta 5000)
   - Workers
   - Frontend (porta 3000)

3. Acesse a aplicação:
   - Frontend: http://localhost:3000
   - API Swagger: http://localhost:5000/swagger
   - RabbitMQ Management: http://localhost:15672 (guest/guest)

### Executando Localmente (Desenvolvimento)

1. Inicie os serviços de infraestrutura:
   ```bash
   docker-compose up -d postgres rabbitmq
   ```

2. Aplique as migrations:
   ```bash
   cd src/UptimeChangeMonitor.API
   dotnet ef database update --project ../UptimeChangeMonitor.Infrastructure
   ```

3. Execute a API:
   ```bash
   cd src/UptimeChangeMonitor.API
   dotnet run
   ```

4. Execute os Workers:
   ```bash
   cd src/UptimeChangeMonitor.Workers
   dotnet run
   ```

5. Execute o Frontend:
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

## Funcionalidades Detalhadas

### Monitoramento de Uptime

- Verificação periódica da disponibilidade de sites
- Medição de tempo de resposta
- Histórico de status (online/offline/timeout/erro)
- Cálculo de porcentagem de uptime
- Gráficos de tendência

### Detecção de Mudanças

- Comparação de conteúdo usando hash SHA256
- Análise detalhada de diferenças:
  - Caracteres adicionados/removidos
  - Palavras adicionadas/removidas
  - Linhas adicionadas/removidas
- Extração e comparação de elementos HTML:
  - Título da página
  - Meta description
- Snippets do conteúdo anterior e atual
- Classificação automática do tipo de mudança (Conteúdo vs Estrutura)

### Dashboard

- Visão geral de todos os monitores
- Status em tempo real
- Gráficos de uptime e tempo de resposta
- Histórico de verificações e mudanças
- Métricas agregadas

## Fluxo de Execução

1. Usuário cadastra um monitor através do dashboard
2. API cria jobs na fila RabbitMQ (uptime check e/ou change detection)
3. Workers consomem os jobs da fila
4. Workers fazem requisições HTTP para os sites monitorados
5. Resultados são comparados com estado anterior
6. Dados são salvos no PostgreSQL
7. Frontend exibe informações atualizadas em tempo real

## Documentação Adicional

- `DOCKER_SETUP.md` - Configuração detalhada do Docker
- `APLICAR_MIGRATIONS.md` - Guia de aplicação de migrations
- `API_VERSIONING.md` - Documentação de versionamento da API
- `ACESSO_API.md` - Informações de acesso à API
