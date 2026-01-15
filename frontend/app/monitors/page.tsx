'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import Header from '@/components/Layout/Header'
import Card from '@/components/Common/Card'
import Loading from '@/components/Common/Loading'
import Error from '@/components/Common/Error'
import { Button } from '@/components/Common/Button'
import MonitorCard from '@/components/Monitor/MonitorCard'
import { MonitorForm, MonitorFormData } from '@/components/Monitor/MonitorForm'
import { ToastContainer } from '@/components/Common/Toast'
import { useMonitorsStatus, useCreateMonitor, useDeleteMonitor } from '@/hooks/useMonitors'
import { UptimeStatus } from '@/types'
import { useToast } from '@/hooks/useToast'
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

export default function MonitorsPage() {
  const router = useRouter()
  const [showCreateForm, setShowCreateForm] = useState(false)
  const [deleteConfirmId, setDeleteConfirmId] = useState<string | null>(null)

  const { data: statusData, isLoading, error, refetch } = useMonitorsStatus()
  const createMonitor = useCreateMonitor()
  const deleteMonitor = useDeleteMonitor()
  const toast = useToast()

  const monitors = statusData?.monitors || []
  const totalMonitors = statusData?.summary?.totalMonitors || monitors.length

  const handleCreate = async (data: MonitorFormData) => {
    try {
      const newMonitor = await createMonitor.mutateAsync(data)
      setShowCreateForm(false)
      toast.showSuccess('Monitor criado com sucesso!')
      // Navegar para a página de detalhes do novo monitor
      if (newMonitor?.Id) {
        // Aguardar um pouco mais para garantir que o monitor está disponível
        setTimeout(() => {
          router.push(`/monitors/${newMonitor.Id}`)
        }, 1500)
      }
    } catch (error: any) {
      const errorMessage = getErrorMessage(error)
      toast.showError(`Erro ao criar monitor: ${errorMessage}`)
    }
  }

  const handleDelete = async (id: string) => {
    if (!id) {
      toast.showError('ID do monitor inválido')
      setDeleteConfirmId(null)
      return
    }

    try {
      await deleteMonitor.mutateAsync(id)
      setDeleteConfirmId(null)
      toast.showSuccess('Monitor deletado com sucesso!')
    } catch (error: any) {
      const errorMessage = getErrorMessage(error)
      toast.showError(`Erro ao deletar monitor: ${errorMessage}`)
      setDeleteConfirmId(null)
    }
  }

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

  if (error) {
    return (
      <div className="min-h-screen bg-dark-900">
        <Header />
        <main className="container mx-auto px-4 py-8">
          <Error 
            message={getErrorMessage(error)} 
            onRetry={() => refetch()}
          />
        </main>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-dark-900">
      <Header />
      <main className="container mx-auto px-4 py-8">
        {/* Header */}
        <div className="flex justify-between items-center mb-6">
          <div>
            <Button
              variant="secondary"
              onClick={() => router.push('/')}
              className="mb-4"
            >
              ← Voltar
            </Button>
            <h1 className="text-3xl font-bold text-gradient">Monitores</h1>
            <p className="text-gray-400 mt-1">
              {totalMonitors} monitor{totalMonitors !== 1 ? 'es' : ''} cadastrado{totalMonitors !== 1 ? 's' : ''}
            </p>
          </div>
          <Button
            variant="primary"
            onClick={() => setShowCreateForm(true)}
          >
            + Novo Monitor
          </Button>
        </div>

        {/* Formulário de criação */}
        {showCreateForm && (
          <div className="mb-6">
            <MonitorForm
              onSubmit={handleCreate}
              onCancel={() => setShowCreateForm(false)}
              isLoading={createMonitor.isLoading}
            />
          </div>
        )}

        {/* Lista de monitores */}
        {!monitors || monitors.length === 0 ? (
          <Card className="p-8 text-center">
            <p className="text-gray-400 text-lg mb-4">Nenhum monitor cadastrado ainda.</p>
            <Button
              variant="primary"
              onClick={() => setShowCreateForm(true)}
            >
              Criar Primeiro Monitor
            </Button>
          </Card>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {monitors.map((monitor) => (
              <div key={monitor.MonitorId} className="relative group">
                <div
                  className="cursor-pointer"
                  onClick={() => {
                    if (monitor.MonitorId) {
                      router.push(`/monitors/${monitor.MonitorId}`)
                    }
                  }}
                >
                  {(() => {
                    const status = getStatusFromUptime(monitor.IsOnline, monitor.CurrentStatus)
                    return (
                  <MonitorCard
                    name={monitor.MonitorName}
                    url={monitor.Url}
                    status={status}
                    lastChecked={monitor.LastCheckedAtFormatted || monitor.TimeSinceLastCheck}
                    responseTime={monitor.LastResponseTimeMs}
                    uptime={monitor.UptimePercentage}
                    changes={monitor.HasRecentChanges ? 1 : 0}
                  />
                    )
                  })()}
                </div>
                <div className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity">
                  <Button
                    variant="danger"
                    size="sm"
                    onClick={(e) => {
                      e.stopPropagation()
                      if (monitor.MonitorId) {
                        setDeleteConfirmId(monitor.MonitorId)
                      }
                    }}
                  >
                    ×
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Confirmação de exclusão */}
        {deleteConfirmId && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
            <Card className="p-6 max-w-md w-full border-danger-500 border-2">
              <h3 className="text-xl font-bold text-danger-500 mb-4">Confirmar Exclusão</h3>
              <p className="text-gray-300 mb-4">
                Tem certeza que deseja deletar este monitor? Esta ação não pode ser desfeita.
              </p>
              <div className="flex gap-3">
                <Button
                  variant="danger"
                  onClick={() => handleDelete(deleteConfirmId)}
                  isLoading={deleteMonitor.isLoading}
                  className="flex-1"
                >
                  Sim, Deletar
                </Button>
                <Button
                  variant="secondary"
                  onClick={() => setDeleteConfirmId(null)}
                  disabled={deleteMonitor.isLoading}
                  className="flex-1"
                >
                  Cancelar
                </Button>
              </div>
            </Card>
          </div>
        )}
      </main>
      <ToastContainer toasts={toast.toasts} onRemove={toast.removeToast} />
    </div>
  )
}
