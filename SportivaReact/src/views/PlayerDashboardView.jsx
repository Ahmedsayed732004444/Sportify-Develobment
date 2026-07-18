import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Calendar, Trophy, Bell, ArrowRight, Star, Heart, Activity, Search } from 'lucide-react';
import { apiFetch } from '../services/api';
import { useLanguage } from '../contexts/LanguageContext';

export default function PlayerDashboardView({ user, addToast }) {
  const navigate = useNavigate();
  const { t } = useLanguage();
  const [bookings, setBookings] = useState([]);
  const [matches, setMatches] = useState([]);
  const [notifications, setNotifications] = useState([]);
  const [favorites, setFavorites] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      // 1. Fetch Bookings
      const bookingsRes = await apiFetch('/bookings/my-bookings');
      if (bookingsRes.ok) {
        const data = await bookingsRes.json();
        setBookings((data.items || []).slice(0, 3));
      }

      // 2. Fetch Friendly Matches
      const matchesRes = await apiFetch('/friendly-matches');
      if (matchesRes.ok) {
        const data = await matchesRes.json();
        // Filter for matches organized by the user or where the user is a participant if backend exposes it,
        // otherwise display matches organized by the user or the top upcoming matches.
        const allMatches = data.items || [];
        const userMatches = allMatches.filter(m => m.organizer?.id === user?.id || m.participants?.some(p => p.id === user?.id));
        setMatches(userMatches.length > 0 ? userMatches.slice(0, 3) : allMatches.slice(0, 3));
      }

      // 3. Fetch Notifications
      const notifsRes = await apiFetch('/notifications');
      if (notifsRes.ok) {
        const data = await notifsRes.json();
        setNotifications((data.items || []).slice(0, 4));
      }

      // 4. Load favorites from localStorage
      const favs = JSON.parse(localStorage.getItem('fav_courts') || '[]');
      setFavorites(favs.slice(0, 3));

    } catch (e) {
      addToast('Error syncing dashboard feeds.', 'error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-[1200px] mx-auto flex flex-col gap-10 py-6 animate-fade-in text-xs text-[#a1a1aa]">
      {/* Header section with Welcome Card */}
      <div className="bg-gradient-to-r from-[#121216]/90 to-[#0e0e12]/80 border border-white/5 rounded-3xl p-8 flex flex-col md:flex-row justify-between items-start md:items-center gap-6 relative overflow-hidden shadow-2xl">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top_left,_var(--tw-gradient-stops))] from-[#84cc16]/10 via-transparent to-transparent pointer-events-none"></div>
        <div className="flex flex-col gap-2 relative z-10 w-full text-left rtl:text-right">
          <span className="text-xs font-bold text-[#84cc16] uppercase tracking-wider">{t('welcomeBack')}</span>
          <h1 className="text-2xl md:text-4xl font-black text-white tracking-tight">
            {t('captain')}, {user?.name || 'Player'}
          </h1>
          <p className="text-xs text-[#71717a] max-w-2xl leading-relaxed">
            {t('dashboardDesc')}
          </p>
        </div>

        {/* Quick actions panel */}
        <div className="flex flex-wrap gap-3 relative z-10 shrink-0">
          <button 
            onClick={() => navigate('/courts')} 
            className="px-5 py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all shadow-md flex items-center gap-2"
          >
            <Search className="w-4 h-4" /> Book New Court
          </button>
          <button 
            onClick={() => navigate('/friendly-matches')} 
            className="px-5 py-3 bg-white/5 hover:bg-white/10 border border-white/10 text-white font-semibold rounded-xl transition-all flex items-center gap-2"
          >
            <Trophy className="w-4 h-4 text-[#84cc16]" /> Find Matches
          </button>
        </div>
      </div>

      {/* Main Grid: Left 2 cols bookings & matches, Right 1 col notifications & favorites */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        {/* Left Column (Span 2) */}
        <div className="lg:col-span-2 flex flex-col gap-8">
          
          {/* Upcoming Bookings */}
          <div className="bg-[#121216]/50 border border-white/5 rounded-3xl p-6 md:p-8 flex flex-col gap-6">
            <div className="flex justify-between items-center">
              <div>
                <h3 className="text-base font-black text-white">Upcoming Bookings</h3>
                <p className="text-[10px] text-[#71717a] mt-0.5">Your active reservations at sports complexes</p>
              </div>
              <button 
                onClick={() => navigate('/bookings')} 
                className="text-xs font-bold text-[#84cc16] hover:underline flex items-center gap-1"
              >
                View Grid <ArrowRight className="w-3.5 h-3.5" />
              </button>
            </div>

            <div className="flex flex-col gap-4">
              {bookings.length > 0 ? (
                bookings.map(b => (
                  <div 
                    key={b.id} 
                    className="flex justify-between items-center p-4 border border-white/5 bg-[#1e1e26]/30 rounded-2xl hover:border-white/10 transition-all"
                  >
                    <div className="flex items-center gap-4">
                      <div className="w-10 h-10 rounded-xl bg-[#84cc16]/10 flex items-center justify-center text-[#84cc16] shrink-0">
                        <Calendar className="w-5 h-5" />
                      </div>
                      <div>
                        <h4 className="font-bold text-white text-xs">{b.courtName}</h4>
                        <p className="text-[10px] text-[#71717a] mt-1">
                          {b.bookingDate} | {b.startTime?.substring(0, 5)} - {b.endTime?.substring(0, 5)}
                        </p>
                      </div>
                    </div>
                    <span className={`px-3 py-1 text-[9px] uppercase font-black tracking-wider rounded-lg ${
                      b.status === 'Confirmed' ? 'bg-[#10b981]/15 text-[#10b981]' : b.status === 'Cancelled' ? 'bg-red-500/15 text-red-500' : 'bg-amber-500/15 text-amber-500'
                    }`}>
                      {b.status}
                    </span>
                  </div>
                ))
              ) : (
                <div className="text-center py-8 bg-[#1e1e26]/20 border border-dashed border-white/5 rounded-2xl flex flex-col items-center gap-2">
                  <Calendar className="w-8 h-8 text-[#71717a]/50" />
                  <p className="text-[11px] text-[#71717a]">No active reservations scheduled.</p>
                </div>
              )}
            </div>
          </div>

          {/* Upcoming Matches */}
          <div className="bg-[#121216]/50 border border-white/5 rounded-3xl p-6 md:p-8 flex flex-col gap-6">
            <div className="flex justify-between items-center">
              <div>
                <h3 className="text-base font-black text-white">Upcoming Friendly Matches</h3>
                <p className="text-[10px] text-[#71717a] mt-0.5">Lobbies you are hosting or have joined</p>
              </div>
              <button 
                onClick={() => navigate('/friendly-matches')} 
                className="text-xs font-bold text-[#84cc16] hover:underline flex items-center gap-1"
              >
                Find Lobbies <ArrowRight className="w-3.5 h-3.5" />
              </button>
            </div>

            <div className="flex flex-col gap-4">
              {matches.length > 0 ? (
                matches.map(m => (
                  <div 
                    key={m.matchId} 
                    onClick={() => navigate(`/friendly-match/${m.matchId}`)}
                    className="flex justify-between items-center p-4 border border-white/5 bg-[#1e1e26]/30 rounded-2xl hover:border-[#84cc16]/20 cursor-pointer transition-all"
                  >
                    <div className="flex items-center gap-4">
                      <div className="w-10 h-10 rounded-xl bg-[#84cc16]/10 flex items-center justify-center text-[#84cc16] shrink-0">
                        <Trophy className="w-5 h-5" />
                      </div>
                      <div>
                        <h4 className="font-bold text-white text-xs">Organized by {m.organizer?.name || 'Player'}</h4>
                        <p className="text-[10px] text-[#71717a] mt-1">
                          {m.sportType} • {m.date}
                        </p>
                      </div>
                    </div>
                    <span className="text-[10px] font-bold text-amber-400">
                      {m.slotsRemaining} slots remaining
                    </span>
                  </div>
                ))
              ) : (
                <div className="text-center py-8 bg-[#1e1e26]/20 border border-dashed border-white/5 rounded-2xl flex flex-col items-center gap-2">
                  <Trophy className="w-8 h-8 text-[#71717a]/50" />
                  <p className="text-[11px] text-[#71717a]">No active match lobbies.</p>
                </div>
              )}
            </div>
          </div>

        </div>

        {/* Right Column (Span 1) */}
        <div className="flex flex-col gap-8">
          
          {/* Notifications feed */}
          <div className="bg-[#121216]/50 border border-white/5 rounded-3xl p-6 flex flex-col gap-6">
            <div className="flex justify-between items-center">
              <h3 className="text-base font-black text-white flex items-center gap-2">
                <Bell className="w-5 h-5 text-[#84cc16]" /> Recent Activity
              </h3>
              <button 
                onClick={() => navigate('/notifications')} 
                className="text-[10px] font-bold text-[#84cc16] hover:underline"
              >
                Clear All
              </button>
            </div>

            <div className="flex flex-col gap-3">
              {notifications.length > 0 ? (
                notifications.map(n => (
                  <div key={n.id} className="p-3 bg-[#1e1e26]/20 border border-white/5 rounded-xl flex flex-col gap-1">
                    <p className="text-[11px] text-[#fafafa] leading-relaxed">{n.message}</p>
                    <span className="text-[9px] text-[#71717a] mt-1">{new Date(n.createdAt).toLocaleDateString()}</span>
                  </div>
                ))
              ) : (
                <p className="text-[11px] text-[#71717a] text-center py-4">No recent alert notifications.</p>
              )}
            </div>
          </div>

          {/* Saved / Favorite Courts */}
          <div className="bg-[#121216]/50 border border-white/5 rounded-3xl p-6 flex flex-col gap-6">
            <h3 className="text-base font-black text-white flex items-center gap-2">
              <Heart className="w-5 h-5 text-red-500 fill-red-500" /> Saved Courts
            </h3>

            <div className="flex flex-col gap-3">
              {favorites.length > 0 ? (
                favorites.map(fav => (
                  <div 
                    key={fav.id} 
                    onClick={() => navigate('/clubs')}
                    className="p-3 bg-[#1e1e26]/20 border border-white/5 rounded-xl flex items-center gap-3 cursor-pointer hover:border-[#84cc16]/20 transition-all"
                  >
                    <img 
                      className="w-10 h-10 rounded-lg object-cover bg-white/5 shrink-0" 
                      src={fav.logoUrl || 'https://images.unsplash.com/photo-1541534741688-6078c6bfb5c5?auto=format&fit=crop&w=80&q=80'} 
                      alt="" 
                    />
                    <div className="flex-1 min-w-0">
                      <h4 className="font-bold text-white text-xs truncate">{fav.name}</h4>
                      <div className="flex items-center gap-1 mt-0.5 text-[9px] text-[#71717a]">
                        {fav.rating ? (
                          <>
                            <Star className="w-3 h-3 fill-amber-400 text-amber-400" />
                            <span>{fav.rating}</span>
                            <span>•</span>
                          </>
                        ) : null}
                        <span>{fav.city}</span>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className="text-center py-6 bg-[#1e1e26]/20 border border-dashed border-white/5 rounded-xl flex flex-col items-center gap-1.5">
                  <Heart className="w-6 h-6 text-[#71717a]/40" />
                  <p className="text-[10px] text-[#71717a]">Your saved venues will appear here.</p>
                </div>
              )}
            </div>
          </div>

        </div>

      </div>
    </div>
  );
}
