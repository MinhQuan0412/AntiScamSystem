// Enums
export enum UserRole {
  User = 0,
  Admin = 1,
}

export enum ScamType {
  BankImpersonation = 0,      // Giả mạo ngân hàng
  EasyJobHighPay = 1,         // Việc nhẹ lương cao
  InvestmentFraud = 2,        // Lừa đảo đầu tư tài chính
  AuthorityImpersonation = 3, // Giả danh cơ quan công an / viện kiểm sát
  RomanceScam = 4,            // Lừa tình cảm
  TravelTicketFraud = 5,      // Lừa vé máy bay / du lịch
  LotteryFraud = 6,           // Lừa trúng thưởng
  OnlineShopping = 7,         // Lừa mua hàng online
  Other = 99,                 // Khác
}

export const ScamTypeNames: Record<ScamType, string> = {
  [ScamType.BankImpersonation]: 'Giả mạo ngân hàng',
  [ScamType.EasyJobHighPay]: 'Việc nhẹ lương cao',
  [ScamType.InvestmentFraud]: 'Lừa đảo đầu tư',
  [ScamType.AuthorityImpersonation]: 'Giả danh cơ quan chức năng',
  [ScamType.RomanceScam]: 'Lừa tình cảm',
  [ScamType.TravelTicketFraud]: 'Vé máy bay / Du lịch',
  [ScamType.LotteryFraud]: 'Trúng thưởng / Quà tặng',
  [ScamType.OnlineShopping]: 'Mua sắm trực tuyến',
  [ScamType.Other]: 'Hình thức khác',
};

export enum ScamStatus {
  Active = 0,
  Evolved = 1,
  Archived = 2,
}

export enum NumberType {
  Phone = 0,
  BankAccount = 1,
}

export enum VerificationStatus {
  Unverified = 0,
  Verified = 1,
  FalseReport = 2,
}

export enum RouteStatus {
  Planned = 0,
  Active = 1,
  Paused = 2,
  Completed = 3,
  Cancelled = 4,
}

export enum AlertType {
  Deviation = 0,
  SOS = 1,
  NoResponse = 2,
}

export enum AlertResolution {
  UserConfirmedSafe = 0,
  UserChangedRoute = 1,
  NotifiedRelatives = 2,
  Expired = 3,
}

export enum RiskLevel {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3,
}

export enum ModelType {
  NaiveBayes = 0,
  LogisticRegression = 1,
  SVM = 2,
  RandomForest = 3,
  DecisionTree = 4,
  ManualNaiveBayes = 5,
}

// Result models
export interface ApiResponse<T> {
  isSuccess: boolean;
  value?: T;
  errors?: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// Auth types
export interface UserInfo {
  id: string;
  fullName: string;
  email: string;
  role: string | UserRole;
  phoneNumber?: string;
  telegramChatId?: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  userId: string;
  fullName: string;
  email: string;
  role: string;
}

// Scam Scenario
export interface ScamScenarioDto {
  id: string;
  title: string;
  scamType: ScamType;
  scamTypeName: string;
  description: string;
  recognitionSigns: string[];
  anonymizedExample?: string;
  preventionAdvice: string;
  status: ScamStatus;
  statusName: string;
  parentScenarioId?: string;
  childScenariosCount: number;
  createdAt: string;
  updatedAt: string;
}

// Phone / Bank Report
export interface RiskScoreDto {
  score: number;
  level: RiskLevel;
  levelName: string;
  explanation: string;
  factors: Record<string, number>;
}

export interface PhoneReportDetailDto {
  id: string;
  phoneReportId: string;
  reportedById: string;
  reporterName: string;
  description?: string;
  scamType?: ScamType;
  evidenceUrl?: string;
  isModerated: boolean;
  isApproved: boolean;
  moderationNote?: string;
  createdAt: string;
}

export interface PhoneReportLookupResponse {
  maskedNumber: string;
  numberType: NumberType;
  bankName?: string;
  reportCount: number;
  uniqueReporterCount: number;
  firstSeenAt: string;
  verificationStatus: VerificationStatus;
  riskScore: RiskScoreDto;
  needsAttention: boolean;
  recentReports: PhoneReportDetailDto[];
}

export interface PhoneReportDto {
  id: string;
  reportedNumber: string;
  numberType: NumberType;
  ownerName?: string;
  bankName?: string;
  reportCount: number;
  uniqueReporterCount: number;
  firstSeenAt: string;
  verificationStatus: VerificationStatus;
  riskScore: number;
  status: string;
}

// Emergency & Relatives
export interface EmergencyContactDto {
  id: string;
  name: string;
  phoneNumber: string;
  category?: string;
  provinceCode?: string;
  provinceName?: string;
  description?: string;
  isNational: boolean;
  verifiedAt?: string;
  verificationSource?: string;
}

export interface RelativeContactDto {
  id: string;
  name: string;
  phoneNumber?: string;
  email?: string;
  telegramChatId?: string;
  contactType: string;
  notes?: string;
  isVerified: boolean;
}

// Safe Route & Tracking
export interface WaypointDto {
  latitude: number;
  longitude: number;
  name?: string;
  order: number;
}

export interface TravelRouteDto {
  id: string;
  userId: string;
  routeName?: string;
  status: RouteStatus;
  waypoints: WaypointDto[];
  deviationThresholdMeters: number;
  isActive: boolean;
  startedAt?: string;
  endedAt?: string;
  notes?: string;
}

export interface LocationReportResult {
  isDeviating: boolean;
  currentDistanceMeters: number;
  alertId?: string;
  consecutiveDeviationCount: number;
  message?: string;
}

// Classification
export interface ClassifyMessageResponse {
  isScam: boolean;
  scamType?: ScamType;
  scamTypeName?: string;
  confidence: number;
  matchedSignals: string[];
  preventionAdvice?: string;
  relatedScenarios?: ScamScenarioDto[];
  disclaimer: string;
}

// Statistics
export interface MonthlyTrendDto {
  month: string;
  count: number;
  byType: Record<string, number>;
}

export interface ScamStatisticsDto {
  totalScamsDetected: number;
  totalByType: Record<string, number>;
  trendByMonth: MonthlyTrendDto[];
  topReportedNumbers: PhoneReportDto[];
  modelPerformanceSummary: ModelVersionDto[];
}

export interface ModelVersionDto {
  id: string;
  versionLabel: string;
  modelType: ModelType;
  modelTypeName: string;
  accuracy: number;
  precision: number;
  recall: number;
  f1Score: number;
  isActive: boolean;
  trainingSampleCount: number;
  trainedAt: string;
  metadata?: Record<string, string>;
}
