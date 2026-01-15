import apiClient from '../client'
import { UptimeCheck } from '@/types'

export interface GetHistoryResponse {
  monitorId: string
  metadata: {
    totalRecords: number
    dateRange: {
      from?: string
      to?: string
      span?: number
    }
    retrievedAt: string
  }
  summary: {
    overall: {
      total: number
      online: number
      offline: number
      timeout: number
      error: number
      uptimePercentage: number
      downtimePercentage: number
      statusDistribution: {
        online: { count: number; percentage: number }
        offline: { count: number; percentage: number }
        timeout: { count: number; percentage: number }
        error: { count: number; percentage: number }
      }
    }
    responseTime: {
      average?: number
      averageFormatted?: string
      min?: number
      minFormatted?: string
      max?: number
      maxFormatted?: string
      median?: number
      medianFormatted?: string
      samples: number
    }
    byPeriod: {
      last24Hours: any
      last7Days: any
      last30Days: any
    }
  }
  charts: {
    timeSeries: Array<{
      timestamp: string
      timestampFormatted: string
      count: number
      online: number
      offline: number
      timeout: number
      error: number
      uptimePercentage: number
      averageResponseTime?: number
    }>
    responseTimeDistribution: Array<{
      bucket: string
      count: number
      percentage: number
    }>
    statusCodeDistribution: Array<{
      statusCode: number
      count: number
      percentage: number
    }>
  }
  trends: any
  checks: UptimeCheck[]
}

export interface GetLatestResponse {
  check: UptimeCheck
  metadata: {
    isStale: boolean
    stalenessWarning?: string
    retrievedAt: string
  }
}

export const uptimeApi = {
  // Obtém o histórico de verificações de uptime
  getHistory: async (monitorId: string, limit?: number): Promise<GetHistoryResponse> => {
    if (!monitorId || !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(monitorId)) {
      throw new Error('ID do monitor inválido')
    }
    
    const params = limit ? { limit } : {}
    const response = await apiClient.get<GetHistoryResponse>(
      `/monitors/${monitorId}/uptimechecks`,
      { params }
    )
    return response.data
  },

  // Obtém a última verificação de uptime
  getLatest: async (monitorId: string): Promise<GetLatestResponse> => {
    if (!monitorId || !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(monitorId)) {
      throw new Error('ID do monitor inválido')
    }
    
    const response = await apiClient.get<GetLatestResponse>(
      `/monitors/${monitorId}/uptimechecks/latest`
    )
    return response.data
  },
}
