import apiClient from '../client'
import { Monitor, MonitorStatusDto } from '@/types'

export interface GetAllStatusResponse {
  summary: {
    totalMonitors: number
    onlineMonitors: number
    offlineMonitors: number
    monitorsWithRecentChanges: number
    overallUptimePercentage: number
    averageResponseTimeMs?: number
    averageResponseTimeFormatted?: string
    statusDistribution: Array<{
      status: string
      count: number
      percentage: number
    }>
  }
  monitors: MonitorStatusDto[]
  metadata: {
    RetrievedAt: string
  }
}

export interface GetStatusResponse {
  status: MonitorStatusDto
  quickStats: {
    recentChecks: Array<{
      Status: number
      StatusDescription: string
      IsOnline: boolean
      ResponseTimeMs?: number
      ResponseTimeFormatted?: string
      CheckedAt: string
      CheckedAtFormatted?: string
      TimeAgo?: string
    }>
    recentChanges: Array<{
      ChangeType: number
      ChangeTypeDescription: string
      DetectedAt: string
      DetectedAtFormatted?: string
      TimeAgo?: string
      HasSignificantChange?: boolean
    }>
  }
  health: {
    score: number
    level: string
    description: string
    uptimePercentage: number
    averageResponseTime?: number
  }
  metadata: {
    RetrievedAt: string
    MonitorId: string
  }
}

export const monitorsApi = {
  // Lista todos os monitores
  getAll: async (): Promise<Monitor[]> => {
    const response = await apiClient.get<Monitor[]>('/monitors')
    return response.data
  },

  // Busca um monitor pelo ID
  getById: async (id: string): Promise<Monitor> => {
    if (!id || !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(id)) {
      throw new Error('ID do monitor inválido')
    }
    
    const response = await apiClient.get<Monitor>(`/monitors/${id}`)
    return response.data
  },

  // Obtém o status de todos os monitores
  getAllStatus: async (): Promise<GetAllStatusResponse> => {
    const response = await apiClient.get<GetAllStatusResponse>('/monitors/status')
    return response.data
  },

  // Obtém o status de um monitor específico
  getStatus: async (id: string): Promise<GetStatusResponse> => {
    if (!id || !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(id)) {
      throw new Error('ID do monitor inválido')
    }
    
    const response = await apiClient.get<GetStatusResponse>(`/monitors/${id}/status`)
    return response.data
  },

  // Cria um novo monitor
  create: async (data: {
    name: string
    url: string
    checkIntervalSeconds: number
    monitorUptime: boolean
    monitorChanges: boolean
  }): Promise<Monitor> => {
    // Validação básica antes de enviar
    if (!data.name || !data.url) {
      throw new Error('Nome e URL são obrigatórios')
    }
    
    try {
      new URL(data.url)
    } catch {
      throw new Error('URL inválida')
    }
    
    const response = await apiClient.post<Monitor>('/monitors', data)
    return response.data
  },

  // Atualiza um monitor
  update: async (id: string, data: {
    name?: string
    url?: string
    checkIntervalSeconds?: number
    monitorUptime?: boolean
    monitorChanges?: boolean
  }): Promise<Monitor> => {
    if (!id || !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(id)) {
      throw new Error('ID do monitor inválido')
    }
    
    // Validação de URL se fornecida
    if (data.url) {
      try {
        new URL(data.url)
      } catch {
        throw new Error('URL inválida')
      }
    }
    
    const response = await apiClient.put<Monitor>(`/monitors/${id}`, data)
    return response.data
  },

  // Deleta um monitor
  delete: async (id: string): Promise<void> => {
    if (!id || !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(id)) {
      throw new Error('ID do monitor inválido')
    }
    
    await apiClient.delete(`/monitors/${id}`)
  },
}
