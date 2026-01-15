# Guia da Estrutura do Projeto

Este documento explica onde cada componente deve ser implementado seguindo a arquitetura em camadas.

## Estrutura de Camadas

### 1. **UptimeChangeMonitor.Domain** (Camada de Domínio)
Camada mais interna, sem dependências externas. Contém apenas a lógica de negócio pura.

#### `Entities/`
- **Monitor.cs** - Entidade principal que representa um site/monitor
- **UptimeCheck.cs** - Histórico de verificações de uptime
- **ChangeDetection.cs** - Histórico de detecções de mudança
- **User.cs** (futuro) - Entidade de usuário para multi-tenant

#### `Enums/`
- **MonitorStatus.cs** - Status do monitor (Active, Inactive, Paused)
- **UptimeStatus.cs** - Status do uptime (Online, Offline, Timeout)
- **ChangeType.cs** - Tipo de mudança detectada (ContentChanged, StructureChanged)

#### `ValueObjects/`
- **Url.cs** - Value object para URL (validação)
- **CheckResult.cs** - Resultado de uma verificação

---

### 2. **UptimeChangeMonitor.Application** (Camada de Aplicação)
Contém a lógica de negócio e orquestração. Depende apenas do Domain.

#### `DTOs/` (Data Transfer Objects)
- **MonitorDto.cs** - DTO para retornar dados do monitor
- **CreateMonitorDto.cs** - DTO para criar monitor
- **UpdateMonitorDto.cs** - DTO para atualizar monitor
- **UptimeCheckDto.cs** - DTO para histórico de uptime
- **ChangeDetectionDto.cs** - DTO para histórico de mudanças
- **MonitorStatusDto.cs** - DTO para status/resumo

#### `Interfaces/` (Contratos)
- **IMonitorService.cs** - Interface do serviço de monitor
- **IUptimeCheckService.cs** - Interface do serviço de uptime
- **IChangeDetectionService.cs** - Interface do serviço de detecção
- **IQueueService.cs** - Interface para publicação de jobs
- **IRepository.cs** - Interface genérica de repositório

#### `Services/` (Implementação dos Serviços)
- **MonitorService.cs** - Lógica de negócio para monitores
- **UptimeCheckService.cs** - Lógica de negócio para uptime checks
- **ChangeDetectionService.cs** - Lógica de negócio para detecção de mudanças
- **QueueService.cs** - Serviço para publicar jobs no RabbitMQ

#### `Mappings/` (AutoMapper Profiles)
- **MappingProfile.cs** - Mapeamentos entre Entities e DTOs

#### `Validators/` (FluentValidation)
- **CreateMonitorValidator.cs** - Validações para criação de monitor
- **UpdateMonitorValidator.cs** - Validações para atualização de monitor

---

### 3. **UptimeChangeMonitor.Infrastructure** (Camada de Infraestrutura)
Implementa detalhes técnicos. Depende do Domain e Application.

#### `Data/`
- **ApplicationDbContext.cs** - DbContext do Entity Framework
- **Migrations/** - Migrations do banco de dados

#### `Repositories/`
- **Repository.cs** - Implementação genérica do repositório
- **MonitorRepository.cs** - Repositório específico para Monitor
- **UptimeCheckRepository.cs** - Repositório específico para UptimeCheck
- **ChangeDetectionRepository.cs** - Repositório específico para ChangeDetection

#### `Messaging/` (RabbitMQ)
- **RabbitMQService.cs** - Serviço para conexão com RabbitMQ
- **MessagePublisher.cs** - Publicador de mensagens
- **MessageConsumer.cs** - Consumidor de mensagens (base)
- **UptimeCheckMessage.cs** - Modelo de mensagem para uptime check
- **ChangeDetectionMessage.cs** - Modelo de mensagem para change detection

#### `Configurations/` (Entity Framework Configurations)
- **MonitorConfiguration.cs** - Configuração da entidade Monitor
- **UptimeCheckConfiguration.cs** - Configuração da entidade UptimeCheck
- **ChangeDetectionConfiguration.cs** - Configuração da entidade ChangeDetection

---

### 4. **UptimeChangeMonitor.API** (Camada de Apresentação)
Camada Web API. Depende do Application e Infrastructure.

#### `Controllers/`
- **MonitorsController.cs** - CRUD de monitores
  - GET /api/monitors
  - GET /api/monitors/{id}
  - POST /api/monitors
  - PUT /api/monitors/{id}
  - DELETE /api/monitors/{id}
  - GET /api/monitors/{id}/status
  - GET /api/monitors/{id}/history

- **HealthController.cs** - Health check
  - GET /api/health

#### `Extensions/`
- **ServiceCollectionExtensions.cs** - Extensions para DI
- **ApplicationBuilderExtensions.cs** - Extensions para middleware

#### `Middleware/`
- **ExceptionHandlingMiddleware.cs** - Tratamento global de exceções
- **RequestLoggingMiddleware.cs** - Logging de requests

---

### 5. **UptimeChangeMonitor.Workers** (Background Workers)
Workers que consomem mensagens do RabbitMQ.

#### `UptimeWorker/`
- **UptimeWorker.cs** - Worker que consome jobs de uptime check
- **UptimeCheckProcessor.cs** - Processador de verificações de uptime
- **HttpClientService.cs** - Serviço para fazer requisições HTTP

#### `ChangeDetectionWorker/`
- **ChangeDetectionWorker.cs** - Worker que consome jobs de change detection
- **ChangeDetectionProcessor.cs** - Processador de detecção de mudanças
- **ContentHasher.cs** - Serviço para calcular hash do conteúdo

#### `Shared/`
- **WorkerBase.cs** - Classe base para workers
- **MessageHandler.cs** - Handler base para mensagens
- **RetryPolicy.cs** - Política de retry

---

## Fluxo de Dependências

```
API
  ↓ depende de
Application + Infrastructure
  ↓ depende de
Domain
```

**Regra de ouro**: Camadas internas NUNCA dependem de camadas externas.

---

## Dependências entre Projetos

```
UptimeChangeMonitor.API
  ├── UptimeChangeMonitor.Application
  ├── UptimeChangeMonitor.Infrastructure
  └── UptimeChangeMonitor.Domain

UptimeChangeMonitor.Application
  └── UptimeChangeMonitor.Domain

UptimeChangeMonitor.Infrastructure
  ├── UptimeChangeMonitor.Domain
  └── UptimeChangeMonitor.Application

UptimeChangeMonitor.Workers
  ├── UptimeChangeMonitor.Application
  ├── UptimeChangeMonitor.Infrastructure
  └── UptimeChangeMonitor.Domain
```

---

## Checklist de Implementação

### Domain
- [ ] Criar entidades (Monitor, UptimeCheck, ChangeDetection)
- [ ] Criar enums (MonitorStatus, UptimeStatus, ChangeType)
- [ ] Criar value objects (se necessário)

### Application
- [ ] Criar DTOs
- [ ] Criar interfaces de serviços
- [ ] Implementar serviços
- [ ] Criar mapeamentos (AutoMapper)
- [ ] Criar validadores (FluentValidation)

### Infrastructure
- [ ] Configurar DbContext
- [ ] Criar repositórios
- [ ] Configurar Entity Framework (Configurations)
- [ ] Implementar RabbitMQ (Publisher/Consumer)
- [ ] Criar migrations

### API
- [ ] Criar controllers
- [ ] Configurar DI (ServiceCollectionExtensions)
- [ ] Configurar middleware
- [ ] Configurar Swagger
- [ ] Configurar CORS

### Workers
- [ ] Implementar UptimeWorker
- [ ] Implementar ChangeDetectionWorker
- [ ] Configurar conexão RabbitMQ
- [ ] Implementar processadores

---

## Próximos Passos

1. Implementar entidades no Domain
2. Configurar DbContext e criar primeira migration
3. Implementar repositórios genéricos
4. Implementar serviços de aplicação
5. Criar controllers
6. Configurar RabbitMQ
7. Implementar workers
8. Testar o fluxo completo
