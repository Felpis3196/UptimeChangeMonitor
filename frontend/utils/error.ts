// Helper function to extract error message
export function getErrorMessage(err: unknown): string {
  try {
    if (!err) {
      return 'Erro ao carregar dados'
    }
    
    if (typeof err === 'string') {
      return err
    }
    
    // Check if it's an Error instance - use explicit type checking
    if (err && typeof err === 'object') {
      const errorLike = err as Record<string, unknown>
      
      // Priorizar mensagem do usuário (do interceptor)
      if ('userMessage' in errorLike && typeof errorLike.userMessage === 'string') {
        return errorLike.userMessage
      }
      
      // Check for message property
      if ('message' in errorLike) {
        const msg = errorLike.message
        if (typeof msg === 'string' && msg) {
          return msg
        }
        if (msg !== undefined && msg !== null) {
          return String(msg)
        }
      }
      
      // Check for response.data.message (axios error)
      if ('response' in errorLike) {
        const response = errorLike.response as Record<string, unknown>
        if (response && typeof response === 'object' && 'data' in response) {
          const data = response.data as Record<string, unknown>
          if (data && typeof data === 'object' && 'message' in data && typeof data.message === 'string') {
            return data.message
          }
        }
      }
      
      // If it has toString, use it
      if ('toString' in errorLike && typeof errorLike.toString === 'function') {
        const str = (errorLike.toString as () => string)()
        if (str && str !== '[object Object]') {
          return str
        }
      }
    }
    
    return 'Erro ao carregar dados'
  } catch {
    return 'Erro ao carregar dados'
  }
}
