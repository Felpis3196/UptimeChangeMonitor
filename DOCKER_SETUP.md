# Guia de Setup Docker

## Pré-requisitos

- Docker Desktop instalado e rodando
- .NET 8 SDK (para desenvolvimento local, opcional)

## Como executar

### 1. Build e Start dos containers

```bash
docker-compose up -d --build
```

Isso irá:
- Construir as imagens da API e Workers
- Subir PostgreSQL
- Subir RabbitMQ
- Subir a API
- Subir os Workers

### 2. Aplicar migrations no banco

Primeiro, entre no container da API:

```bash
docker exec -it uptime_api bash
```

Dentro do container, execute:

```bash
dotnet ef database update --project /src/src/UptimeChangeMonitor.Infrastructure --startup-project /src/src/UptimeChangeMonitor.API
```

Ou, se preferir fazer localmente (com .NET SDK instalado):

```bash
cd src/UptimeChangeMonitor.API
dotnet ef database update --project ../UptimeChangeMonitor.Infrastructure --startup-project .
```

### 3. Verificar se está tudo rodando

```bash
docker-compose ps
```

Você deve ver:
- `uptime_postgres` - Running
- `uptime_rabbitmq` - Running
- `uptime_api` - Running
- `uptime_workers` - Running

### 4. Acessar os serviços

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

### 5. Ver logs

```bash
# Todos os serviços
docker-compose logs -f

# Apenas API
docker-compose logs -f api

# Apenas Workers
docker-compose logs -f workers
```

## Estrutura dos Containers

```
┌─────────────────┐
│   API (5000)    │
└────────┬────────┘
         │
    ┌────┴────┐
    │         │
┌───▼───┐ ┌──▼──────┐
│Postgres│ │RabbitMQ │
└────────┘ └────┬───┘
                │
         ┌──────▼──────┐
         │   Workers   │
         └─────────────┘
```

## Troubleshooting

### Container não inicia

```bash
# Ver logs detalhados
docker-compose logs api
docker-compose logs workers

# Rebuild forçado
docker-compose up -d --build --force-recreate
```

### Erro de conexão com banco

Verifique se o PostgreSQL está saudável:

```bash
docker-compose ps postgres
docker-compose logs postgres
```

### Erro de conexão com RabbitMQ

Verifique se o RabbitMQ está saudável:

```bash
docker-compose ps rabbitmq
docker-compose logs rabbitmq
```

### Workers não processam mensagens

1. Verifique os logs dos workers:
```bash
docker-compose logs workers
```

2. Verifique se as filas existem no RabbitMQ:
   - Acesse http://localhost:15672
   - Login: guest / guest
   - Vá em "Queues"
   - Deve ver: `uptime_check_queue` e `change_detection_queue`

## Parar os containers

```bash
docker-compose down
```

## Parar e remover volumes (limpar tudo)

```bash
docker-compose down -v
```

## Rebuild completo

```bash
docker-compose down -v
docker-compose up -d --build
```

## Variáveis de Ambiente

As variáveis estão configuradas no `docker-compose.yml`. Para produção, considere usar um arquivo `.env` ou secrets do Docker.

## Próximos Passos

1. Criar um monitor via API (POST /api/monitors)
2. Verificar se o job foi publicado no RabbitMQ
3. Verificar se o worker processou e salvou no banco
4. Consultar o status (GET /api/monitors/{id}/status)
