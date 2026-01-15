import axios from 'axios'

// Get API URL from environment variable
// In Docker, this will be http://api:80
// In development, this will be http://localhost:5000
const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000'
const API_VERSION = '1.0'

// Construct base URL - handle both with and without protocol
const baseURL = API_URL.startsWith('http') 
  ? `${API_URL}/api/v${API_VERSION}`
  : `http://${API_URL}/api/v${API_VERSION}`

export const apiClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
})

// Request interceptor
apiClient.interceptors.request.use(
  (config) => {
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor
apiClient.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    // Melhorar mensagens de erro para o usuário
    if (error.response) {
      // Server responded with error
      const status = error.response.status
      const data = error.response.data
      
      // Criar mensagem de erro mais amigável
      let message = 'Erro ao processar requisição'
      
      if (status === 404) {
        message = data?.message || 'Recurso não encontrado'
      } else if (status === 400) {
        message = data?.message || 'Dados inválidos'
      } else if (status === 401) {
        message = 'Não autorizado'
      } else if (status === 403) {
        message = 'Acesso negado'
      } else if (status === 500) {
        message = 'Erro interno do servidor'
      } else if (data?.message) {
        message = data.message
      }
      
      // Adicionar mensagem ao erro para facilitar tratamento
      error.userMessage = message
      console.error('API Error:', { status, data, message })
    } else if (error.request) {
      // Request made but no response
      error.userMessage = 'Erro de conexão. Verifique sua internet.'
      console.error('Network Error:', error.request)
    } else {
      // Something else happened
      error.userMessage = error.message || 'Erro desconhecido'
      console.error('Error:', error.message)
    }
    return Promise.reject(error)
  }
)

export default apiClient
