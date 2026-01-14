// Type Definitions

export interface Monitor {
  id: string
  name: string
  url: string
  status: MonitorStatus
  checkIntervalSeconds: number
  monitorUptime: boolean
  monitorChanges: boolean
  createdAt: string
  updatedAt: string
  lastCheckedAt?: string
}

export enum MonitorStatus {
  Active = 1,
  Inactive = 2,
  Paused = 3
}

export interface UptimeCheck {
  id: string
  monitorId: string
  status: UptimeStatus
  responseTimeMs?: number
  statusCode?: number
  checkedAt: string
  statusDescription?: string
  isOnline?: boolean
  responseTimeFormatted?: string
  statusCodeDescription?: string
  checkedAtFormatted?: string
  timeAgo?: string
}

export enum UptimeStatus {
  Online = 1,
  Offline = 2,
  Timeout = 3,
  Error = 4
}

export interface ChangeDetection {
  id: string
  monitorId: string
  changeType: ChangeType
  previousContentHash?: string
  currentContentHash: string
  changeDescription?: string
  detectedAt: string
  changeTypeDescription?: string
  hasSignificantChange?: boolean
  detectedAtFormatted?: string
  timeAgo?: string
}

export enum ChangeType {
  ContentChanged = 1,
  StructureChanged = 2,
  StatusChanged = 3
}

export interface MonitorStatusDto {
  status: MonitorStatus
  lastResponseTime?: number
  lastCheckedAt?: string
  lastChangeDetectedAt?: string
  statusDescription?: string
  lastResponseTimeFormatted?: string
  lastCheckedAtFormatted?: string
  timeSinceLastCheck?: string
  lastChangeDetectedAtFormatted?: string
  timeSinceLastChange?: string
  hasRecentChanges?: boolean
}
