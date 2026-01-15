'use client'

import { useParams, useRouter } from 'next/navigation'
import { useState, useEffect } from 'react'
import Header from '@/components/Layout/Header'
import Card from '@/components/Common/Card'
import Loading from '@/components/Common/Loading'
import Error from '@/components/Common/Error'
import { Button } from '@/components/Common/Button'
import { MonitorForm, MonitorFormData } from '@/components/Monitor/MonitorForm'
import { ToastContainer } from '@/components/Common/Toast'
import UptimeChart from '@/components/Charts/UptimeChart'
import ResponseTimeChart from '@/components/Charts/ResponseTimeChart'
import { useMonitor, useMonitorStatus, useUpdateMonitor, useDeleteMonitor } from '@/hooks/useMonitors'
import { useUptimeHistory } from '@/hooks/useUptimeChecks'
import { useChangeHistory } from '@/hooks/useChangeDetections'
import { useToast } from '@/hooks/useToast'
import { getErrorMessage } from '@/utils/error'

export default function MonitorDetailPage() {
  const params = useParams()
  const router = useRouter()
  const monitorId = params.id as string
  const toast = useToast()

  const [isEditing, setIsEditing] = useState(false)
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false)

  // Validar se o ID é válido
  const isValidId = monitorId && /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(monitorId)

  const { data: monitor, isLoading: isLoadingMonitor, error: monitorError, refetch: refetchMonitor } = useMonitor(isValidId ? monitorId : null)
  const { data: statusData, isLoading: isLoadingStatus, error: statusError, refetch: refetchStatus } = useMonitorStatus(isValidId ? monitorId : null)
  const { data: uptimeData } = useUptimeHistory(isValidId ? monitorId : null, 100)
  const { data: changesData } = useChangeHistory(isValidId ? monitorId : null, 50)

  const updateMonitor = useUpdateMonitor()
  const deleteMonitor = useDeleteMonitor()

  const isLoading = isLoadingMonitor || isLoadingStatus
  const error = monitorError || statusError

  // Verificar se o monitor foi deletado ou não existe
  useEffect(() => {
    if (monitorError && !isLoadingMonitor && isValidId) {
      const errorMsg = getErrorMessage(monitorError)
      if (errorMsg.includes('404') || errorMsg.includes('não encontrado') || errorMsg.includes('not found')) {
        // Tentar refetch uma vez antes de redirecionar (pode ser problema de timing após criação)
        const refetchTimer = setTimeout(async () => {
          try {
            // Tentar buscar novamente
            await refetchMonitor()
            await refetchStatus()
          } catch {
            // Se ainda falhar, redirecionar
            toast.showError('Monitor não encontrado. Redirecionando...')
            setTimeout(() => {
              router.push('/monitors')
            }, 2000)
          }
        }, 2000)
        
        return () => clearTimeout(refetchTimer)
      }
    }
  }, [monitorError, isLoadingMonitor, isValidId, monitorId, router, toast, refetchMonitor, refetchStatus])

  const handleUpdate = async (data: MonitorFormData) => {
    if (!monitorId || !isValidId) {
      toast.showError('ID do monitor inválido')
      return
    }

    try {
      await updateMonitor.mutateAsync({ id: monitorId, data })
      setIsEditing(false)
      toast.showSuccess('Monitor atualizado com sucesso!')
    } catch (error: any) {
      const errorMessage = getErrorMessage(error)
      toast.showError(`Erro ao atualizar monitor: ${errorMessage}`)
    }
  }

  const handleDelete = async () => {
    if (!monitorId || !isValidId) {
      toast.showError('ID do monitor inválido')
      setShowDeleteConfirm(false)
      return
    }

    try {
      await deleteMonitor.mutateAsync(monitorId)
      toast.showSuccess('Monitor deletado com sucesso!')
      setTimeout(() => {
        router.push('/monitors')
      }, 1000)
    } catch (error: any) {
      const errorMessage = getErrorMessage(error)
      toast.showError(`Erro ao deletar monitor: ${errorMessage}`)
      setShowDeleteConfirm(false)
    }
  }

  // Validar ID antes de fazer chamadas
  if (!isValidId) {
    return (
      <div className="min-h-screen bg-dark-900">
        <Header />
        <main className="container mx-auto px-4 py-8">
          <Error 
            message="ID do monitor inválido" 
            onRetry={() => router.push('/monitors')}
          />
        </main>
        <ToastContainer toasts={toast.toasts} onRemove={toast.removeToast} />
      </div>
    )
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-dark-900">
        <Header />
        <main className="container mx-auto px-4 py-8">
          <Loading />
        </main>
        <ToastContainer toasts={toast.toasts} onRemove={toast.removeToast} />
      </div>
    )
  }

  if (error || !monitor || !statusData) {
    return (
      <div className="min-h-screen bg-dark-900">
        <Header />
        <main className="container mx-auto px-4 py-8">
          <Error 
            message={error ? getErrorMessage(error) : 'Monitor não encontrado'} 
            onRetry={() => {
              if (error) {
                window.location.reload()
              } else {
                router.push('/monitors')
              }
            }}
          />
        </main>
        <ToastContainer toasts={toast.toasts} onRemove={toast.removeToast} />
      </div>
    )
  }

  const status = statusData.status
  const health = statusData.health
  const quickStats = statusData.quickStats

  // Preparar dados dos gráficos
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

  if (isEditing) {
    return (
      <div className="min-h-screen bg-dark-900">
        <Header />
        <main className="container mx-auto px-4 py-8 max-w-4xl">
          <div className="mb-6">
            <Button
              variant="secondary"
              onClick={() => setIsEditing(false)}
              className="mb-4"
            >
              ← Voltar
            </Button>
          </div>
          <MonitorForm
            monitor={monitor}
            onSubmit={handleUpdate}
            onCancel={() => setIsEditing(false)}
            isLoading={updateMonitor.isLoading}
          />
        </main>
        <ToastContainer toasts={toast.toasts} onRemove={toast.removeToast} />
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-dark-900">
      <Header />
      <main className="container mx-auto px-4 py-8">
        {/* Header com ações */}
        <div className="flex justify-between items-center mb-6">
          <div>
            <Button
              variant="secondary"
              onClick={() => router.push('/')}
              className="mb-4"
            >
              ← Voltar
            </Button>
            <h1 className="text-3xl font-bold text-gradient">{monitor.Name}</h1>
            <p className="text-gray-400 mt-1">{monitor.Url}</p>
          </div>
          <div className="flex gap-3">
            <Button
              variant="primary"
              onClick={() => setIsEditing(true)}
            >
              Editar
            </Button>
            <Button
              variant="danger"
              onClick={() => setShowDeleteConfirm(true)}
            >
              Deletar
            </Button>
          </div>
        </div>

        {/* Confirmação de exclusão */}
        {showDeleteConfirm && (
          <Card className="mb-6 p-6 border-danger-500 border-2">
            <h3 className="text-xl font-bold text-danger-500 mb-4">Confirmar Exclusão</h3>
            <p className="text-gray-300 mb-4">
              Tem certeza que deseja deletar o monitor <strong>{monitor.Name}</strong>?
              Esta ação não pode ser desfeita.
            </p>
            <div className="flex gap-3">
              <Button
                variant="danger"
                onClick={handleDelete}
                isLoading={deleteMonitor.isLoading}
              >
                Sim, Deletar
              </Button>
              <Button
                variant="secondary"
                onClick={() => setShowDeleteConfirm(false)}
                disabled={deleteMonitor.isLoading}
              >
                Cancelar
              </Button>
            </div>
          </Card>
        )}

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">
          {/* Status Card */}
          <Card className="p-6">
            <h3 className="text-lg font-semibold text-gray-300 mb-4">Status Atual</h3>
            <div className="space-y-3">
              <div>
                <p className="text-sm text-gray-400">Status</p>
                <p className={`text-xl font-bold ${
                  status.IsOnline ? 'text-success-500' : 'text-danger-500'
                }`}>
                  {status.IsOnline ? 'Online' : 'Offline'}
                </p>
              </div>
              {status.LastResponseTimeMs && (
                <div>
                  <p className="text-sm text-gray-400">Tempo de Resposta</p>
                  <p className="text-xl font-bold text-gray-200">
                    {status.LastResponseTimeFormatted || `${status.LastResponseTimeMs}ms`}
                  </p>
                </div>
              )}
              {status.LastCheckedAtFormatted && (
                <div>
                  <p className="text-sm text-gray-400">Última Verificação</p>
                  <p className="text-sm text-gray-300">{status.LastCheckedAtFormatted}</p>
                  {status.TimeSinceLastCheck && (
                    <p className="text-xs text-gray-500">{status.TimeSinceLastCheck}</p>
                  )}
                </div>
              )}
            </div>
          </Card>

          {/* Health Score */}
          <Card className="p-6">
            <h3 className="text-lg font-semibold text-gray-300 mb-4">Health Score</h3>
            <div className="space-y-3">
              <div>
                <p className="text-sm text-gray-400">Score</p>
                <p className={`text-3xl font-bold ${
                  health.score >= 90 ? 'text-success-500' :
                  health.score >= 75 ? 'text-primary-500' :
                  health.score >= 50 ? 'text-warning-500' :
                  'text-danger-500'
                }`}>
                  {health.score !== undefined && health.score !== null 
                    ? health.score.toFixed(1) 
                    : 'N/A'}
                </p>
                <p className="text-sm text-gray-400 mt-1">{health.description}</p>
              </div>
              <div>
                <p className="text-sm text-gray-400">Uptime</p>
                <p className="text-xl font-bold text-gray-200">
                  {health.uptimePercentage !== undefined && health.uptimePercentage !== null 
                    ? `${health.uptimePercentage.toFixed(2)}%` 
                    : 'N/A'}
                </p>
              </div>
            </div>
          </Card>

          {/* Informações do Monitor */}
          <Card className="p-6">
            <h3 className="text-lg font-semibold text-gray-300 mb-4">Configurações</h3>
            <div className="space-y-3">
              <div>
                <p className="text-sm text-gray-400">Intervalo de Verificação</p>
                <p className="text-sm text-gray-300">{monitor.CheckIntervalSeconds}s</p>
              </div>
              <div>
                <p className="text-sm text-gray-400">Monitorar Uptime</p>
                <p className="text-sm text-gray-300">
                  {monitor.MonitorUptime ? 'Ativo' : 'Inativo'}
                </p>
              </div>
              <div>
                <p className="text-sm text-gray-400">Monitorar Mudanças</p>
                <p className="text-sm text-gray-300">
                  {monitor.MonitorChanges ? 'Ativo' : 'Inativo'}
                </p>
              </div>
              {status.UptimePercentage !== undefined && (
                <div>
                  <p className="text-sm text-gray-400">Uptime Total</p>
                  <p className="text-xl font-bold text-primary-500">
                    {status.UptimePercentage.toFixed(2)}%
                  </p>
                </div>
              )}
            </div>
          </Card>
        </div>

        {/* Gráficos */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
          {uptimeChartData.length > 0 && (
            <Card className="p-6">
              <h3 className="text-lg font-semibold text-gray-300 mb-4">Histórico de Uptime</h3>
              <UptimeChart data={uptimeChartData} />
            </Card>
          )}
          {responseTimeChartData.length > 0 && (
            <Card className="p-6">
              <h3 className="text-lg font-semibold text-gray-300 mb-4">Tempo de Resposta</h3>
              <ResponseTimeChart data={responseTimeChartData} />
            </Card>
          )}
        </div>

        {/* Verificações Recentes */}
        {quickStats.recentChecks && quickStats.recentChecks.length > 0 && (
          <Card className="p-6 mb-6">
            <h3 className="text-lg font-semibold text-gray-300 mb-4">Verificações Recentes</h3>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-dark-600">
                    <th className="text-left py-2 px-4 text-sm text-gray-400">Status</th>
                    <th className="text-left py-2 px-4 text-sm text-gray-400">Tempo de Resposta</th>
                    <th className="text-left py-2 px-4 text-sm text-gray-400">Verificado em</th>
                  </tr>
                </thead>
                <tbody>
                  {quickStats.recentChecks.map((check: any, index: number) => (
                    <tr key={index} className="border-b border-dark-700">
                      <td className="py-2 px-4">
                        <span className={`px-2 py-1 rounded text-xs font-semibold ${
                          check.IsOnline
                            ? 'bg-success-500/20 text-success-500'
                            : 'bg-danger-500/20 text-danger-500'
                        }`}>
                          {check.StatusDescription}
                        </span>
                      </td>
                      <td className="py-2 px-4 text-gray-300">
                        {check.ResponseTimeFormatted || `${check.ResponseTimeMs || 0}ms`}
                      </td>
                      <td className="py-2 px-4 text-gray-400 text-sm">
                        {check.CheckedAtFormatted || check.CheckedAt}
                        {check.TimeAgo && (
                          <span className="ml-2 text-xs">({check.TimeAgo})</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </Card>
        )}

        {/* Mudanças Recentes */}
        {quickStats.recentChanges && quickStats.recentChanges.length > 0 && (
          <Card className="p-6 mb-6">
            <h3 className="text-lg font-semibold text-gray-300 mb-4">Mudanças Recentes</h3>
            <div className="space-y-3">
              {quickStats.recentChanges.map((change: any, index: number) => (
                <div
                  key={index}
                  className="p-4 bg-dark-800 rounded-lg border border-dark-600"
                >
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="font-semibold text-gray-200">{change.ChangeTypeDescription}</p>
                      <p className="text-sm text-gray-400 mt-1">
                        {change.DetectedAtFormatted || change.DetectedAt}
                        {change.TimeAgo && (
                          <span className="ml-2">({change.TimeAgo})</span>
                        )}
                      </p>
                    </div>
                    {change.HasSignificantChange && (
                      <span className="px-2 py-1 bg-warning-500/20 text-warning-500 rounded text-xs font-semibold">
                        Significativa
                      </span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </Card>
        )}

        {/* Histórico Completo de Mudanças */}
        {changesData && changesData.detections && changesData.detections.length > 0 && (
          <Card className="p-6">
            <h3 className="text-lg font-semibold text-gray-300 mb-4">
              Histórico Completo de Mudanças ({changesData.summary?.overall?.total || 0})
            </h3>
            <div className="space-y-3 max-h-96 overflow-y-auto">
              {changesData.detections.map((change: any) => (
                <div
                  key={change.Id}
                  className="p-4 bg-dark-800 rounded-lg border border-dark-600"
                >
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="font-semibold text-gray-200">{change.ChangeTypeDescription}</p>
                      {change.ChangeDescription && (
                        <p className="text-sm text-gray-400 mt-1">{change.ChangeDescription}</p>
                      )}
                      <p className="text-xs text-gray-500 mt-1">
                        {change.DetectedAtFormatted || change.DetectedAt}
                        {change.TimeAgo && (
                          <span className="ml-2">({change.TimeAgo})</span>
                        )}
                      </p>
                    </div>
                    {change.HasSignificantChange && (
                      <span className="px-2 py-1 bg-warning-500/20 text-warning-500 rounded text-xs font-semibold">
                        Significativa
                      </span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </Card>
        )}
      </main>
      <ToastContainer toasts={toast.toasts} onRemove={toast.removeToast} />
    </div>
  )
}
