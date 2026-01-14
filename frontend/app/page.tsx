'use client'

import Header from '@/components/Layout/Header'
import MonitorCard from '@/components/Monitor/MonitorCard'
import UptimeChart from '@/components/Charts/UptimeChart'
import ResponseTimeChart from '@/components/Charts/ResponseTimeChart'
import Card from '@/components/Common/Card'

// Mock data - será substituído pela integração com API
const mockMonitors = [
  {
    id: '1',
    name: 'API Principal',
    url: 'https://api.example.com',
    status: 'online' as const,
    lastChecked: 'há 2 minutos',
    responseTime: 145,
    uptime: 99.8,
    changes: 3
  },
  {
    id: '2',
    name: 'Dashboard Web',
    url: 'https://dashboard.example.com',
    status: 'online' as const,
    lastChecked: 'há 1 minuto',
    responseTime: 89,
    uptime: 99.9,
    changes: 1
  },
  {
    id: '3',
    name: 'Serviço de Autenticação',
    url: 'https://auth.example.com',
    status: 'offline' as const,
    lastChecked: 'há 5 minutos',
    responseTime: null,
    uptime: 95.2,
    changes: 0
  },
  {
    id: '4',
    name: 'API de Pagamentos',
    url: 'https://payments.example.com',
    status: 'online' as const,
    lastChecked: 'há 30 segundos',
    responseTime: 234,
    uptime: 99.5,
    changes: 5
  }
]

const mockUptimeData = [
  { date: '00:00', online: 4, offline: 0, uptime: 100 },
  { date: '04:00', online: 4, offline: 0, uptime: 100 },
  { date: '08:00', online: 3, offline: 1, uptime: 75 },
  { date: '12:00', online: 4, offline: 0, uptime: 100 },
  { date: '16:00', online: 4, offline: 0, uptime: 100 },
  { date: '20:00', online: 4, offline: 0, uptime: 100 },
  { date: '24:00', online: 4, offline: 0, uptime: 100 },
]

const mockResponseTimeData = [
  { date: '00:00', responseTime: 120 },
  { date: '04:00', responseTime: 135 },
  { date: '08:00', responseTime: 98 },
  { date: '12:00', responseTime: 145 },
  { date: '16:00', responseTime: 112 },
  { date: '20:00', responseTime: 128 },
  { date: '24:00', responseTime: 115 },
]

export default function HomePage() {
  const totalMonitors = mockMonitors.length
  const onlineMonitors = mockMonitors.filter(m => m.status === 'online').length
  const offlineMonitors = mockMonitors.filter(m => m.status === 'offline').length
  const avgUptime = mockMonitors.reduce((acc, m) => acc + (m.uptime || 0), 0) / totalMonitors
  const avgResponseTime = mockMonitors
    .filter(m => m.responseTime)
    .reduce((acc, m) => acc + (m.responseTime || 0), 0) / onlineMonitors

  return (
    <div className="min-h-screen bg-dark-900">
      <Header />
      
      <main className="container mx-auto px-4 py-8">
        {/* Hero Section */}
        <section className="mb-12">
          <div className="text-center mb-8">
            <h2 className="text-4xl md:text-5xl font-bold mb-4">
              <span className="text-gradient">Monitoramento em Tempo Real</span>
            </h2>
            <p className="text-lg text-dark-300 max-w-2xl mx-auto">
              Acompanhe o status, performance e mudanças dos seus serviços com precisão e confiabilidade
            </p>
          </div>

          {/* Stats Cards */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
            <Card glow="primary">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-dark-400 mb-1">Total de Monitores</p>
                  <p className="text-3xl font-bold text-gray-100">{totalMonitors}</p>
                </div>
                <div className="w-12 h-12 rounded-lg bg-primary-500/20 flex items-center justify-center">
                  <svg className="w-6 h-6 text-primary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                </div>
              </div>
            </Card>

            <Card glow="success">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-dark-400 mb-1">Online</p>
                  <p className="text-3xl font-bold text-success-400">{onlineMonitors}</p>
                </div>
                <div className="w-12 h-12 rounded-lg bg-success-500/20 flex items-center justify-center">
                  <svg className="w-6 h-6 text-success-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
              </div>
            </Card>

            <Card glow={offlineMonitors > 0 ? 'danger' : 'none'}>
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-dark-400 mb-1">Offline</p>
                  <p className="text-3xl font-bold text-danger-400">{offlineMonitors}</p>
                </div>
                <div className="w-12 h-12 rounded-lg bg-danger-500/20 flex items-center justify-center">
                  <svg className="w-6 h-6 text-danger-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
              </div>
            </Card>

            <Card>
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-dark-400 mb-1">Uptime Médio</p>
                  <p className="text-3xl font-bold text-gray-100">{avgUptime.toFixed(1)}%</p>
                </div>
                <div className="w-12 h-12 rounded-lg bg-primary-500/20 flex items-center justify-center">
                  <svg className="w-6 h-6 text-primary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                  </svg>
                </div>
              </div>
            </Card>
          </div>
        </section>

        {/* Monitors Section */}
        <section className="mb-12">
          <div className="mb-6">
            <h3 className="text-2xl font-bold text-gray-100 mb-2">Monitores Ativos</h3>
            <p className="text-dark-400">Acompanhe o status de todos os seus serviços</p>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {mockMonitors.map((monitor) => (
              <MonitorCard
                key={monitor.id}
                name={monitor.name}
                url={monitor.url}
                status={monitor.status}
                lastChecked={monitor.lastChecked}
                responseTime={monitor.responseTime || undefined}
                uptime={monitor.uptime}
                changes={monitor.changes}
              />
            ))}
          </div>
        </section>

        {/* Charts Section */}
        <section className="mb-12">
          <div className="mb-6">
            <h3 className="text-2xl font-bold text-gray-100 mb-2">Análise e Métricas</h3>
            <p className="text-dark-400">Visualize tendências e performance histórica</p>
          </div>
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <UptimeChart data={mockUptimeData} />
            <ResponseTimeChart data={mockResponseTimeData} />
          </div>
        </section>

        {/* Additional Stats */}
        <section className="mb-12">
          <Card>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div className="text-center">
                <p className="text-sm text-dark-400 mb-2">Tempo Médio de Resposta</p>
                <p className="text-2xl font-bold text-primary-400">{Math.round(avgResponseTime)}ms</p>
              </div>
              <div className="text-center border-l border-r border-dark-700">
                <p className="text-sm text-dark-400 mb-2">Total de Verificações</p>
                <p className="text-2xl font-bold text-gray-100">1,247</p>
              </div>
              <div className="text-center">
                <p className="text-sm text-dark-400 mb-2">Mudanças Detectadas</p>
                <p className="text-2xl font-bold text-warning-400">
                  {mockMonitors.reduce((acc, m) => acc + m.changes, 0)}
                </p>
              </div>
            </div>
          </Card>
        </section>
      </main>

      {/* Footer */}
      <footer className="border-t border-dark-700 mt-12">
        <div className="container mx-auto px-4 py-6">
          <div className="text-center text-sm text-dark-400">
            <p>Uptime Change Monitor © {new Date().getFullYear()} - Monitoramento Inteligente</p>
          </div>
        </div>
      </footer>
    </div>
  )
}
