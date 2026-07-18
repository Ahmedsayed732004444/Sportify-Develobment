import React, { useState, useEffect } from 'react';
import { 
  Building, Calendar, Trophy, Sparkles, Search, MapPin, 
  ChevronRight, Quote, Check, Star, ArrowRight, UserPlus, Heart, History, HelpCircle
} from 'lucide-react';
import { apiFetch } from '../services/api';

export default function HomeView({ onNavigate, user, onTriggerAuth, addToast }) {
  const [clubs, setClubs] = useState([]);
  const [matches, setMatches] = useState([]);
  
  // Search state
  const [gov, setGov] = useState('');
  const [city, setCity] = useState('');
  const [sport, setSport] = useState('Football');
  const [date, setDate] = useState('');

  // Favorites & Recently viewed states (Local Storage backed)
  const [favorites, setFavorites] = useState([]);
  const [recentlyViewed, setRecentlyViewed] = useState([]);

  // Testimonials state
  const [testimonialIdx, setTestimonialIdx] = useState(0);
  const testimonials = [
    { name: "Ahmed Salem", role: "Padel Enthusiast", quote: "Sportify transformed how we schedule our weekly padel matches. In 2 minutes, we book courts and invite friends." },
    { name: "Mariam Ali", role: "Club Owner", quote: "The Club Management dashboard streamlined our reservations. Our court utilization increased by 40%!" },
    { name: "Kareem Fahmy", role: "Football Captain", quote: "We always struggled to find a tenth player for our matches. Now we host public rosters and meet great new teammates." }
  ];

  // FAQ state
  const [faqOpenIdx, setFaqOpenIdx] = useState(null);
  const faqs = [
    { q: "How do I book a court slot?", a: "Browse our Clubs & Courts panel, select a court, choose an open date/time slot, and proceed to booking instantly!" },
    { q: "Can I cancel a booking?", a: "Yes, bookings can be cancelled up to 24 hours prior to the match start time from your Bookings history tab." },
    { q: "How do I become a Club Partner?", a: "Click on 'Become Owner' in the navigation menu, select a subscription tier, fill in representative credentials, and submit for admin approval." }
  ];

  useEffect(() => {
    loadFeaturedData();
    loadLocalStoreData();
  }, []);

  const loadLocalStoreData = () => {
    try {
      const favs = JSON.parse(localStorage.getItem('fav_courts') || '[]');
      setFavorites(favs);
      
      const recents = JSON.parse(localStorage.getItem('recent_courts') || '[]');
      setRecentlyViewed(recents);
    } catch (e) {}
  };

  const loadFeaturedData = async () => {
    try {
      const clubsRes = await apiFetch('/clubs');
      if (clubsRes.ok) {
        const clubsData = await clubsRes.json();
        const items = clubsData.items || [];
        
        // Fetch reviews for each of the top 3 clubs to calculate real average rating
        const featuredClubs = items.slice(0, 3);
        const clubsWithRatings = await Promise.all(featuredClubs.map(async (club) => {
          try {
            const revRes = await apiFetch(`/clubs/${club.id}/reviews`);
            if (revRes.ok) {
              const revData = await revRes.json();
              const reviewsList = revData.items || revData || [];
              if (reviewsList.length > 0) {
                const avg = reviewsList.reduce((sum, r) => sum + r.rating, 0) / reviewsList.length;
                return { ...club, rating: avg.toFixed(1), reviewsCount: reviewsList.length };
              }
            }
          } catch (e) {}
          return { ...club, rating: null, reviewsCount: 0 };
        }));
        setClubs(clubsWithRatings);
      }

      const matchesRes = await apiFetch('/friendly-matches');
      if (matchesRes.ok) {
        const matchesData = await matchesRes.json();
        setMatches((matchesData.items || []).slice(0, 3));
      }
    } catch (e) {}
  };

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    onNavigate('/clubs');
    addToast('Applying search filters...', 'info');
  };

  const toggleFavorite = (club, e) => {
    e.stopPropagation();
    let updated;
    const isFav = favorites.some(f => f.id === club.id);
    if (isFav) {
      updated = favorites.filter(f => f.id !== club.id);
      addToast('Removed from saved courts', 'info');
    } else {
      updated = [...favorites, { id: club.id, name: club.name, city: club.city, logoUrl: club.logoUrl, rating: club.rating || null }];
      addToast('Saved to favorites!', 'success');
    }
    setFavorites(updated);
    localStorage.setItem('fav_courts', JSON.stringify(updated));
  };

  return (
    <div className="flex flex-col gap-20 pb-20 animate-fade-in">
      
      {/* 1. HERO SECTION */}
      <section className="relative rounded-3xl overflow-hidden bg-gradient-to-br from-[#121216]/90 via-[#0e0e12]/85 to-[#050508]/90 border border-white/5 p-8 md:p-16 flex flex-col items-center text-center gap-6 shadow-2xl">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top,_var(--tw-gradient-stops))] from-[#84cc16]/10 via-transparent to-transparent pointer-events-none"></div>
        
        <span className="px-3.5 py-1.5 bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#a3e635] rounded-full text-xs font-bold uppercase tracking-widest flex items-center gap-1.5">
          <Sparkles className="w-4 h-4 animate-pulse" /> Sportify Marketplace Platform
        </span>

        <h1 className="text-4xl md:text-6xl font-black tracking-tight text-white max-w-[800px] leading-tight">
          Find Clubs, Book Courts, & Organise <span className="text-[#84cc16]">Friendly Matches</span>
        </h1>
        
        <p className="text-sm md:text-base text-[#a1a1aa] max-w-[600px] leading-relaxed">
          The premium sports venue marketplace. Find facilities, book slots, host matches, and join tournaments easily.
        </p>

        <div className="flex gap-4 mt-4">
          <button onClick={() => onNavigate('/clubs')} className="px-6 py-3.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl text-sm transition-all shadow-lg shadow-[#84cc16]/15 flex items-center gap-2">
            Explore Facilities <ArrowRight className="w-4 h-4" />
          </button>
          {!user && (
            <button onClick={onTriggerAuth} className="px-6 py-3.5 bg-white/5 hover:bg-white/10 border border-white/10 text-white font-bold rounded-xl text-sm transition-all flex items-center gap-2">
              <UserPlus className="w-4 h-4" /> Create Account
            </button>
          )}
        </div>
      </section>

      {/* 2. DYNAMIC SEARCH BAR */}
      <section className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl shadow-xl -mt-10 mx-4 relative z-10">
        <form onSubmit={handleSearchSubmit} className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="flex flex-col gap-1.5">
            <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">Governorate</label>
            <input type="text" placeholder="e.g. Cairo" value={gov} onChange={(e) => setGov(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
          </div>
          <div className="flex flex-col gap-1.5">
            <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">City</label>
            <input type="text" placeholder="e.g. Maadi" value={city} onChange={(e) => setCity(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
          </div>
          <div className="flex flex-col gap-1.5">
            <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">Sport Type</label>
            <select value={sport} onChange={(e) => setSport(e.target.value)} className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]">
              <option value="Football">Football</option>
              <option value="Padel">Padel</option>
              <option value="Tennis">Tennis</option>
              <option value="Basketball">Basketball</option>
            </select>
          </div>
          <button type="submit" className="bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all flex items-center justify-center gap-2 mt-auto py-3">
            <Search className="w-4 h-4" /> Apply Filter
          </button>
        </form>
      </section>

      {/* 3. SPORTS CATEGORIES */}
      <section className="flex flex-col gap-6">
        <div>
          <h2 className="text-2xl font-extrabold text-white">Popular Sports</h2>
          <p className="text-xs text-[#a1a1aa] mt-1">Book specialized slots instantly across core disciplines</p>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {['Football', 'Padel', 'Tennis', 'Basketball'].map(s => (
            <div key={s} onClick={() => { setSport(s); onNavigate('/clubs'); }} className="bg-[#121216]/50 border border-white/5 rounded-2xl p-6 text-center cursor-pointer hover:border-[#84cc16]/30 transition-all flex flex-col items-center gap-3">
              <div className="w-12 h-12 rounded-xl bg-[#84cc16]/10 flex items-center justify-center text-[#84cc16]"><Trophy className="w-6 h-6" /></div>
              <h3 className="font-bold text-white text-sm">{s}</h3>
            </div>
          ))}
        </div>
      </section>

      {/* 4. FEATURED CLUBS WITH SAVED/FAVORITES CHECK */}
      <section className="flex flex-col gap-6">
        <div className="flex justify-between items-end">
          <div>
            <h2 className="text-2xl font-extrabold text-white">Featured Sports Clubs</h2>
            <p className="text-xs text-[#a1a1aa] mt-1">Browse premium verified sports venues</p>
          </div>
          <button onClick={() => onNavigate('/clubs')} className="text-xs font-bold text-[#84cc16] hover:underline flex items-center gap-1">View All <ChevronRight className="w-4 h-4" /></button>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {clubs.length === 0 ? (
            <p className="text-xs text-[#71717a]">No active clubs available.</p>
          ) : (
            clubs.map(c => {
              const isFav = favorites.some(f => f.id === c.id);
              return (
                <div key={c.id} className="bg-[#121216]/50 border border-white/5 rounded-2xl overflow-hidden hover:border-[#84cc16]/30 transition-all relative group">
                  <button onClick={(e) => toggleFavorite(c, e)} className="absolute top-4 right-4 z-10 w-9 h-9 rounded-full bg-black/45 border border-white/10 flex items-center justify-center text-white hover:text-red-500 transition-all">
                    <Heart className={`w-4.5 h-4.5 ${isFav ? 'fill-red-500 text-red-500' : 'text-white'}`} />
                  </button>
                  <div className="h-44 bg-cover bg-center" style={{ backgroundImage: `url(${c.logoUrl || 'https://images.unsplash.com/photo-1541534741688-6078c6bfb5c5?auto=format&fit=crop&w=500&q=80'})` }}></div>
                  <div className="p-6 flex flex-col gap-3">
                    <div className="flex justify-between items-start">
                      <h3 className="font-bold text-white text-base truncate max-w-[70%]">{c.name}</h3>
                      <div className="flex items-center gap-0.5 text-xs text-amber-400 font-bold shrink-0">
                        {c.rating ? (
                          <>
                            <Star className="w-3.5 h-3.5 fill-amber-400" />
                            <span>{c.rating} <b className="text-[10px] text-[#71717a] font-normal">({c.reviewsCount})</b></span>
                          </>
                        ) : (
                          <span className="text-[10px] text-[#84cc16] bg-[#84cc16]/10 px-2 py-0.5 rounded font-bold uppercase">New</span>
                        )}
                      </div>
                    </div>
                    <p className="text-xs text-[#a1a1aa] flex items-center gap-1"><MapPin className="w-3.5 h-3.5 text-[#84cc16]" /> {c.city}, {c.governorate}</p>
                    <button onClick={() => onNavigate('/clubs')} className="w-full mt-2 py-3 bg-[#84cc16]/10 hover:bg-[#84cc16]/20 text-[#a3e635] text-xs font-bold rounded-xl transition-all">View Courts & Book</button>
                  </div>
                </div>
              );
            })
          )}
        </div>
      </section>

      {/* 5. SAVED COURTS / RECENTS ROW */}
      {(favorites.length > 0 || recentlyViewed.length > 0) && (
        <section className="grid grid-cols-1 md:grid-cols-2 gap-8">
          {/* Favorites */}
          {favorites.length > 0 && (
            <div className="flex flex-col gap-4">
              <h3 className="font-extrabold text-white text-lg flex items-center gap-2"><Heart className="w-5 h-5 text-red-500 fill-red-500" /> Saved Favorites</h3>
              <div className="flex flex-col gap-3">
                {favorites.map(fav => (
                  <div key={fav.id} onClick={() => onNavigate('/clubs')} className="bg-[#121216]/40 border border-white/5 p-4 rounded-xl flex items-center gap-4 cursor-pointer hover:border-[#84cc16]/20 transition-all">
                    <img className="w-12 h-12 rounded-lg object-cover" src={fav.logoUrl || 'https://images.unsplash.com/photo-1541534741688-6078c6bfb5c5?auto=format&fit=crop&w=120&q=80'} alt="" />
                    <div className="flex-1 min-w-0">
                      <h4 className="text-xs font-bold text-white truncate">{fav.name}</h4>
                      <p className="text-[10px] text-[#71717a] mt-0.5">{fav.city}</p>
                    </div>
                    <ChevronRight className="w-4 h-4 text-[#71717a]" />
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Recents */}
          {recentlyViewed.length > 0 && (
            <div className="flex flex-col gap-4">
              <h3 className="font-extrabold text-white text-lg flex items-center gap-2"><History className="w-5 h-5 text-[#84cc16]" /> Recently Viewed</h3>
              <div className="flex flex-col gap-3">
                {recentlyViewed.map(r => (
                  <div key={r.id} onClick={() => onNavigate('/clubs')} className="bg-[#121216]/40 border border-white/5 p-4 rounded-xl flex items-center gap-4 cursor-pointer hover:border-[#84cc16]/20 transition-all">
                    <img className="w-12 h-12 rounded-lg object-cover" src={r.logoUrl || 'https://images.unsplash.com/photo-1541534741688-6078c6bfb5c5?auto=format&fit=crop&w=120&q=80'} alt="" />
                    <div className="flex-1 min-w-0">
                      <h4 className="text-xs font-bold text-white truncate">{r.name}</h4>
                      <p className="text-[10px] text-[#71717a] mt-0.5">{r.city}</p>
                    </div>
                    <ChevronRight className="w-4 h-4 text-[#71717a]" />
                  </div>
                ))}
              </div>
            </div>
          )}
        </section>
      )}

      {/* 6. FRIENDLY MATCHES NEARBY */}
      <section className="flex flex-col gap-6">
        <div className="flex justify-between items-end">
          <div>
            <h2 className="text-2xl font-extrabold text-white">Friendly Matches Nearby</h2>
            <p className="text-xs text-[#a1a1aa] mt-1">Join active lobbies looking for extra players</p>
          </div>
          <button onClick={() => onNavigate('/friendly-matches')} className="text-xs font-bold text-[#84cc16] hover:underline flex items-center gap-1">Browse Matches <ChevronRight className="w-4 h-4" /></button>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {matches.length === 0 ? (
            <p className="text-xs text-[#71717a]">No public match lobbies currently active.</p>
          ) : (
            matches.map(m => (
              <div key={m.matchId} className="bg-[#121216]/50 border border-white/5 rounded-2xl p-6 flex flex-col justify-between gap-4">
                <div className="flex flex-col gap-2">
                  <div className="flex justify-between items-center">
                    <span className="px-2.5 py-0.5 bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#a3e635] rounded text-[10px] font-bold uppercase">{m.sportType}</span>
                    <span className="text-[10px] text-[#71717a]">{m.date}</span>
                  </div>
                  <h4 className="font-bold text-white text-base mt-1">Organizer: {m.organizer?.name || 'Player'}</h4>
                  <span className="text-xs text-[#a1a1aa] flex items-center gap-1"><MapPin className="w-3.5 h-3.5 text-[#84cc16]" /> {m.court?.clubName || 'Local Complex'}</span>
                </div>
                <div className="flex justify-between items-center border-t border-white/5 pt-3">
                  <span className="text-xs text-amber-400 font-bold">{m.slotsRemaining} Slots Left</span>
                  <button onClick={() => onNavigate('/friendly-matches')} className="px-4 py-2 bg-[#84cc16] hover:bg-[#65a30d] text-black font-bold text-xs rounded-lg transition-colors">Join Roster</button>
                </div>
              </div>
            ))
          )}
        </div>
      </section>

      {/* 7. PARTNER PRICING & BECOME OWNER SECTION */}
      <section className="bg-[#121216]/50 border border-white/5 rounded-3xl p-8 md:p-12 flex flex-col md:flex-row justify-between items-center gap-8 relative overflow-hidden">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_bottom_right,_var(--tw-gradient-stops))] from-[#84cc16]/5 via-transparent to-transparent pointer-events-none"></div>
        <div className="flex flex-col gap-4 text-left md:max-w-[55%]">
          <span className="text-xs font-bold text-[#84cc16] uppercase tracking-widest">Business Partnership</span>
          <h2 className="text-3xl font-black text-white leading-tight">Own a Sports Club? Monetize Your Courts</h2>
          <p className="text-sm text-[#a1a1aa] leading-relaxed">
            Gain absolute control of schedules, avoid double bookings, generate flexible time-slots, and broadcast matches to our community player base.
          </p>
          <button onClick={() => onNavigate('/become-owner')} className="w-fit px-6 py-3.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl text-sm transition-all shadow-lg flex items-center gap-2">
            Get Started <ArrowRight className="w-4 h-4" />
          </button>
        </div>
        <div className="grid grid-cols-2 gap-4 w-full md:w-[40%]">
          <div className="bg-[#1e1e26]/30 border border-white/5 p-4 rounded-xl flex flex-col gap-2">
            <span className="text-[#84cc16] font-bold text-base">0% Booking Fees</span>
            <span className="text-xs text-[#71717a]">On Starter tiers</span>
          </div>
          <div className="bg-[#1e1e26]/30 border border-white/5 p-4 rounded-xl flex flex-col gap-2">
            <span className="text-[#84cc16] font-bold text-base">Instant Sockets</span>
            <span className="text-xs text-[#71717a]">Sync bookings real-time</span>
          </div>
          <div className="bg-[#1e1e26]/30 border border-white/5 p-4 rounded-xl flex flex-col gap-2">
            <span className="text-[#84cc16] font-bold text-base">Dynamic Pricing</span>
            <span className="text-xs text-[#71717a]">Create peak/off-peak rates</span>
          </div>
          <div className="bg-[#1e1e26]/30 border border-white/5 p-4 rounded-xl flex flex-col gap-2">
            <span className="text-[#84cc16] font-bold text-base">Verification Dossier</span>
            <span className="text-xs text-[#71717a]">Approved in 24 hours</span>
          </div>
        </div>
      </section>

      {/* 8. TESTIMONIALS */}
      <section className="bg-[#121216]/30 border border-white/5 rounded-3xl p-8 text-center flex flex-col items-center gap-5">
        <Quote className="w-10 h-10 text-[#84cc16]/40" />
        <p className="text-lg text-white font-medium italic max-w-[700px]">"{testimonials[testimonialIdx].quote}"</p>
        <div className="flex flex-col mt-2">
          <span className="text-sm font-bold text-white">{testimonials[testimonialIdx].name}</span>
          <span className="text-xs text-[#71717a] mt-0.5">{testimonials[testimonialIdx].role}</span>
        </div>
        <div className="flex gap-2 mt-4">
          {testimonials.map((t, idx) => (
            <button key={idx} onClick={() => setTestimonialIdx(idx)} className={`w-2.5 h-2.5 rounded-full transition-all ${idx === testimonialIdx ? 'bg-[#84cc16] w-6' : 'bg-white/10'}`}></button>
          ))}
        </div>
      </section>

      {/* 9. FAQ ACCORDION SECTION */}
      <section className="flex flex-col gap-6 max-w-[800px] mx-auto w-full">
        <div className="text-center">
          <h2 className="text-2xl font-extrabold text-white flex items-center justify-center gap-2"><HelpCircle className="w-6 h-6 text-[#84cc16]" /> Frequently Asked Questions</h2>
          <p className="text-xs text-[#a1a1aa] mt-1">Get immediate guidance on platform operations</p>
        </div>
        <div className="flex flex-col gap-4 mt-4">
          {faqs.map((faq, idx) => {
            const isOpen = faqOpenIdx === idx;
            return (
              <div key={idx} className="bg-[#121216]/50 border border-white/5 rounded-2xl overflow-hidden">
                <button type="button" onClick={() => setFaqOpenIdx(isOpen ? null : idx)} className="w-full p-5 flex justify-between items-center text-xs font-bold text-white text-left hover:bg-white/5 transition-all">
                  <span>{faq.q}</span>
                  <span className="text-[#84cc16] font-bold text-sm">{isOpen ? '−' : '+'}</span>
                </button>
                {isOpen && (
                  <div className="p-5 border-t border-white/5 text-[11px] text-[#a1a1aa] leading-relaxed bg-[#0c0c0f]/20">
                    {faq.a}
                  </div>
                )}
              </div>
            );
          })}
        </div>
      </section>

      {/* 10. PREMIUM MARKETPLACE FOOTER */}
      <footer className="border-t border-white/5 pt-12 flex flex-col gap-8 text-[#71717a] text-[11px]">
        <div className="grid grid-cols-2 md:grid-cols-4 gap-8">
          <div className="flex flex-col gap-3">
            <span className="text-xs font-bold text-white uppercase tracking-wider">Explore</span>
            <button onClick={() => onNavigate('/clubs')} className="hover:text-white transition-all text-left">Browse Clubs</button>
            <button onClick={() => onNavigate('/friendly-matches')} className="hover:text-white transition-all text-left">Friendly Matches</button>
            <button onClick={() => onNavigate('/tournaments')} className="hover:text-white transition-all text-left">Tournaments</button>
          </div>
          <div className="flex flex-col gap-3">
            <span className="text-xs font-bold text-white uppercase tracking-wider">Partnership</span>
            <button onClick={() => onNavigate('/become-owner')} className="hover:text-white transition-all text-left">Become Partner</button>
            <button onClick={() => onNavigate('/become-owner')} className="hover:text-white transition-all text-left">Pricing Tiers</button>
          </div>
          <div className="flex flex-col gap-3">
            <span className="text-xs font-bold text-white uppercase tracking-wider">Company</span>
            <span className="hover:text-white transition-all cursor-pointer">About Us</span>
            <span className="hover:text-white transition-all cursor-pointer">Support Center</span>
          </div>
          <div className="flex flex-col gap-4">
            <div className="flex items-center gap-3">
              <div className="w-8 h-8 rounded-lg bg-[#84cc16] flex items-center justify-center text-black font-extrabold text-sm">S</div>
              <span className="text-sm font-bold text-white">Sportify</span>
            </div>
            <p className="leading-relaxed">Premium sports venues reservation booking dashboard matching captains and owners.</p>
          </div>
        </div>
        <div className="border-t border-white/5 pt-6 flex flex-col md:flex-row justify-between items-center gap-4 text-[10px]">
          <span>© 2026 Sportify Technologies. All rights reserved.</span>
          <div className="flex gap-6">
            <span className="hover:text-white transition-all cursor-pointer">Privacy Policy</span>
            <span className="hover:text-white transition-all cursor-pointer">Terms of Service</span>
          </div>
        </div>
      </footer>

    </div>
  );
}
