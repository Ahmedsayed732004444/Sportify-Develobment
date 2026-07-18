import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Star, MapPin, Calendar, Clock, CheckCircle, Info } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function CourtDetailView({ addToast, onTriggerAuth }) {
  const { clubId, courtId } = useParams();
  const navigate = useNavigate();
  const [court, setCourt] = useState(null);
  const [date, setDate] = useState(() => new Date().toISOString().split('T')[0]);
  const [slots, setSlots] = useState([]);
  const [selectedSlot, setSelectedSlot] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  const [reviews, setReviews] = useState([]);

  useEffect(() => {
    loadCourtDetails();
    if (courtId) {
      loadCourtReviews();
    }
  }, [clubId, courtId]);

  const loadCourtReviews = async () => {
    try {
      const res = await apiFetch(`/courts/${courtId}/reviews`);
      if (res.ok) {
        const data = await res.json();
        setReviews(data.items || data || []);
      }
    } catch (e) {}
  };

  useEffect(() => {
    if (courtId && date) {
      loadAvailability();
    }
  }, [courtId, date]);

  const loadCourtDetails = async () => {
    try {
      setIsLoading(true);
      const res = await apiFetch(`/clubs/${clubId}/courts/${courtId}`);
      if (res.ok) {
        const data = await res.json();
        setCourt(data);
      }
    } catch (e) {
      addToast('Failed to load court specifications.', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const loadAvailability = async () => {
    try {
      const res = await apiFetch(`/courts/${courtId}/availability?date=${date}`);
      if (res.ok) {
        const data = await res.json();
        setSlots(data || []);
      }
    } catch (e) {}
  };

  const handleBookingSubmit = async () => {
    if (!selectedSlot) {
      addToast('Please select a time slot.', 'error');
      return;
    }

    try {
      const token = localStorage.getItem('token');
      if (!token) {
        addToast('Please sign in to complete bookings.', 'info');
        if (onTriggerAuth) {
          onTriggerAuth(`/club/${clubId}/court/${courtId}`);
        }
        return;
      }

      const res = await apiFetch('/bookings', {
        method: 'POST',
        body: JSON.stringify({
          courtId: courtId,
          date: date,
          startTime: selectedSlot.startTime,
          endTime: selectedSlot.endTime
        })
      });

      if (res.ok) {
        addToast('Court booked successfully! Checking status in Bookings Grid.', 'success');
        setSelectedSlot(null);
        loadAvailability();
        navigate('/bookings');
      } else {
        const err = await res.json();
        throw new Error(err.detail || 'Could not complete reservation.');
      }
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  if (isLoading) {
    return (
      <div className="max-w-[700px] mx-auto flex flex-col gap-6 py-20 animate-fade-in text-[#71717a]">
        <div className="h-6 w-32 bg-[#1e1e26]/30 rounded animate-pulse"></div>
        <div className="h-40 bg-[#1e1e26]/30 rounded-3xl animate-pulse"></div>
      </div>
    );
  }

  if (!court) {
    return (
      <div className="text-center py-20 text-[#71717a]">
        <h3 className="font-bold text-white text-lg">Court Not Found</h3>
        <p className="text-xs mt-1">The specified court is currently unavailable for bookings.</p>
      </div>
    );
  }

  return (
    <div className="max-w-[850px] mx-auto flex flex-col lg:flex-row gap-8 pb-20 animate-fade-in text-xs text-[#a1a1aa]">
      
      {/* LEFT COLUMN: Specifications and Ratings */}
      <div className="flex-1 flex flex-col gap-6">
        <div>
          <span className="px-2 py-0.5 bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#a3e635] rounded font-bold text-[9px] uppercase tracking-wider w-fit">{court.sportType}</span>
          <h2 className="text-2xl font-extrabold text-white mt-2">{court.name}</h2>
          <p className="text-xs text-[#a1a1aa] mt-1 flex items-center gap-1"><MapPin className="w-3.5 h-3.5" /> Premium Complex Facility</p>
        </div>

        <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4 text-[#a1a1aa]">
          <h3 className="font-bold text-white text-sm">Court Specifications</h3>
          <div className="grid grid-cols-2 gap-4">
            <div className="flex flex-col gap-1">
              <span className="text-[10px] text-[#71717a] font-bold uppercase">Price Rate</span>
              <span className="text-white font-bold">{court.pricePerHour} EGP / Hour</span>
            </div>
            <div className="flex flex-col gap-1">
              <span className="text-[10px] text-[#71717a] font-bold uppercase">Capacity Limit</span>
              <span className="text-white font-bold">{court.maxCapacity || 10} Players</span>
            </div>
          </div>
        </div>

        {/* Quality Ratings breakdown */}
        <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4">
          <h3 className="font-bold text-white text-sm">Court Quality Reviews</h3>
          {reviews.length > 0 && (
            <div className="flex items-center gap-2 border-b border-white/5 pb-4">
              <span className="text-xl font-black text-white">
                {(reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length).toFixed(1)}
              </span>
              <div className="flex items-center text-amber-400 font-bold gap-1">
                <Star className="w-4 h-4 fill-amber-400" />
                <span className="text-xs text-[#a1a1aa]">({reviews.length} reviews)</span>
              </div>
            </div>
          )}

          <div className="flex flex-col gap-4 mt-2">
            <h4 className="font-bold text-white text-xs">Player Testimonials</h4>
            {reviews.length === 0 ? (
              <p className="text-[10px] text-[#71717a] py-4">No reviews left for this court yet.</p>
            ) : (
              reviews.map((r) => (
                <div key={r.reviewId} className="bg-[#1e1e26]/30 border border-white/5 p-4 rounded-xl flex flex-col gap-1.5">
                  <div className="flex justify-between text-[10px]">
                    <span className="font-bold text-white">{r.author?.fullName || 'Sportify Player'}</span>
                    <span className="text-[#71717a]">{new Date(r.createdAt).toLocaleDateString()}</span>
                  </div>
                  <div className="flex items-center gap-0.5 text-amber-400 font-bold mb-1">
                    {[...Array(r.rating)].map((_, i) => (
                      <Star key={i} className="w-3 h-3 fill-amber-400 text-amber-400" />
                    ))}
                  </div>
                  <p className="text-white/90 leading-relaxed">{r.comment}</p>
                </div>
              ))
            )}
          </div>
        </div>
      </div>

      {/* RIGHT COLUMN: Date Picker and Booking Checkout */}
      <div className="w-full lg:w-[320px] flex flex-col gap-6 shrink-0">
        
        {/* Reservation Widget */}
        <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4 shadow-xl">
          <h3 className="font-extrabold text-white text-sm flex items-center gap-1.5"><Calendar className="w-4 h-4 text-[#84cc16]" /> Booking Schedule</h3>
          
          <div className="flex flex-col gap-1">
            <label className="text-[10px] uppercase font-bold text-[#71717a]">Select Play Date</label>
            <input type="date" value={date} onChange={(e) => setDate(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]" />
          </div>

          <div className="flex flex-col gap-2 mt-2">
            <label className="text-[10px] uppercase font-bold text-[#71717a]">Available Time Slots</label>
            <div className="grid grid-cols-2 gap-2 max-h-[160px] overflow-y-auto pr-1">
              {slots.length === 0 ? (
                <span className="text-[10px] text-[#71717a] col-span-2 py-4 text-center">No slots generated.</span>
              ) : (
                slots.map(s => {
                  const isSelected = selectedSlot?.startTime === s.startTime;
                  return (
                    <button key={s.startTime} type="button" disabled={s.isBooked} onClick={() => setSelectedSlot(s)} className={`p-2.5 border rounded-lg text-center font-bold transition-all ${
                      s.isBooked ? 'opacity-30 cursor-not-allowed border-white/5' : isSelected ? 'bg-[#84cc16] border-[#84cc16] text-black' : 'border-white/5 hover:border-[#84cc16]/50 text-white'
                    }`}>
                      {s.startTime.substring(0, 5)}
                    </button>
                  );
                })
              )}
            </div>
          </div>

          <div className="border-t border-white/5 pt-4 mt-2 flex flex-col gap-3">
            <div className="flex justify-between text-xs font-semibold">
              <span className="text-[#a1a1aa]">Hourly Rate</span>
              <span className="text-white font-bold">{court.pricePerHour} EGP</span>
            </div>
            
            <button onClick={handleBookingSubmit} className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all flex items-center justify-center gap-1.5 shadow-lg shadow-[#84cc16]/10">
              <CheckCircle className="w-4 h-4" /> Book Slot Now
            </button>
          </div>
        </div>

        {/* Cancellation Warning */}
        <div className="bg-[#ef4444]/5 border border-[#ef4444]/10 p-5 rounded-2xl flex gap-3 text-[#ef4444] text-[11px] leading-relaxed">
          <Info className="w-5 h-5 shrink-0" />
          <p>Bookings can be cancelled up to 24 hours prior to the match start time from your Bookings history tab.</p>
        </div>

      </div>

    </div>
  );
}
