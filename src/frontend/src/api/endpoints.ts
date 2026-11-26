import api from './client'
import axios from 'axios'

// ── Auth ──────────────────────────────────────────────────────────────────────
export const authApi = {
  login:    (email: string, password: string) =>
    api.post('/auth/login', { email, password }),
  register: (email: string, password: string, confirmPassword: string, fullName: string) =>
    api.post('/auth/register', { email, password, confirmPassword, fullName }),
  me:       () => api.get('/auth/me'),
}

// ── Classification ────────────────────────────────────────────────────────────
export const classifyApi = {
  classifyText: (content: string, consentToStore = false) =>
    api.post('/classification/classify', { content, consentToStore }),

  classifyImage: (file: File, consentToStore = false) => {
    const form = new FormData()
    form.append('image', file)
    form.append('consentToStore', String(consentToStore))
    return api.post('/classification/classify-image', form, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
  },

  classifyChat: (messages: { sender: string; text: string }[], consentToStore = false) =>
    api.post('/classification/classify-chat', { messages, consentToStore }),

  getModels: () => api.get('/classification/models'),
}

// ── Phone Reports ─────────────────────────────────────────────────────────────
export const phoneApi = {
  lookup: (number: string) =>
    api.get('/phone-reports/lookup', { params: { number } }),
  report: (data: object) =>
    api.post('/phone-reports', data),
  getPending: (page = 1, pageSize = 10) =>
    api.get('/phone-reports/pending-moderation', { params: { page, pageSize } }),
  moderate: (detailId: string, approve: boolean, note: string) =>
    api.post(`/phone-reports/moderate/${detailId}`, { approve, note }),
}

// ── Scam Scenarios ────────────────────────────────────────────────────────────
export const scenarioApi = {
  getAll:   (page = 1, pageSize = 10, type?: string) =>
    api.get('/scam-scenarios', { params: { page, pageSize, type } }),
  getById:  (id: string) => api.get(`/scam-scenarios/${id}`),
  search:   (q: string)  => api.get('/scam-scenarios/search', { params: { q } }),
  create:   (data: object) => api.post('/scam-scenarios', data),
  update:   (id: string, data: object) => api.put(`/scam-scenarios/${id}`, data),
  delete:   (id: string) => api.delete(`/scam-scenarios/${id}`),
}

// ── Emergency Contacts ────────────────────────────────────────────────────────
export const emergencyApi = {
  getNational:   () => api.get('/emergency-contacts/national'),
  getByProvince: (provinceCode?: string) =>
    api.get('/emergency-contacts', { params: { provinceCode } }),
}

// ── Travel Routes ─────────────────────────────────────────────────────────────
export const travelApi = {
  create:          (data: object) => api.post('/travel-routes', data),
  getMyRoutes:     () => api.get('/travel-routes'),
  getActive:       () => api.get('/travel-routes/active'),
  start:           (id: string)  => api.post(`/travel-routes/${id}/start`),
  stop:            (id: string)  => api.post(`/travel-routes/${id}/stop`),
  reportLocation:  (data: object) => api.post('/travel-routes/report-location', data),
  respondToAlert:  (data: object) => api.post('/travel-routes/alerts/respond', data),
  sos:             (id: string)  => api.post(`/travel-routes/${id}/sos`),
}

// ── Relative Contacts ─────────────────────────────────────────────────────────
export const relativeApi = {
  getAll:  () => api.get('/relative-contacts'),
  create:  (data: object) => api.post('/relative-contacts', data),
  update:  (id: string, data: object) => api.put(`/relative-contacts/${id}`, data),
  delete:  (id: string) => api.delete(`/relative-contacts/${id}`),
}

// ── Statistics ────────────────────────────────────────────────────────────────
export const statsApi = {
  get:        (from?: string, to?: string) =>
    api.get('/statistics', { params: { from, to } }),
  exportExcel: () =>
    api.get('/statistics/export/excel', { responseType: 'blob' }),
  exportPdf:   () =>
    api.get('/statistics/export/pdf', { responseType: 'blob' }),
}
