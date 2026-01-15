import { useQuery } from 'react-query'
import { changesApi } from '@/lib/api/changes/changes'

const isValidUUID = (id: string | null): boolean => {
  if (!id) return false
  return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(id)
}

export const useChangeHistory = (monitorId: string | null, limit?: number) => {
  const isValid = isValidUUID(monitorId)
  
  return useQuery(
    ['change-history', monitorId, limit],
    () => changesApi.getHistory(monitorId!, limit),
    {
      enabled: isValid,
      refetchInterval: 30000,
      staleTime: 10000,
    }
  )
}

export const useLatestChangeDetection = (monitorId: string | null) => {
  const isValid = isValidUUID(monitorId)
  
  return useQuery(
    ['latest-change', monitorId],
    () => changesApi.getLatest(monitorId!),
    {
      enabled: isValid,
      refetchInterval: 30000,
    }
  )
}
