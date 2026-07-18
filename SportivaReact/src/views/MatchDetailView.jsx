import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Trophy, Clock, MapPin, Users, User, Star, Check, Ban, X, Send } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function MatchDetailView({ user, addToast, onTriggerAuth }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const [match, setMatch] = useState(null);
  const [joinRequests, setJoinRequests] = useState([]);
  const [isLoading, setIsLoading] = useState(true);



  useEffect(() => {
    loadMatchDetails();
  }, [id]);

  const loadMatchDetails = async () => {
    try {
      setIsLoading(true);
      const res = await apiFetch(`/friendly-matches/${id}`);
      if (res.ok) {
        const data = await res.json();
        setMatch(data);
        if (user && data.organizer?.id === user.id) {
          loadJoinRequests(data.matchId);
        }
      }
    } catch (e) {
      addToast('Failed to load friendly match lobby details.', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const loadJoinRequests = async (matchId) => {
    try {
      const res = await apiFetch(`/friendly-matches/${matchId}/join-requests`);
      if (res.ok) {
        const data = await res.json();
        setJoinRequests(data.items || []);
      }
    } catch (e) {}
  };

  const handleJoinRequest = async () => {
    if (!user) {
      addToast('Please login to request joining a friendly match.', 'info');
      if (onTriggerAuth) {
        onTriggerAuth(`/friendly-match/${id}`);
      }
      return;
    }
    try {
      const res = await apiFetch(`/friendly-matches/${id}/join-requests`, {
        method: 'POST',
        body: JSON.stringify({ notes: 'Requesting roster join' })
      });
      if (res.ok) {
        addToast('Join application submitted to organizer!', 'success');
        loadMatchDetails();
      } else {
        const err = await res.json();
        throw new Error(err.detail || 'Could not join');
      }
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const handleRequestDecision = async (requestId, decision) => {
    try {
      const res = await apiFetch(`/friendly-matches/${match.matchId}/join-requests/${requestId}/${decision}`, {
        method: 'PUT'
      });
      if (res.ok) {
        addToast(`Player application ${decision}ed successfully!`, 'success');
        loadJoinRequests(match.matchId);
        loadMatchDetails();
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to register join decision.', 'error');
    }
  };

  if (isLoading) {
    return (
      <div className="max-w-[800px] mx-auto flex flex-col gap-6 py-20 text-[#71717a] animate-pulse">
        <div className="h-28 bg-[#1e1e26]/30 rounded-2xl"></div>
        <div className="h-6 w-36 bg-[#1e1e26]/30 rounded"></div>
      </div>
    );
  }

  if (!match) {
    return (
      <div className="text-center py-20 text-[#71717a]">
        <h3 className="font-bold text-white text-lg">Match Roster Not Found</h3>
        <p className="text-xs mt-1">This friendly match lobby has been closed or removed.</p>
      </div>
    );
  }

  const isMine = user && match.organizer?.id === user.id;

  return (
    <div className="max-w-[850px] mx-auto flex flex-col lg:flex-row gap-8 pb-20 animate-fade-in text-xs text-[#a1a1aa]">
      
      {/* LEFT COLUMN: Match Details & Participants */}
      <div className="flex-1 flex flex-col gap-6">
        <div>
          <span className="px-2.5 py-0.5 bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#a3e635] rounded font-bold text-[10px] uppercase tracking-wider">{match.sportType} Match</span>
          <h2 className="text-2xl font-extrabold text-white mt-2">Roster Lobby Detail</h2>
          <p className="text-xs text-[#a1a1aa] mt-1 flex items-center gap-1"><MapPin className="w-3.5 h-3.5" /> Organized for standalone players</p>
        </div>

        <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4">
          <h3 className="font-bold text-white text-sm">Venue Information</h3>
          <div className="flex flex-col gap-2">
            <span className="text-white font-bold text-base flex items-center gap-1.5"><MapPin className="w-4 h-4 text-[#84cc16]" /> {match.court?.clubName || 'Local Complex'}</span>
            <span className="text-[#a1a1aa] font-semibold">{match.court?.name || 'Main Court'} ({match.court?.sportType})</span>
            <span className="text-xs text-[#71717a] mt-1 flex items-center gap-1.5"><Clock className="w-4 h-4" /> {match.date} at {match.startTime.substring(0, 5)} - {match.endTime.substring(0, 5)}</span>
          </div>
        </div>

        {/* Accepted roster list */}
        <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4">
          <h3 className="font-bold text-white text-sm">Accepted Players ({match.acceptedPlayersCount} / {match.requiredPlayers})</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {match.participantsPreview?.length === 0 ? (
              <p className="text-xs text-[#71717a] col-span-2">No players accepted on roster yet.</p>
            ) : (
              match.participantsPreview?.map((p, idx) => (
                <div key={idx} className="bg-[#1e1e26]/30 border border-white/5 p-3.5 rounded-xl flex items-center gap-3">
                  <div className="w-8 h-8 rounded-full bg-[#84cc16]/10 flex items-center justify-center text-[#84cc16] font-bold text-xs">P</div>
                  <div>
                    <h4 className="font-bold text-white">{p.playerName || 'Player'}</h4>
                    <span className="text-[10px] text-[#71717a]">Roster Slot Verified</span>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        {/* Rating feedback prompt simulator */}
        {match.status === 'Finished' && (
          <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4">
            <h3 className="font-bold text-white text-sm">Rate Your Teammates</h3>
            <p className="text-xs text-[#71717a]">Match completed! Click to rate teammates on sportsmanship and punctuality.</p>
            <button onClick={() => addToast('Reputation ratings submitted to players!', 'success')} className="w-fit px-4 py-2 bg-[#84cc16] hover:bg-[#65a30d] text-black font-bold text-xs rounded-xl transition-all">Submit Feedback</button>
          </div>
        )}
      </div>

      {/* RIGHT COLUMN: Organizer Reputation and Applications */}
      <div className="w-full lg:w-[320px] flex flex-col gap-6 shrink-0">
        
        {/* Organizer reputation summary card */}
        <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4 shadow-xl">
          <h3 className="font-extrabold text-white text-sm flex items-center gap-1.5"><User className="w-4 h-4 text-[#84cc16]" /> Organizer Profile</h3>
          
          <div className="flex items-center gap-3 bg-[#1e1e26]/30 border border-white/5 p-3 rounded-xl">
            {match.organizer?.profilePictureUrl ? (
              <img className="w-10 h-10 rounded-full border border-white/10 object-cover" src={match.organizer.profilePictureUrl} alt="" />
            ) : (
              <div className="w-10 h-10 rounded-full bg-[#84cc16]/10 flex items-center justify-center font-bold text-[#84cc16] text-sm shrink-0">
                {match.organizer?.name ? match.organizer.name.charAt(0) : 'P'}
              </div>
            )}
            <div>
              <h4 className="font-bold text-white text-xs">{match.organizer?.name || 'Player'}</h4>
              <span className="text-[9px] text-[#84cc16] font-bold uppercase tracking-wider">Host Captain</span>
            </div>
          </div>

          <div className="flex flex-col gap-2 border-t border-white/5 pt-3">
            <span className="text-[10px] uppercase font-bold text-[#71717a]">Host Reputation Score</span>
            <div className="text-[10px] text-[#71717a] mt-1 p-3 bg-[#1e1e26]/10 rounded-xl border border-white/5 leading-relaxed">
              Reputation metrics will populate once this user hosts more completed matches.
            </div>
          </div>

          {/* Action Join CTA */}
          <div className="border-t border-white/5 pt-4 mt-2">
            {isMine ? (
              <span className="text-xs font-bold text-center block text-[#84cc16] bg-[#84cc16]/10 p-3.5 border border-[#84cc16]/20 rounded-xl">You are hosting this match</span>
            ) : (
              <button onClick={handleJoinRequest} className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg">
                {match.iapplied ? 'Join Request Pending' : match.iparticipating ? 'Already on Roster' : 'Request to Join Roster'}
              </button>
            )}
          </div>
        </div>

        {/* Requests Queue if host */}
        {isMine && (
          <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4 shadow-xl">
            <h3 className="font-extrabold text-white text-sm">Join Requests Queue</h3>
            <div className="flex flex-col gap-3">
              {joinRequests.length === 0 ? (
                <p className="text-xs text-[#71717a] text-center py-4 border border-dashed border-white/5 rounded-xl">No pending requests.</p>
              ) : (
                joinRequests.map(req => (
                  <div key={req.id} className="bg-[#1e1e26]/30 border border-white/5 p-3 rounded-xl flex justify-between items-center">
                    <div>
                      <h4 className="font-bold text-white text-xs">{req.playerName}</h4>
                      <span className="text-[9px] text-[#71717a] block mt-0.5">Applicant</span>
                    </div>
                    {req.status === 'Pending' && (
                      <div className="flex gap-1.5 shrink-0">
                        <button onClick={() => handleRequestDecision(req.id, 'accept')} className="w-7 h-7 rounded bg-[#10b981]/10 text-[#10b981] flex items-center justify-center hover:bg-[#10b981]/20"><Check className="w-3.5 h-3.5" /></button>
                        <button onClick={() => handleRequestDecision(req.id, 'reject')} className="w-7 h-7 rounded bg-red-500/10 text-red-400 flex items-center justify-center hover:bg-red-500/20"><X className="w-3.5 h-3.5" /></button>
                      </div>
                    )}
                  </div>
                ))
              )}
            </div>
          </div>
        )}

      </div>

    </div>
  );
}
