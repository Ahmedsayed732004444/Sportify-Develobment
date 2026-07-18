import React, { useState, useEffect } from 'react';
import { Building, Plus, Settings, Check, Ban, Calendar, MapPin, Mail, Phone, Users } from 'lucide-react';
import { apiFetch, getApiBaseUrl } from '../services/api';

export default function ClubManagementView({ addToast }) {
  const [clubs, setClubs] = useState([]);
  const [selectedClub, setSelectedClub] = useState(null);
  const [courts, setCourts] = useState([]);
  const [selectedCourt, setSelectedCourt] = useState(null);
  const [clubBookings, setClubBookings] = useState([]);

  // Create Club Form States
  const [clubName, setClubName] = useState('');
  const [clubGov, setClubGov] = useState('Cairo');
  const [clubCity, setClubCity] = useState('');
  const [clubAddr, setClubAddr] = useState('');
  const [clubPhone, setClubPhone] = useState('');
  const [clubEmail, setClubEmail] = useState('');
  const [logoFile, setLogoFile] = useState(null);

  // Create Court Form States
  const [courtName, setCourtName] = useState('');
  const [courtDesc, setCourtDesc] = useState('');
  const [sportType, setSportType] = useState('Football');
  const [maxCapacity, setMaxCapacity] = useState(10);
  const [pricePerHour, setPricePerHour] = useState(150);
  const [courtImageFile, setCourtImageFile] = useState(null);

  useEffect(() => {
    loadMyClubs();
  }, []);

  const loadMyClubs = async () => {
    try {
      const res = await apiFetch('/clubs');
      if (res.ok) {
        const data = await res.json();
        // Since there's no custom owner filtering endpoint, we display clubs here
        setClubs(data.items || []);
      }
    } catch (e) {}
  };

  const selectClub = async (club) => {
    setSelectedClub(club);
    setSelectedCourt(null);
    setClubBookings([]);
    loadClubCourts(club.id);
    loadClubBookings(club.id);
  };

  const loadClubCourts = async (clubId) => {
    try {
      const res = await apiFetch(`/clubs/${clubId}/courts`);
      if (res.ok) {
        const data = await res.json();
        setCourts(data);
      }
    } catch (e) {}
  };

  const loadClubBookings = async (clubId) => {
    try {
      const res = await apiFetch(`/clubs/${clubId}/bookings`);
      if (res.ok) {
        const data = await res.json();
        setClubBookings(data.items || []);
      }
    } catch (e) {}
  };

  const handleCreateClubSubmit = async (e) => {
    e.preventDefault();
    try {
      const formData = new FormData();
      formData.append('Name', clubName);
      formData.append('Governorate', clubGov);
      formData.append('City', clubCity);
      formData.append('Address', clubAddr);
      formData.append('PhoneNumber', clubPhone);
      formData.append('Email', clubEmail);
      if (logoFile) {
        formData.append('Logo', logoFile);
      }

      const apiBase = getApiBaseUrl();
      const token = localStorage.getItem('token');
      const res = await fetch(`${apiBase}/clubs`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        },
        body: formData
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Could not register club.');
      }

      addToast('Club registered successfully!', 'success');
      setClubName('');
      setClubCity('');
      setClubAddr('');
      setClubPhone('');
      setClubEmail('');
      setLogoFile(null);
      loadMyClubs();
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const handleCreateCourtSubmit = async (e) => {
    e.preventDefault();
    if (!selectedClub) return;
    try {
      const formData = new FormData();
      formData.append('Name', courtName);
      formData.append('Description', courtDesc);
      formData.append('SportType', sportType);
      formData.append('MaxCapacity', maxCapacity);
      formData.append('PricePerHour', pricePerHour);
      if (courtImageFile) {
        formData.append('Image', courtImageFile);
      }

      const apiBase = getApiBaseUrl();
      const token = localStorage.getItem('token');
      const res = await fetch(`${apiBase}/clubs/${selectedClub.id}/courts`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        },
        body: formData
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Could not register court.');
      }

      addToast('Court added successfully!', 'success');
      setCourtName('');
      setCourtDesc('');
      setSportType('Football');
      setMaxCapacity(10);
      setPricePerHour(150);
      setCourtImageFile(null);
      loadClubCourts(selectedClub.id);
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const handleGenerateTimeSlots = async (courtId) => {
    try {
      const res = await apiFetch(`/courts/${courtId}/time-slots/generate-weekly`, {
        method: 'POST'
      });
      if (res.ok) {
        addToast('Weekly time slots generated successfully!', 'success');
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to generate slots.', 'error');
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
        addToast(`Booking request ${decision}ed successfully!`, 'success');
        if (selectedClub) {
          loadClubBookings(selectedClub.id);
        }
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Review decision failed.', 'error');
    }
  };

  return (
    <section className="animate-fade-in flex flex-col gap-10">
      <div>
        <h2 className="text-2xl font-bold tracking-tight">Complex Facility Management</h2>
        <p className="text-[#a1a1aa] text-sm">Register clubs, add play courts, generate slots, and manage reservations</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-10">
        
        {/* Left Side: Create Club Form & Clubs selector */}
        <div className="lg:col-span-1 flex flex-col gap-8">
          <div className="glass-panel rounded-2xl p-8 flex flex-col gap-6">
            <h3 className="font-bold text-lg">Register Sports Club</h3>
            <form onSubmit={handleCreateClubSubmit} className="flex flex-col gap-4">
              <div className="flex flex-col gap-1">
                <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Club Name</label>
                <input type="text" required value={clubName} onChange={(e) => setClubName(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none" />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1">
                  <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Governorate</label>
                  <input type="text" required value={clubGov} onChange={(e) => setClubGov(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none" />
                </div>
                <div className="flex flex-col gap-1">
                  <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">City</label>
                  <input type="text" required value={clubCity} onChange={(e) => setClubCity(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none" />
                </div>
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Logo Image</label>
                <input type="file" accept="image/*" onChange={(e) => setLogoFile(e.target.files[0])} className="text-xs text-[#a1a1aa] file:mr-4 file:py-2 file:px-4 file:rounded-lg file:border-0 file:text-xs file:font-semibold file:bg-[#84cc16]/15 file:text-[#a3e635]" />
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Address</label>
                <input type="text" required value={clubAddr} onChange={(e) => setClubAddr(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none" />
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Phone</label>
                <input type="text" required value={clubPhone} onChange={(e) => setClubPhone(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none" />
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Email</label>
                <input type="email" required value={clubEmail} onChange={(e) => setClubEmail(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none" placeholder="club@complex.com" />
              </div>
              <button type="submit" className="w-full py-3.5 bg-[#84cc16] hover:bg-[#84cc16]/80 text-white font-bold rounded-xl text-xs shadow-lg mt-2">Publish Club</button>
            </form>
          </div>

          {/* Clubs list selection */}
          <div className="glass-panel rounded-2xl p-6 flex flex-col gap-4">
            <h3 className="font-bold text-sm">Select Managed Club</h3>
            <div className="flex flex-col gap-2">
              {clubs.map(club => (
                <button key={club.id} onClick={() => selectClub(club)} className={`flex items-center gap-3 p-3 rounded-xl transition-all text-left border ${
                  selectedClub?.id === club.id ? 'border-[#84cc16] bg-[#84cc16]/10' : 'border-transparent hover:bg-white/5'
                }`}>
                  <img className="w-9 h-9 rounded-lg object-cover" src={club.logoUrl || "https://images.unsplash.com/photo-1574629810360-7efbbe195018?auto=format&fit=crop&w=80&q=80"} alt={club.name} />
                  <div>
                    <h4 className="text-xs font-semibold text-white">{club.name}</h4>
                    <span className="text-[10px] text-[#a1a1aa]">{club.city}</span>
                  </div>
                </button>
              ))}
            </div>
          </div>
        </div>

        {/* Right Side: Manage Courts & Booking requests */}
        <div className="lg:col-span-2 flex flex-col gap-8">
          {selectedClub ? (
            <>
              {/* Add Court to Club */}
              <div className="glass-panel rounded-2xl p-8 flex flex-col gap-6">
                <h3 className="font-bold text-lg">Add Play Court to {selectedClub.name}</h3>
                <form onSubmit={handleCreateCourtSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div className="flex flex-col gap-4">
                    <div className="flex flex-col gap-1">
                      <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Court Name</label>
                      <input type="text" required value={courtName} onChange={(e) => setCourtName(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none" />
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                      <div className="flex flex-col gap-1">
                        <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Sport Type</label>
                        <select value={sportType} onChange={(e) => setSportType(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none">
                          <option value="Football">Football</option>
                          <option value="Padel">Padel</option>
                          <option value="Tennis">Tennis</option>
                          <option value="Basketball">Basketball</option>
                        </select>
                      </div>
                      <div className="flex flex-col gap-1">
                        <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Capacity</label>
                        <input type="number" required value={maxCapacity} onChange={(e) => setMaxCapacity(parseInt(e.target.value))} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none" />
                      </div>
                    </div>
                    <div className="flex flex-col gap-1">
                      <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Price Per Hour (EGP)</label>
                      <input type="number" required value={pricePerHour} onChange={(e) => setPricePerHour(parseFloat(e.target.value))} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none" />
                    </div>
                  </div>

                  <div className="flex flex-col gap-4">
                    <div className="flex flex-col gap-1">
                      <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Court Picture</label>
                      <input type="file" accept="image/*" onChange={(e) => setCourtImageFile(e.target.files[0])} className="text-xs text-[#a1a1aa] file:mr-4 file:py-2 file:px-4 file:rounded-lg file:border-0 file:text-xs file:font-semibold file:bg-[#84cc16]/15 file:text-[#a3e635]" />
                    </div>
                    <div className="flex flex-col gap-1">
                      <label className="text-[10px] font-bold text-[#a1a1aa] uppercase tracking-wider">Description</label>
                      <textarea value={courtDesc} onChange={(e) => setCourtDesc(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl p-4 text-xs text-white outline-none resize-none h-[75px]" placeholder="Lights quality, grass type..." />
                    </div>
                    <button type="submit" className="w-full py-3.5 bg-[#84cc16] hover:bg-[#84cc16]/80 text-white font-bold rounded-xl text-xs shadow-lg mt-auto">Add Court</button>
                  </div>
                </form>
              </div>

              {/* Courts Listing with Slot Generator */}
              <div className="glass-panel rounded-2xl p-8">
                <h3 className="font-bold text-lg mb-6">Play Courts & Schedulers</h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {courts.map(c => (
                    <div key={c.id} className="p-4 border border-[#ffffff08] bg-white/3 rounded-xl flex justify-between items-center">
                      <div>
                        <h4 className="font-semibold text-sm">{c.name}</h4>
                        <span className="text-[10px] text-[#a1a1aa]">{c.sportType} | {c.pricePerHour} EGP/hr</span>
                      </div>
                      <button onClick={() => handleGenerateTimeSlots(c.id)} className="px-4 py-2 bg-[#84cc16]/15 hover:bg-[#84cc16]/30 text-[#a3e635] border border-[#84cc16]/20 rounded-lg text-[10px] font-bold transition-all">Generate Slots</button>
                    </div>
                  ))}
                </div>
              </div>

              {/* Bookings requests review list */}
              <div className="glass-panel rounded-2xl p-8">
                <h3 className="font-bold text-lg mb-6">Reservation Requests Review</h3>
                <div className="flex flex-col gap-4">
                  {clubBookings.length > 0 ? clubBookings.map(b => (
                    <div key={b.id} className="flex justify-between items-center p-4 border border-[#ffffff08] bg-white/3 rounded-xl">
                      <div>
                        <h4 className="font-semibold text-sm">{b.courtName}</h4>
                        <p className="text-xs text-[#a1a1aa] mt-0.5">{b.bookingDate} | {b.startTime?.substring(0,5)} - {b.endTime?.substring(0,5)}</p>
                        <span className="text-[10px] text-[#71717a] block mt-1">Player: {b.playerName || 'Member'}</span>
                      </div>
                      {b.status === 'Pending' ? (
                        <div className="flex gap-2">
                          <button onClick={() => handleReviewBooking(b.id, 'Confirm')} className="px-3.5 py-2 bg-[#10b981]/15 hover:bg-[#10b981]/30 text-[#10b981] border border-[#10b981]/25 rounded-lg text-xs font-semibold flex items-center gap-1 transition-all"><Check className="w-3.5 h-3.5" /> Confirm</button>
                          <button onClick={() => handleReviewBooking(b.id, 'Reject')} className="px-3.5 py-2 bg-[#ef4444]/15 hover:bg-[#ef4444]/30 text-[#ef4444] border border-[#ef4444]/25 rounded-lg text-xs font-semibold flex items-center gap-1 transition-all"><Ban className="w-3.5 h-3.5" /> Reject</button>
                        </div>
                      ) : (
                        <span className={`text-xs font-bold ${b.status === 'Confirmed' ? 'text-[#10b981]' : 'text-[#ef4444]'}`}>{b.status}</span>
                      )}
                    </div>
                  )) : (
                    <p className="text-sm text-[#71717a] text-center py-6">No reservations found.</p>
                  )}
                </div>
              </div>
            </>
          ) : (
            <div className="glass-panel rounded-2xl p-12 text-center text-[#71717a] h-full flex flex-col justify-center items-center">
              <Settings className="w-12 h-12 mb-3 animate-spin" style={{ animationDuration: '4s' }} />
              <p className="text-sm">Select a club from the left panel to configure its courts and review bookings.</p>
            </div>
          )}
        </div>
      </div>
    </section>
  );
}
