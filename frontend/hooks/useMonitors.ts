import { useQuery, useMutation, useQueryClient } from 'react-query'
import { monitorsApi } from '@/lib/api/monitors/monitors'
import { Monitor } from '@/types'

const isValidUUID = (id: string | null): boolean => {
  if (!id) return false
  return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(id)
}

export const useMonitors = () => {
  return useQuery('monitors', monitorsApi.getAll, {
    refetchInterval: 30000, // Refetch a cada 30 segundos
    staleTime: 10000, // Considera dados frescos por 10 segundos
  })
}

export const useMonitor = (id: string | null) => {
  const isValid = isValidUUID(id)
  
  return useQuery(
    ['monitor', id],
    () => monitorsApi.getById(id!),
    {
      enabled: isValid,
      refetchInterval: 30000,
    }
  )
}

export const useMonitorsStatus = () => {
  return useQuery('monitors-status', monitorsApi.getAllStatus, {
    refetchInterval: 30000,
    staleTime: 10000,
  })
}

export const useMonitorStatus = (id: string | null) => {
  const isValid = isValidUUID(id)
  
  return useQuery(
    ['monitor-status', id],
    () => monitorsApi.getStatus(id!),
    {
      enabled: isValid,
      refetchInterval: 30000,
    }
  )
}

export const useCreateMonitor = () => {
  const queryClient = useQueryClient()
  
  return useMutation(monitorsApi.create, {
    onSuccess: (data) => {
      // Adicionar o novo monitor ao cache imediatamente
      queryClient.setQueryData(['monitor', data.Id], data)
      // Invalidar queries relacionadas para garantir dados atualizados
      queryClient.invalidateQueries('monitors')
      queryClient.invalidateQueries('monitors-status')
      queryClient.invalidateQueries(['monitor-status', data.Id])
    },
    onError: (error) => {
      console.error('Erro ao criar monitor:', error)
    },
  })
}

export const useUpdateMonitor = () => {
  const queryClient = useQueryClient()
  
  return useMutation(
    ({ id, data }: { id: string; data: any }) => monitorsApi.update(id, data),
    {
      onSuccess: (updatedMonitor, variables) => {
        // Atualizar cache imediatamente
        queryClient.setQueryData(['monitor', variables.id], updatedMonitor)
        // Invalidar queries relacionadas
        queryClient.invalidateQueries('monitors')
        queryClient.invalidateQueries('monitors-status')
        queryClient.invalidateQueries(['monitor-status', variables.id])
      },
      onError: (error) => {
        console.error('Erro ao atualizar monitor:', error)
      },
    }
  )
}

export const useDeleteMonitor = () => {
  const queryClient = useQueryClient()
  
  return useMutation(monitorsApi.delete, {
    onSuccess: (_, deletedId) => {
      // Remover do cache imediatamente
      queryClient.removeQueries(['monitor', deletedId])
      queryClient.removeQueries(['monitor-status', deletedId])
      // Remover queries de uptime e changes relacionadas
      queryClient.removeQueries(['uptime-history', deletedId])
      queryClient.removeQueries(['latest-uptime', deletedId])
      queryClient.removeQueries(['change-history', deletedId])
      queryClient.removeQueries(['latest-change', deletedId])
      // Invalidar queries relacionadas
      queryClient.invalidateQueries('monitors')
      queryClient.invalidateQueries('monitors-status')
    },
    onError: (error) => {
      console.error('Erro ao deletar monitor:', error)
    },
  })
}
