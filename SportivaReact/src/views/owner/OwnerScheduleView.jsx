import React, { useState, useEffect } from 'react';
import { useParams, useOutletContext, useNavigate } from 'react-router-dom';
import { apiFetch } from '../../services/api';
import { Calendar, Clock, Check, Ban, Sparkles, AlertCircle, ArrowLeft, RefreshCw } from 'lucide-react';

export default function OwnerScheduleView({ addToast }) {
  const { courtId } = useParams();
  const { selectedClub } = useOutletContext();
  const navigate = useNavigate();

  const [court, setCourt] = useState(null);
  const [selectedDate, setSelectedDate] = useState(new Date().toISOString().split('T')[0]);
  const [timeSlots, setTimeSlots] = useState([]);
  const [selectedSlots, setSelectedSlots] = useState([]);
  const [loading, setLoading] = useState(false);
  const [generating, setGenerating] = useState(false);

  useEffect(() => {
    if (selectedClub && courtId) {
      loadCourtDetails();
    }
  }, [selectedClub, courtId]);

  useEffect(() => {
    if (courtId && selectedDate) {
      loadTimeSlots();
    }
  }, [courtId, selectedDate]);

  const loadCourtDetails = async () => {
    try {
      const res = await apiFetch(`/clubs/${selectedClub.id}/courts/${courtId}`);
      if (res.ok) {
        const data = await res.json();
        setCourt(data);
      }
    } catch (e) {
      console.error(e);
    }
  };

  const loadTimeSlots = async () => {
    setLoading(true);
    try {
      const res = await apiFetch(`/courts/${courtId}/time-slots?date=${selectedDate}`);
      if (res.ok) {
        const data = await res.json();
        // The endpoint returns list of time slots
        setTimeSlots(data || []);
        setSelectedSlots([]);
      }
    } catch (e) {
      addToast('Failed to load time slots.', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleSelectSlot = (slot) => {
    // Cannot toggle availability of booked slots
    if (slot.isBooked) return;
    
    if (selectedSlots.includes(slot.id)) {
      setSelectedSlots(prev => prev.filter(id => id !== slot.id));
    } else {
      setSelectedSlots(prev => [...prev, slot.id]);
    }
  };

  const handleGenerateWeekly = async () => {
    setGenerating(true);
    try {
      const res = await apiFetch(`/courts/${courtId}/time-slots/generate`, {
        method: 'POST'
      });
      if (res.ok) {
        const data = await res.json();
        addToast(`Weekly slots generated successfully! (Count: ${data.generatedCount || 0})`, 'success');
        loadTimeSlots();
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to generate weekly time slots.', 'error');
    } finally {
      setGenerating(false);
    }
  };

  const handleSetAvailability = async (isActive) => {
    if (selectedSlots.length === 0) return;
    try {
      const res = await apiFetch(`/courts/${courtId}/time-slots/availability`, {
        method: 'PATCH',
        body: JSON.stringify({
          slotIds: selectedSlots,
          isActive: isActive
        })
      });

      if (res.ok) {
        addToast(`Selected slots updated successfully!`, 'success');
        loadTimeSlots();
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to update slot availability.', 'error');
    }
  };

  return (
    <div className="flex flex-col gap-8 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div className="flex items-center gap-3">
          <button
            onClick={() => navigate('/owner/courts')}
            className="p-2 hover:bg-white/5 text-[#a1a1aa] hover:text-white rounded-xl border border-white/5 transition-all"
          >
            <ArrowLeft className="w-5 h-5" />
          </button>
          <div>
            <h2 className="text-2xl font-bold tracking-tight text-white">{court ? `${court.name} Slots` : 'Manage Schedule'}</h2>
            <p className="text-[#a1a1aa] text-xs mt-1">Configure daily calendar, block slot availability, or generate weekly schedule</p>
          </div>
        </div>

        <button
          onClick={handleGenerateWeekly}
          disabled={generating}
          className="flex items-center gap-1.5 px-5 py-2.5 bg-[#84cc16] hover:bg-[#65a30d] disabled:opacity-50 text-black font-extrabold text-xs rounded-xl transition-all shadow-lg self-start sm:self-center cursor-pointer"
        >
          <Sparkles className="w-4 h-4" /> {generating ? 'Generating...' : 'Generate Weekly Slots'}
        </button>
      </div>

      {/* Date Picker & Controls bar */}
      <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex flex-col md:flex-row md:items-center md:justify-between gap-4 shadow-lg">
        <div className="flex items-center gap-3">
          <Calendar className="w-5 h-5 text-[#84cc16] shrink-0" />
          <input
            type="date"
            value={selectedDate}
            onChange={(e) => setSelectedDate(e.target.value)}
            className="bg-white/5 border border-white/10 rounded-xl px-4 py-2.5 text-xs text-white focus:border-[#84cc16] outline-none cursor-pointer"
          />
        </div>

        {selectedSlots.length > 0 && (
          <div className="flex items-center gap-3 animate-fade-in">
            <span className="text-xs text-[#a1a1aa] font-medium">{selectedSlots.length} slots selected</span>
            <button
              onClick={() => handleSetAvailability(true)}
              className="flex items-center gap-1 px-4 py-2 bg-[#84cc16]/10 hover:bg-[#84cc16]/20 border border-[#84cc16]/20 text-[#84cc16] font-bold text-xs rounded-xl transition-all cursor-pointer"
            >
              <Check className="w-4 h-4" /> Enable
            </button>
            <button
              onClick={() => handleSetAvailability(false)}
              className="flex items-center gap-1 px-4 py-2 bg-red-500/10 hover:bg-red-500/20 border border-red-500/20 text-red-400 font-bold text-xs rounded-xl transition-all cursor-pointer"
            >
              <Ban className="w-4 h-4" /> Disable
            </button>
          </div>
        )}
      </div>

      {/* Slots Grid */}
      {loading ? (
        <div className="text-xs text-[#a1a1aa] py-20 text-center flex items-center justify-center gap-2">
          <RefreshCw className="w-4 h-4 animate-spin text-[#84cc16]" /> Loading schedule...
        </div>
      ) : timeSlots.length === 0 ? (
        <div className="bg-[#121216]/40 border border-dashed border-white/10 rounded-2xl p-12 text-center flex flex-col items-center">
          <Clock className="w-12 h-12 text-[#71717a] mb-4" />
          <h3 className="text-sm font-bold text-white mb-1">No slots for this date</h3>
          <p className="text-xs text-[#a1a1aa] max-w-sm mb-6">
            There are no time slots configured for {selectedDate}. Click the button below to generate weekly slots for this court.
          </p>
          <button
            onClick={handleGenerateWeekly}
            disabled={generating}
            className="px-5 py-2.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg"
          >
            Generate Slots Now
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
          {timeSlots.map(slot => {
            const isSelected = selectedSlots.includes(slot.id);
            const isBooked = slot.isBooked;
            const isActive = slot.isActive !== false;

            let borderStyle = 'border-white/5 bg-white/5';
            let textStyle = 'text-[#fafafa]';
            let label = 'Available';

            if (isBooked) {
              borderStyle = 'border-red-500/30 bg-red-500/5 cursor-not-allowed';
              textStyle = 'text-red-400';
              label = 'Booked';
            } else if (!isActive) {
              borderStyle = 'border-white/5 bg-white/5 opacity-40';
              textStyle = 'text-[#71717a]';
              label = 'Disabled';
            } else if (isSelected) {
              borderStyle = 'border-[#84cc16] bg-[#84cc16]/10';
              textStyle = 'text-[#84cc16]';
            }

            return (
              <button
                key={slot.id}
                disabled={isBooked}
                onClick={() => handleSelectSlot(slot)}
                className={`p-4 rounded-xl border flex flex-col items-center justify-center text-center transition-all cursor-pointer ${borderStyle}`}
              >
                <span className={`text-xs font-bold ${textStyle}`}>
                  {slot.startTime} - {slot.endTime}
                </span>
                
                <span className={`text-[9px] font-bold uppercase tracking-wider mt-2 px-1.5 py-0.5 rounded ${
                  isBooked 
                    ? 'bg-red-500/10 text-red-400' 
                    : !isActive 
                    ? 'bg-white/5 text-[#71717a]' 
                    : isSelected 
                    ? 'bg-[#84cc16]/20 text-[#84cc16]' 
                    : 'bg-[#84cc16]/10 text-[#84cc16]'
                }`}>
                  {label}
                </span>
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}
