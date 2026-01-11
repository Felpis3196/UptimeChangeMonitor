# 📁 Guia da Estrutura do Frontend

Este documento explica onde cada componente deve ser implementado no frontend.

## 🎯 Estrutura de Pastas Detalhada

### 1. **app/** (Next.js App Router)
Páginas e rotas da aplicação.

#### `app/`
- **layout.tsx** - Layout raiz da aplicação
  - Providers (React Query, Zustand, etc)
  - Fontes globais
  - Metadata

- **page.tsx** - Página inicial (Dashboard)
  - Visão geral de todos os monitores
  - Resumo estatístico
  - Cards de status geral

- **globals.css** - Estilos globais
  - Reset CSS
  - Tailwind directives
  - Variáveis CSS

#### `app/monitors/`
- **page.tsx** - Lista de monitores
  - Lista todos os monitores cadastrados
  - Filtros e busca
  - Ações (criar, editar, deletar)

#### `app/monitors/[id]/`
- **page.tsx** - Detalhes do monitor
  - Informações do monitor
  - Status atual
  - Histórico de verificações
  - Gráficos de uptime e tempo de resposta

---

### 2. **components/** (Componentes React)

#### `components/Layout/`
- **Header/**
  - `Header.tsx` - Cabeçalho da aplicação
  - Logo, navegação, ações do usuário

- **Sidebar/**
  - `Sidebar.tsx` - Menu lateral (opcional)
  - Links de navegação

- **Footer/**
  - `Footer.tsx` - Rodapé
  - Informações da aplicação

#### `components/Monitor/`
- **MonitorCard/**
  - `MonitorCard.tsx` - Card exibindo monitor
  - Status visual, nome, URL, última verificação
  - Ações rápidas

- **MonitorList/**
  - `MonitorList.tsx` - Lista de monitores
  - Renderiza múltiplos MonitorCard
  - Paginação/infinite scroll

- **MonitorForm/**
  - `MonitorForm.tsx` - Formulário de criação/edição
  - Inputs: nome, URL, intervalo
  - Validação

- **MonitorStatus/**
  - `MonitorStatus.tsx` - Badge/indicador de status
  - Online/Offline/Checking
  - Cores e ícones

#### `components/Charts/`
- **UptimeChart/**
  - `UptimeChart.tsx` - Gráfico de uptime
  - Recharts (LineChart ou AreaChart)
  - Timeline de disponibilidade

- **ResponseTimeChart/**
  - `ResponseTimeChart.tsx` - Gráfico de tempo de resposta
  - Recharts (LineChart)
  - Timeline de performance

#### `components/Common/`
- **Button/**
  - `Button.tsx` - Botão reutilizável
  - Variantes (primary, secondary, danger)
  - Tamanhos, estados (loading, disabled)

- **Card/**
  - `Card.tsx` - Card reutilizável
  - Header, body, footer
  - Padding, shadow

- **Loading/**
  - `Loading.tsx` - Componente de loading
  - Spinner, skeleton

- **Error/**
  - `Error.tsx` - Componente de erro
  - Mensagem de erro, botão de retry

---

### 3. **lib/** (Bibliotecas e Utilitários)

#### `lib/api/`
- **client.ts**
  - Configuração do Axios
  - Interceptors (auth, errors)
  - Base URL

- **monitors/**
  - `monitorsApi.ts` - Endpoints de monitores
    - `getMonitors()` - Lista monitores
    - `getMonitor(id)` - Busca monitor
    - `createMonitor(data)` - Cria monitor
    - `updateMonitor(id, data)` - Atualiza monitor
    - `deleteMonitor(id)` - Deleta monitor

- **uptime/**
  - `uptimeApi.ts` - Endpoints de uptime
    - `getUptimeHistory(monitorId)` - Histórico de uptime
    - `getUptimeStatus(monitorId)` - Status atual

- **changes/**
  - `changesApi.ts` - Endpoints de mudanças
    - `getChangeHistory(monitorId)` - Histórico de mudanças
    - `getLastChange(monitorId)` - Última mudança detectada

#### `lib/store/`
- **monitorStore.ts** (Zustand)
  - Estado global de monitores
  - Ações: fetch, create, update, delete
  - Cache local

---

### 4. **hooks/** (Custom React Hooks)

- **useMonitors.ts**
  - Hook para buscar monitores (React Query)
  - Cache, refetch, loading states

- **useMonitor.ts** (id)
  - Hook para buscar monitor específico
  - Dados, histórico, status

- **useUptimeHistory.ts** (monitorId)
  - Hook para histórico de uptime
  - Dados formatados para gráficos

- **useChangeHistory.ts** (monitorId)
  - Hook para histórico de mudanças

- **useCreateMonitor.ts**
  - Hook para criar monitor
  - Mutation (React Query)
  - Otimistic updates

- **useUpdateMonitor.ts**
  - Hook para atualizar monitor

- **useDeleteMonitor.ts**
  - Hook para deletar monitor

---

### 5. **types/** (TypeScript Types)

#### `types/index.ts`
- **Monitor** - Interface do monitor
- **UptimeCheck** - Interface do check de uptime
- **ChangeDetection** - Interface da detecção de mudança
- **MonitorStatus** - Enum de status
- **ApiResponse** - Tipo genérico de resposta da API
- **CreateMonitorDto** - DTO de criação
- **UpdateMonitorDto** - DTO de atualização

---

### 6. **utils/** (Funções Utilitárias)

- **formatDate.ts**
  - Formatação de datas (date-fns)
  - Relative time, format padrão

- **formatDuration.ts**
  - Formatação de duração
  - Milissegundos para formato legível

- **validateUrl.ts**
  - Validação de URL
  - Regex, validação customizada

- **constants.ts**
  - Constantes da aplicação
  - Status colors, intervals, etc

---

### 7. **public/** (Arquivos Estáticos)

- **images/** - Imagens
- **icons/** - Ícones SVG
- **favicon.ico** - Favicon
- **robots.txt** - Robots.txt

---

## 🔄 Fluxo de Dados

```
Component (UI)
    ↓ chama
Hook (useMonitors, useMonitor, etc)
    ↓ usa
API Service (monitorsApi, uptimeApi, etc)
    ↓ faz request
Axios Client (client.ts)
    ↓ envia
Backend API
```

---

## 📦 Dependências Principais

### Core
- **next** - Framework React
- **react** - Biblioteca React
- **typescript** - Tipagem estática

### UI & Styling
- **tailwindcss** - CSS utility-first
- **recharts** - Gráficos

### Data Fetching & State
- **axios** - Cliente HTTP
- **react-query** - Server state management
- **zustand** - Client state management

### Utils
- **date-fns** - Manipulação de datas

---

## ✅ Checklist de Implementação

### Setup Inicial
- [ ] Configurar Next.js
- [ ] Configurar TypeScript
- [ ] Configurar Tailwind CSS
- [ ] Configurar ESLint
- [ ] Configurar path aliases (@/*)

### Tipos
- [ ] Criar interfaces TypeScript
- [ ] Tipar respostas da API
- [ ] Tipar componentes

### API Layer
- [ ] Configurar Axios client
- [ ] Criar serviços de API
- [ ] Implementar endpoints

### Components
- [ ] Criar componentes de Layout
- [ ] Criar componentes de Monitor
- [ ] Criar componentes de Charts
- [ ] Criar componentes Common

### Hooks
- [ ] Criar hooks de data fetching
- [ ] Criar hooks de mutations
- [ ] Configurar React Query

### Pages
- [ ] Implementar página inicial
- [ ] Implementar lista de monitores
- [ ] Implementar detalhes do monitor

### Styling
- [ ] Estilizar componentes
- [ ] Criar tema/cores
- [ ] Responsividade

### Features
- [ ] Integrar com API
- [ ] Implementar gráficos
- [ ] Implementar filtros/busca
- [ ] Implementar ações CRUD

---

## 🚀 Próximos Passos

1. Configurar projeto Next.js
2. Configurar TypeScript e paths
3. Criar tipos/interfaces
4. Configurar Axios e serviços de API
5. Criar componentes base
6. Implementar páginas
7. Integrar com backend
8. Adicionar gráficos
9. Testar e refinar
