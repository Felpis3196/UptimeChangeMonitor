# Uptime & Change Monitor - Frontend

Dashboard em tempo real para monitoramento de sites e detecção de mudanças.

## Tecnologias

- **Next.js 14** - Framework React com App Router
- **TypeScript** - Tipagem estática
- **Tailwind CSS** - Estilização
- **Recharts** - Gráficos e visualizações
- **React Query** - Gerenciamento de estado do servidor
- **Zustand** - Gerenciamento de estado local
- **Axios** - Cliente HTTP

## Estrutura do Projeto

```
frontend/
├── app/                          # Next.js App Router
│   ├── layout.tsx               # Layout raiz
│   ├── page.tsx                 # Página inicial
│   ├── globals.css              # Estilos globais
│   └── monitors/
│       ├── page.tsx             # Lista de monitores
│       └── [id]/
│           └── page.tsx         # Detalhes do monitor
│
├── components/                   # Componentes React
│   ├── Layout/
│   │   ├── Header/              # Cabeçalho
│   │   ├── Sidebar/             # Barra lateral
│   │   └── Footer/              # Rodapé
│   ├── Monitor/
│   │   ├── MonitorCard/         # Card do monitor
│   │   ├── MonitorList/         # Lista de monitores
│   │   ├── MonitorForm/         # Formulário de criação/edição
│   │   └── MonitorStatus/       # Indicador de status
│   ├── Charts/
│   │   ├── UptimeChart/         # Gráfico de uptime
│   │   └── ResponseTimeChart/   # Gráfico de tempo de resposta
│   └── Common/
│       ├── Button/              # Botão reutilizável
│       ├── Card/                # Card reutilizável
│       ├── Loading/             # Componente de loading
│       └── Error/               # Componente de erro
│
├── lib/                          # Bibliotecas e utilitários
│   ├── api/
│   │   ├── client.ts            # Cliente Axios configurado
│   │   ├── monitors/            # Endpoints de monitores
│   │   ├── uptime/              # Endpoints de uptime
│   │   └── changes/             # Endpoints de mudanças
│   └── store/
│       └── (zustand stores)     # Stores do Zustand
│
├── hooks/                        # Custom React Hooks
│   └── (custom hooks)           # Hooks reutilizáveis
│
├── types/                        # Definições TypeScript
│   └── index.ts                 # Tipos compartilhados
│
├── utils/                        # Funções utilitárias
│   └── (utility functions)      # Helpers e utilitários
│
├── public/                       # Arquivos estáticos
│   └── (imagens, ícones, etc)
│
└── styles/                       # Estilos adicionais
    └── (estilos customizados)
```

## Scripts

```bash
# Desenvolvimento
npm run dev

# Build de produção
npm run build

# Iniciar produção
npm start

# Linting
npm run lint

# Type checking
npm run type-check
```

## Variáveis de Ambiente

Crie um arquivo `.env.local`:

```env
NEXT_PUBLIC_API_URL=http://localhost:5000
```

## Instalação

```bash
npm install
```

## Funcionalidades (MVP)

- Lista de monitores
- Status em tempo real (Online/Offline)
- Detalhes do monitor
- Histórico de verificações
- Gráficos de uptime e tempo de resposta
- Dashboard com resumo geral

## 📱 Rotas

- `/` - Dashboard/Home
- `/monitors` - Lista de monitores
- `/monitors/[id]` - Detalhes do monitor específico

## Componentes Principais

### Layout
- **Header** - Cabeçalho com navegação
- **Sidebar** - Menu lateral (opcional)
- **Footer** - Rodapé

### Monitor
- **MonitorCard** - Card exibindo status e informações básicas
- **MonitorList** - Lista de todos os monitores
- **MonitorForm** - Formulário para criar/editar monitor
- **MonitorStatus** - Badge/indicador de status

### Charts
- **UptimeChart** - Gráfico de disponibilidade ao longo do tempo
- **ResponseTimeChart** - Gráfico de tempo de resposta

### Common
- Componentes reutilizáveis (Button, Card, Loading, Error)

## Integração com API

O frontend consome a API REST do backend:

- `GET /api/monitors` - Lista monitores
- `GET /api/monitors/{id}` - Detalhes do monitor
- `POST /api/monitors` - Criar monitor
- `PUT /api/monitors/{id}` - Atualizar monitor
- `DELETE /api/monitors/{id}` - Deletar monitor
- `GET /api/monitors/{id}/status` - Status atual
- `GET /api/monitors/{id}/history` - Histórico

## Próximas Evoluções

- Alertas em tempo real (WebSocket)
- Filtros e busca avançada
- Exportação de relatórios
- Dark mode
- Notificações push
- Multi-tenant (quando backend suportar)
