import { Outlet } from 'react-router-dom'
import Navbar from './Navbar'

export default function Layout() {
  return (
    <div className="min-h-screen flex flex-col">
      <Navbar />
      <main className="flex-1">
        <Outlet />
      </main>
      <footer className="bg-gray-900 text-gray-400 text-xs text-center py-4 px-4 mt-auto">
        © 2026 AntiScam System – Hệ thống cảnh báo lừa đảo & an toàn du lịch
      </footer>
    </div>
  )
}
