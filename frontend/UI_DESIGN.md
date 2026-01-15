# Design da UI - Uptime Change Monitor

## Paleta de Cores

A interface utiliza uma paleta moderna e profissional com as seguintes cores:

### Cores Principais
- **Dark Background**: `#0f172a` (dark-900) - Fundo principal
- **Dark Cards**: `#1e293b` (dark-800) - Cards e elementos
- **Primary Blue**: `#3b82f6` (primary-500) - Acentos e links
- **Success Green**: `#22c55e` (success-500) - Status online
- **Danger Red**: `#ef4444` (danger-500) - Status offline/erro
- **Warning Orange**: `#f59e0b` (warning-500) - Avisos e timeouts

### Efeitos Visuais
- **Gradientes**: Utilizados em textos e backgrounds para profundidade
- **Glow Effects**: Efeitos de brilho sutis em cards de status
- **Backdrop Blur**: Header com efeito de vidro fosco

## 📐 Estrutura da Página

A página principal é uma única página com rolagem contendo:

1. **Header Fixo** (Sticky)
   - Logo e título
   - Indicador de status do sistema
   - Design minimalista

2. **Hero Section**
   - Título principal com gradiente
   - Descrição do sistema
   - Cards de estatísticas gerais (4 cards)

3. **Seção de Monitores**
   - Grid responsivo de cards de monitores
   - Cada card mostra:
     - Nome e URL
     - Status visual com ícone
     - Métricas (tempo de resposta, uptime, mudanças)
     - Última verificação

4. **Seção de Gráficos**
   - Gráfico de Uptime (Area Chart)
   - Gráfico de Tempo de Resposta (Line Chart)
   - Layout em 2 colunas (desktop)

5. **Estatísticas Adicionais**
   - Card com métricas consolidadas

6. **Footer**
   - Informações de copyright

## Características de Design

### Responsividade
- Mobile-first approach
- Grid adaptativo (1 coluna mobile, 2 desktop)
- Breakpoints do Tailwind

### Interatividade
- Hover effects nos cards
- Transições suaves
- Estados visuais claros (online/offline)

### Acessibilidade
- Contraste adequado
- Ícones descritivos
- Textos legíveis

## Componentes Criados

1. **Header** - Cabeçalho fixo com logo e status
2. **Card** - Componente base para cards com efeitos de glow
3. **MonitorCard** - Card específico para monitores com todas as métricas
4. **UptimeChart** - Gráfico de área para histórico de uptime
5. **ResponseTimeChart** - Gráfico de linha para tempo de resposta

## 📱 Visual para LinkedIn

A UI foi projetada para ser:
- **Profissional**: Cores e tipografia modernas
- **Impactante**: Gradientes e efeitos sutis chamam atenção
- **Limpa**: Espaçamento adequado e organização clara
- **Informativa**: Todas as informações importantes visíveis

## Próximos Passos

1. Integração com API (endpoints do backend)
2. Atualização em tempo real (WebSockets ou polling)
3. Filtros e busca
4. Detalhes expandidos por monitor
