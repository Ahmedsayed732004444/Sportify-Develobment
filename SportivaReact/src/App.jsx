import React, { useState, useEffect } from 'react';
import { HashRouter, Routes, Route, useNavigate, useLocation, Navigate } from 'react-router-dom';
import { SocketProvider, useSocket } from './contexts/SocketContext';
import { LanguageProvider, useLanguage } from './contexts/LanguageContext';
import { apiFetch, getApiBaseUrl } from './services/api';
import {
  Building, Calendar, Trophy, MessageSquare, Bell, LogOut,
  Search, Wifi, WifiOff, Sparkles, User, MessageCircle, Shield, Settings, Home, LogIn,
  LayoutDashboard, Store
} from 'lucide-react';

// Subviews Import
import LoginView from './views/LoginView';
import HomeView from './views/HomeView';
import ClubsView from './views/ClubsView';
import BookingsView from './views/BookingsView';
import MatchesView from './views/MatchesView';
import TournamentsView from './views/TournamentsView';
import SocialView from './views/SocialView';
import ChatView from './views/ChatView';
import NotificationsView from './views/NotificationsView';
import ProfileView from './views/ProfileView';
import BecomeOwnerView from './views/BecomeOwnerView';
import AdminDashboardView from './views/AdminDashboardView';
import AdminLayout from './views/admin/AdminLayout';
import AdminProfileView from './views/admin/AdminProfileView';
import OwnerProfileView from './views/owner/OwnerProfileView';
import OwnerLayout from './views/owner/OwnerLayout';
import OwnerDashboardHome from './views/owner/OwnerDashboardHome';
import OwnerTodayView from './views/owner/OwnerTodayView';
import OwnerClubsView from './views/owner/OwnerClubsView';
import OwnerCourtsView from './views/owner/OwnerCourtsView';
import OwnerScheduleView from './views/owner/OwnerScheduleView';
import OwnerBookingsView from './views/owner/OwnerBookingsView';
import OwnerMatchesView from './views/owner/OwnerMatchesView';
import OwnerTournamentsView from './views/owner/OwnerTournamentsView';
import OwnerReviewsView from './views/owner/OwnerReviewsView';
import OwnerSubscriptionView from './views/owner/OwnerSubscriptionView';
import SettingsView from './views/SettingsView';
import ClubDetailView from './views/ClubDetailView';
import CourtDetailView from './views/CourtDetailView';
import MatchDetailView from './views/MatchDetailView';
import TournamentDetailView from './views/TournamentDetailView';
import CourtsView from './views/CourtsView';
import PlayerDashboardView from './views/PlayerDashboardView';

// ────────────────────────────────────────────────────────────────
// Routes that ALWAYS use the public (no-sidebar) marketplace layout,
// regardless of whether the user is authenticated or not.
// ────────────────────────────────────────────────────────────────
const MARKETPLACE_ROUTES = [
  '/home', '/courts', '/clubs', '/club/',
  '/friendly-matches', '/friendly-match/',
  '/tournaments', '/tournament/',
  '/social', '/pricing', '/become-owner'
];

function isMarketplaceRoute(pathname) {
  if (pathname === '/' || pathname === '/home') return true;
  return MARKETPLACE_ROUTES.some(r => pathname.startsWith(r));
}

function DashboardLayout({ token, onLogout, user, onTriggerAuth, authModalOpen, handleLoginSuccess, pendingRedirect, setPendingRedirect }) {
  const { t, isRtl } = useLanguage();
  const socketCtx = useSocket();
  const notifConnected = socketCtx?.notifConnected || false;
  const chatConnected = socketCtx?.chatConnected || false;

  const navigate = useNavigate();
  const location = useLocation();

  const [toasts, setToasts] = useState([]);
  const [notifCounters, setNotifCounters] = useState({ unreadCount: 0 });
  const [upgradeRequestStatus, setUpgradeRequestStatus] = useState(null);

  const addToast = (msg, type = 'info') => {
    const id = Date.now();
    setToasts(prev => [...prev, { id, msg, type }]);
    setTimeout(() => {
      setToasts(prev => prev.filter(t => t.id !== id));
    }, 4000);
  };

  useEffect(() => {
    if (user) {
      loadNotificationsCount();
      fetchMembershipUpgradeStatus();
    }
  }, [location.pathname, user]);

  // Auth gate / role redirect logic
  useEffect(() => {
    const protectedPaths = ['/bookings', '/messages', '/notifications', '/profile', '/settings', '/owner', '/admin', '/player'];
    const path = location.pathname;

    if (!user && protectedPaths.some(p => path.startsWith(p))) {
      navigate('/home');
      onTriggerAuth(path);
      addToast('Authentication is required to perform this action.', 'info');
      return;
    } else if (user) {
      if (path.startsWith('/owner') && user.role !== 'Owner') {
        navigate('/home');
        addToast('Access denied: Owner role required.', 'error');
        return;
      }
      if (path.startsWith('/admin') && user.role !== 'Admin') {
        navigate('/home');
        addToast('Access denied: Admin role required.', 'error');
        return;
      }

      // Persona strict separation redirect gates:
      if (user.role === 'Owner') {
        if (path === '/settings') { navigate('/owner/settings'); return; }
        if (path === '/profile') { navigate('/owner/profile'); return; }
        if (path === '/messages') { navigate('/owner/dashboard'); addToast('Messaging is not available for Owners.', 'error'); return; }
        if (path === '/notifications') { navigate('/owner/notifications'); return; }
      }
      if (user.role === 'Admin') {
        if (path === '/settings') { navigate('/admin/settings'); return; }
        if (path === '/profile') { navigate('/admin/profile'); return; }
        if (path === '/messages') { navigate('/admin/messages'); return; }
        if (path === '/notifications') { navigate('/admin/notifications'); return; }
      }
    }
  }, [location.pathname, user]);

  const loadNotificationsCount = async () => {
    try {
      const res = await apiFetch('/notifications/counters');
      if (res.ok) {
        const data = await res.json();
        setNotifCounters(data);
      }
    } catch (e) { }
  };

  const fetchMembershipUpgradeStatus = async () => {
    if (user.role !== 'Member' && user.role !== 'Player') return;
    try {
      const res = await apiFetch('/me/membership-request');
      if (res.ok) {
        const data = await res.json();
        setUpgradeRequestStatus(data.status);
      }
    } catch (e) { }
  };

  const handleTabClick = (path) => {
    const protectedPaths = ['/bookings', '/messages', '/notifications', '/profile', '/settings', '/player/dashboard', '/owner/dashboard', '/admin/dashboard'];
    if (!user && protectedPaths.includes(path)) {
      onTriggerAuth();
      addToast('Authentication is required to perform this action.', 'info');
      return;
    }
    navigate(path);
  };

  // Check path matching for highlights
  const isTabActive = (path) => {
    if (path === '/home') {
      return location.pathname === '/home' || location.pathname === '/';
    }
    return location.pathname.startsWith(path);
  };

  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isProfileDropdownOpen, setIsProfileDropdownOpen] = useState(false);

  const handleLoginSuccessWithRedirect = (newToken) => {
    handleLoginSuccess(newToken);

    try {
      const payload = JSON.parse(atob(newToken.split('.')[1]));
      let role = payload.role || 'Player';
      if (payload.roles) {
        try {
          const rolesArray = typeof payload.roles === 'string' ? JSON.parse(payload.roles) : payload.roles;
          if (Array.isArray(rolesArray) && rolesArray.length > 0) role = rolesArray[0];
        } catch (e) {}
      }
      if (role === 'Member') role = 'Player';

      if (pendingRedirect) {
        navigate(pendingRedirect);
        setPendingRedirect(null);
      } else {
        if (role === 'Admin') {
          navigate('/admin/dashboard');
        } else if (role === 'Owner') {
          navigate('/owner/dashboard');
        } else {
          navigate('/player/dashboard');
        }
      }
    } catch (e) {
      navigate('/home');
    }
  };

  // ══════════════════════════════════════════════════════════════
  //  Viewport routes mapping — shared across ALL layouts
  // ══════════════════════════════════════════════════════════════
  const viewportContent = (
    <Routes>
      <Route path="/" element={<Navigate to="/home" replace />} />
      <Route path="/home" element={<HomeView onNavigate={handleTabClick} user={user} onTriggerAuth={onTriggerAuth} addToast={addToast} />} />
      <Route path="/courts" element={<CourtsView addToast={addToast} />} />
      <Route path="/clubs" element={<ClubsView addToast={addToast} />} />
      <Route path="/club/:id" element={<ClubDetailView addToast={addToast} />} />
      <Route path="/club/:clubId/court/:courtId" element={<CourtDetailView addToast={addToast} onTriggerAuth={onTriggerAuth} />} />
      <Route path="/bookings" element={<BookingsView addToast={addToast} />} />
      <Route path="/friendly-matches" element={<MatchesView user={user} addToast={addToast} />} />
      <Route path="/friendly-match/:id" element={<MatchDetailView user={user} addToast={addToast} onTriggerAuth={onTriggerAuth} />} />
      <Route path="/tournaments" element={<TournamentsView addToast={addToast} />} />
      <Route path="/tournament/:id" element={<TournamentDetailView user={user} addToast={addToast} />} />
      <Route path="/social" element={<SocialView addToast={addToast} />} />
      <Route path="/messages" element={<ChatView user={user} addToast={addToast} />} />
      <Route path="/notifications" element={<NotificationsView addToast={addToast} />} />
      <Route path="/profile" element={<ProfileView user={user} addToast={addToast} />} />
      <Route path="/settings" element={<SettingsView addToast={addToast} />} />
      <Route path="/become-owner" element={<BecomeOwnerView addToast={addToast} initialStatus={upgradeRequestStatus} onStatusChange={(status) => setUpgradeRequestStatus(status)} user={user} onTriggerAuth={onTriggerAuth} />} />
      <Route path="/pricing" element={<Navigate to="/become-owner" replace />} />
      <Route path="/player/dashboard" element={<PlayerDashboardView user={user} addToast={addToast} />} />
      <Route path="/owner" element={<OwnerLayout user={user} onLogout={onLogout} addToast={addToast} />}>
        <Route path="dashboard" element={<OwnerDashboardHome />} />
        <Route path="today" element={<OwnerTodayView addToast={addToast} />} />
        <Route path="clubs" element={<OwnerClubsView addToast={addToast} />} />
        <Route path="courts" element={<OwnerCourtsView addToast={addToast} />} />
        <Route path="courts/:courtId/schedule" element={<OwnerScheduleView addToast={addToast} />} />
        <Route path="bookings" element={<OwnerBookingsView addToast={addToast} />} />
        <Route path="matches" element={<OwnerMatchesView addToast={addToast} />} />
        <Route path="tournaments" element={<OwnerTournamentsView addToast={addToast} />} />
        <Route path="reviews" element={<OwnerReviewsView addToast={addToast} />} />
        <Route path="subscription" element={<OwnerSubscriptionView addToast={addToast} />} />
        <Route path="profile" element={<OwnerProfileView user={user} addToast={addToast} />} />
        <Route path="settings" element={<SettingsView addToast={addToast} />} />
        <Route path="notifications" element={<NotificationsView addToast={addToast} />} />
      </Route>
      <Route path="/admin" element={<AdminLayout user={user} onLogout={onLogout} addToast={addToast} />}>
        <Route path="dashboard" element={<AdminDashboardView addToast={addToast} />} />
        <Route path="profile" element={<AdminProfileView user={user} addToast={addToast} />} />
        <Route path="settings" element={<SettingsView addToast={addToast} />} />
        <Route path="messages" element={<ChatView user={user} addToast={addToast} />} />
        <Route path="notifications" element={<NotificationsView addToast={addToast} />} />
      </Route>
    </Routes>
  );

  // ══════════════════════════════════════════════════════════════
  //  Toast Stack — shared across all layouts
  // ══════════════════════════════════════════════════════════════
  const toastStack = (
    <div className="fixed top-8 right-8 z-[100] flex flex-col gap-3">
      {toasts.map(t => (
        <div key={t.id} className={`flex items-center gap-3 px-5 py-3.5 rounded-xl border backdrop-blur-md shadow-2xl transition-all duration-300 min-w-[320px] ${t.type === 'success' ? 'border-[#10b981] bg-[#10b981]/15 text-[#10b981]' : t.type === 'error' ? 'border-[#ef4444] bg-[#ef4444]/15 text-[#ef4444]' : 'border-[#84cc16] bg-[#84cc16]/15 text-[#84cc16]'
          }`}>
          <span className="text-sm font-semibold">{t.msg}</span>
        </div>
      ))}
    </div>
  );

  // ══════════════════════════════════════════════════════════════
  //  Auth Modal — shared across all layouts
  // ══════════════════════════════════════════════════════════════
  const authModal = authModalOpen && (
    <div className="fixed inset-0 bg-black/80 backdrop-blur-md z-[110] flex items-center justify-center p-4">
      <div className="relative max-w-[450px] w-full">
        <button onClick={() => onTriggerAuth()} className="absolute top-6 right-6 text-[#71717a] hover:text-white text-lg font-bold z-10 transition-colors">✕</button>
        <LoginView onLoginSuccess={handleLoginSuccessWithRedirect} />
      </div>
    </div>
  );

  // ══════════════════════════════════════════════════════════════
  //  LAYOUT 1: Owner & Admin Portals
  //  OwnerLayout and AdminLayout have their own sidebars — bypass.
  // ══════════════════════════════════════════════════════════════
  const isOwnerRoute = location.pathname.startsWith('/owner');
  if (user && user.role === 'Owner' && isOwnerRoute) {
    return (
      <div className="flex h-screen w-screen bg-[#09090b]">
        {toastStack}
        <div className="flex-1 h-screen w-screen overflow-hidden">
          {viewportContent}
        </div>
        {authModal}
      </div>
    );
  }

  const isAdminRoute = location.pathname.startsWith('/admin');
  if (user && user.role === 'Admin' && isAdminRoute) {
    return (
      <div className="flex h-screen w-screen bg-[#09090b]">
        {toastStack}
        <div className="flex-1 h-screen w-screen overflow-hidden">
          {viewportContent}
        </div>
        {authModal}
      </div>
    );
  }

  // ══════════════════════════════════════════════════════════════
  //  LAYOUT 2: Public Marketplace  (/home, /courts, /clubs, etc.)
  //  No sidebar. Horizontal navbar. Works for BOTH guest & auth.
  // ══════════════════════════════════════════════════════════════
  const showMarketplace = isMarketplaceRoute(location.pathname);

  if (showMarketplace) {
    return (
      <div className="flex h-screen w-screen bg-[#09090b]">
        {toastStack}

        <div className="flex-1 flex flex-col h-screen overflow-hidden bg-[#0c0c0f]">
          {/* Public Horizontal Navbar */}
          <header className="h-20 border-b border-[#ffffff08] flex items-center justify-between px-6 md:px-12 backdrop-blur-md bg-[#121216]/50 shrink-0 relative z-30">
            <div className="flex items-center gap-10">
              <div className="flex items-center gap-3 cursor-pointer" onClick={() => navigate('/home')}>
                <div className="w-9 h-9 rounded-xl bg-[#84cc16] flex items-center justify-center text-black font-extrabold text-lg shadow-lg shadow-[#84cc16]/25">S</div>
                <h2 className="text-xl font-bold tracking-tight text-[#f4f4f5]">Sportify</h2>
              </div>

              <nav className="hidden md:flex items-center gap-6">
                <button onClick={() => navigate('/home')} className={`text-xs font-bold transition-all ${isTabActive('/home') ? 'text-[#84cc16]' : 'text-[#a1a1aa] hover:text-white'}`}>{t('home')}</button>
                <button onClick={() => navigate('/courts')} className={`text-xs font-bold transition-all ${isTabActive('/courts') ? 'text-[#84cc16]' : 'text-[#a1a1aa] hover:text-white'}`}>{t('courts')}</button>
                <button onClick={() => navigate('/clubs')} className={`text-xs font-bold transition-all ${isTabActive('/clubs') ? 'text-[#84cc16]' : 'text-[#a1a1aa] hover:text-white'}`}>{t('clubs')}</button>
                <button onClick={() => navigate('/friendly-matches')} className={`text-xs font-bold transition-all ${isTabActive('/friendly-matches') ? 'text-[#84cc16]' : 'text-[#a1a1aa] hover:text-white'}`}>{t('matches')}</button>
                <button onClick={() => navigate('/tournaments')} className={`text-xs font-bold transition-all ${isTabActive('/tournaments') ? 'text-[#84cc16]' : 'text-[#a1a1aa] hover:text-white'}`}>{t('tournaments')}</button>
                <button onClick={() => navigate('/pricing')} className={`text-xs font-bold transition-all ${isTabActive('/pricing') ? 'text-[#84cc16]' : 'text-[#a1a1aa] hover:text-white'}`}>{t('becomeOwner')}</button>
              </nav>
            </div>

            <div className="flex items-center gap-4">
              {user ? (
                /* Authenticated user on marketplace — show avatar + dashboard link */
                <div className="flex items-center gap-3">
                  <button
                    onClick={() => navigate(user.role === 'Owner' ? '/owner/dashboard' : user.role === 'Admin' ? '/admin/dashboard' : '/player/dashboard')}
                    className="hidden md:flex px-4 py-2 bg-white/5 hover:bg-white/10 border border-white/10 text-white font-bold text-xs rounded-xl transition-all items-center gap-1.5"
                  >
                    <LayoutDashboard className="w-4 h-4" /> {t('myDashboard')}
                  </button>
                  <div className="relative">
                    <button
                      onClick={() => setIsProfileDropdownOpen(!isProfileDropdownOpen)}
                      className="flex items-center gap-2 p-1.5 hover:bg-white/5 rounded-2xl transition-all border border-white/5 cursor-pointer"
                    >
                      <img className="w-8 h-8 rounded-full border border-white/10 object-cover" src="https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=80&q=80" alt="avatar" />
                      <span className="hidden sm:inline text-xs font-bold text-[#fafafa] pr-1">{user.name}</span>
                    </button>

                    {isProfileDropdownOpen && (
                      <div className="absolute right-0 rtl:right-auto rtl:left-0 mt-3 w-52 bg-[#121216] border border-white/5 rounded-2xl p-2 shadow-2xl flex flex-col gap-0.5 z-50">
                        {user.role === 'Admin' ? (
                          <>
                            <button onClick={() => { navigate('/admin/profile'); setIsProfileDropdownOpen(false); }} className="w-full text-left rtl:text-right px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all">
                              {t('adminProfile')}
                            </button>
                            <button onClick={() => { navigate('/admin/dashboard'); setIsProfileDropdownOpen(false); }} className="w-full text-left rtl:text-right px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all">
                              {t('adminDashboard')}
                            </button>
                          </>
                        ) : user.role === 'Owner' ? (
                          <>
                            <button onClick={() => { navigate('/owner/profile'); setIsProfileDropdownOpen(false); }} className="w-full text-left rtl:text-right px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all">
                              {t('businessProfile')}
                            </button>
                            <button onClick={() => { navigate('/owner/dashboard'); setIsProfileDropdownOpen(false); }} className="w-full text-left rtl:text-right px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all">
                              {t('ownerDashboard')}
                            </button>
                            <button onClick={() => { navigate('/owner/subscription'); setIsProfileDropdownOpen(false); }} className="w-full text-left rtl:text-right px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all">
                              {t('subscription')}
                            </button>
                          </>
                        ) : (
                          <>
                            <button onClick={() => { navigate('/profile'); setIsProfileDropdownOpen(false); }} className="w-full text-left rtl:text-right px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all">
                              {t('myProfile')}
                            </button>
                            <button onClick={() => { navigate('/player/dashboard'); setIsProfileDropdownOpen(false); }} className="w-full text-left rtl:text-right px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all">
                              {t('myDashboard')}
                            </button>
                            <button onClick={() => { navigate('/bookings'); setIsProfileDropdownOpen(false); }} className="w-full text-left rtl:text-right px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all">
                              {t('bookings')}
                            </button>
                          </>
                        )}
                        <button onClick={() => { navigate(user.role === 'Owner' ? '/owner/settings' : user.role === 'Admin' ? '/admin/settings' : '/settings'); setIsProfileDropdownOpen(false); }} className="w-full text-left rtl:text-right px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all">
                          {t('settings')}
                        </button>
                        <div className="h-px bg-white/5 my-1"></div>
                        <button onClick={() => { onLogout(); setIsProfileDropdownOpen(false); }} className="w-full text-left rtl:text-right px-4 py-2.5 hover:bg-red-500/10 hover:text-red-500 rounded-xl text-xs font-semibold text-[#a1a1aa] transition-all">
                          {t('logout')}
                        </button>
                      </div>
                    )}
                  </div>
                </div>
              ) : (
                /* Guest — show sign in button */
                <button onClick={() => onTriggerAuth('/player/dashboard')} className="hidden md:flex px-5 py-2.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg items-center gap-1.5">
                  <LogIn className="w-4 h-4" /> {t('signInJoin')}
                </button>
              )}

              {/* Mobile Hamburger toggle */}
              <button onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)} className="md:hidden text-[#fafafa] p-2 hover:bg-white/5 rounded-lg transition-all">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  {isMobileMenuOpen ? (
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  ) : (
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                  )}
                </svg>
              </button>
            </div>

            {/* Mobile slide-down menu */}
            {isMobileMenuOpen && (
              <div className="md:hidden absolute top-20 left-0 right-0 bg-[#121216] border-b border-[#ffffff08] p-6 flex flex-col gap-4 animate-fade-in shadow-2xl z-25">
                <button onClick={() => { navigate('/home'); setIsMobileMenuOpen(false); }} className="text-left text-xs font-bold text-[#fafafa] py-2">Home</button>
                <button onClick={() => { navigate('/courts'); setIsMobileMenuOpen(false); }} className="text-left text-xs font-bold text-[#fafafa] py-2">Search</button>
                <button onClick={() => { navigate('/home'); setIsMobileMenuOpen(false); }} className="text-left text-xs font-bold text-[#fafafa] py-2">{t('home')}</button>
                <button onClick={() => { navigate('/courts'); setIsMobileMenuOpen(false); }} className="text-left text-xs font-bold text-[#fafafa] py-2">{t('courts')}</button>
                <button onClick={() => { navigate('/clubs'); setIsMobileMenuOpen(false); }} className="text-left text-xs font-bold text-[#fafafa] py-2">{t('clubs')}</button>
                <button onClick={() => { navigate('/friendly-matches'); setIsMobileMenuOpen(false); }} className="text-left text-xs font-bold text-[#fafafa] py-2">{t('matches')}</button>
                <button onClick={() => { navigate('/tournaments'); setIsMobileMenuOpen(false); }} className="text-left text-xs font-bold text-[#fafafa] py-2">{t('tournaments')}</button>
                <button onClick={() => { navigate('/pricing'); setIsMobileMenuOpen(false); }} className="text-left text-xs font-bold text-[#fafafa] py-2">{t('becomeOwner')}</button>
                {user ? (
                  <>
                    <div className="h-px bg-white/5 my-1"></div>
                    <button onClick={() => { navigate(user.role === 'Owner' ? '/owner/dashboard' : user.role === 'Admin' ? '/admin/dashboard' : '/player/dashboard'); setIsMobileMenuOpen(false); }} className="text-left text-xs font-bold text-[#84cc16] py-2">{t('myDashboard')}</button>
                    <button onClick={() => { onLogout(); setIsMobileMenuOpen(false); }} className="text-left text-xs font-bold text-red-400 py-2">{t('logout')}</button>
                  </>
                ) : (
                  <button onClick={() => { onTriggerAuth('/player/dashboard'); setIsMobileMenuOpen(false); }} className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl text-center shadow-lg mt-2">
                    {t('signInJoin')}
                  </button>
                )}
              </div>
            )}
          </header>

          {/* Marketplace Main Content Viewport */}
          <div className="flex-1 p-6 md:p-12 overflow-y-auto flex flex-col min-h-0 bg-[#060608]/40">
            {viewportContent}
          </div>
        </div>

        {authModal}
      </div>
    );
  }

  // ══════════════════════════════════════════════════════════════
  //  LAYOUT 3: Authenticated Dashboard  (/player/*, /bookings,
  //  /messages, /notifications, /profile, /settings, /admin/*)
  //  Full sidebar + header layout.
  // ══════════════════════════════════════════════════════════════
  return (
    <div className="flex h-screen w-screen bg-[#09090b]">
      {toastStack}

      {isMobileMenuOpen && (
        <div className="md:hidden fixed inset-0 bg-black/60 z-20" onClick={() => setIsMobileMenuOpen(false)}></div>
      )}

      <aside className={`bg-[#121216]/70 ${isRtl ? 'border-l' : 'border-r'} border-[#ffffff08] flex flex-col p-6 backdrop-blur-lg justify-between shrink-0 transition-all duration-300 relative z-30 ${isSidebarCollapsed ? 'w-[84px] items-center' : 'w-[280px]'
        } ${isMobileMenuOpen ? `fixed inset-y-0 ${isRtl ? 'right-0' : 'left-0'} w-[280px] translate-x-0 bg-[#121216]` : 'hidden md:flex translate-x-0'
        }`}>
        <div className="flex flex-col gap-10 w-full">
          <div className="flex items-center justify-between w-full">
            <div className="flex items-center gap-3 cursor-pointer" onClick={() => navigate('/home')}>
              <div className="w-9 h-9 rounded-xl bg-[#84cc16] flex items-center justify-center text-black font-extrabold text-lg shadow-lg shadow-[#84cc16]/25 shrink-0">S</div>
              {!isSidebarCollapsed && <h2 className="text-xl font-bold tracking-tight text-[#f4f4f5] truncate">Sportify</h2>}
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

          <nav className="flex flex-col gap-1.5 w-full">
            {/* ROLE DASHBOARD */}
            <button onClick={() => handleTabClick(user?.role === 'Admin' ? '/admin/dashboard' : user?.role === 'Owner' ? '/owner/dashboard' : '/player/dashboard')} className={`flex items-center gap-4 px-4 py-3.5 rounded-xl font-bold text-xs transition-all duration-300 w-full ${isSidebarCollapsed ? 'justify-center' : ''} ${isTabActive(user?.role === 'Admin' ? '/admin' : user?.role === 'Owner' ? '/owner' : '/player') ? `bg-[#84cc16]/15 text-[#84cc16] ${isRtl ? 'border-r-4' : 'border-l-4'} border-[#84cc16]` : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'
              }`}>
              <LayoutDashboard className="w-5 h-5 shrink-0" />
              {!isSidebarCollapsed && <span>{t('dashboard')}</span>}
            </button>

            <button onClick={() => handleTabClick('/courts')} className={`flex items-center gap-4 px-4 py-3.5 rounded-xl font-bold text-xs transition-all duration-300 w-full ${isSidebarCollapsed ? 'justify-center' : ''} ${isTabActive('/courts') ? `bg-[#84cc16]/15 text-[#84cc16] ${isRtl ? 'border-r-4' : 'border-l-4'} border-[#84cc16]` : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'}`}>
              <Search className="w-5 h-5 shrink-0" />
              {!isSidebarCollapsed && <span>{t('exploreCourts')}</span>}
            </button>

            {user?.role !== 'Admin' && (
              <button onClick={() => handleTabClick('/bookings')} className={`flex items-center gap-4 px-4 py-3.5 rounded-xl font-bold text-xs transition-all duration-300 w-full ${isSidebarCollapsed ? 'justify-center' : ''} ${isTabActive('/bookings') ? `bg-[#84cc16]/15 text-[#84cc16] ${isRtl ? 'border-r-4' : 'border-l-4'} border-[#84cc16]` : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'}`}>
                <Calendar className="w-5 h-5 shrink-0" />
                {!isSidebarCollapsed && <span>{t('bookings')}</span>}
              </button>
            )}

            <button onClick={() => handleTabClick('/friendly-matches')} className={`flex items-center gap-4 px-4 py-3.5 rounded-xl font-bold text-xs transition-all duration-300 w-full ${isSidebarCollapsed ? 'justify-center' : ''} ${isTabActive('/friendly-matches') ? `bg-[#84cc16]/15 text-[#84cc16] ${isRtl ? 'border-r-4' : 'border-l-4'} border-[#84cc16]` : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'}`}>
              <Trophy className="w-5 h-5 shrink-0" />
              {!isSidebarCollapsed && <span>{t('friendlyMatches')}</span>}
            </button>

            <button onClick={() => handleTabClick('/tournaments')} className={`flex items-center gap-4 px-4 py-3.5 rounded-xl font-bold text-xs transition-all duration-300 w-full ${isSidebarCollapsed ? 'justify-center' : ''} ${isTabActive('/tournaments') ? `bg-[#84cc16]/15 text-[#84cc16] ${isRtl ? 'border-r-4' : 'border-l-4'} border-[#84cc16]` : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'}`}>
              <Trophy className="w-5 h-5 shrink-0" />
              {!isSidebarCollapsed && <span>{t('tournaments')}</span>}
            </button>

            <button onClick={() => handleTabClick('/messages')} className={`flex items-center gap-4 px-4 py-3.5 rounded-xl font-bold text-xs transition-all duration-300 w-full ${isSidebarCollapsed ? 'justify-center' : ''} ${isTabActive('/messages') ? `bg-[#84cc16]/15 text-[#84cc16] ${isRtl ? 'border-r-4' : 'border-l-4'} border-[#84cc16]` : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'}`}>
              <MessageSquare className="w-5 h-5 shrink-0" />
              {!isSidebarCollapsed && <span>{t('messages')}</span>}
            </button>

            <button onClick={() => handleTabClick('/notifications')} className={`flex items-center gap-4 px-4 py-3.5 rounded-xl font-bold text-xs transition-all duration-300 w-full ${isSidebarCollapsed ? 'justify-center' : ''} ${isTabActive('/notifications') ? `bg-[#84cc16]/15 text-[#84cc16] ${isRtl ? 'border-r-4' : 'border-l-4'} border-[#84cc16]` : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'}`}>
              <Bell className="w-5 h-5 shrink-0" />
              {!isSidebarCollapsed && (
                <div className="flex items-center justify-between w-full">
                  <span>{t('notifications')}</span>
                  {notifCounters.unreadCount > 0 && (
                    <span className="px-1.5 py-0.5 text-[8px] font-bold bg-[#ef4444] text-white rounded-full leading-none">{notifCounters.unreadCount}</span>
                  )}
                </div>
              )}
            </button>

            <button onClick={() => handleTabClick('/profile')} className={`flex items-center gap-4 px-4 py-3.5 rounded-xl font-bold text-xs transition-all duration-300 w-full ${isSidebarCollapsed ? 'justify-center' : ''} ${isTabActive('/profile') ? `bg-[#84cc16]/15 text-[#84cc16] ${isRtl ? 'border-r-4' : 'border-l-4'} border-[#84cc16]` : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'}`}>
              <User className="w-5 h-5 shrink-0" />
              {!isSidebarCollapsed && <span>{t('profile')}</span>}
            </button>

            <button onClick={() => handleTabClick('/settings')} className={`flex items-center gap-4 px-4 py-3.5 rounded-xl font-bold text-xs transition-all duration-300 w-full ${isSidebarCollapsed ? 'justify-center' : ''} ${isTabActive('/settings') ? `bg-[#84cc16]/15 text-[#84cc16] ${isRtl ? 'border-r-4' : 'border-l-4'} border-[#84cc16]` : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'}`}>
              <Settings className="w-5 h-5 shrink-0" />
              {!isSidebarCollapsed && <span>{t('settings')}</span>}
            </button>
          </nav>
        </div>

        <div className={`flex items-center gap-3 pt-6 border-t border-[#ffffff08] mt-6 w-full ${isSidebarCollapsed ? 'flex-col items-center gap-4' : ''}`}>
          <div className="relative shrink-0 cursor-pointer" onClick={() => navigate('/profile')}>
            <img className="w-10 h-10 rounded-full border border-white/10 object-cover" src={user?.profilePictureUrl || 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=80&q=80'} alt="avatar" />
            <span className="absolute bottom-0 right-0 w-2.5 h-2.5 bg-[#10b981] rounded-full border-2 border-[#09090b]"></span>
          </div>
          {!isSidebarCollapsed && (
            <div className="flex-1 min-w-0">
              <h4 className="text-xs font-semibold truncate text-white">{user?.name}</h4>
              <span className="text-[10px] font-bold uppercase tracking-wider text-[#84cc16]">{user?.role}</span>
            </div>
          )}
          <button onClick={onLogout} className="text-[#71717a] hover:text-[#ef4444] transition-colors duration-300 p-1.5 hover:bg-white/5 rounded-xl shrink-0">
            <LogOut className="w-5 h-5" />
          </button>
        </div>
      </aside>

      {/* Authenticated viewport container */}
      <main className="flex-1 flex flex-col h-screen overflow-hidden bg-[#0c0c0f]">
        {/* Header */}
        <header className="h-20 border-b border-[#ffffff08] flex items-center justify-between px-6 md:px-12 backdrop-blur-md shrink-0">

          {/* Mobile hamburger menu button + search */}
          <div className="flex items-center gap-4">
            <button onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)} className="md:hidden text-[#fafafa] p-2 hover:bg-white/5 rounded-lg transition-all shrink-0">
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>

            <div className="hidden sm:flex items-center gap-3 bg-[#1e1e26]/40 border border-[#ffffff08] rounded-full px-5 py-2.5 w-[320px]">
              <Search className="w-4 h-4 text-[#71717a]" />
              <input className="bg-transparent border-none outline-none text-xs text-[#fafafa] w-full" type="text" placeholder="Search facilities, matches..." />
            </div>
          </div>

          {/* Status Icons & Profile Dropdown */}
          <div className="flex items-center gap-6 relative">

            {/* Messages Icon */}
            <button onClick={() => navigate('/messages')} className="relative p-2 text-[#a1a1aa] hover:text-white transition-all hover:bg-white/5 rounded-xl shrink-0">
              <MessageSquare className="w-5 h-5" />
              <span className={`absolute top-0 right-0 w-2 h-2 rounded-full border border-[#0c0c0f] ${chatConnected ? 'bg-[#10b981]' : 'bg-[#ef4444]'}`}></span>
            </button>

            {/* Notifications Bell Icon */}
            <button onClick={() => navigate('/notifications')} className="relative p-2 text-[#a1a1aa] hover:text-white transition-all hover:bg-white/5 rounded-xl shrink-0">
              <Bell className="w-5 h-5" />
              {notifCounters.unreadCount > 0 && (
                <span className="absolute -top-1 -right-1 px-1.5 py-0.5 text-[8px] font-bold bg-[#ef4444] text-white rounded-full leading-none">
                  {notifCounters.unreadCount}
                </span>
              )}
              <span className={`absolute top-0 right-0 w-2 h-2 rounded-full border border-[#0c0c0f] ${notifConnected ? 'bg-[#10b981]' : 'bg-[#ef4444]'}`}></span>
            </button>

            {/* Profile dropdown container */}
            <div className="relative">
              <button
                onClick={() => setIsProfileDropdownOpen(!isProfileDropdownOpen)}
                className="flex items-center gap-2 p-1.5 hover:bg-white/5 rounded-2xl transition-all border border-white/5 cursor-pointer"
              >
                <img className="w-8 h-8 rounded-full border border-white/10 object-cover" src={user?.profilePictureUrl || 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=80&q=80'} alt="avatar" />
                <span className="hidden sm:inline text-xs font-bold text-[#fafafa] pr-1">{user?.name}</span>
              </button>

              {/* Profile Dropdown Menu */}
              {isProfileDropdownOpen && (
                <div className="absolute right-0 mt-3 w-48 bg-[#121216] border border-white/5 rounded-2xl p-2 shadow-2xl flex flex-col gap-0.5 z-50">
                  <button
                    onClick={() => { navigate('/profile'); setIsProfileDropdownOpen(false); }}
                    className="w-full text-left px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all"
                  >
                    {t('myProfile')}
                  </button>
                  <button
                    onClick={() => { navigate('/player/dashboard'); setIsProfileDropdownOpen(false); }}
                    className="w-full text-left px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all"
                  >
                    {t('myDashboard')}
                  </button>
                  <button
                    onClick={() => { navigate('/bookings'); setIsProfileDropdownOpen(false); }}
                    className="w-full text-left px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all"
                  >
                    {t('bookings')}
                  </button>
                  <button
                    onClick={() => { navigate('/settings'); setIsProfileDropdownOpen(false); }}
                    className="w-full text-left px-4 py-2.5 hover:bg-white/5 rounded-xl text-xs font-semibold text-[#fafafa] transition-all"
                  >
                    {t('settings')}
                  </button>
                  <div className="h-px bg-white/5 my-1"></div>
                  <button
                    onClick={() => { onLogout(); setIsProfileDropdownOpen(false); }}
                    className="w-full text-left px-4 py-2.5 hover:bg-red-500/10 hover:text-red-500 rounded-xl text-xs font-semibold text-[#a1a1aa] transition-all"
                  >
                    {t('logout')}
                  </button>
                </div>
              )}
            </div>

          </div>
        </header>

        <div className="flex-1 p-6 md:p-12 overflow-y-auto flex flex-col min-h-0 bg-[#060608]/40">
          {viewportContent}
        </div>
      </main>

      {authModal}
    </div>
  );
}

export default function App() {
  const [token, setToken] = useState(() => localStorage.getItem('token'));
  const [user, setUser] = useState(() => {
    const savedToken = localStorage.getItem('token');
    if (savedToken) {
      try {
        const payload = JSON.parse(atob(savedToken.split('.')[1]));
        if (payload.exp && payload.exp * 1000 < Date.now()) {
          localStorage.clear();
          return null;
        }
        let role = payload.role || 'Player';
        if (payload.roles) {
          try {
            const rolesArray = typeof payload.roles === 'string' ? JSON.parse(payload.roles) : payload.roles;
            if (Array.isArray(rolesArray) && rolesArray.length > 0) role = rolesArray[0];
          } catch (e) {}
        }
        if (role === 'Member') role = 'Player';
        return {
          id: payload.nameid || payload.sub,
          email: payload.email,
          name: localStorage.getItem('userName') || 'User',
          role: role
        };
      } catch (e) {
        return null;
      }
    }
    return null;
  });
  const [authModalOpen, setAuthModalOpen] = useState(false);
  const [pendingRedirect, setPendingRedirect] = useState(null);

  useEffect(() => {
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        if (payload.exp && payload.exp * 1000 < Date.now()) {
          logout();
          return;
        }
        let role = payload.role || 'Player';
        if (payload.roles) {
          try {
            const rolesArray = typeof payload.roles === 'string' ? JSON.parse(payload.roles) : payload.roles;
            if (Array.isArray(rolesArray) && rolesArray.length > 0) role = rolesArray[0];
          } catch (e) {}
        }
        if (role === 'Member') role = 'Player';
        setUser({
          id: payload.nameid || payload.sub,
          email: payload.email,
          name: localStorage.getItem('userName') || 'User',
          role: role
        });
      } catch (e) {
        logout();
      }
    } else {
      setUser(null);
    }
  }, [token]);

  // Fetch dynamic user profile details whenever user logs in or token changes
  useEffect(() => {
    if (!token || !user || !user.id) return;
    let active = true;
    const fetchUserProfile = async () => {
      try {
        const apiBase = getApiBaseUrl();
        const res = await fetch(`${apiBase}/profiles/${user.id}`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        if (res.ok && active) {
          const data = await res.json();
          setUser(prev => {
            if (!prev) return null;
            return {
              ...prev,
              name: `${data.firstName} ${data.lastName}`,
              profilePictureUrl: data.profilePictureUrl
            };
          });
        }
      } catch (e) {
        console.error("Failed to load user profile in App.jsx root", e);
      }
    };
    fetchUserProfile();
    return () => { active = false; };
  }, [user?.id, token]);

  const logout = () => {
    localStorage.clear();
    setToken(null);
    setUser(null);
    window.location.hash = '/home';
  };

  const handleTriggerAuth = (redirectPath) => {
    if (redirectPath && typeof redirectPath === 'string') {
      setPendingRedirect(redirectPath);
    }
    setAuthModalOpen(!authModalOpen);
  };

  const handleLoginSuccess = (newToken) => {
    setToken(newToken);
    setAuthModalOpen(false);
  };

  const layoutContent = (
    <DashboardLayout
      token={token}
      user={user}
      onLogout={logout}
      setToken={setToken}
      onTriggerAuth={handleTriggerAuth}
      authModalOpen={authModalOpen}
      handleLoginSuccess={handleLoginSuccess}
      pendingRedirect={pendingRedirect}
      setPendingRedirect={setPendingRedirect}
    />
  );

  return (
    <LanguageProvider>
      <HashRouter>
        {token && user ? (
          <SocketProvider token={token}>
            {layoutContent}
          </SocketProvider>
        ) : (
          layoutContent
        )}
      </HashRouter>
    </LanguageProvider>
  );
}
