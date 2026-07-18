import React, { useState, useEffect } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import { apiFetch } from '../../services/api';
import { Clock, Play, User, Check, RefreshCw, AlertCircle, DollarSign, Calendar } from 'lucide-react';

export default function OwnerTodayView({ addToast }) {
  const { selectedClub } = useOutletContext();
  const navigate = useNavigate();

  const [todayBookings, setTodayBookings] = useState([]);
  const [loading, setLoading] = useState(false);
  const [currentTime, setCurrentTime] = useState(new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }));

  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentTime(new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }));
    }, 30000);
    return () => clearInterval(timer);
  }, []);

  useEffect(() => {
    if (selectedClub) {
      loadTodayBookings();
    }
  }, [selectedClub]);

  const loadTodayBookings = async () => {
    setLoading(true);
    try {
      const res = await apiFetch(`/clubs/${selectedClub.id}/bookings`);
      if (res.ok) {
        const data = await res.json();
        const items = data.items || [];
        const todayStr = new Date().toISOString().split('T')[0];
        
        // Filter for bookings matching today's date
        const filtered = items.filter(b => b.date === todayStr);
        // Sort by start time or time slot
        filtered.sort((a, b) => (a.startTime || '').localeCompare(b.startTime || ''));
        setTodayBookings(filtered);
      }
    } catch (e) {
      addToast('Failed to load today\'s bookings.', 'error');
    } finally {
      setLoading(false);
    }
  };

  if (!selectedClub) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-6 bg-[#121216]/40 border border-white/5 rounded-2xl shadow-lg">
        <AlertCircle className="w-12 h-12 text-[#a1a1aa] mb-4" />
        <h3 className="text-lg font-bold text-white mb-2">No active venue selected</h3>
        <p className="text-xs text-[#a1a1aa] max-w-sm">Please register or select a club from the sidebar to view today's operations.</p>
      </div>
    );
  }

  // Calculate quick stats
  const confirmed = todayBookings.filter(b => b.status === 1 || b.status === 'Confirmed');
  const revenue = confirmed.reduce((sum, b) => sum + (b.totalPrice || 0), 0);
  const pending = todayBookings.filter(b => b.status === 0 || b.status === 'Pending');

  // Next booking logic
  const now = new Date();
  const nextBooking = confirmed.find(b => {
    if (!b.startTime) return false;
    const [hours, minutes] = b.startTime.split(':');
    const bookingTime = new Date();
    bookingTime.setHours(parseInt(hours), parseInt(minutes), 0);
    return bookingTime > now;
  }) || confirmed[0];

  return (
    <div className="flex flex-col gap-8 animate-fade-in">
      {/* Top Banner */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight text-white">Live Operations Dashboard</h2>
          <p className="text-[#a1a1aa] text-xs mt-1">Live tracking of check-ins, pitch occupancy, and daily schedule grid</p>
        </div>

        <div className="flex items-center gap-3">
          <div className="bg-[#121216]/60 border border-white/5 px-4 py-2.5 rounded-xl flex items-center gap-2 text-xs font-semibold text-white">
            <Clock className="w-4 h-4 text-[#84cc16]" />
            <span>Local Time: {currentTime}</span>
          </div>
          <button
            onClick={loadTodayBookings}
            className="p-2.5 bg-white/5 hover:bg-white/10 text-white rounded-xl border border-white/5 transition-all"
            title="Refresh Schedule"
          >
            <RefreshCw className="w-4 h-4" />
          </button>
        </div>
      </div>

      {/* Quick Metrics Bar */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-6">
        <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex items-center justify-between">
          <div>
            <span className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider block">Today's Schedule</span>
            <span className="text-2xl font-extrabold text-white mt-1 block">{todayBookings.length} Bookings</span>
          </div>
          <div className="w-10 h-10 rounded-lg bg-[#84cc16]/10 text-[#84cc16] flex items-center justify-center">
            <Calendar className="w-5 h-5" />
          </div>
        </div>

        <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex items-center justify-between">
          <div>
            <span className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider block">Confirmed Revenue</span>
            <span className="text-2xl font-extrabold text-[#84cc16] mt-1 block">EGP {revenue}</span>
          </div>
          <div className="w-10 h-10 rounded-lg bg-white/5 text-white flex items-center justify-center">
            <DollarSign className="w-5 h-5" />
          </div>
        </div>

        <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex items-center justify-between">
          <div>
            <span className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider block">Pending Actions</span>
            <span className="text-2xl font-extrabold text-white mt-1 block">{pending.length} Requests</span>
          </div>
          <div className={`w-10 h-10 rounded-lg flex items-center justify-center ${pending.length > 0 ? 'bg-orange-500/10 text-orange-400' : 'bg-white/5 text-white'}`}>
            <AlertCircle className="w-5 h-5" />
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Left: Occupancy Grid & Booking List */}
        <div className="lg:col-span-2 flex flex-col gap-6">
          <div className="bg-[#121216]/40 border border-white/5 p-6 rounded-2xl shadow-lg">
            <h3 className="text-sm font-bold text-white mb-6">Today's Schedule Grid</h3>

            {loading ? (
              <div className="text-xs text-[#a1a1aa] py-20 text-center flex items-center justify-center gap-2">
                <RefreshCw className="w-4 h-4 animate-spin text-[#84cc16]" /> Loading live grid...
              </div>
            ) : todayBookings.length === 0 ? (
              <div className="text-xs text-[#a1a1aa] py-16 text-center">No bookings scheduled for today.</div>
            ) : (
              <div className="flex flex-col gap-3">
                {todayBookings.map((b) => {
                  const isPending = b.status === 0 || b.status === 'Pending';
                  const isConfirmed = b.status === 1 || b.status === 'Confirmed';
                  
                  return (
                    <div key={b.id} className="flex items-center justify-between p-4 bg-white/5 border border-white/5 rounded-xl hover:bg-white/10 transition-all">
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 rounded-lg bg-[#84cc16]/10 flex items-center justify-center text-[#84cc16] font-bold text-xs shrink-0">
                          {b.startTime ? b.startTime.substring(0, 5) : '00'}
                        </div>
                        <div>
                          <h4 className="text-xs font-semibold text-white">{b.playerName || 'Guest Player'}</h4>
                          <span className="text-[10px] text-[#71717a] font-medium">{b.courtName}</span>
                        </div>
                      </div>

                      <div className="flex items-center gap-6">
                        <div className="text-right">
                          <span className="text-xs text-white font-bold block">{b.timeSlotText || `${b.startTime} - ${b.endTime}`}</span>
                          <span className="text-[10px] text-[#84cc16] font-bold block">EGP {b.totalPrice}</span>
                        </div>

                        <span className={`text-[9px] px-2 py-0.5 rounded font-bold uppercase tracking-wider ${
                          isPending ? 'bg-orange-500/10 text-orange-400' : isConfirmed ? 'bg-[#84cc16]/10 text-[#84cc16]' : 'bg-red-500/10 text-red-400'
                        }`}>
                          {isPending ? 'Pending' : isConfirmed ? 'Confirmed' : 'Rejected'}
                        </span>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        </div>

        {/* Right: Next Up & Quick actions */}
        <div className="flex flex-col gap-6">
          {nextBooking && (
            <div className="bg-[#121216]/40 border border-white/5 p-6 rounded-2xl shadow-lg">
              <div className="flex items-center gap-2 mb-4">
                <Play className="w-4 h-4 text-[#84cc16] fill-[#84cc16]" />
                <h3 className="text-sm font-bold text-white">Next Session Scheduled</h3>
              </div>

              <div className="bg-white/5 border border-white/5 p-4 rounded-xl flex flex-col gap-4">
                <div>
                  <span className="text-[9px] text-[#71717a] font-bold uppercase tracking-wider block">Pitch / Court</span>
                  <span className="text-xs text-white font-semibold mt-0.5 block">{nextBooking.courtName}</span>
                </div>

                <div>
                  <span className="text-[9px] text-[#71717a] font-bold uppercase tracking-wider block">Scheduled Player</span>
                  <div className="flex items-center gap-2 mt-1">
                    <div className="w-7 h-7 rounded-full bg-white/10 flex items-center justify-center text-[10px] font-bold text-white">
                      {nextBooking.playerName ? nextBooking.playerName.charAt(0) : 'P'}
                    </div>
                    <div>
                      <span className="text-xs text-white font-bold block leading-none">{nextBooking.playerName}</span>
                      <span className="text-[9px] text-[#a1a1aa] mt-0.5 block">{nextBooking.playerEmail || 'No Email'}</span>
                    </div>
                  </div>
                </div>

                <div className="flex justify-between border-t border-white/5 pt-3">
                  <div>
                    <span className="text-[9px] text-[#71717a] font-bold uppercase tracking-wider">Start Time</span>
                    <span className="text-xs text-white font-semibold block">{nextBooking.startTime || 'N/A'}</span>
                  </div>
                  <div className="text-right">
                    <span className="text-[9px] text-[#71717a] font-bold uppercase tracking-wider">Estimated Price</span>
                    <span className="text-xs text-[#84cc16] font-bold block">EGP {nextBooking.totalPrice}</span>
                  </div>
                </div>
              </div>
            </div>
          )}

          <div className="bg-[#121216]/40 border border-white/5 p-6 rounded-2xl shadow-lg">
            <h3 className="text-sm font-bold text-white mb-4">Live Check-ins</h3>
            <p className="text-xs text-[#a1a1aa] mb-4">
              Match schedule is locked on client side. Ensure players pay at complex desk before matching keys.
            </p>
            <button
              onClick={() => navigate('/owner/bookings?status=Pending')}
              className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl shadow-lg transition-all text-center"
            >
              Review Pending Requests
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
