'use client'

import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Area, AreaChart } from 'recharts'
import Card from '@/components/Common/Card'

interface UptimeChartProps {
  data: Array<{
    date: string
    online: number
    offline: number
    uptime: number
  }>
}

export default function UptimeChart({ data }: UptimeChartProps) {
  return (
    <Card className="h-80">
      <div className="mb-4">
        <h3 className="text-lg font-semibold text-gray-100 mb-1">Histórico de Uptime</h3>
        <p className="text-sm text-dark-400">Evolução do status ao longo do tempo</p>
      </div>
      <ResponsiveContainer width="100%" height="100%">
        <AreaChart data={data}>
          <defs>
            <linearGradient id="colorUptime" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#22c55e" stopOpacity={0.3}/>
              <stop offset="95%" stopColor="#22c55e" stopOpacity={0}/>
            </linearGradient>
          </defs>
          <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
          <XAxis 
            dataKey="date" 
            stroke="#64748b"
            style={{ fontSize: '12px' }}
          />
          <YAxis 
            stroke="#64748b"
            style={{ fontSize: '12px' }}
            domain={[0, 100]}
            tickFormatter={(value) => `${value}%`}
          />
          <Tooltip 
            contentStyle={{ 
              backgroundColor: '#1e293b', 
              border: '1px solid #334155',
              borderRadius: '8px',
              color: '#f1f5f9'
            }}
          />
          <Area 
            type="monotone" 
            dataKey="uptime" 
            stroke="#22c55e" 
            strokeWidth={2}
            fillOpacity={1}
            fill="url(#colorUptime)" 
          />
        </AreaChart>
      </ResponsiveContainer>
    </Card>
  )
}
