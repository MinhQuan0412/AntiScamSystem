import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  HiShieldCheck,
  HiOutlineSparkles,
  HiOutlinePhone,
  HiOutlineMap,
  HiOutlineBookOpen,
} from 'react-icons/hi';

export const BottomNav: React.FC = () => {
  const navItems = [
    { to: '/', label: 'Trang chủ', icon: HiShieldCheck },
    { to: '/classify', label: 'Quét AI', icon: HiOutlineSparkles },
    { to: '/phone-lookup', label: 'Tra cứu', icon: HiOutlinePhone },
    { to: '/safe-route', label: 'Bản đồ SOS', icon: HiOutlineMap },
    { to: '/scenarios', label: 'Cảnh giác', icon: HiOutlineBookOpen },
  ];

  return (
    <div className="lg:hidden fixed bottom-0 left-0 right-0 z-50 bg-white/95 backdrop-blur-md border-t border-slate-200 px-2 py-1 shadow-lg pb-[env(safe-area-inset-bottom,4px)]">
      <div className="flex items-center justify-around">
        {navItems.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                `flex flex-col items-center justify-center py-1.5 px-2 rounded-xl transition text-[11px] font-medium ${
                  isActive
                    ? 'text-blue-600 font-bold'
                    : 'text-slate-500 hover:text-slate-800'
                }`
              }
            >
              {({ isActive }) => (
                <>
                  <div
                    className={`p-1 rounded-xl transition ${
                      isActive ? 'bg-blue-50 text-blue-600' : ''
                    }`}
                  >
                    <Icon className="w-5 h-5" />
                  </div>
                  <span className="mt-0.5 tracking-tight">{item.label}</span>
                </>
              )}
            </NavLink>
          );
        })}
      </div>
    </div>
  );
};

