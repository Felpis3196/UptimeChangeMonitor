'use client'

import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts'
import Card from '@/components/Common/Card'

interface ResponseTimeChartProps {
  data: Array<{
    date: string
    responseTime: number
  }>
}

export default function ResponseTimeChart({ data }: ResponseTimeChartProps) {
  return (
    <Card className="h-80">
      <div className="mb-4">
        <h3 className="text-lg font-semibold text-gray-100 mb-1">Tempo de Resposta</h3>
        <p className="text-sm text-dark-400">Performance ao longo do tempo</p>
      </div>
      <ResponsiveContainer width="100%" height="100%">
        <LineChart data={data}>
          <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
          <XAxis 
            dataKey="date" 
            stroke="#64748b"
            style={{ fontSize: '12px' }}
          />
          <YAxis 
            stroke="#64748b"
            style={{ fontSize: '12px' }}
            tickFormatter={(value) => `${value}ms`}
          />
          <Tooltip 
            contentStyle={{ 
              backgroundColor: '#1e293b', 
              border: '1px solid #334155',
              borderRadius: '8px',
              color: '#f1f5f9'
            }}
            formatter={(value: number) => [`${value}ms`, 'Tempo de Resposta']}
          />
          <Line 
            type="monotone" 
            dataKey="responseTime" 
            stroke="#3b82f6" 
            strokeWidth={2}
            dot={{ fill: '#3b82f6', r: 3 }}
            activeDot={{ r: 5 }}
          />
        </LineChart>
      </ResponsiveContainer>
    </Card>
  )
}
