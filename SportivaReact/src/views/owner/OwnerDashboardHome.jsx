import React, { useState, useEffect } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import { apiFetch } from '../../services/api';
import { Calendar, Shield, AlertCircle, DollarSign, ArrowRight, User, Clock, Activity, Trophy, Star } from 'lucide-react';

export default function OwnerDashboardHome() {
  const { selectedClub } = useOutletContext();
  const navigate = useNavigate();

  const [stats, setStats] = useState({
    todayRevenue: 0,
    todayBookings: 0,
    occupancyRate: 0,
    pendingBookings: 0,
    totalCourts: 0,
    activeSubscription: null
  });

  const [upcomingMatches, setUpcomingMatches] = useState([]);
  const [upcomingTournaments, setUpcomingTournaments] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (selectedClub) {
      loadDashboardData();
    }
  }, [selectedClub]);

  const loadDashboardData = async () => {
    setLoading(true);
    try {
      const todayStr = new Date().toISOString().split('T')[0];

      // 1. Fetch active subscription
      let activeSub = null;
      try {
        const subRes = await apiFetch(`/clubs/${selectedClub.id}/subscriptions/active`);
        if (subRes.ok) {
          activeSub = await subRes.json();
        }
      } catch (e) {}

      // 2. Load courts count
      const courtsRes = await apiFetch(`/clubs/${selectedClub.id}/courts`);
      let courtsList = [];
      if (courtsRes.ok) {
        courtsList = await courtsRes.json();
      }

      // 3. Load bookings
      const bookingsRes = await apiFetch(`/clubs/${selectedClub.id}/bookings`);
      let bookingsList = [];
      if (bookingsRes.ok) {
        const bookingsData = await bookingsRes.json();
        bookingsList = bookingsData.items || [];
      }

      // Compute bookings count and revenue for TODAY specifically
      const todayBookingsList = bookingsList.filter(b => b.date === todayStr);
      const todayConfirmed = todayBookingsList.filter(b => b.status === 1 || b.status === 'Confirmed');
      const todayRev = todayConfirmed.reduce((sum, b) => sum + (b.totalPrice || 0), 0);

      // Compute pending bookings (overall)
      const pendingCount = bookingsList.filter(b => b.status === 0 || b.status === 'Pending').length;

      // 4. Compute Occupancy Rate for TODAY specifically
      // We load time slots of all courts for today, and find the ratio of booked slots
      let totalSlotsCount = 0;
      let bookedSlotsCount = 0;

      const slotsPromises = courtsList.map(async (court) => {
        try {
          const res = await apiFetch(`/courts/${court.id}/time-slots?date=${todayStr}`);
          if (res.ok) {
            const slots = await res.json();
            totalSlotsCount += slots.length;
            bookedSlotsCount += slots.filter(s => s.isBooked).length;
          }
        } catch (e) {}
      });
      await Promise.all(slotsPromises);

      const occupancy = totalSlotsCount > 0 ? Math.round((bookedSlotsCount / totalSlotsCount) * 100) : 0;

      setStats({
        todayRevenue: todayRev,
        todayBookings: todayBookingsList.length,
        occupancyRate: occupancy,
        pendingBookings: pendingCount,
        totalCourts: courtsList.length,
        activeSubscription: activeSub
      });

      // 5. Fetch upcoming friendly matches for the courts
      const matchesPromises = courtsList.map(async (court) => {
        try {
          const res = await apiFetch(`/friendly-matches/court/${court.id}`);
          if (res.ok) {
            const data = await res.json();
            return (data.items || data || []).map(m => ({ ...m, courtName: court.name }));
          }
        } catch (e) {}
        return [];
      });
      const matchesResults = await Promise.all(matchesPromises);
      setUpcomingMatches(matchesResults.flat().slice(0, 4));

      // 6. Fetch tournaments and filter client-side
      const tournRes = await apiFetch('/tournaments');
      if (tournRes.ok) {
        const tournData = await tournRes.json();
        const courtIds = courtsList.map(c => c.id);
        const filtered = (tournData.items || tournData || []).filter(t => courtIds.includes(t.courtId));
        setUpcomingTournaments(filtered.slice(0, 3));
      }

    } catch (e) {
      console.error('Error fetching dashboard stats', e);
    } finally {
      setLoading(false);
    }
  };

  if (!selectedClub) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-6 bg-[#121216]/40 border border-white/5 rounded-2xl shadow-lg animate-fade-in">
        <AlertCircle className="w-12 h-12 text-[#a1a1aa] mb-4" />
        <h3 className="text-lg font-bold text-white mb-2">No Active Venue Selected</h3>
        <p className="text-xs text-[#a1a1aa] max-w-sm mb-6">
          To manage court bookings and inspect analytics, first register or select your club from the sidebar.
        </p>
        <button
          onClick={() => navigate('/owner/clubs')}
          className="px-5 py-2.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg"
        >
          Manage Venue Clubs
        </button>
      </div>
    );
  }

  // Compute subscription limits description
  const courtLimit = stats.activeSubscription?.maxCourts || 3;
  const limitsUsedPercentage = Math.round((stats.totalCourts / courtLimit) * 100);

  return (
    <div className="flex flex-col gap-8 animate-fade-in text-xs text-[#a1a1aa]">
      {/* Header Info */}
      <div>
        <h2 className="text-2xl font-bold tracking-tight text-white">Complex Overview</h2>
        <p className="text-[#a1a1aa] text-xs mt-1">Real-time daily operations, pitch occupancy rates, and subscription limits usage</p>
      </div>

      {/* Stats Widgets */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        {/* Today's Revenue */}
        <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex items-center justify-between shadow-lg">
          <div>
            <span className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Today's Revenue</span>
            <h3 className="text-2xl font-black text-[#84cc16] mt-1">EGP {loading ? '...' : stats.todayRevenue.toLocaleString()}</h3>
          </div>
          <div className="w-12 h-12 rounded-xl bg-white/5 flex items-center justify-center text-white">
            <DollarSign className="w-5 h-5" />
          </div>
        </div>

        {/* Today's Bookings */}
        <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex items-center justify-between shadow-lg">
          <div>
            <span className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Today's Bookings</span>
            <h3 className="text-2xl font-black text-white mt-1">{loading ? '...' : stats.todayBookings} Bookings</h3>
          </div>
          <div className="w-12 h-12 rounded-xl bg-white/5 flex items-center justify-center text-white">
            <Calendar className="w-5 h-5" />
          </div>
        </div>

        {/* Occupancy Rate */}
        <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex items-center justify-between shadow-lg">
          <div>
            <span className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Today's Occupancy Rate</span>
            <h3 className="text-2xl font-black text-white mt-1">{loading ? '...' : `${stats.occupancyRate}%`}</h3>
          </div>
          <div className="w-12 h-12 rounded-xl bg-white/5 flex items-center justify-center text-white">
            <Clock className="w-5 h-5" />
          </div>
        </div>

        {/* Pending Requests */}
        <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex items-center justify-between shadow-lg">
          <div>
            <span className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Action Requests</span>
            <h3 className={`text-2xl font-black mt-1 ${stats.pendingBookings > 0 ? 'text-orange-400' : 'text-white'}`}>
              {loading ? '...' : `${stats.pendingBookings} Pending`}
            </h3>
          </div>
          <div className={`w-12 h-12 rounded-xl flex items-center justify-center ${stats.pendingBookings > 0 ? 'bg-orange-500/10 text-orange-400' : 'bg-white/5 text-white'}`}>
            <AlertCircle className="w-5 h-5" />
          </div>
        </div>
      </div>

      {/* Main Content Layout Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Matches and Tournaments Monitor */}
        <div className="lg:col-span-2 flex flex-col gap-8">
          {/* Matches */}
          <div className="bg-[#121216]/40 border border-white/5 p-6 rounded-3xl shadow-lg flex flex-col gap-4">
            <div className="flex justify-between items-center">
              <h3 className="text-sm font-bold text-white flex items-center gap-1.5"><Activity className="w-4 h-4 text-[#84cc16]" /> Live Friendly Match Lobbies</h3>
              <button onClick={() => navigate('/owner/matches')} className="text-[#84cc16] hover:underline font-bold text-[10px]">View all matches</button>
            </div>
            {loading ? (
              <p className="text-center py-6">Loading...</p>
            ) : upcomingMatches.length === 0 ? (
              <p className="text-center py-6 text-[#71717a]">No active friendly lobbies organized on your pitches.</p>
            ) : (
              <div className="flex flex-col gap-3">
                {upcomingMatches.map(match => (
                  <div key={match.id} className="p-4 bg-white/5 border border-white/5 rounded-xl flex justify-between items-center">
                    <div>
                      <h4 className="font-bold text-white text-xs">{match.title || 'Friendly Match'}</h4>
                      <span className="text-[10px] text-[#71717a]">{match.courtName} • {match.date}</span>
                    </div>
                    <span className="text-xs font-bold text-amber-400">{match.currentPlayersCount} / {match.maxPlayersCount} Players</span>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Tournaments */}
          <div className="bg-[#121216]/40 border border-white/5 p-6 rounded-3xl shadow-lg flex flex-col gap-4">
            <div className="flex justify-between items-center">
              <h3 className="text-sm font-bold text-white flex items-center gap-1.5"><Trophy className="w-4 h-4 text-[#84cc16]" /> Registered League Tournaments</h3>
              <button onClick={() => navigate('/owner/tournaments')} className="text-[#84cc16] hover:underline font-bold text-[10px]">View all tournaments</button>
            </div>
            {loading ? (
              <p className="text-center py-6">Loading...</p>
            ) : upcomingTournaments.length === 0 ? (
              <p className="text-center py-6 text-[#71717a]">No league tournaments registered on your premises.</p>
            ) : (
              <div className="flex flex-col gap-3">
                {upcomingTournaments.map(t => (
                  <div key={t.id} className="p-4 bg-white/5 border border-white/5 rounded-xl flex justify-between items-center">
                    <div>
                      <h4 className="font-bold text-white text-xs">{t.name}</h4>
                      <span className="text-[10px] text-[#71717a]">Start Date: {t.startDate}</span>
                    </div>
                    <span className="text-xs font-extrabold text-[#84cc16]">Pool: EGP {t.prizePool}</span>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Side panel: quick action, subscription limit usage indicator */}
        <div className="flex flex-col gap-8">
          <div className="bg-[#121216]/40 border border-white/5 p-6 rounded-3xl shadow-lg flex flex-col gap-5">
            <h3 className="text-sm font-bold text-white">Subscription Resource Usage</h3>
            
            <div className="flex flex-col gap-4">
              <div>
                <div className="flex justify-between text-[10px] font-bold text-[#a1a1aa] mb-1.5">
                  <span>Registered Courts ({stats.totalCourts} / {courtLimit})</span>
                  <span>{limitsUsedPercentage}%</span>
                </div>
                <div className="w-full h-2 bg-white/5 rounded-full overflow-hidden border border-white/5">
                  <div className={`h-full rounded-full transition-all duration-500 ${limitsUsedPercentage >= 100 ? 'bg-red-500' : 'bg-[#84cc16]'}`} style={{ width: `${Math.min(limitsUsedPercentage, 100)}%` }}></div>
                </div>
              </div>

              <div className="p-3.5 bg-white/5 rounded-xl border border-white/5 leading-relaxed text-[11px]">
                Active subscription: <b className="text-white">{stats.activeSubscription?.planName || 'Sportify Premium Partner Plan'}</b>. Need more court slots? Go to subscription management to upgrade billing.
              </div>

              <button
                onClick={() => navigate('/owner/subscription')}
                className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl shadow-lg transition-all text-center cursor-pointer"
              >
                Configure Billing Subscription
              </button>
            </div>
          </div>

          <div className="bg-[#121216]/40 border border-white/5 p-6 rounded-3xl shadow-lg flex flex-col gap-4">
            <h3 className="text-sm font-bold text-white">Live Operations</h3>
            <button
              onClick={() => navigate('/owner/today')}
              className="w-full py-3 bg-white/5 border border-white/10 hover:bg-white/10 text-white font-extrabold text-xs rounded-xl transition-all text-center cursor-pointer"
            >
              Live Today Schedule Grid
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
