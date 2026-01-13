# 📌 Guia de Versionamento da API

## 🎯 Estrutura de Versionamento

A API utiliza **URL Versioning** com suporte a múltiplas formas de especificar a versão:

### Formas de Especificar Versão

1. **URL Path** (Recomendado):
   ```
   GET /api/v1/monitors
   GET /api/v2/monitors
   ```

2. **Query String**:
   ```
   GET /api/monitors?version=1.0
   ```

3. **Header**:
   ```
   X-Version: 1.0
   ```

## 📁 Estrutura de Pastas

```
Controllers/
├── V1/
│   ├── MonitorsController.cs
│   ├── UptimeChecksController.cs
│   └── ChangeDetectionsController.cs
└── V2/ (futuro)
    └── ...
```

## 🔄 Como Adicionar Nova Versão

### 1. Criar pasta da nova versão

```bash
mkdir src/UptimeChangeMonitor.API/Controllers/V2
```

### 2. Criar controllers na nova versão

Copie os controllers da V1 e ajuste:
- Namespace: `UptimeChangeMonitor.API.Controllers.V2`
- `[ApiVersion("2.0")]`
- `[Route("api/v{version:apiVersion}/[controller]")]`

### 3. Implementar mudanças

Faça as alterações necessárias na nova versão mantendo compatibilidade com a anterior.

## 📝 Exemplo de Uso

### Criar Monitor (v1)
```http
POST /api/v1/monitors
Content-Type: application/json

{
  "name": "Google",
  "url": "https://www.google.com",
  "checkIntervalSeconds": 60,
  "monitorUptime": true,
  "monitorChanges": false
}
```

### Listar Monitores (v1)
```http
GET /api/v1/monitors
```

### Status do Monitor (v1)
```http
GET /api/v1/monitors/{id}/status
```

## 🔧 Configuração

O versionamento está configurado no `Program.cs`:

- **Versão padrão**: v1.0
- **Assumir versão padrão**: Sim (se não especificar, usa v1.0)
- **Reportar versões**: Sim (header `api-supported-versions`)

## 📊 Swagger

O Swagger mostra todas as versões disponíveis:
- Acesse: `http://localhost:5000/swagger`
- Selecione a versão no dropdown no topo

## ✅ Boas Práticas

1. **Mantenha compatibilidade**: Versões antigas devem continuar funcionando
2. **Documente mudanças**: Use comentários XML para documentar diferenças
3. **Deprecie gradualmente**: Marque versões antigas como deprecated antes de remover
4. **Teste todas as versões**: Garanta que todas as versões funcionam

## 🚀 Próximos Passos

Quando precisar criar a V2:
1. Criar pasta `Controllers/V2`
2. Copiar controllers da V1
3. Ajustar namespace e versão
4. Implementar mudanças
5. Testar ambas as versões
