import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Search, MapPin, Star, Calendar, Shield, Wifi, Compass, Layers, Users } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function CourtsView({ addToast }) {
  const navigate = useNavigate();
  const [courts, setCourts] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  // Search filter states
  const [sport, setSport] = useState('Football');
  const [city, setCity] = useState('');
  const [date, setDate] = useState(() => new Date().toISOString().split('T')[0]);

  useEffect(() => {
    loadCourts();
  }, []);

  const loadCourts = async () => {
    try {
      setIsLoading(true);
      let query = `/courts?sport=${sport}&date=${date}`;
      if (city.trim()) {
        query += `&city=${encodeURIComponent(city.trim())}`;
      }
      
      const res = await apiFetch(query);
      if (res.ok) {
        const data = await res.json();
        const items = data.items || data || [];
        
        // Fetch reviews dynamically for each court to get real rating
        const courtsWithRatings = await Promise.all(items.map(async (crt) => {
          try {
            const revRes = await apiFetch(`/courts/${crt.id}/reviews`);
            if (revRes.ok) {
              const revData = await revRes.json();
              const reviewsList = revData.items || revData || [];
              if (reviewsList.length > 0) {
                const avg = reviewsList.reduce((sum, r) => sum + r.rating, 0) / reviewsList.length;
                return { ...crt, rating: avg.toFixed(1), reviewsCount: reviewsList.length };
              }
            }
          } catch (e) {}
          return { ...crt, rating: null, reviewsCount: 0 };
        }));
        setCourts(courtsWithRatings);
      }
    } catch (e) {
      addToast('Failed to load courts directory.', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    loadCourts();
  };

  return (
    <div className="flex flex-col gap-8 animate-fade-in text-xs text-[#a1a1aa]">
      <div>
        <h2 className="text-2xl font-bold tracking-tight text-white">Find Available Courts</h2>
        <p className="text-[#a1a1aa] text-sm mt-1">Search, compare rates, and book slots instantly</p>
      </div>

      {/* Search Filter Header */}
      <form onSubmit={handleSearchSubmit} className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl grid grid-cols-1 md:grid-cols-4 gap-4 shadow-xl">
        <div className="flex flex-col gap-1">
          <label className="text-[10px] uppercase font-bold text-[#71717a]">Sport Type</label>
          <select value={sport} onChange={(e) => setSport(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]">
            <option value="Football">Football</option>
            <option value="Padel">Padel</option>
            <option value="Tennis">Tennis</option>
            <option value="Basketball">Basketball</option>
          </select>
        </div>

        <div className="flex flex-col gap-1">
          <label className="text-[10px] uppercase font-bold text-[#71717a]">City Location</label>
          <input type="text" placeholder="e.g. Maadi" value={city} onChange={(e) => setCity(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]" />
        </div>

        <div className="flex flex-col gap-1">
          <label className="text-[10px] uppercase font-bold text-[#71717a]">Target Date</label>
          <input type="date" value={date} onChange={(e) => setDate(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]" />
        </div>

        <button type="submit" className="bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all flex items-center justify-center gap-2 mt-auto py-2.5">
          <Search className="w-4 h-4" /> Search Slots
        </button>
      </form>

      {/* Courts Marketplace Catalog list */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {isLoading ? (
          [1, 2, 3].map(n => (
            <div key={n} className="bg-[#121216]/50 border border-white/5 rounded-2xl h-64 animate-pulse"></div>
          ))
        ) : courts.length === 0 ? (
          <div className="col-span-3 text-center py-16 text-[#71717a]">
            <Layers className="w-12 h-12 mx-auto text-[#71717a] opacity-30 mb-3" />
            <h3 className="font-bold text-white text-base">No Matching Courts Found</h3>
            <p className="text-xs mt-1">Try expanding your search parameters or check alternative dates.</p>
          </div>
        ) : (
          courts.map(crt => (
            <div key={crt.id} onClick={() => navigate(`/club/${crt.clubId}/court/${crt.id}`)} className="bg-[#121216]/50 border border-white/5 rounded-2xl overflow-hidden hover:border-[#84cc16]/30 transition-all duration-300 cursor-pointer group">
              <div className="h-44 bg-cover bg-center relative" style={{ backgroundImage: `url(https://images.unsplash.com/photo-1541534741688-6078c6bfb5c5?auto=format&fit=crop&w=500&q=80)` }}>
                <span className="absolute top-4 left-4 px-2.5 py-1 bg-[#84cc16]/90 text-black font-extrabold text-[9px] rounded uppercase tracking-wider">{crt.sportType}</span>
                <span className="absolute bottom-4 right-4 px-3 py-1 bg-black/70 border border-white/10 text-white font-extrabold rounded-lg">{crt.pricePerHour} EGP / Hr</span>
              </div>

              <div className="p-5 flex flex-col gap-4">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="font-bold text-white text-base truncate">{crt.name}</h3>
                    <p className="text-[11px] text-[#71717a] mt-0.5 flex items-center gap-1"><MapPin className="w-3.5 h-3.5" /> Complex Facility</p>
                  </div>
                  <div className="flex items-center gap-0.5 text-xs text-amber-400 font-bold shrink-0">
                    {crt.rating ? (
                      <>
                        <Star className="w-3.5 h-3.5 fill-amber-400" />
                        <span>{crt.rating} <b className="text-[10px] text-[#71717a] font-normal">({crt.reviewsCount})</b></span>
                      </>
                    ) : (
                      <span className="text-[10px] text-[#84cc16] bg-[#84cc16]/10 px-2 py-0.5 rounded font-bold uppercase">New</span>
                    )}
                  </div>
                </div>

                {/* Distances and Facilities badges */}
                <div className="flex flex-wrap gap-2 border-t border-white/5 pt-3 text-[10px]">
                  <span className="px-2 py-1 rounded bg-white/5 border border-white/5 flex items-center gap-1 text-[#a1a1aa]"><Compass className="w-3 h-3 text-[#84cc16]" /> {crt.sportType}</span>
                  <span className="px-2 py-1 rounded bg-white/5 border border-white/5 flex items-center gap-1 text-[#a1a1aa]"><Users className="w-3 h-3 text-[#84cc16]" /> Max {crt.maxCapacity || 10} Players</span>
                  <span className="px-2 py-1 rounded bg-white/5 border border-white/5 flex items-center gap-1 text-[#a1a1aa]"><Shield className="w-3 h-3 text-[#84cc16]" /> Active Facility</span>
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
