import React, { useState, useEffect } from 'react';
import { useOutletContext } from 'react-router-dom';
import { apiFetch } from '../../services/api';
import { Calendar, User, Phone, DollarSign, Clock, Shield, Check, Ban, AlertCircle, RefreshCw } from 'lucide-react';

export default function OwnerBookingsView({ addToast }) {
  const { selectedClub } = useOutletContext();
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(false);
  const [filterStatus, setFilterStatus] = useState('All'); // 'All', 'Pending', 'Confirmed', 'Rejected'

  useEffect(() => {
    if (selectedClub) {
      loadBookings();
    }
  }, [selectedClub]);

  const loadBookings = async () => {
    setLoading(true);
    try {
      const res = await apiFetch(`/clubs/${selectedClub.id}/bookings`);
      if (res.ok) {
        const data = await res.json();
        setBookings(data.items || []);
      }
    } catch (e) {
      addToast('Failed to load bookings.', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleReviewBooking = async (bookingId, decision) => {
    try {
      const statusValue = decision === 'Confirm' ? 1 : 2; // 1 = Confirmed, 2 = Rejected
      const res = await apiFetch(`/bookings/${bookingId}/review`, {
        method: 'PATCH',
        body: JSON.stringify({ newStatus: statusValue })
      });

      if (res.ok) {
        addToast(`Booking ${decision === 'Confirm' ? 'approved' : 'rejected'} successfully!`, 'success');
        loadBookings();
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to submit review decision.', 'error');
    }
  };

  const filteredBookings = bookings.filter(b => {
    if (filterStatus === 'All') return true;
    
    // Status mappings: 0=Pending, 1=Confirmed, 2=Rejected/Cancelled
    const status = b.status;
    if (filterStatus === 'Pending') return status === 0 || status === 'Pending';
    if (filterStatus === 'Confirmed') return status === 1 || status === 'Confirmed';
    if (filterStatus === 'Rejected') return status === 2 || status === 'Rejected' || status === 'Cancelled';
    return true;
  });

  if (!selectedClub) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-6 bg-[#121216]/40 border border-white/5 rounded-2xl shadow-lg">
        <AlertCircle className="w-12 h-12 text-[#a1a1aa] mb-4" />
        <h3 className="text-lg font-bold text-white mb-2">No active venue selected</h3>
        <p className="text-xs text-[#a1a1aa] max-w-sm">Please register or select a club from the sidebar before managing bookings.</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-8 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight text-white">Bookings & Reservations</h2>
          <p className="text-[#a1a1aa] text-xs mt-1">Review player bookings, approve court reservations, or reject conflict requests</p>
        </div>
      </div>

      {/* Tabs / Filters */}
      <div className="flex gap-2 p-1 bg-white/5 rounded-xl border border-white/5 self-start">
        {['All', 'Pending', 'Confirmed', 'Rejected'].map(tab => (
          <button
            key={tab}
            onClick={() => setFilterStatus(tab)}
            className={`px-4 py-2 text-xs font-bold rounded-lg transition-all cursor-pointer ${
              filterStatus === tab ? 'bg-[#84cc16] text-black shadow-lg shadow-[#84cc16]/10' : 'text-[#a1a1aa] hover:text-white hover:bg-white/5'
            }`}
          >
            {tab}
          </button>
        ))}
      </div>

      {/* Bookings Grid */}
      {loading ? (
        <div className="text-xs text-[#a1a1aa] py-20 text-center flex items-center justify-center gap-2">
          <RefreshCw className="w-4 h-4 animate-spin text-[#84cc16]" /> Loading reservations...
        </div>
      ) : filteredBookings.length === 0 ? (
        <div className="bg-[#121216]/40 border border-dashed border-white/10 rounded-2xl p-12 text-center flex flex-col items-center">
          <Calendar className="w-12 h-12 text-[#71717a] mb-4" />
          <h3 className="text-sm font-bold text-white mb-1">No reservations found</h3>
          <p className="text-xs text-[#a1a1aa] max-w-sm">There are no bookings matching the selected status filter.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {filteredBookings.map(booking => {
            const isPending = booking.status === 0 || booking.status === 'Pending';
            const isConfirmed = booking.status === 1 || booking.status === 'Confirmed';
            const isRejected = booking.status === 2 || booking.status === 'Rejected' || booking.status === 'Cancelled';

            return (
              <div key={booking.id} className="bg-[#121216]/60 border border-white/5 rounded-2xl p-6 shadow-lg flex flex-col justify-between hover:border-white/10 transition-all gap-6">
                
                <div className="flex flex-col gap-4">
                  {/* Top info and status badge */}
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 rounded-xl bg-white/5 flex items-center justify-center text-white">
                        <User className="w-5 h-5" />
                      </div>
                      <div>
                        <h3 className="text-sm font-bold text-white">{booking.playerName || 'Guest Player'}</h3>
                        <span className="text-[10px] text-[#71717a] font-bold">{booking.playerEmail || 'No Email'}</span>
                      </div>
                    </div>

                    <span className={`text-[9px] px-2.5 py-1 rounded-full font-bold uppercase tracking-wider ${
                      isPending 
                        ? 'bg-[#84cc16]/10 text-[#84cc16] border border-[#84cc16]/20'
                        : isConfirmed 
                        ? 'bg-blue-500/10 text-blue-400 border border-blue-500/20'
                        : 'bg-red-500/10 text-red-400 border border-red-500/20'
                    }`}>
                      {isPending ? 'Pending Approval' : isConfirmed ? 'Confirmed' : 'Cancelled/Rejected'}
                    </span>
                  </div>

                  {/* Booking details grid */}
                  <div className="grid grid-cols-2 gap-4 bg-white/5 p-4 rounded-xl border border-white/5">
                    <div className="flex flex-col gap-1.5">
                      <span className="text-[9px] text-[#71717a] font-bold uppercase tracking-wider">Date & Time</span>
                      <div className="flex items-center gap-1.5 text-xs text-white font-semibold">
                        <Clock className="w-4 h-4 text-[#71717a]" />
                        <span>{booking.date}</span>
                      </div>
                      <span className="text-[10px] text-[#a1a1aa] font-medium pl-5.5">
                        {booking.timeSlotText || `${booking.startTime} - ${booking.endTime}`}
                      </span>
                    </div>

                    <div className="flex flex-col gap-1.5">
                      <span className="text-[9px] text-[#71717a] font-bold uppercase tracking-wider">Facility Pitch</span>
                      <div className="flex items-center gap-1.5 text-xs text-white font-semibold">
                        <Shield className="w-4 h-4 text-[#71717a]" />
                        <span>{booking.courtName}</span>
                      </div>
                    </div>
                  </div>
                </div>

                {/* Pricing and Actions row */}
                <div className="flex items-center justify-between border-t border-white/5 pt-4">
                  <div className="flex items-center gap-1.5">
                    <DollarSign className="w-4 h-4 text-[#84cc16]" />
                    <span className="text-xs text-[#a1a1aa] font-bold">Total Price:</span>
                    <span className="text-sm text-white font-extrabold">EGP {booking.totalPrice}</span>
                  </div>

                  {isPending && (
                    <div className="flex items-center gap-2">
                      <button
                        onClick={() => handleReviewBooking(booking.id, 'Reject')}
                        className="flex items-center gap-1 px-3.5 py-2 bg-red-500/10 hover:bg-red-500/20 border border-red-500/20 text-red-400 font-bold text-xs rounded-xl transition-all cursor-pointer"
                      >
                        <Ban className="w-3.5 h-3.5" /> Reject
                      </button>
                      <button
                        onClick={() => handleReviewBooking(booking.id, 'Confirm')}
                        className="flex items-center gap-1 px-3.5 py-2 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg cursor-pointer"
                      >
                        <Check className="w-3.5 h-3.5" /> Approve
                      </button>
                    </div>
                  )}
                </div>

              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
