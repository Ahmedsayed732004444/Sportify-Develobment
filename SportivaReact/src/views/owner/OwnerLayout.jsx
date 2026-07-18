import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import { apiFetch } from '../../services/api';
import { useLanguage } from '../../contexts/LanguageContext';
import {
  Building, Calendar, Trophy, MessageSquare, Bell, LogOut,
  Shield, Settings, Home, ArrowLeft, Menu, ChevronDown, Clock, Star, CreditCard, Activity, User
} from 'lucide-react';

export default function OwnerLayout({ user, onLogout, addToast }) {
  const { t, isRtl } = useLanguage();
  const navigate = useNavigate();
  const location = useLocation();

  const [clubs, setClubs] = useState([]);
  const [selectedClub, setSelectedClub] = useState(null);
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isClubDropdownOpen, setIsClubDropdownOpen] = useState(false);

  useEffect(() => {
    loadOwnerClubs();
  }, []);

  const loadOwnerClubs = async () => {
    try {
      const res = await apiFetch('/clubs/me');
      if (res.ok) {
        const data = await res.json();
        const items = data.items || [];
        setClubs(items);
        if (items.length > 0) {
          // Keep previous selection if still valid, or default to first
          const cached = localStorage.getItem('owner_selected_club_id');
          const found = items.find(c => c.id === cached);
          const active = found || items[0];
          setSelectedClub(active);
          localStorage.setItem('owner_selected_club_id', active.id);
        } else {
          setSelectedClub(null);
        }
      }
    } catch (e) {
      addToast('Failed to load your clubs.', 'error');
    }
  };

  const handleClubChange = (club) => {
    setSelectedClub(club);
    localStorage.setItem('owner_selected_club_id', club.id);
    setIsClubDropdownOpen(false);
  };

  const isTabActive = (path) => {
    if (path === '/owner/dashboard') {
      return location.pathname === '/owner/dashboard';
    }
    return location.pathname.startsWith(path);
  };

  const navLinks = [
    { name: 'Overview', path: '/owner/dashboard', icon: Home },
    { name: 'Today', path: '/owner/today', icon: Clock },
    { name: 'My Clubs', path: '/owner/clubs', icon: Building },
    { name: 'Courts', path: '/owner/courts', icon: Shield },
    { name: 'Bookings Grid', path: '/owner/bookings', icon: Calendar },
    { name: 'Matches', path: '/owner/matches', icon: Activity },
    { name: 'Tournaments', path: '/owner/tournaments', icon: Trophy },
    { name: 'Reviews', path: '/owner/reviews', icon: Star },
    { name: 'Subscription', path: '/owner/subscription', icon: CreditCard },
    { name: 'Notifications', path: '/owner/notifications', icon: Bell },
    { name: 'Profile', path: '/owner/profile', icon: User },
    { name: 'Settings', path: '/owner/settings', icon: Settings },
  ];

  return (
    <div className="flex h-screen w-screen bg-[#09090b] text-[#f4f4f5] overflow-hidden">
      {/* Mobile Backdrop */}
      {isMobileMenuOpen && (
        <div className="md:hidden fixed inset-0 bg-black/60 z-40 transition-opacity" onClick={() => setIsMobileMenuOpen(false)}></div>
      )}

      {/* Owner Sidebar */}
      <aside className={`bg-[#121216] ${isRtl ? 'border-l' : 'border-r'} border-white/5 flex flex-col justify-between shrink-0 transition-all duration-300 relative z-50 ${
        isSidebarCollapsed ? 'w-[84px] items-center px-3' : 'w-[280px] px-6'
      } ${isMobileMenuOpen ? `fixed inset-y-0 ${isRtl ? 'right-0' : 'left-0'} w-[280px] translate-x-0 bg-[#121216] py-6` : 'hidden md:flex py-6'}`}>
        
        <div className="flex flex-col gap-8 w-full min-h-0 overflow-y-auto pr-1">
          {/* Logo / Complex Branding */}
          <div className="flex items-center justify-between w-full">
            <div className="flex items-center gap-3 cursor-pointer" onClick={() => navigate('/home')}>
              <div className="w-9 h-9 rounded-xl bg-[#84cc16] flex items-center justify-center text-black font-extrabold text-lg shadow-lg shadow-[#84cc16]/25 shrink-0">
                O
              </div>
              {!isSidebarCollapsed && (
                <div>
                  <h2 className="text-sm font-bold tracking-tight text-white">Owner Portal</h2>
                  <p className="text-[10px] text-[#84cc16] font-bold uppercase tracking-wider">Complex Mgr</p>
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

          {/* Active Club Selector Dropdown */}
          {clubs.length > 0 && !isSidebarCollapsed ? (
            <div className="relative">
              <button
                onClick={() => setIsClubDropdownOpen(!isClubDropdownOpen)}
                className="w-full flex items-center justify-between gap-3 px-4 py-3 bg-white/5 border border-white/10 rounded-2xl text-left text-xs font-semibold hover:bg-white/10 hover:border-white/20 transition-all cursor-pointer"
              >
                <div className="flex items-center gap-2.5 truncate">
                  {selectedClub?.logoUrl ? (
                    <img src={selectedClub.logoUrl} alt="club logo" className="w-6 h-6 rounded-lg object-cover bg-white/5" />
                  ) : (
                    <div className="w-6 h-6 rounded-lg bg-[#84cc16]/20 flex items-center justify-center text-[#84cc16] font-bold">
                      {selectedClub?.name?.charAt(0)}
                    </div>
                  )}
                  <span className="truncate text-white">{selectedClub?.name}</span>
                </div>
                <ChevronDown className="w-4 h-4 text-[#a1a1aa]" />
              </button>

              {isClubDropdownOpen && (
                <div className="absolute top-full left-0 right-0 mt-2 bg-[#1c1c24] border border-white/10 rounded-2xl shadow-2xl p-2 z-50 flex flex-col gap-1">
                  {clubs.map(club => (
                    <button
                      key={club.id}
                      onClick={() => handleClubChange(club)}
                      className={`w-full flex items-center gap-2.5 px-3 py-2 rounded-xl text-left text-xs font-medium transition-all ${
                        selectedClub?.id === club.id ? 'bg-[#84cc16]/10 text-[#84cc16]' : 'text-[#fafafa] hover:bg-white/5'
                      }`}
                    >
                      {club.logoUrl ? (
                        <img src={club.logoUrl} alt="logo" className="w-5 h-5 rounded-md object-cover bg-white/5" />
                      ) : (
                        <div className="w-5 h-5 rounded-md bg-white/10 flex items-center justify-center text-xs font-bold text-white">
                          {club.name.charAt(0)}
                        </div>
                      )}
                      <span className="truncate">{club.name}</span>
                    </button>
                  ))}
                </div>
              )}
            </div>
          ) : clubs.length > 0 && isSidebarCollapsed ? (
            <div className="flex items-center justify-center py-2">
              <div className="w-8 h-8 rounded-lg bg-[#84cc16]/20 flex items-center justify-center text-[#84cc16] font-bold text-sm">
                {selectedClub?.name?.charAt(0)}
              </div>
            </div>
          ) : null}

          {/* Navigation Links */}
          <nav className="flex flex-col gap-1 w-full">
            {navLinks.map(link => {
              const Icon = link.icon;
              const active = isTabActive(link.path);
              const mapKey = link.name === 'Overview' ? 'dashboard' :
                             link.name === 'Today' ? 'todaySchedule' :
                             link.name === 'My Clubs' ? 'clubs' :
                             link.name === 'Courts' ? 'exploreCourts' :
                             link.name === 'Bookings Grid' ? 'bookingsGrid' :
                             link.name === 'Matches' ? 'friendlyMatches' :
                             link.name === 'Tournaments' ? 'tournaments' :
                             link.name === 'Reviews' ? 'reviews' :
                             link.name === 'Subscription' ? 'subscription' :
                             link.name === 'Messages' ? 'messages' :
                             link.name === 'Notifications' ? 'notifications' :
                             link.name === 'Profile' ? 'profile' :
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
            <div className="flex items-center gap-2.5 truncate cursor-pointer hover:opacity-85 transition-opacity" onClick={() => navigate('/owner/profile')}>
              <img className="w-9 h-9 rounded-full border border-white/10 object-cover" src={user?.profilePictureUrl || 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=80&q=80'} alt="avatar" />
              {!isSidebarCollapsed && (
                <div className="truncate">
                  <h4 className="text-xs font-semibold text-white truncate">{user?.name}</h4>
                  <span className="text-[10px] text-[#71717a] font-bold">Owner Profile</span>
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
        {/* Owner Header */}
        <header className="h-20 border-b border-white/5 flex items-center justify-between px-6 md:px-12 backdrop-blur-md bg-[#121216]/50 shrink-0">
          <div className="flex items-center gap-4">
            <button
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              className="md:hidden text-white p-2 hover:bg-white/5 rounded-xl transition-all"
            >
              <Menu className="w-6 h-6" />
            </button>
            <h1 className="text-lg font-bold tracking-tight text-white hidden sm:block">
              {selectedClub ? `Venue Manager: ${selectedClub.name}` : 'Venue Management'}
            </h1>
          </div>

          <div className="flex items-center gap-6">
            <div className="hidden md:flex items-center gap-2 bg-[#84cc16]/10 px-3 py-1.5 rounded-full border border-[#84cc16]/20">
              <span className="w-2 h-2 rounded-full bg-[#84cc16] animate-pulse"></span>
              <span className="text-[10px] font-bold text-[#84cc16] tracking-wider uppercase">Live Operations</span>
            </div>
          </div>
        </header>

        {/* Viewport Render Area */}
        <main className="flex-1 overflow-y-auto p-6 md:p-10 bg-[#060608]/40">
          <Outlet context={{ selectedClub, setSelectedClub, clubs, refreshClubs: loadOwnerClubs }} />
        </main>
      </div>
    </div>
  );
}
