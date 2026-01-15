# Front-end no Docker

O front-end foi integrado ao Docker Compose e pode ser executado junto com todos os serviços.

## Estrutura

- **Dockerfile**: Localizado em `frontend/Dockerfile`
- **Serviço no docker-compose.yml**: `frontend`
- **Porta**: 3000 (http://localhost:3000)

## Como usar

### Subir todos os serviços (incluindo front-end)

```bash
docker-compose up -d --build
```

Isso irá:
1. Construir e subir PostgreSQL
2. Construir e subir RabbitMQ
3. Executar migrations
4. Construir e subir a API (porta 5000)
5. Construir e subir Workers
6. Construir e subir Front-end (porta 3000)

### Acessar o front-end

Após todos os serviços estarem rodando:
- Front-end: http://localhost:3000
- API: http://localhost:5000
- RabbitMQ Management: http://localhost:15672 (guest/guest)

### Reconstruir apenas o front-end

```bash
docker-compose build frontend
docker-compose up -d frontend
```

### Ver logs do front-end

```bash
docker-compose logs -f frontend
```

## Configuração

O front-end está configurado para:
- **Modo Standalone**: Next.js compilado em modo standalone para produção
- **API URL**: Configurada via `NEXT_PUBLIC_API_URL` (padrão: http://localhost:5000)
- **Build otimizado**: Multi-stage build para reduzir tamanho da imagem

## Variáveis de Ambiente

O front-end usa a variável `NEXT_PUBLIC_API_URL` para se conectar à API:
- No Docker: `http://localhost:5000` (acessível pelo navegador)
- Em desenvolvimento local: `http://localhost:5000`

**Nota**: Como o front-end roda no navegador do usuário, ele precisa acessar a API através do host do usuário (localhost), não através do nome do serviço Docker.

## Troubleshooting

### Front-end não consegue acessar a API

1. Verifique se a API está rodando: `docker-compose ps`
2. Teste a API diretamente: `curl http://localhost:5000/api/v1/monitors/status`
3. Verifique os logs: `docker-compose logs frontend`

### Erro de build do front-end

1. Limpe o cache: `docker-compose build --no-cache frontend`
2. Verifique se o `package.json` está correto
3. Verifique os logs de build: `docker-compose build frontend`

### Porta 3000 já em uso

Altere a porta no `docker-compose.yml`:
```yaml
ports:
  - "3001:3000"  # Usa 3001 no host
```
