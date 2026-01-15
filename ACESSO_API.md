# Guia de Acesso à API

## URLs de Acesso

### Execução Local (dotnet run)

- **API Base**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`
- **Swagger JSON**: `http://localhost:5000/swagger/v1.0/swagger.json`

### Execução via Docker

- **API Base**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`
- **Swagger JSON**: `http://localhost:5000/swagger/v1.0/swagger.json`

## Endpoints Principais (v1)

### Monitores
- `GET /api/v1/monitors` - Lista todos os monitores
- `GET /api/v1/monitors/{id}` - Busca monitor por ID
- `POST /api/v1/monitors` - Cria novo monitor
- `PUT /api/v1/monitors/{id}` - Atualiza monitor
- `DELETE /api/v1/monitors/{id}` - Deleta monitor
- `GET /api/v1/monitors/{id}/status` - Status do monitor
- `GET /api/v1/monitors/status` - Status de todos os monitores

### Uptime Checks
- `GET /api/v1/monitors/{monitorId}/uptimechecks` - Histórico de verificações
- `GET /api/v1/monitors/{monitorId}/uptimechecks/latest` - Última verificação

### Change Detections
- `GET /api/v1/monitors/{monitorId}/changedetections` - Histórico de mudanças
- `GET /api/v1/monitors/{monitorId}/changedetections/latest` - Última detecção

## Testando no Swagger

1. Acesse `http://localhost:5000/swagger`
2. Selecione a versão no dropdown (V1.0)
3. Expanda um endpoint
4. Clique em "Try it out"
5. Preencha os parâmetros (se necessário)
6. Clique em "Execute"
7. Veja a resposta

## Exemplo de Requisição

### Criar Monitor

```http
POST http://localhost:5000/api/v1/monitors
Content-Type: application/json

{
  "name": "Google",
  "url": "https://www.google.com",
  "checkIntervalSeconds": 60,
  "monitorUptime": true,
  "monitorChanges": false
}
```

### Listar Monitores

```http
GET http://localhost:5000/api/v1/monitors
```

## Troubleshooting

### Swagger não abre

1. Verifique se a API está rodando:
   ```bash
   docker-compose ps api
   ```

2. Verifique os logs:
   ```bash
   docker-compose logs api
   ```

3. Teste se a API responde:
   ```bash
   curl http://localhost:5000/swagger
   ```

### Erro 404 no Swagger

- Verifique se o Swagger está configurado no `Program.cs`
- Verifique se `app.UseSwagger()` está antes de `app.UseAuthorization()`

### Porta já em uso

- Altere a porta no `docker-compose.yml`:
  ```yaml
  ports:
    - "5002:80"  # Use outra porta
  ```

## Segurança

**Nota**: Em produção, considere:
- Proteger o Swagger com autenticação
- Desabilitar o Swagger em produção
- Usar HTTPS
