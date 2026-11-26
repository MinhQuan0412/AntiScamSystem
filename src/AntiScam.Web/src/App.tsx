import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { AuthProvider } from './contexts/AuthContext';
import { Layout } from './components/layout/Layout';
import { ProtectedRoute } from './components/layout/ProtectedRoute';

import { HomePage } from './pages/HomePage';
import { ClassifyPage } from './pages/ClassifyPage';
import { PhoneReportPage } from './pages/PhoneReportPage';
import { SafeRoutePage } from './pages/SafeRoutePage';
import { ScenariosPage } from './pages/ScenariosPage';
import { EmergencyContactsPage } from './pages/EmergencyContactsPage';
import { LoginPage, RegisterPage } from './pages/AuthPages';
import { AdminDashboardPage } from './pages/admin/DashboardPage';

export function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Toaster position="top-right" toastOptions={{ duration: 4000 }} />
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<HomePage />} />
            <Route path="classify" element={<ClassifyPage />} />
            <Route path="phone-lookup" element={<PhoneReportPage />} />
            <Route path="safe-route" element={<SafeRoutePage />} />
            <Route path="scenarios" element={<ScenariosPage />} />
            <Route path="emergency" element={<EmergencyContactsPage />} />
            <Route path="login" element={<LoginPage />} />
            <Route path="register" element={<RegisterPage />} />

            {/* Admin Routes */}
            <Route
              path="admin/dashboard"
              element={
                <ProtectedRoute requireAdmin={true}>
                  <AdminDashboardPage />
                </ProtectedRoute>
              }
            />

            <Route path="*" element={<Navigate to="/" replace />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
