import React, { useState, useEffect } from 'react';
import { useOutletContext } from 'react-router-dom';
import { apiFetch } from '../../services/api';
import { Star, MessageSquare, AlertCircle, RefreshCw, User } from 'lucide-react';

export default function OwnerReviewsView({ addToast }) {
  const { selectedClub } = useOutletContext();
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (selectedClub) {
      loadReviews();
    }
  }, [selectedClub]);

  const loadReviews = async () => {
    setLoading(true);
    try {
      const res = await apiFetch(`/clubs/${selectedClub.id}/reviews`);
      if (res.ok) {
        const data = await res.json();
        setReviews(data.items || data || []);
      }
    } catch (e) {
      addToast('Failed to load reviews.', 'error');
    } finally {
      setLoading(false);
    }
  };

  const avgRating = reviews.length > 0 
    ? (reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length).toFixed(1) 
    : 0;

  // Star distribution counting
  const starCounts = [0, 0, 0, 0, 0];
  reviews.forEach(r => {
    const starIdx = Math.min(Math.max(Math.round(r.rating) - 1, 0), 4);
    starCounts[starIdx]++;
  });

  if (!selectedClub) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-6 bg-[#121216]/40 border border-white/5 rounded-2xl shadow-lg">
        <AlertCircle className="w-12 h-12 text-[#a1a1aa] mb-4" />
        <h3 className="text-lg font-bold text-white mb-2">No active venue selected</h3>
        <p className="text-xs text-[#a1a1aa] max-w-sm">Please register or select a club from the sidebar to check customer reviews.</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-8 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight text-white">Customer Reviews & Ratings</h2>
          <p className="text-[#a1a1aa] text-xs mt-1">Monitor feedback, court ratings, and match host testimonials</p>
        </div>
        <button
          onClick={loadReviews}
          className="p-2.5 bg-white/5 hover:bg-white/10 text-white rounded-xl border border-white/5 transition-all self-start sm:self-center cursor-pointer"
        >
          <RefreshCw className="w-4 h-4" />
        </button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Left: Summary and Distribution */}
        <div className="flex flex-col gap-6">
          <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl shadow-lg flex flex-col items-center text-center">
            <h3 className="text-xs font-bold text-[#71717a] uppercase tracking-wider mb-2">Average Score</h3>
            <span className="text-5xl font-extrabold text-white">{avgRating}</span>
            
            {/* Stars row */}
            <div className="flex items-center gap-1 mt-3 text-orange-400">
              {[1, 2, 3, 4, 5].map(star => {
                const filled = star <= Math.round(avgRating);
                return <Star key={star} className={`w-5 h-5 ${filled ? 'fill-current' : 'opacity-20'}`} />;
              })}
            </div>
            
            <span className="text-[10px] text-[#a1a1aa] mt-2 font-medium">{reviews.length} Ratings total</span>
          </div>

          <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl shadow-lg">
            <h3 className="text-sm font-bold text-white mb-4">Rating Breakdown</h3>
            <div className="flex flex-col gap-3">
              {[5, 4, 3, 2, 1].map((stars) => {
                const count = starCounts[stars - 1] || 0;
                const percent = reviews.length > 0 ? (count / reviews.length) * 100 : 0;
                return (
                  <div key={stars} className="flex items-center gap-3">
                    <span className="text-xs font-semibold text-[#a1a1aa] w-12 shrink-0">{stars} Stars</span>
                    <div className="flex-1 h-2 bg-white/5 rounded-full overflow-hidden">
                      <div className="h-full bg-orange-400 rounded-full" style={{ width: `${percent}%` }}></div>
                    </div>
                    <span className="text-[10px] font-bold text-white w-6 text-right">{count}</span>
                  </div>
                );
              })}
            </div>
          </div>
        </div>

        {/* Right: Reviews List */}
        <div className="lg:col-span-2 flex flex-col gap-4">
          {loading ? (
            <div className="text-xs text-[#a1a1aa] py-20 text-center flex items-center justify-center gap-2 bg-[#121216]/40 border border-white/5 rounded-2xl">
              <RefreshCw className="w-4 h-4 animate-spin text-[#84cc16]" /> Loading reviews...
            </div>
          ) : reviews.length === 0 ? (
            <div className="bg-[#121216]/40 border border-dashed border-white/10 rounded-2xl p-12 text-center flex flex-col items-center">
              <MessageSquare className="w-12 h-12 text-[#71717a] mb-4" />
              <h3 className="text-sm font-bold text-white mb-1">No reviews yet</h3>
              <p className="text-xs text-[#a1a1aa] max-w-sm">
                Customers haven't submitted ratings or feedback for this sports complex yet.
              </p>
            </div>
          ) : (
            <div className="flex flex-col gap-4">
              {reviews.map(review => (
                <div key={review.id} className="bg-[#121216]/60 border border-white/5 rounded-2xl p-6 shadow-lg flex flex-col gap-4 hover:border-white/10 transition-all">
                  
                  {/* Review top row */}
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex items-center gap-2.5">
                      <div className="w-8 h-8 rounded-full bg-white/10 flex items-center justify-center text-[10px] font-bold text-white">
                        {review.playerName ? review.playerName.charAt(0) : 'U'}
                      </div>
                      <div>
                        <h4 className="text-xs font-bold text-white">{review.playerName || 'Anonymous Player'}</h4>
                        <span className="text-[9px] text-[#71717a] font-bold block mt-0.5">{review.createdAt || review.date}</span>
                      </div>
                    </div>

                    <div className="flex items-center gap-0.5 text-orange-400">
                      {[1, 2, 3, 4, 5].map(star => {
                        const filled = star <= review.rating;
                        return <Star key={star} className={`w-3.5 h-3.5 ${filled ? 'fill-current' : 'opacity-20'}`} />;
                      })}
                    </div>
                  </div>

                  {/* Comment */}
                  <p className="text-xs text-[#fafafa] italic leading-relaxed">
                    "{review.comment || 'No comment text provided.'}"
                  </p>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
