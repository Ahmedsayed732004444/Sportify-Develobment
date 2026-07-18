import React, { useState } from 'react';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import { Shield, Home, User, LogOut, ArrowLeft, Menu, Settings, MessageSquare, Bell, LayoutDashboard, Users, Building, CreditCard, ShieldAlert, BarChart2 } from 'lucide-react';
import { useLanguage } from '../../contexts/LanguageContext';

export default function AdminLayout({ user, onLogout, addToast }) {
  const { t, isRtl } = useLanguage();
  const navigate = useNavigate();
  const location = useLocation();

  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const isTabActive = (path) => {
    const currentPath = location.pathname + location.search;
    if (path.includes('?')) {
      return currentPath.includes(path);
    }
    if (path === '/admin/dashboard') {
      return location.pathname === '/admin/dashboard' && !location.search;
    }
    return location.pathname.startsWith(path);
  };

  const navLinks = [
    { name: 'Dashboard', path: '/admin/dashboard', icon: LayoutDashboard },
    { name: 'Owner Requests', path: '/admin/dashboard?tab=upgrades', icon: ShieldAlert },
    { name: 'Owners', path: '/admin/dashboard?tab=users', icon: Users },
    { name: 'Players', path: '/admin/dashboard?tab=users', icon: User },
    { name: 'Clubs', path: '/admin/dashboard?tab=clubs', icon: Building },
    { name: 'Reports', path: '/admin/dashboard?tab=reports', icon: BarChart2 },
    { name: 'Subscriptions', path: '/admin/dashboard?tab=upgrades', icon: CreditCard },
    { name: 'Moderation', path: '/admin/dashboard?tab=clubs', icon: Shield },
    { name: 'Analytics', path: '/admin/dashboard?tab=reports', icon: BarChart2 },
    { name: 'Messages', path: '/admin/messages', icon: MessageSquare },
    { name: 'Notifications', path: '/admin/notifications', icon: Bell },
    { name: 'Admin Profile', path: '/admin/profile', icon: User },
    { name: 'Settings', path: '/admin/settings', icon: Settings },
  ];

  return (
    <div className="flex h-screen w-screen bg-[#09090b] text-[#f4f4f5] overflow-hidden">
      {/* Mobile Backdrop */}
      {isMobileMenuOpen && (
        <div className="fixed inset-0 bg-black/60 z-40 md:hidden" onClick={() => setIsMobileMenuOpen(false)}></div>
      )}

      {/* Admin Sidebar */}
      <aside className={`bg-[#121216] ${isRtl ? 'border-l' : 'border-r'} border-white/5 flex flex-col justify-between shrink-0 transition-all duration-300 relative z-50 ${
        isSidebarCollapsed ? 'w-[84px] items-center px-3' : 'w-[280px] px-6'
      } ${isMobileMenuOpen ? `fixed inset-y-0 ${isRtl ? 'right-0' : 'left-0'} w-[280px] translate-x-0 bg-[#121216] py-6` : 'hidden md:flex py-6'}`}>

        <div className="flex flex-col gap-8 w-full min-h-0 overflow-y-auto pr-1">
          {/* Logo / Branding */}
          <div className="flex items-center justify-between w-full">
            <div className="flex items-center gap-3 cursor-pointer" onClick={() => navigate('/home')}>
              <div className="w-9 h-9 rounded-xl bg-[#84cc16] flex items-center justify-center text-black font-extrabold text-lg shadow-lg shadow-[#84cc16]/25 shrink-0">
                A
              </div>
              {!isSidebarCollapsed && (
                <div>
                  <h2 className="text-sm font-bold tracking-tight text-white">System Admin</h2>
                  <p className="text-[10px] text-[#84cc16] font-bold uppercase tracking-wider">Root Panel</p>
                </div>
              )}
            </div>

            {/* Collapse toggle (desktop only) */}
            <button onClick={() => setIsSidebarCollapsed(!isSidebarCollapsed)} className="hidden md:flex text-[#71717a] hover:text-white p-1.5 hover:bg-white/5 rounded-xl transition-all shrink-0">
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                {isSidebarCollapsed ? (
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 5l7 7-7 7M5 5l7 7-7 7" />
                ) : (
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 19l-7-7 7-7M19 19l-7-7 7-7" />
                )}
              </svg>
            </button>
          </div>

          {/* Navigation Links */}
          <nav className="flex flex-col gap-1 w-full">
            {navLinks.map(link => {
              const Icon = link.icon;
              const active = isTabActive(link.path);
              const mapKey = link.name === 'Dashboard' ? 'adminDashboard' :
                             link.name === 'Owner Requests' ? 'ownerRequests' :
                             link.name === 'Owners' ? 'owners' :
                             link.name === 'Players' ? 'players' :
                             link.name === 'Clubs' ? 'clubs' :
                             link.name === 'Reports' ? 'reports' :
                             link.name === 'Subscriptions' ? 'subscriptions' :
                             link.name === 'Moderation' ? 'moderation' :
                             link.name === 'Analytics' ? 'analytics' :
                             link.name === 'Admin Profile' ? 'adminProfile' :
                             link.name === 'Messages' ? 'messages' :
                             link.name === 'Notifications' ? 'notifications' :
                             link.name === 'Settings' ? 'settings' : link.name.toLowerCase();
              const label = t(mapKey);
              return (
                <button
                  key={link.path}
                  onClick={() => {
                    navigate(link.path);
                    setIsMobileMenuOpen(false);
                  }}
                  className={`flex items-center gap-4 px-4 py-3.5 rounded-2xl font-bold text-xs transition-all duration-300 w-full ${
                    isSidebarCollapsed ? 'justify-center' : ''
                  } ${
                    active
                      ? `bg-[#84cc16]/15 text-[#84cc16] ${isRtl ? 'border-r-4' : 'border-l-4'} border-[#84cc16]`
                      : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'
                  }`}
                >
                  <Icon className="w-5 h-5 shrink-0" />
                  {!isSidebarCollapsed && <span>{label}</span>}
                </button>
              );
            })}
          </nav>
        </div>

        {/* Back & Logout Section */}
        <div className="flex flex-col gap-3 pt-6 border-t border-white/5 mt-6 w-full">
          <button
            onClick={() => navigate('/home')}
            className={`flex items-center gap-4 px-4 py-3 text-xs font-bold text-[#a1a1aa] hover:text-white transition-all w-full ${
              isSidebarCollapsed ? 'justify-center' : ''
            }`}
          >
            <ArrowLeft className="w-5 h-5 shrink-0" />
            {!isSidebarCollapsed && <span>{t('backToMarketplace')}</span>}
          </button>

          <div className={`flex items-center gap-3 w-full ${isSidebarCollapsed ? 'flex-col items-center gap-3' : 'justify-between'}`}>
            <div className="flex items-center gap-2.5 truncate cursor-pointer hover:opacity-85 transition-opacity" onClick={() => navigate('/admin/profile')}>
              <img className="w-9 h-9 rounded-full border border-white/10 object-cover" src={user?.profilePictureUrl || 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=80&q=80'} alt="avatar" />
              {!isSidebarCollapsed && (
                <div className="truncate">
                  <h4 className="text-xs font-semibold text-white truncate">{user?.name}</h4>
                  <span className="text-[10px] text-[#71717a] font-bold">Admin Profile</span>
                </div>
              )}
            </div>
            <button onClick={onLogout} className="text-[#71717a] hover:text-[#ef4444] p-1.5 hover:bg-white/5 rounded-xl transition-all shrink-0">
              <LogOut className="w-5 h-5" />
            </button>
          </div>
        </div>
      </aside>

      {/* Main Content Area */}
      <div className="flex-1 flex flex-col h-screen overflow-hidden bg-[#0c0c0f]">
        {/* Admin Header */}
        <header className="h-20 border-b border-white/5 flex items-center justify-between px-6 md:px-12 backdrop-blur-md bg-[#121216]/50 shrink-0">
          <div className="flex items-center gap-4">
            <button
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              className="md:hidden text-white p-2 hover:bg-white/5 rounded-xl transition-all"
            >
              <Menu className="w-6 h-6" />
            </button>
            <h1 className="text-lg font-bold tracking-tight text-white hidden sm:block">
              Control Panel Console
            </h1>
          </div>

          <div className="flex items-center gap-6">
            <div className="hidden md:flex items-center gap-2 bg-[#84cc16]/10 px-3 py-1.5 rounded-full border border-[#84cc16]/20">
              <span className="w-2 h-2 rounded-full bg-[#84cc16] animate-pulse"></span>
              <span className="text-[10px] font-bold text-[#84cc16] tracking-wider uppercase text-[9px]">Root Active Operations</span>
            </div>
          </div>
        </header>

        {/* Viewport Render Area */}
        <main className="flex-1 overflow-y-auto p-6 md:p-10 bg-[#060608]/40">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
