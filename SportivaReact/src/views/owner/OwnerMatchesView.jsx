import React, { useState, useEffect } from 'react';
import { useOutletContext } from 'react-router-dom';
import { apiFetch } from '../../services/api';
import { Trophy, Calendar, Users, Shield, Clock, AlertCircle, RefreshCw, Star } from 'lucide-react';

export default function OwnerMatchesView({ addToast }) {
  const { selectedClub } = useOutletContext();
  const [matches, setMatches] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (selectedClub) {
      loadClubFriendlyMatches();
    }
  }, [selectedClub]);

  const loadClubFriendlyMatches = async () => {
    setLoading(true);
    try {
      // 1. Get all courts of the active club
      const courtsRes = await apiFetch(`/clubs/${selectedClub.id}/courts`);
      if (!courtsRes.ok) throw new Error();
      const courtsData = await courtsRes.json();
      const courtsList = courtsData.items || courtsData || [];

      // 2. Fetch matches for all courts in parallel
      const matchPromises = courtsList.map(async (court) => {
        try {
          const res = await apiFetch(`/friendly-matches/court/${court.id}`);
          if (res.ok) {
            const data = await res.json();
            // Attach court name to match objects
            return (data.items || data || []).map(m => ({
              ...m,
              courtName: court.name,
              pricePerHour: court.pricePerHour
            }));
          }
        } catch (e) {
          console.error(`Error loading matches for court ${court.id}`, e);
        }
        return [];
      });

      const results = await Promise.all(matchPromises);
      // Flatten all results into one array
      const allMatches = results.flat();
      
      // Sort matches by date/time
      allMatches.sort((a, b) => (a.date || '').localeCompare(b.date || ''));
      setMatches(allMatches);

    } catch (e) {
      addToast('Failed to load court friendly matches.', 'error');
    } finally {
      setLoading(false);
    }
  };

  if (!selectedClub) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-6 bg-[#121216]/40 border border-white/5 rounded-2xl shadow-lg">
        <AlertCircle className="w-12 h-12 text-[#a1a1aa] mb-4" />
        <h3 className="text-lg font-bold text-white mb-2">No active venue selected</h3>
        <p className="text-xs text-[#a1a1aa] max-w-sm">Please register or select a club from the sidebar to monitor friendly matches.</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-8 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight text-white">Friendly Matches Monitor</h2>
          <p className="text-[#a1a1aa] text-xs mt-1">Live tracking of user-organized friendly matches and player registration slots</p>
        </div>
        <button
          onClick={loadClubFriendlyMatches}
          className="p-2.5 bg-white/5 hover:bg-white/10 text-white rounded-xl border border-white/5 transition-all self-start sm:self-center cursor-pointer"
        >
          <RefreshCw className="w-4 h-4" />
        </button>
      </div>

      {/* Matches List */}
      {loading ? (
        <div className="text-xs text-[#a1a1aa] py-20 text-center flex items-center justify-center gap-2">
          <RefreshCw className="w-4 h-4 animate-spin text-[#84cc16]" /> Loading friendly matches...
        </div>
      ) : matches.length === 0 ? (
        <div className="bg-[#121216]/40 border border-dashed border-white/10 rounded-2xl p-12 text-center flex flex-col items-center">
          <Trophy className="w-12 h-12 text-[#71717a] mb-4" />
          <h3 className="text-sm font-bold text-white mb-1">No matches hosted</h3>
          <p className="text-xs text-[#a1a1aa] max-w-sm">
            There are no friendly matches scheduled on your courts. Player-created matches will display here.
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {matches.map(match => {
            const isCompleted = match.status === 'Completed' || match.isCompleted === true;
            const isCancelled = match.status === 'Cancelled' || match.isCancelled === true;
            const isFull = match.currentPlayersCount >= match.maxPlayersCount;

            return (
              <div key={match.id} className="bg-[#121216]/60 border border-white/5 rounded-2xl p-6 shadow-lg flex flex-col justify-between hover:border-white/10 transition-all gap-5">
                <div>
                  {/* Top: Match Title & Badge */}
                  <div className="flex items-start justify-between gap-4 mb-4">
                    <div>
                      <h3 className="text-xs font-bold text-white uppercase tracking-wider">{match.title || 'Friendly Match'}</h3>
                      <span className="text-[10px] text-[#71717a] font-bold">{match.courtName}</span>
                    </div>

                    <span className={`text-[9px] px-2 py-0.5 rounded font-bold uppercase tracking-wider ${
                      isCompleted 
                        ? 'bg-blue-500/10 text-blue-400 border border-blue-500/20'
                        : isCancelled 
                        ? 'bg-red-500/10 text-red-400 border border-red-500/20'
                        : isFull 
                        ? 'bg-orange-500/10 text-orange-400 border border-orange-500/20'
                        : 'bg-[#84cc16]/10 text-[#84cc16] border border-[#84cc16]/20'
                    }`}>
                      {isCompleted ? 'Completed' : isCancelled ? 'Cancelled' : isFull ? 'Full' : 'Joinable'}
                    </span>
                  </div>

                  {/* Pitch specifications */}
                  <div className="flex flex-col gap-2 text-xs text-[#a1a1aa]">
                    <div className="flex items-center gap-2">
                      <Calendar className="w-4 h-4 text-[#71717a] shrink-0" />
                      <span>{match.date}</span>
                    </div>

                    <div className="flex items-center gap-2">
                      <Clock className="w-4 h-4 text-[#71717a] shrink-0" />
                      <span>{match.startTime || '00:00'} - {match.endTime || '00:00'}</span>
                    </div>

                    <div className="flex items-center gap-2">
                      <Users className="w-4 h-4 text-[#71717a] shrink-0" />
                      <span>Capacity: {match.currentPlayersCount || 0} / {match.maxPlayersCount || 10} Players</span>
                    </div>

                    <div className="flex items-center gap-2">
                      <Shield className="w-4 h-4 text-[#71717a] shrink-0" />
                      <span>Sport: {match.sportType || 'Football'}</span>
                    </div>
                  </div>
                </div>

                {/* Organizer details */}
                <div className="border-t border-white/5 pt-4 flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className="w-7 h-7 rounded-full bg-white/10 flex items-center justify-center text-[10px] font-bold text-white shrink-0">
                      {match.creatorName ? match.creatorName.charAt(0) : 'O'}
                    </div>
                    <div>
                      <span className="text-[10px] text-[#71717a] block leading-none">Organizer</span>
                      <span className="text-xs text-white font-bold block mt-0.5">{match.creatorName || 'Member Player'}</span>
                    </div>
                  </div>

                  <div className="text-right">
                    <span className="text-[10px] text-[#71717a] block">Rate/Player</span>
                    <span className="text-xs text-white font-extrabold block">EGP {match.pricePerPlayer || 0}</span>
                  </div>
                </div>

              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
