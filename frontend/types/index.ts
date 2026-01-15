// Type Definitions

export interface Monitor {
  Id: string
  Name: string
  Url: string
  Status: MonitorStatus
  StatusDescription: string
  CheckIntervalSeconds: number
  CheckIntervalFormatted: string
  MonitorUptime: boolean
  MonitorChanges: boolean
  CreatedAt: string
  CreatedAtFormatted: string
  CreatedAtTimeAgo: string
  UpdatedAt: string
  UpdatedAtFormatted: string
  UpdatedAtTimeAgo: string
  LastCheckedAt?: string
  LastCheckedAtFormatted?: string
  LastCheckedAtTimeAgo?: string
}

export enum MonitorStatus {
  Active = 1,
  Inactive = 2,
  Paused = 3
}

export interface UptimeCheck {
  Id: string
  MonitorId: string
  Status: UptimeStatus
  StatusDescription: string
  IsOnline: boolean
  ResponseTimeMs?: number
  ResponseTimeFormatted?: string
  StatusCode?: number
  StatusCodeDescription?: string
  ErrorMessage?: string
  CheckedAt: string
  CheckedAtFormatted: string
  TimeAgo: string
}

export enum UptimeStatus {
  Online = 1,
  Offline = 2,
  Timeout = 3,
  Error = 4
}

export interface ChangeDetection {
  Id: string
  MonitorId: string
  ChangeType: ChangeType
  ChangeTypeDescription: string
  PreviousContentHash?: string
  CurrentContentHash?: string
  ChangeDescription?: string
  HasSignificantChange: boolean
  DetectedAt: string
  DetectedAtFormatted: string
  TimeAgo: string
}

export enum ChangeType {
  ContentChanged = 1,
  StructureChanged = 2,
  StatusChanged = 3
}

export interface MonitorStatusDto {
  MonitorId: string
  MonitorName: string
  Url: string
  CurrentStatus?: UptimeStatus
  IsOnline: boolean
  StatusDescription?: string
  LastResponseTimeMs?: number
  LastResponseTimeFormatted?: string
  LastCheckedAt?: string
  LastCheckedAtFormatted?: string
  TimeSinceLastCheck?: string
  LastChangeDetectedAt?: string
  LastChangeDetectedAtFormatted?: string
  TimeSinceLastChange?: string
  HasRecentChanges: boolean
  UptimePercentage?: number
}
