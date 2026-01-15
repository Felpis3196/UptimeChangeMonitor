'use client'

import Card from '@/components/Common/Card'

interface MonitorCardProps {
  name: string
  url: string
  status: 'online' | 'offline' | 'timeout' | 'error'
  lastChecked?: string
  responseTime?: number
  uptime?: number
  changes?: number
}

export default function MonitorCard({
  name,
  url,
  status,
  lastChecked,
  responseTime,
  uptime,
  changes
}: MonitorCardProps) {
  const statusConfig = {
    online: {
      color: 'text-success-400',
      bg: 'bg-success-500/10',
      border: 'border-success-500/30',
      glow: 'success' as const,
      label: 'Online',
      icon: (
        <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
          <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
        </svg>
      )
    },
    offline: {
      color: 'text-danger-400',
      bg: 'bg-danger-500/10',
      border: 'border-danger-500/30',
      glow: 'danger' as const,
      label: 'Offline',
      icon: (
        <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
          <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
        </svg>
      )
    },
    timeout: {
      color: 'text-warning-400',
      bg: 'bg-warning-500/10',
      border: 'border-warning-500/30',
      glow: 'none' as const,
      label: 'Timeout',
      icon: (
        <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
          <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clipRule="evenodd" />
        </svg>
      )
    },
    error: {
      color: 'text-danger-400',
      bg: 'bg-danger-500/10',
      border: 'border-danger-500/30',
      glow: 'danger' as const,
      label: 'Erro',
      icon: (
        <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
          <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
        </svg>
      )
    }
  }

  const config = statusConfig[status]

  return (
    <Card glow={config.glow} className="hover:scale-[1.02] transition-transform duration-200 h-full">
      <div className="flex items-start justify-between mb-4">
        <div className="flex-1 min-w-0">
          <h3 className="text-lg font-semibold text-gray-100 mb-1 truncate">{name}</h3>
          <p className="text-sm text-dark-400 truncate" title={url}>{url}</p>
        </div>
        <div className={`flex items-center space-x-2 px-3 py-1.5 rounded-lg ${config.bg} ${config.border} border`}>
          <span className={config.color}>{config.icon}</span>
          <span className={`text-xs font-medium ${config.color}`}>{config.label}</span>
        </div>
      </div>

      <div className="grid grid-cols-3 gap-4 mt-4 pt-4 border-t border-dark-700">
        <div>
          <p className="text-xs text-dark-400 mb-1">Tempo de Resposta</p>
          <p className="text-sm font-semibold text-gray-200">
            {responseTime ? `${responseTime}ms` : 'N/A'}
          </p>
        </div>
        <div>
          <p className="text-xs text-dark-400 mb-1">Uptime</p>
          <p className="text-sm font-semibold text-gray-200">
            {uptime !== undefined ? `${uptime.toFixed(1)}%` : 'N/A'}
          </p>
        </div>
        <div>
          <p className="text-xs text-dark-400 mb-1">Mudanças</p>
          <p className="text-sm font-semibold text-gray-200">
            {changes ?? 0}
          </p>
        </div>
      </div>

      {lastChecked && (
        <div className="mt-3 pt-3 border-t border-dark-700">
          <p className="text-xs text-dark-400">
            Última verificação: <span className="text-dark-300">{lastChecked}</span>
          </p>
        </div>
      )}
    </Card>
  )
}
