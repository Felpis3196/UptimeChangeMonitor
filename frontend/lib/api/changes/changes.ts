import apiClient from '../client'
import { ChangeDetection } from '@/types'

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
      contentChanged: number
      structureChanged: number
      statusChanged: number
      significantChanges: number
      changeTypeDistribution: Array<{
        type: string
        label: string
        count: number
        percentage: number
      }>
      significantChangesPercentage: number
    }
    timing: {
      firstChangeAt?: string
      firstChangeAtFormatted?: string
      lastChangeAt?: string
      lastChangeAtFormatted?: string
      timeSinceLastChange?: string
      timeSinceLastChangeHours?: number
      changeFrequency?: number
      changeFrequencyFormatted?: string
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
      contentChanged: number
      structureChanged: number
      statusChanged: number
      significantChanges: number
    }>
    changeTypeDistribution: Array<{
      type: string
      label: string
      count: number
      percentage: number
    }>
    changesByDayOfWeek: Array<{
      dayOfWeek: string
      dayOfWeekPt: string
      count: number
      percentage: number
      dayOfWeekOrder: number
    }>
    changesByHour: Array<{
      hour: number
      hourFormatted: string
      count: number
      percentage: number
    }>
  }
  intervals: any
  trends: any
  detections: ChangeDetection[]
}

export interface GetLatestResponse {
  detection: ChangeDetection
  metadata: {
    isRecent: boolean
    timeSinceDetection: string
    recentChangeWarning: string
    retrievedAt: string
  }
}

export const changesApi = {
  // Obtém o histórico de detecções de mudança
  getHistory: async (monitorId: string, limit?: number): Promise<GetHistoryResponse> => {
    if (!monitorId || !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(monitorId)) {
      throw new Error('ID do monitor inválido')
    }
    
    const params = limit ? { limit } : {}
    const response = await apiClient.get<GetHistoryResponse>(
      `/monitors/${monitorId}/changedetections`,
      { params }
    )
    return response.data
  },

  // Obtém a última detecção de mudança
  getLatest: async (monitorId: string): Promise<GetLatestResponse> => {
    if (!monitorId || !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(monitorId)) {
      throw new Error('ID do monitor inválido')
    }
    
    const response = await apiClient.get<GetLatestResponse>(
      `/monitors/${monitorId}/changedetections/latest`
    )
    return response.data
  },
}
