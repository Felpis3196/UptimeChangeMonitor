import { ReactNode } from 'react'

interface CardProps {
  children: ReactNode
  className?: string
  glow?: 'primary' | 'success' | 'danger' | 'none'
}

export default function Card({ children, className = '', glow = 'none' }: CardProps) {
  const glowClass = glow !== 'none' ? `glow-${glow}` : ''
  
  return (
    <div className={`card-gradient rounded-xl p-6 ${glowClass} ${className}`}>
      {children}
    </div>
  )
}
