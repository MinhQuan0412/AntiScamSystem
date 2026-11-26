import { createContext, useContext, useState, useCallback, type ReactNode } from 'react'
import { authApi } from '../lib/api'
import type { LoginRequest, RegisterRequest, AuthResponse } from '../types'

interface AuthUser { userId: string; fullName: string; email: string; role: string }

interface AuthContextValue {
  user: AuthUser | null
  isAuthenticated: boolean
  isAdmin: boolean
  login:    (data: LoginRequest)    => Promise<void>
  register: (data: RegisterRequest) => Promise<void>
  logout:   () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

function loadUser(): AuthUser | null {
  try { return JSON.parse(localStorage.getItem('user') ?? 'null') }
  catch { return null }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(loadUser)

  const saveAuth = useCallback((data: AuthResponse) => {
    localStorage.setItem('accessToken', data.accessToken)
    const u = { userId: data.userId, fullName: data.fullName, email: data.email, role: data.role }
    localStorage.setItem('user', JSON.stringify(u))
    setUser(u)
  }, [])

  const login = useCallback(async (data: LoginRequest) => {
    const res = await authApi.login(data)
    saveAuth(res.data)
  }, [saveAuth])

  const register = useCallback(async (data: RegisterRequest) => {
    const res = await authApi.register(data)
    saveAuth(res.data)
  }, [saveAuth])

  const logout = useCallback(() => {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('user')
    setUser(null)
  }, [])

  return (
    <AuthContext.Provider value={{
      user, isAuthenticated: !!user, isAdmin: user?.role === 'Admin',
      login, register, logout,
    }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider')
  return ctx
}
