import { useQuery } from 'react-query'
import { uptimeApi } from '@/lib/api/uptime/uptime'

const isValidUUID = (id: string | null): boolean => {
  if (!id) return false
  return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(id)
}

export const useUptimeHistory = (monitorId: string | null, limit?: number) => {
  const isValid = isValidUUID(monitorId)
  
  return useQuery(
    ['uptime-history', monitorId, limit],
    () => uptimeApi.getHistory(monitorId!, limit),
    {
      enabled: isValid,
      refetchInterval: 30000,
      staleTime: 10000,
    }
  )
}

export const useLatestUptimeCheck = (monitorId: string | null) => {
  const isValid = isValidUUID(monitorId)
  
  return useQuery(
    ['latest-uptime', monitorId],
    () => uptimeApi.getLatest(monitorId!),
    {
      enabled: isValid,
      refetchInterval: 30000,
    }
  )
}
