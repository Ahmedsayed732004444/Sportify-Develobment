import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Building, MapPin, Mail, Phone, Star, Sparkles, X } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function ClubsView({ addToast }) {
  const navigate = useNavigate();
  const [clubs, setClubs] = useState([]);
  const [selectedClub, setSelectedClub] = useState(null); // Club object for review
  const [rating, setRating] = useState(5);
  const [reviewComment, setReviewComment] = useState('');

  useEffect(() => {
    loadClubs();
  }, []);

  const loadClubs = async () => {
    try {
      const res = await apiFetch('/clubs');
      if (!res.ok) throw new Error('Failed to retrieve clubs.');
      const data = await res.json();
      setClubs(data.items || []);
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const submitReview = async (e) => {
    e.preventDefault();
    if (!selectedClub) return;
    try {
      const res = await apiFetch('/reviews', {
        method: 'POST',
        body: JSON.stringify({ 
          clubId: selectedClub.id, 
          rating, 
          comment: reviewComment 
        })
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Could not submit review.');
      }

      addToast('Review submitted successfully!', 'success');
      setSelectedClub(null);
      setReviewComment('');
      setRating(5);
      loadClubs(); // reload to reflect any rating updates if needed
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  return (
    <section className="animate-fade-in flex flex-col gap-8 text-xs text-[#a1a1aa]">
      <div>
        <h2 className="text-2xl font-bold tracking-tight text-white">Discover Facilities</h2>
        <p className="text-[#a1a1aa] text-sm mt-1">Explore sports complexes, view rates, and rate your play experiences</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
        {clubs.map(club => (
          <div key={club.id} className="bg-[#121216]/50 border border-white/5 rounded-2xl p-7 flex flex-col justify-between gap-5 hover:-translate-y-1 transition-all duration-300">
            <div className="flex flex-col gap-4">
              <div className="flex items-center gap-4">
                <img className="w-16 h-16 rounded-xl object-cover border border-white/10" src={club.logoUrl || "https://images.unsplash.com/photo-1574629810360-7efbbe195018?auto=format&fit=crop&w=300&q=80"} alt={club.name} />
                <div>
                  <h3 className="font-semibold text-lg text-white">{club.name}</h3>
                  <p className="text-xs text-[#a1a1aa] flex items-center gap-1 mt-0.5"><MapPin className="w-3.5 h-3.5 text-[#84cc16]" /> {club.city}, {club.governorate}</p>
                </div>
              </div>
              <div className="flex flex-wrap gap-2.5">
                <span className="text-xs px-3 py-1.5 rounded-lg border border-[#ffffff08] bg-white/5 flex items-center gap-1.5 text-[#a1a1aa]"><Mail className="w-3.5 h-3.5" /> {club.email || 'N/A'}</span>
                <span className="text-xs px-3 py-1.5 rounded-lg border border-[#ffffff08] bg-white/5 flex items-center gap-1.5 text-[#a1a1aa]"><Phone className="w-3.5 h-3.5" /> {club.phone || 'N/A'}</span>
              </div>
            </div>
            
            <div className="flex gap-3">
              <button onClick={() => navigate(`/club/${club.id}`)} className="flex-1 py-3 rounded-xl bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs transition-all flex items-center justify-center gap-1.5">
                Explore Venue
              </button>
              <button onClick={() => setSelectedClub(club)} className="px-3.5 rounded-xl border border-white/5 hover:bg-white/5 transition-all text-[#a1a1aa] hover:text-white" title="Rate Club">
                <Star className="w-4 h-4 text-amber-400 fill-amber-400" />
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Review Modal popup */}
      {selectedClub && (
        <div className="fixed inset-0 z-50 bg-[#000]/60 backdrop-blur-sm flex items-center justify-center p-4">
          <div className="bg-[#121216] border border-white/5 rounded-2xl w-full max-w-[450px] p-8 shadow-2xl relative animate-scale-up">
            <button onClick={() => setSelectedClub(null)} className="absolute top-6 right-6 text-[#71717a] hover:text-white transition-colors">
              <X className="w-5 h-5" />
            </button>
            <h3 className="text-lg font-bold mb-1 text-white">Rate Facility</h3>
            <p className="text-xs text-[#a1a1aa] mb-6">Leave rating for {selectedClub.name}</p>

            <form onSubmit={submitReview} className="flex flex-col gap-5">
              <div className="flex flex-col gap-2">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Score</label>
                <div className="flex gap-2">
                  {[1, 2, 3, 4, 5].map(stars => (
                    <button key={stars} type="button" onClick={() => setRating(stars)} className="transition-transform hover:scale-110">
                      <Star className={`w-8 h-8 ${stars <= rating ? 'text-amber-400 fill-amber-400' : 'text-[#71717a]'}`} />
                    </button>
                  ))}
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Comments</label>
                <textarea required value={reviewComment} onChange={(e) => setReviewComment(e.target.value)} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl p-4 text-xs outline-none text-white focus:border-[#84cc16] resize-none h-[100px]" placeholder="Share your experience..." />
              </div>

              <button type="submit" className="w-full py-4 rounded-xl bg-[#84cc16] text-black font-bold hover:bg-[#65a30d] transition-colors shadow-lg">Submit Review</button>
            </form>
          </div>
        </div>
      )}

    </section>
  );
}
