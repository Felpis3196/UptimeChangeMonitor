'use client'

import React, { useState, useEffect } from 'react'
import { Button } from '@/components/Common/Button'
import Card from '@/components/Common/Card'
import { Monitor } from '@/types'

export interface MonitorFormData {
  name: string
  url: string
  checkIntervalSeconds: number
  monitorUptime: boolean
  monitorChanges: boolean
}

export interface MonitorFormProps {
  monitor?: Monitor
  onSubmit: (data: MonitorFormData) => Promise<void>
  onCancel: () => void
  isLoading?: boolean
}

export const MonitorForm: React.FC<MonitorFormProps> = ({
  monitor,
  onSubmit,
  onCancel,
  isLoading = false,
}) => {
  const [formData, setFormData] = useState<MonitorFormData>({
    name: monitor?.Name || '',
    url: monitor?.Url || '',
    checkIntervalSeconds: monitor?.CheckIntervalSeconds || 60,
    monitorUptime: monitor?.MonitorUptime ?? true,
    monitorChanges: monitor?.MonitorChanges ?? false,
  })

  const [errors, setErrors] = useState<Partial<Record<keyof MonitorFormData, string>>>({})

  useEffect(() => {
    if (monitor) {
      setFormData({
        name: monitor.Name,
        url: monitor.Url,
        checkIntervalSeconds: monitor.CheckIntervalSeconds,
        monitorUptime: monitor.MonitorUptime,
        monitorChanges: monitor.MonitorChanges,
      })
    }
  }, [monitor])

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof MonitorFormData, string>> = {}

    if (!formData.name.trim()) {
      newErrors.name = 'Nome é obrigatório'
    }

    if (!formData.url.trim()) {
      newErrors.url = 'URL é obrigatória'
    } else {
      try {
        new URL(formData.url)
      } catch {
        newErrors.url = 'URL inválida'
      }
    }

    if (formData.checkIntervalSeconds < 10) {
      newErrors.checkIntervalSeconds = 'Intervalo mínimo é 10 segundos'
    }

    if (formData.checkIntervalSeconds > 86400) {
      newErrors.checkIntervalSeconds = 'Intervalo máximo é 86400 segundos (24 horas)'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    
    if (!validate()) {
      return
    }

    try {
      await onSubmit(formData)
    } catch (error) {
      console.error('Erro ao salvar monitor:', error)
    }
  }

  const handleChange = (field: keyof MonitorFormData, value: string | number | boolean) => {
    setFormData((prev) => ({ ...prev, [field]: value }))
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors((prev) => ({ ...prev, [field]: undefined }))
    }
  }

  return (
    <Card className="p-6">
      <h2 className="text-2xl font-bold text-gradient mb-6">
        {monitor ? 'Editar Monitor' : 'Novo Monitor'}
      </h2>

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Nome */}
        <div>
          <label htmlFor="name" className="block text-sm font-medium text-gray-300 mb-2">
            Nome do Monitor *
          </label>
          <input
            type="text"
            id="name"
            value={formData.name}
            onChange={(e) => handleChange('name', e.target.value)}
            className={`w-full px-4 py-2 bg-dark-800 border rounded-lg text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500 ${
              errors.name ? 'border-danger-500' : 'border-dark-600'
            }`}
            placeholder="Ex: Site Principal"
            disabled={isLoading}
          />
          {errors.name && (
            <p className="mt-1 text-sm text-danger-500">{errors.name}</p>
          )}
        </div>

        {/* URL */}
        <div>
          <label htmlFor="url" className="block text-sm font-medium text-gray-300 mb-2">
            URL *
          </label>
          <input
            type="url"
            id="url"
            value={formData.url}
            onChange={(e) => handleChange('url', e.target.value)}
            className={`w-full px-4 py-2 bg-dark-800 border rounded-lg text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500 ${
              errors.url ? 'border-danger-500' : 'border-dark-600'
            }`}
            placeholder="https://exemplo.com"
            disabled={isLoading}
          />
          {errors.url && (
            <p className="mt-1 text-sm text-danger-500">{errors.url}</p>
          )}
        </div>

        {/* Intervalo de Verificação */}
        <div>
          <label htmlFor="checkIntervalSeconds" className="block text-sm font-medium text-gray-300 mb-2">
            Intervalo de Verificação (segundos) *
          </label>
          <input
            type="number"
            id="checkIntervalSeconds"
            min="10"
            max="86400"
            value={formData.checkIntervalSeconds}
            onChange={(e) => handleChange('checkIntervalSeconds', parseInt(e.target.value) || 60)}
            className={`w-full px-4 py-2 bg-dark-800 border rounded-lg text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500 ${
              errors.checkIntervalSeconds ? 'border-danger-500' : 'border-dark-600'
            }`}
            disabled={isLoading}
          />
          {errors.checkIntervalSeconds && (
            <p className="mt-1 text-sm text-danger-500">{errors.checkIntervalSeconds}</p>
          )}
          <p className="mt-1 text-xs text-gray-400">
            Mínimo: 10s | Máximo: 86400s (24h) | Padrão: 60s
          </p>
        </div>

        {/* Opções de Monitoramento */}
        <div className="space-y-4">
          <label className="block text-sm font-medium text-gray-300 mb-2">
            Opções de Monitoramento
          </label>

          {/* Monitorar Uptime */}
          <div className="flex items-center">
            <input
              type="checkbox"
              id="monitorUptime"
              checked={formData.monitorUptime}
              onChange={(e) => handleChange('monitorUptime', e.target.checked)}
              className="w-5 h-5 text-primary-600 bg-dark-800 border-dark-600 rounded focus:ring-primary-500"
              disabled={isLoading}
            />
            <label htmlFor="monitorUptime" className="ml-3 text-sm text-gray-300">
              Monitorar Uptime (disponibilidade)
            </label>
          </div>

          {/* Monitorar Mudanças */}
          <div className="flex items-center">
            <input
              type="checkbox"
              id="monitorChanges"
              checked={formData.monitorChanges}
              onChange={(e) => handleChange('monitorChanges', e.target.checked)}
              className="w-5 h-5 text-primary-600 bg-dark-800 border-dark-600 rounded focus:ring-primary-500"
              disabled={isLoading}
            />
            <label htmlFor="monitorChanges" className="ml-3 text-sm text-gray-300">
              Monitorar Mudanças no Conteúdo
            </label>
          </div>
        </div>

        {/* Botões */}
        <div className="flex gap-4 pt-4">
          <Button
            type="submit"
            variant="primary"
            isLoading={isLoading}
            className="flex-1"
          >
            {monitor ? 'Salvar Alterações' : 'Criar Monitor'}
          </Button>
          <Button
            type="button"
            variant="secondary"
            onClick={onCancel}
            disabled={isLoading}
            className="flex-1"
          >
            Cancelar
          </Button>
        </div>
      </form>
    </Card>
  )
}
