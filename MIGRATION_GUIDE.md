# Guia de Migrations

## IMPORTANTE: Aplicar Migrations ANTES de usar a API

O erro `relation "Monitors" does not exist` significa que as tabelas não foram criadas. **Sempre aplique as migrations antes de usar a API!**

## Aplicar Migrations Automaticamente (Docker)

O Docker Compose agora aplica migrations automaticamente na inicialização do container da API.

## Como criar a primeira migration

1. Certifique-se de que o PostgreSQL está rodando:
   ```bash
   docker-compose up -d postgres
   ```

2. Navegue até a pasta da API:
   ```bash
   cd src/UptimeChangeMonitor.API
   ```

3. Crie a migration:
   ```bash
   dotnet ef migrations add InitialCreate --project ../UptimeChangeMonitor.Infrastructure --startup-project .
   ```

4. Aplique a migration no banco:
   ```bash
   dotnet ef database update --project ../UptimeChangeMonitor.Infrastructure --startup-project .
   ```

## Comandos úteis

### Ver migrations pendentes
```bash
dotnet ef migrations list --project ../UptimeChangeMonitor.Infrastructure --startup-project .
```

### Remover última migration (se não foi aplicada)
```bash
dotnet ef migrations remove --project ../UptimeChangeMonitor.Infrastructure --startup-project .
```

### Reverter para uma migration específica
```bash
dotnet ef database update NomeDaMigration --project ../UptimeChangeMonitor.Infrastructure --startup-project .
```

## Estrutura do Banco de Dados

Após aplicar a migration, você terá as seguintes tabelas:

- **Monitors** - Tabela principal de monitores
- **UptimeChecks** - Histórico de verificações de uptime
- **ChangeDetections** - Histórico de detecções de mudança

## Configuração da Connection String

A connection string está configurada no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=UptimeChangeMonitor;Username=postgres;Password=postgres"
  }
}
```

Certifique-se de que ela corresponde às configurações do seu Docker Compose.
