// ── Auth ──────────────────────────────────────────────────────────
export interface LoginRequest  { email: string; password: string }
export interface RegisterRequest {
  email: string; password: string; confirmPassword: string; fullName: string; phone?: string
}
export interface AuthResponse {
  accessToken: string; refreshToken: string; expiresAt: string
  userId: string; fullName: string; email: string; role: string
}

// ── Classification ────────────────────────────────────────────────
export interface ClassifyRequest  { content: string; consentToStore?: boolean }
export interface ClassifyResponse {
  isScam: boolean
  scamType?: string; scamTypeName?: string
  confidence: number
  matchedSignals: string[]
  preventionAdvice: string
  relatedScenarios: ScamScenario[]
  disclaimer: string
}

// ── Phone Report ──────────────────────────────────────────────────
export interface PhoneReportLookup {
  maskedNumber: string; numberType: string; bankName?: string
  reportCount: number; uniqueReporterCount: number; firstSeenAt: string
  verificationStatus: string
  riskScore: { score: number; level: string; levelName: string; explanation: string; factors: Record<string,number> }
  needsAttention: boolean
  recentReports: PhoneReportDetail[]
}
export interface PhoneReportDetail {
  id: string; description?: string; scamType?: string; createdAt: string; reporterName: string
}
export interface CreatePhoneReportRequest {
  reportedNumber: string; numberType: 'Phone'|'BankAccount'; bankName?: string
  scamType?: string; description: string; evidenceUrl?: string
}

// ── Scam Scenario ─────────────────────────────────────────────────
export interface ScamScenario {
  id: string; title: string; scamType: number; scamTypeName: string
  description: string; recognitionSigns: string[]; anonymizedExample?: string
  preventionAdvice: string; status: number; statusName: string
  parentScenarioId?: string; childScenariosCount: number; createdAt: string
}
export interface PagedResult<T> {
  items: T[]; totalCount: number; page: number; pageSize: number
  totalPages: number; hasPreviousPage: boolean; hasNextPage: boolean
}

// ── Emergency Contact ─────────────────────────────────────────────
export interface EmergencyContact {
  id: string; name: string; phoneNumber: string; category: string
  provinceCode?: string; provinceName?: string; description?: string; isNational: boolean
}

// ── Travel Route ─────────────────────────────────────────────────
export interface Waypoint { latitude: number; longitude: number; name?: string; order: number }
export interface TravelRoute {
  id: string; userId: string; routeName: string; status: string
  waypoints: Waypoint[]; deviationThresholdMeters: number
  isActive: boolean; startedAt?: string; endedAt?: string; notes?: string
}
export interface LocationReportResult {
  isDeviating: boolean; currentDistanceMeters: number
  alertId?: string; consecutiveDeviationCount: number; message: string
}

// ── Statistics ────────────────────────────────────────────────────
export interface ScamStatistics {
  totalScamsDetected: number
  totalByType: Record<string, number>
  topReportedNumbers: PhoneReportDto[]
  modelPerformanceSummary: ModelVersion[]
}
export interface PhoneReportDto {
  id: string; reportedNumber: string; numberType: string
  reportCount: number; riskScore: number; verificationStatus: string
}
export interface ModelVersion {
  id: string; versionLabel: string; modelType: string; isActive: boolean
  accuracy?: number; precision?: number; recall?: number; f1Score?: number; createdAt: string
}

// ── Relative Contact ──────────────────────────────────────────────
export interface RelativeContact {
  id: string; name: string; phoneNumber?: string; email?: string
  telegramChatId?: string; contactType: string; notes?: string
}
