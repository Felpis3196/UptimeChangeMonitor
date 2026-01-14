'use client'

export default function Header() {
  return (
    <header className="sticky top-0 z-50 w-full bg-dark-900/95 backdrop-blur-sm border-b border-dark-700">
      <div className="container mx-auto px-4 py-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-3">
            <div className="w-10 h-10 rounded-lg bg-gradient-primary flex items-center justify-center">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div>
              <h1 className="text-xl font-bold text-gradient">Uptime Monitor</h1>
              <p className="text-xs text-dark-400">Monitoramento Inteligente</p>
            </div>
          </div>
          <div className="flex items-center space-x-4">
            <div className="hidden md:flex items-center space-x-6 text-sm">
              <div className="flex items-center space-x-2">
                <div className="w-2 h-2 rounded-full bg-success-500 animate-pulse"></div>
                <span className="text-dark-300">Sistema Online</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </header>
  )
}
