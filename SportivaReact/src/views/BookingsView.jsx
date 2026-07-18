import React, { useState, useEffect } from 'react';
import { Calendar, Building, X } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function BookingsView({ addToast }) {
  const [courts, setCourts] = useState([]);
  const [selectedCourt, setSelectedCourt] = useState('');
  const [bookingDate, setBookingDate] = useState('');
  const [timeSlots, setTimeSlots] = useState([]);
  const [myBookings, setMyBookings] = useState([]);

  useEffect(() => {
    loadCourtsDropdown();
    loadMyBookings();
  }, []);

  const loadCourtsDropdown = async () => {
    try {
      const res = await apiFetch('/clubs');
      if (!res.ok) return;
      const data = await res.json();
      const loadedClubs = data.items || [];
      const list = [];
      for (const club of loadedClubs) {
        const courtsRes = await apiFetch(`/clubs/${club.id}/courts`);
        if (courtsRes.ok) {
          const courtsData = await courtsRes.json();
          const courtsList = courtsData.items || courtsData || [];
          courtsList.forEach(court => {
            list.push({ ...court, clubName: club.name });
          });
        }
      }
      setCourts(list);
    } catch (e) {}
  };

  const loadMyBookings = async () => {
    try {
      const res = await apiFetch('/bookings/my-bookings');
      if (res.ok) {
        const data = await res.json();
        setMyBookings(data.items || []);
      }
    } catch (e) {}
  };

  const checkAvailability = async () => {
    if (!selectedCourt || !bookingDate) {
      addToast('Please select court and date.', 'error');
      return;
    }
    try {
      const res = await apiFetch(`/courts/${selectedCourt}/time-slots?date=${bookingDate}`);
      if (!res.ok) throw new Error('Failed to fetch availability.');
      const data = await res.json();
      setTimeSlots(data);
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const bookTimeSlot = async (slotId) => {
    if (!confirm('Proceed to request slot booking?')) return;
    try {
      const res = await apiFetch('/bookings', {
        method: 'POST',
        body: JSON.stringify({ courtId: selectedCourt, timeSlotId: slotId })
      });
      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Reservation rejected.');
      }
      addToast('Reservation successfully requested!', 'success');
      checkAvailability();
      loadMyBookings();
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const cancelBooking = async (bookingId) => {
    if (!confirm('Cancel this booking request?')) return;
    try {
      const res = await apiFetch(`/bookings/${bookingId}/cancel`, { method: 'PUT' });
      if (!res.ok) throw new Error();
      addToast('Booking cancelled.', 'info');
      loadMyBookings();
    } catch (e) {
      addToast('Cancellation failed.', 'error');
    }
  };

  return (
    <section className="animate-fade-in flex flex-col gap-10">
      <div>
        <h2 className="text-2xl font-bold tracking-tight">Booking Engine & Availability</h2>
        <p className="text-[#a1a1aa] text-sm">Coordinate slots availability and submit reservations</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-10">
        {/* Check Slots Form */}
        <div className="glass-panel rounded-2xl p-8 flex flex-col gap-6 lg:col-span-1 h-fit">
          <h3 className="font-bold text-lg">Check Available Court Slots</h3>
          <div className="flex flex-col gap-4">
            <div className="flex flex-col gap-1.5">
              <label className="text-xs text-[#a1a1aa] uppercase font-bold tracking-wider">Court Selection</label>
              <select onChange={(e) => setSelectedCourt(e.target.value)} value={selectedCourt} className="w-full bg-[#1e1e26]/40 border border-[#ffffff08] rounded-xl px-5 py-3.5 text-sm text-[#fafafa] outline-none">
                <option value="">Select a Court</option>
                {courts.map(c => (
                  <option key={c.id} value={c.id}>{c.clubName} — {c.name} ({c.sportType})</option>
                ))}
              </select>
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-xs text-[#a1a1aa] uppercase font-bold tracking-wider">Reservation Date</label>
              <input type="date" value={bookingDate} onChange={(e) => setBookingDate(e.target.value)} className="w-full bg-[#1e1e26]/40 border border-[#ffffff08] rounded-xl px-5 py-3.5 text-sm text-[#fafafa] outline-none" />
            </div>
            <button onClick={checkAvailability} className="w-full py-4 rounded-xl bg-[#84cc16] text-white font-bold hover:bg-[#84cc16]/80 transition-colors mt-2 shadow-lg shadow-[#84cc16]/20">Verify Slots</button>
          </div>
        </div>

        {/* Slots grid / History */}
        <div className="lg:col-span-2 flex flex-col gap-10">
          {timeSlots.length > 0 && (
            <div className="glass-panel rounded-2xl p-8">
              <h3 className="font-bold text-lg mb-6">Available Time Slots</h3>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                {timeSlots.map(slot => (
                  <button key={slot.id} disabled={slot.isBooked} onClick={() => bookTimeSlot(slot.id)} className={`flex flex-col items-center gap-1.5 p-4 rounded-xl border transition-all ${
                    slot.isBooked ? 'opacity-40 cursor-not-allowed border-red-500/20 bg-red-500/5 text-red-400' : 'border-white/10 hover:border-[#84cc16] hover:bg-[#84cc16]/10'
                  }`}>
                    <span className="text-sm font-bold">{slot.startTime.substring(0, 5)} - {slot.endTime.substring(0, 5)}</span>
                    <span className="text-xs">{slot.isBooked ? 'Booked' : 'Available'}</span>
                  </button>
                ))}
              </div>
            </div>
          )}

          {/* My Bookings History List */}
          <div className="glass-panel rounded-2xl p-8">
            <h3 className="font-bold text-lg mb-6">My Bookings & Reservations</h3>
            <div className="flex flex-col gap-4">
              {myBookings.length > 0 ? myBookings.map(b => (
                <div key={b.id} className="flex justify-between items-center p-4 border border-[#ffffff08] bg-white/3 rounded-xl">
                  <div>
                    <h4 className="font-semibold text-sm">{b.courtName}</h4>
                    <p className="text-xs text-[#a1a1aa] mt-0.5">{b.bookingDate} | {b.startTime?.substring(0,5)} - {b.endTime?.substring(0,5)}</p>
                    <span className={`text-[10px] uppercase font-bold tracking-wider inline-block mt-1 ${
                      b.status === 'Confirmed' ? 'text-[#10b981]' : b.status === 'Cancelled' ? 'text-[#ef4444]' : 'text-amber-400'
                    }`}>{b.status}</span>
                  </div>
                  {b.status === 'Pending' && (
                    <button onClick={() => cancelBooking(b.id)} className="px-4 py-2 border border-red-500/20 bg-red-500/5 hover:bg-red-500/20 text-[#ef4444] rounded-lg text-xs font-semibold transition-all">Cancel</button>
                  )}
                </div>
              )) : (
                <p className="text-sm text-[#71717a]">No active reservations found.</p>
              )}
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
