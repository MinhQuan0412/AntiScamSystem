interface User { userId: string; email: string; role: string; fullName?: string }

export function getUser(): User | null {
  try { return JSON.parse(localStorage.getItem('user') || 'null') } catch { return null }
}
export function getToken(): string | null {
  return localStorage.getItem('access_token')
}
export function isAdmin(): boolean {
  return getUser()?.role?.toLowerCase() === 'admin'
}
export function isLoggedIn(): boolean {
  return !!getToken()
}
export function saveAuth(token: string, user: User) {
  localStorage.setItem('access_token', token)
  localStorage.setItem('user', JSON.stringify(user))
}
export function clearAuth() {
  localStorage.removeItem('access_token')
  localStorage.removeItem('user')
}
