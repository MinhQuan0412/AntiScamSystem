import { Routes, Route, Navigate } from 'react-router-dom'
import Layout from './components/Layout'
import HomePage           from './pages/HomePage'
import ClassifyPage       from './pages/ClassifyPage'
import LookupPage         from './pages/LookupPage'
import ScenariosPage      from './pages/ScenariosPage'
import ScenarioDetailPage from './pages/ScenarioDetailPage'
import EmergencyPage      from './pages/EmergencyPage'
import TravelPage         from './pages/TravelPage'
import LoginPage          from './pages/LoginPage'
import RegisterPage       from './pages/RegisterPage'
import AdminPage          from './pages/AdminPage'

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/"             element={<HomePage />} />
        <Route path="/classify"     element={<ClassifyPage />} />
        <Route path="/lookup"       element={<LookupPage />} />
        <Route path="/scenarios"    element={<ScenariosPage />} />
        <Route path="/scenarios/:id" element={<ScenarioDetailPage />} />
        <Route path="/emergency"    element={<EmergencyPage />} />
        <Route path="/travel"       element={<TravelPage />} />
        <Route path="/login"        element={<LoginPage />} />
        <Route path="/register"     element={<RegisterPage />} />
        <Route path="/admin"        element={<AdminPage />} />
        <Route path="*"             element={<Navigate to="/" replace />} />
      </Routes>
    </Layout>
  )
}
