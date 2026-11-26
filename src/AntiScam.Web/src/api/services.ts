import api from './client';
import {
  ApiResponse,
  LoginResponse,
  ScamScenarioDto,
  PhoneReportLookupResponse,
  EmergencyContactDto,
  RelativeContactDto,
  TravelRouteDto,
  LocationReportResult,
  ClassifyMessageResponse,
  ScamStatisticsDto,
  PagedResult,
  PhoneReportDetailDto,
} from '../types';

// Auth API
export const authApi = {
  login: async (email: string, password: string): Promise<ApiResponse<LoginResponse>> => {
    const res = await api.post('/auth/login', { email, password });
    return res.data;
  },
  register: async (data: { email: string; password: string; confirmPassword: string; fullName: string; phone?: string }) => {
    const res = await api.post('/auth/register', data);
    return res.data;
  },
  getCurrentUser: async () => {
    const res = await api.get('/auth/me');
    return res.data;
  },
};

// AI Classification API
export const classificationApi = {
  classify: async (content: string, consentToStore = true): Promise<ApiResponse<ClassifyMessageResponse>> => {
    const res = await api.post('/classification/classify', { content, consentToStore });
    return res.data;
  },
  getModels: async () => {
    const res = await api.get('/classification/models');
    return res.data;
  },
};

// Phone & Bank Report API
export const phoneReportApi = {
  lookup: async (number: string): Promise<ApiResponse<PhoneReportLookupResponse>> => {
    const res = await api.get('/phone-reports/lookup', { params: { number } });
    return res.data;
  },
  report: async (data: { reportedNumber: string; numberType: number; bankName?: string; scamType?: number; description: string; evidenceUrl?: string }) => {
    const res = await api.post('/phone-reports', data);
    return res.data;
  },
  getPendingModeration: async (page = 1, pageSize = 20): Promise<PagedResult<PhoneReportDetailDto>> => {
    const res = await api.get('/phone-reports/pending-moderation', { params: { page, pageSize } });
    return res.data;
  },
  moderate: async (detailId: string, approve: boolean, note: string) => {
    const res = await api.post(`/phone-reports/moderate/${detailId}`, { approve, note });
    return res.data;
  },
};

// Safe Routes API
export const travelRouteApi = {
  getActive: async (): Promise<ApiResponse<TravelRouteDto | null>> => {
    const res = await api.get('/travel-routes/active');
    return res.data;
  },
  getUserRoutes: async (): Promise<ApiResponse<TravelRouteDto[]>> => {
    const res = await api.get('/travel-routes');
    return res.data;
  },
  createRoute: async (data: { routeName: string; waypoints: { latitude: number; longitude: number; name?: string; order: number }[]; deviationThresholdMeters?: number; notes?: string }) => {
    const res = await api.post('/travel-routes', data);
    return res.data;
  },
  startTracking: async (routeId: string) => {
    const res = await api.post(`/travel-routes/${routeId}/start`);
    return res.data;
  },
  stopTracking: async (routeId: string) => {
    const res = await api.post(`/travel-routes/${routeId}/stop`);
    return res.data;
  },
  reportLocation: async (data: { travelRouteId: string; latitude: number; longitude: number; accuracy?: number; speed?: number; bearing?: number }): Promise<ApiResponse<LocationReportResult>> => {
    const res = await api.post('/travel-routes/report-location', data);
    return res.data;
  },
  respondToAlert: async (data: { alertId: string; action: number; newWaypoints?: { latitude: number; longitude: number; order: number }[] }) => {
    const res = await api.post('/travel-routes/alerts/respond', data);
    return res.data;
  },
  triggerSos: async (routeId: string) => {
    const res = await api.post(`/travel-routes/${routeId}/sos`);
    return res.data;
  },
};

// Scam Scenarios API
export const scamScenarioApi = {
  getAll: async (page = 1, pageSize = 10, type?: number, status?: number): Promise<PagedResult<ScamScenarioDto>> => {
    const res = await api.get('/scam-scenarios', { params: { page, pageSize, type, status } });
    return res.data;
  },
  getById: async (id: string): Promise<ApiResponse<ScamScenarioDto>> => {
    const res = await api.get(`/scam-scenarios/${id}`);
    return res.data;
  },
  search: async (q: string): Promise<ApiResponse<ScamScenarioDto[]>> => {
    const res = await api.get('/scam-scenarios/search', { params: { q } });
    return res.data;
  },
};

// Emergency & Relative Contacts API
export const emergencyContactApi = {
  getNational: async (): Promise<ApiResponse<EmergencyContactDto[]>> => {
    const res = await api.get('/emergency-contacts/national');
    return res.data;
  },
  getByProvince: async (provinceCode?: string): Promise<ApiResponse<EmergencyContactDto[]>> => {
    const res = await api.get('/emergency-contacts', { params: { provinceCode } });
    return res.data;
  },
  getUserRelatives: async (): Promise<ApiResponse<RelativeContactDto[]>> => {
    const res = await api.get('/relative-contacts');
    return res.data;
  },
  createRelative: async (data: { name: string; phoneNumber?: string; email?: string; telegramChatId?: string; contactType?: string; notes?: string }) => {
    const res = await api.post('/relative-contacts', data);
    return res.data;
  },
  deleteRelative: async (id: string) => {
    const res = await api.delete(`/relative-contacts/${id}`);
    return res.data;
  },
};

// Statistics API
export const statisticsApi = {
  getStats: async (from?: string, to?: string): Promise<ApiResponse<ScamStatisticsDto>> => {
    const res = await api.get('/statistics', { params: { from, to } });
    return res.data;
  },
  exportExcel: async (from?: string, to?: string) => {
    const res = await api.get('/statistics/export/excel', { params: { from, to }, responseType: 'blob' });
    return res.data;
  },
  exportPdf: async (from?: string, to?: string) => {
    const res = await api.get('/statistics/export/pdf', { params: { from, to }, responseType: 'blob' });
    return res.data;
  },
};

