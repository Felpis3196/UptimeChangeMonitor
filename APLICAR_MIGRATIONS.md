# Como Aplicar Migrations

## Erro: "relation 'Monitors' does not exist"

Este erro significa que as **migrations não foram aplicadas** no banco de dados. 

## Container Exclusivo de Migrations

O projeto agora possui um **container exclusivo** para aplicar migrations automaticamente. Ele roda antes da API iniciar e garante que o banco esteja atualizado.

### Como funciona:

1. **Container `migrations`**: Executa as migrations e termina
2. **Container `api`**: Só inicia após as migrations serem aplicadas com sucesso
3. **Separação de responsabilidades**: API usa imagem runtime leve (aspnet), migrations usa SDK completo

### Executar migrations manualmente:

```bash
# Aplicar migrations via container dedicado
docker-compose up migrations

# Ou forçar rebuild e aplicar
docker-compose up --build migrations
```

## Opção 1: Aplicar via Docker (Recomendado)

### Passo 1: Criar a migration (se ainda não criou)

```bash
cd src/UptimeChangeMonitor.API
dotnet ef migrations add InitialCreate --project ../UptimeChangeMonitor.Infrastructure --startup-project .
```

### Passo 2: Aplicar migrations no container

**Método A - Entrar no container:**
```bash
# Entre no container da API
docker exec -it uptime_api bash

# Instale dotnet-ef (se necessário)
dotnet tool install --global dotnet-ef

# Execute as migrations
cd /src/src/UptimeChangeMonitor.API
dotnet ef database update --project ../UptimeChangeMonitor.Infrastructure --startup-project . --no-build
```

**Método B - Executar comando direto:**
```bash
docker exec -it uptime_api dotnet tool install --global dotnet-ef
docker exec -it uptime_api bash -c "cd /src/src/UptimeChangeMonitor.API && dotnet ef database update --project ../UptimeChangeMonitor.Infrastructure --startup-project . --no-build"
```

## Opção 2: Aplicar Localmente (Desenvolvimento)

### Pré-requisitos
- PostgreSQL rodando (via Docker ou local)
- .NET 8 SDK instalado

### Passos

1. **Inicie o PostgreSQL:**
   ```bash
   docker-compose up -d postgres
   ```

2. **Crie a migration (se ainda não criou):**
   ```bash
   cd src/UptimeChangeMonitor.API
   dotnet ef migrations add InitialCreate --project ../UptimeChangeMonitor.Infrastructure --startup-project .
   ```

3. **Aplique a migration:**
   ```bash
   dotnet ef database update --project ../UptimeChangeMonitor.Infrastructure --startup-project .
   ```

## Verificar se as migrations foram aplicadas

```bash
# Conecte ao PostgreSQL
docker exec -it uptime_postgres psql -U postgres -d UptimeChangeMonitor

# Liste as tabelas
\dt

# Você deve ver:
# - Monitors
# - UptimeChecks
# - ChangeDetections
# - __EFMigrationsHistory
```

## Após aplicar as migrations

A API deve funcionar normalmente:
- Acesse: `http://localhost:5000/swagger`
- Teste criar um monitor via Swagger
- Verifique se os dados são salvos no banco

## Troubleshooting

### Erro: "dotnet ef not found"
```bash
dotnet tool install --global dotnet-ef
```

### Erro: "Connection refused"
- Verifique se o PostgreSQL está rodando: `docker-compose ps postgres`
- Verifique a connection string no `appsettings.json`

### Erro: "Migration already applied"
- Isso é normal se você já aplicou antes
- Para recriar: `dotnet ef database drop` e depois `dotnet ef database update`
