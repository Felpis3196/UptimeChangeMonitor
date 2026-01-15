'use client'

import { useRouter } from 'next/navigation'
import Header from '@/components/Layout/Header'
import MonitorCard from '@/components/Monitor/MonitorCard'
import UptimeChart from '@/components/Charts/UptimeChart'
import ResponseTimeChart from '@/components/Charts/ResponseTimeChart'
import Card from '@/components/Common/Card'
import Loading from '@/components/Common/Loading'
import Error from '@/components/Common/Error'
import { Button } from '@/components/Common/Button'
import { useMonitorsStatus } from '@/hooks/useMonitors'
import { useUptimeHistory } from '@/hooks/useUptimeChecks'
import { UptimeStatus } from '@/types'
import { getErrorMessage } from '@/utils/error'

function getStatusFromUptime(isOnline?: boolean, status?: number): 'online' | 'offline' | 'timeout' | 'error' {
  if (isOnline === true) return 'online'
  if (isOnline === false) return 'offline'
  
  if (status === UptimeStatus.Online) return 'online'
  if (status === UptimeStatus.Offline) return 'offline'
  if (status === UptimeStatus.Timeout) return 'timeout'
  if (status === UptimeStatus.Error) return 'error'
  
  return 'offline'
}

export default function HomePage() {
  const router = useRouter()
  const { data: statusData, isLoading, isError, error, refetch } = useMonitorsStatus()
  
  // Hooks must be called before any early returns
  const summary = statusData?.summary
  const monitors = statusData?.monitors || []
  const firstMonitor = monitors[0]
  const { data: uptimeData } = useUptimeHistory(firstMonitor ? firstMonitor.MonitorId : null, 100)

  if (isLoading) {
    return (
      <div className="min-h-screen bg-dark-900">
        <Header />
        <main className="container mx-auto px-4 py-8">
          <Loading />
        </main>
      </div>
    )
  }

  if (isError) {
    const errorMessage = getErrorMessage(error as unknown)
    return (
      <div className="min-h-screen bg-dark-900">
        <Header />
        <main className="container mx-auto px-4 py-8">
          <Error 
            message={errorMessage} 
            onRetry={() => refetch()}
          />
        </main>
      </div>
    )
  }

  const totalMonitors = summary?.totalMonitors || 0
  const onlineMonitors = summary?.onlineMonitors || 0
  const offlineMonitors = summary?.offlineMonitors || 0
  const avgUptime = summary?.overallUptimePercentage || 0
  const avgResponseTime = summary?.averageResponseTimeMs || 0
  const totalChanges = monitors.reduce((acc, m) => acc + (m.HasRecentChanges ? 1 : 0), 0)

  // Preparar dados para gráficos
  const uptimeChartData = uptimeData?.charts?.timeSeries?.map((item: any) => ({
    date: new Date(item.timestamp).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }),
    online: item.online || 0,
    offline: item.offline || 0,
    uptime: item.uptimePercentage || 0,
  })) || []

  const responseTimeChartData = uptimeData?.charts?.timeSeries?.map((item: any) => ({
    date: new Date(item.timestamp).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }),
    responseTime: item.averageResponseTime || 0,
  })) || []

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
            <p className="text-lg text-dark-300 max-w-2xl mx-auto mb-6">
              Acompanhe o status, performance e mudanças dos seus serviços com precisão e confiabilidade
            </p>
            <div className="flex gap-4 justify-center">
              <Button
                variant="primary"
                onClick={() => router.push('/monitors')}
              >
                Gerenciar Monitores
              </Button>
              <Button
                variant="secondary"
                onClick={() => router.push('/monitors')}
              >
                Ver Todos
              </Button>
            </div>
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
                  <p className="text-3xl font-bold text-gray-100">
                    {avgUptime !== undefined && avgUptime !== null 
                      ? `${avgUptime.toFixed(1)}%` 
                      : 'N/A'}
                  </p>
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
          <div className="flex justify-between items-center mb-6">
            <div>
              <h3 className="text-2xl font-bold text-gray-100 mb-2">Monitores Ativos</h3>
              <p className="text-dark-400">Acompanhe o status de todos os seus serviços</p>
            </div>
            <Button
              variant="primary"
              onClick={() => router.push('/monitors')}
            >
              + Novo Monitor
            </Button>
          </div>
          {monitors.length === 0 ? (
            <Card>
              <div className="text-center py-12">
                <p className="text-dark-400 mb-4">Nenhum monitor cadastrado ainda</p>
                <Button
                  variant="primary"
                  onClick={() => router.push('/monitors')}
                >
                  Criar Primeiro Monitor
                </Button>
              </div>
            </Card>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {monitors.map((monitor) => {
                const status = getStatusFromUptime(monitor.IsOnline, monitor.CurrentStatus)
                return (
                  <div
                    key={monitor.MonitorId}
                    onClick={() => {
                      if (monitor.MonitorId) {
                        router.push(`/monitors/${monitor.MonitorId}`)
                      }
                    }}
                    className="cursor-pointer"
                  >
                    <MonitorCard
                      name={monitor.MonitorName || 'Monitor sem nome'}
                      url={monitor.Url || ''}
                      status={status}
                      lastChecked={monitor.TimeSinceLastCheck}
                      responseTime={monitor.LastResponseTimeMs}
                      uptime={monitor.UptimePercentage}
                      changes={monitor.HasRecentChanges ? 1 : 0}
                    />
                  </div>
                )
              })}
            </div>
          )}
        </section>

        {/* Charts Section */}
        {uptimeChartData.length > 0 && (
          <section className="mb-12">
            <div className="mb-6">
              <h3 className="text-2xl font-bold text-gray-100 mb-2">Análise e Métricas</h3>
              <p className="text-dark-400">Visualize tendências e performance histórica</p>
            </div>
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <UptimeChart data={uptimeChartData} />
              <ResponseTimeChart data={responseTimeChartData} />
            </div>
          </section>
        )}

        {/* Additional Stats */}
        <section className="mb-12">
          <Card>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div className="text-center">
                <p className="text-sm text-dark-400 mb-2">Tempo Médio de Resposta</p>
                <p className="text-2xl font-bold text-primary-400">
                  {avgResponseTime > 0 ? `${Math.round(avgResponseTime)}ms` : 'N/A'}
                </p>
              </div>
              <div className="text-center border-l border-r border-dark-700">
                <p className="text-sm text-dark-400 mb-2">Total de Verificações</p>
                <p className="text-2xl font-bold text-gray-100">
                  {uptimeData?.metadata?.totalRecords || 0}
                </p>
              </div>
              <div className="text-center">
                <p className="text-sm text-dark-400 mb-2">Mudanças Detectadas</p>
                <p className="text-2xl font-bold text-warning-400">{totalChanges}</p>
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
