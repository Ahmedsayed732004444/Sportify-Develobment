import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { MapPin, Phone, Mail, Star, Calendar, ChevronRight } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function ClubDetailView({ addToast }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const [club, setClub] = useState(null);
  const [courts, setCourts] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadClubDetails();
  }, [id]);

  const loadClubDetails = async () => {
    try {
      setIsLoading(true);
      const clubRes = await apiFetch(`/clubs/${id}`);
      if (clubRes.ok) {
        const clubData = await clubRes.json();
        setClub(clubData);
        
        // Log this club to recently viewed list in localStorage
        logRecentlyViewed(clubData);
      }

      const courtsRes = await apiFetch(`/clubs/${id}/courts`);
      if (courtsRes.ok) {
        const courtsData = await courtsRes.json();
        setCourts(courtsData.items || courtsData || []);
      }

      const reviewsRes = await apiFetch(`/clubs/${id}/reviews`);
      if (reviewsRes.ok) {
        const reviewsData = await reviewsRes.json();
        setReviews(reviewsData.items || reviewsData || []);
      }
    } catch (e) {
      addToast('Failed to retrieve club details.', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const logRecentlyViewed = (clubData) => {
    try {
      let recents = JSON.parse(localStorage.getItem('recent_courts') || '[]');
      recents = recents.filter(r => r.id !== clubData.id);
      recents.unshift({
        id: clubData.id,
        name: clubData.name,
        city: clubData.city,
        logoUrl: clubData.logoUrl,
        rating: null
      });
      localStorage.setItem('recent_courts', JSON.stringify(recents.slice(0, 5)));
    } catch (err) {}
  };

  if (isLoading) {
    return (
      <div className="max-w-[800px] mx-auto flex flex-col gap-6 py-20 animate-fade-in text-[#71717a]">
        <div className="h-40 bg-[#1e1e26]/30 rounded-3xl animate-pulse"></div>
        <div className="h-6 w-48 bg-[#1e1e26]/30 rounded animate-pulse"></div>
        <div className="h-4 w-96 bg-[#1e1e26]/30 rounded animate-pulse"></div>
      </div>
    );
  }

  if (!club) {
    return (
      <div className="text-center py-20 text-[#71717a]">
        <h3 className="font-bold text-white text-lg">Club Not Found</h3>
        <p className="text-xs mt-1">The specified facility has been disabled or does not exist.</p>
      </div>
    );
  }

  const reviewsCount = reviews.length;
  const overallRating = reviewsCount > 0
    ? (reviews.reduce((sum, r) => sum + r.rating, 0) / reviewsCount).toFixed(1)
    : null;

  return (
    <div className="max-w-[900px] mx-auto flex flex-col gap-10 pb-20 animate-fade-in text-xs text-[#a1a1aa]">
      
      {/* Club Cover Banner */}
      <div className="bg-[#121216]/50 border border-white/5 rounded-3xl overflow-hidden shadow-2xl relative">
        <div className="h-48 w-full bg-cover bg-center" style={{ backgroundImage: `url(${club.logoUrl || 'https://images.unsplash.com/photo-1541534741688-6078c6bfb5c5?auto=format&fit=crop&w=800&q=80'})` }}></div>
        <div className="p-8 flex flex-col gap-4">
          <div className="flex justify-between items-start">
            <div>
              <h2 className="text-2xl font-extrabold text-white">{club.name}</h2>
              <p className="text-xs text-[#84cc16] font-bold mt-1 flex items-center gap-1"><MapPin className="w-4 h-4" /> {club.address}, {club.city}, {club.governorate}</p>
            </div>
            <div className="flex items-center gap-1 bg-[#84cc16]/10 border border-[#84cc16]/20 px-3 py-1.5 rounded-xl text-[#a3e635] font-extrabold shrink-0">
              <Star className="w-4 h-4 fill-[#a3e635]" />
              <span>{overallRating ? `${overallRating} (${reviewsCount} Reviews)` : 'New'}</span>
            </div>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3 mt-2 border-t border-white/5 pt-4 text-[11px]">
            <span className="flex items-center gap-1.5"><Phone className="w-4 h-4 text-[#84cc16]" /> {club.phoneNumber || 'N/A'}</span>
            <span className="flex items-center gap-1.5"><Mail className="w-4 h-4 text-[#84cc16]" /> {club.email || 'N/A'}</span>
          </div>
        </div>
      </div>

      {/* Club Reviews Section */}
      <div className="bg-[#121216]/50 border border-white/5 rounded-3xl p-8 flex flex-col gap-6">
        <h3 className="font-extrabold text-white text-base">Reviews & Ratings</h3>
        {reviews.length === 0 ? (
          <p className="text-xs text-[#71717a]">No customer reviews submitted yet.</p>
        ) : (
          <div className="flex flex-col gap-4">
            {reviews.map(r => (
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
            ))}
          </div>
        )}
      </div>

      {/* Courts Available list */}
      <div className="flex flex-col gap-6">
        <h3 className="font-extrabold text-white text-lg">Available Sports Courts</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {courts.length === 0 ? (
            <p className="text-xs text-[#71717a] py-4">No active courts available under this complex.</p>
          ) : (
            courts.map(crt => (
              <div key={crt.id} onClick={() => navigate(`/club/${id}/court/${crt.id}`)} className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex justify-between items-center cursor-pointer hover:border-[#84cc16]/30 transition-all duration-300">
                <div className="flex flex-col gap-1.5">
                  <span className="px-2 py-0.5 bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#a3e635] rounded font-bold text-[9px] uppercase tracking-wider w-fit">{crt.sportType}</span>
                  <h4 className="font-bold text-white text-base mt-1">{crt.name}</h4>
                  <p className="text-xs text-[#84cc16] font-bold mt-1">{crt.pricePerHour} EGP <span className="text-[#a1a1aa] font-normal">/ Hour</span></p>
                </div>
                <div className="w-10 h-10 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center text-white"><ChevronRight className="w-5 h-5" /></div>
              </div>
            ))
          )}
        </div>
      </div>

    </div>
  );
}
