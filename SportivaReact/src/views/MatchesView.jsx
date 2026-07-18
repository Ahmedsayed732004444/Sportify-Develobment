import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Trophy, Plus, MapPin, X, Users, Check, Ban, Clock, Award, Star } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function MatchesView({ user, addToast }) {
  const navigate = useNavigate();
  const [matches, setMatches] = useState([]);
  const [showMatchModal, setShowMatchModal] = useState(false);
  
  // Dynamic clubs & courts loading for location lock
  const [clubsList, setClubsList] = useState([]);
  const [courtsList, setCourtsList] = useState([]);
  const [selectedClubId, setSelectedClubId] = useState('');

  const [newMatch, setNewMatch] = useState({
    courtId: '',
    date: '',
    startTime: '16:00',
    endTime: '17:00',
    sportType: 'Football',
    requiredPlayers: 10,
    note: ''
  });

  // Organizer Roster requests state
  const [selectedMatchForRequests, setSelectedMatchForRequests] = useState(null);
  const [joinRequests, setJoinRequests] = useState([]);

  useEffect(() => {
    loadFriendlyMatches();
  }, []);

  const loadFriendlyMatches = async () => {
    try {
      const res = await apiFetch('/friendly-matches');
      if (!res.ok) throw new Error();
      const data = await res.json();
      setMatches(data.items || []);
    } catch (e) {
      addToast('Failed to retrieve friendly matches list.', 'error');
    }
  };

  const handleOpenModal = async () => {
    setShowMatchModal(true);
    try {
      const res = await apiFetch('/clubs');
      if (res.ok) {
        const data = await res.json();
        setClubsList(data.items || []);
      }
    } catch (e) {}
  };

  const handleClubChange = async (clubId) => {
    setSelectedClubId(clubId);
    setNewMatch(prev => ({ ...prev, courtId: '' }));
    setCourtsList([]);
    if (!clubId) return;

    try {
      const res = await apiFetch(`/clubs/${clubId}/courts`);
      if (res.ok) {
        const data = await res.json();
        setCourtsList(data || []);
      }
    } catch (e) {}
  };

  const requestJoinMatch = async (matchId, e) => {
    e.stopPropagation(); // prevent card routing click
    if (!user) {
      addToast('Please login to join friendly matches.', 'info');
      return;
    }
    try {
      const res = await apiFetch(`/friendly-matches/${matchId}/join-requests`, {
        method: 'POST',
        body: JSON.stringify({ notes: 'Requesting roster join' })
      });
      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Failed to submit application.');
      }
      addToast('Roster request submitted!', 'success');
      loadFriendlyMatches();
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const createFriendlyMatchSubmit = async (e) => {
    e.preventDefault();
    if (!newMatch.courtId) {
      addToast('Location court is required. You cannot create a match without selecting a location.', 'error');
      return;
    }

    const formattedStartTime = newMatch.startTime.length === 5 ? `${newMatch.startTime}:00` : newMatch.startTime;
    const formattedEndTime = newMatch.endTime.length === 5 ? `${newMatch.endTime}:00` : newMatch.endTime;

    const payload = {
      courtId: newMatch.courtId,
      date: newMatch.date,
      startTime: formattedStartTime,
      endTime: formattedEndTime,
      sportType: newMatch.sportType,
      requiredPlayers: parseInt(newMatch.requiredPlayers),
      note: newMatch.note
    };

    try {
      const res = await apiFetch('/friendly-matches', {
        method: 'POST',
        body: JSON.stringify(payload)
      });
      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Could not launch match.');
      }
      addToast('Match successfully created!', 'success');
      setShowMatchModal(false);
      setNewMatch({
        courtId: '',
        date: '',
        startTime: '16:00',
        endTime: '17:00',
        sportType: 'Football',
        requiredPlayers: 10,
        note: ''
      });
      setSelectedClubId('');
      setCourtsList([]);
      loadFriendlyMatches();
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const viewJoinRequests = async (match, e) => {
    e.stopPropagation(); // prevent card routing click
    setSelectedMatchForRequests(match);
    try {
      const res = await apiFetch(`/friendly-matches/${match.matchId}/join-requests`);
      if (res.ok) {
        const data = await res.json();
        setJoinRequests(data.items || []);
      }
    } catch (e) {
      addToast('Failed to load roster requests.', 'error');
    }
  };

  const handleRequestDecision = async (requestId, status) => {
    if (!selectedMatchForRequests) return;
    try {
      const res = await apiFetch(`/friendly-matches/${selectedMatchForRequests.matchId}/join-requests/${requestId}/${status}`, {
        method: 'PUT'
      });
      if (res.ok) {
        addToast(`Request ${status}ed successfully!`, 'success');
        viewJoinRequests(selectedMatchForRequests);
        loadFriendlyMatches();
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Could not register request decision.', 'error');
    }
  };

  return (
    <section className="animate-fade-in flex flex-col gap-8 text-xs text-[#a1a1aa]">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-3xl font-extrabold text-white">Friendly Matches Hub</h2>
          <p className="text-[#a1a1aa] text-sm mt-1">Browse active rosters, search players, and join nearby matches</p>
        </div>
        {user?.role === 'Player' && (
          <button onClick={handleOpenModal} className="flex items-center gap-2 px-5 py-3 rounded-xl bg-[#84cc16] text-black text-sm font-extrabold hover:bg-[#65a30d] transition-colors shadow-lg shadow-[#84cc16]/10">
            <Plus className="w-4.5 h-4.5" /> Host Match
          </button>
        )}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        {/* Left main matches list */}
        <div className="lg:col-span-2 grid grid-cols-1 md:grid-cols-2 gap-6 h-fit">
          {matches.length === 0 ? (
            <p className="text-xs text-[#71717a] py-6 text-center col-span-2 bg-[#121216]/20 border border-white/5 rounded-2xl">No friendly matches scheduled yet. Host a new match lobby!</p>
          ) : (
            matches.map(m => {
              const isMine = user && m.organizer?.id === user.id;
              return (
                <div key={m.matchId} onClick={() => navigate(`/friendly-match/${m.matchId}`)} className="bg-[#121216]/50 border border-white/5 rounded-2xl p-6 flex flex-col justify-between gap-5 hover:border-[#84cc16]/30 transition-all duration-300 cursor-pointer group relative">
                  <div className="flex flex-col gap-2">
                    <div className="flex justify-between items-center">
                      <span className="px-2.5 py-0.5 bg-[#84cc16]/15 border border-[#84cc16]/25 text-[#a3e635] rounded-md text-[10px] font-bold uppercase tracking-wider">{m.sportType}</span>
                      <span className="text-[10px] px-2 py-0.5 rounded bg-white/5 text-[#a1a1aa] uppercase font-bold tracking-wider">{m.status}</span>
                    </div>
                    
                    <h3 className="font-extrabold text-white text-base mt-2">Hosted by {m.organizer?.name || 'Player'}</h3>
                    
                    <div className="flex flex-col gap-1.5 mt-1 text-xs text-[#a1a1aa]">
                      <span className="flex items-center gap-1.5"><MapPin className="w-3.5 h-3.5 text-[#84cc16]" /> {m.court?.clubName || 'Unknown Club'} - {m.court?.name || 'Venue'}</span>
                      <span className="flex items-center gap-1.5"><Clock className="w-3.5 h-3.5 text-[#84cc16]" /> {m.date} at {m.startTime.substring(0, 5)} - {m.endTime.substring(0, 5)}</span>
                    </div>

                    {m.note && (
                      <p className="text-[11px] text-[#71717a] bg-[#1e1e26]/30 p-2 rounded mt-1 italic">Note: {m.note}</p>
                    )}
                  </div>

                  <div className="flex flex-col gap-3 border-t border-white/5 pt-4">
                    <div className="flex justify-between text-xs font-semibold">
                      <span className="text-[#a1a1aa]">Roster Slots</span>
                      <span className="text-[#84cc16]">{m.acceptedPlayersCount} / {m.requiredPlayers} Players</span>
                    </div>

                    {isMine ? (
                      <button onClick={(e) => viewJoinRequests(m, e)} className="w-full py-3 rounded-xl bg-white/5 hover:bg-white/10 text-white text-xs font-bold transition-colors flex items-center justify-center gap-2 border border-white/10">
                        <Users className="w-4 h-4" /> Manage Requests
                      </button>
                    ) : (
                      user?.role === 'Player' ? (
                        <button onClick={(e) => requestJoinMatch(m.matchId, e)} className="w-full py-3 rounded-xl bg-[#84cc16] hover:bg-[#65a30d] text-black text-xs font-bold transition-colors">
                          {m.iapplied ? 'Requested (Pending)' : m.iparticipating ? 'Joined (Roster Player)' : 'Request to Join'}
                        </button>
                      ) : (
                        <span className="text-xs font-bold text-center block text-[#71717a] bg-white/5 py-3 border border-white/10 rounded-xl">Only players can join</span>
                      )
                    )}
                  </div>
                </div>
              );
            })
          )}
        </div>

        {/* Right side details / roster requests panel */}
        <div className="lg:col-span-1">
          {selectedMatchForRequests ? (
            <div className="bg-[#121216]/50 border border-white/5 rounded-2xl p-6 flex flex-col gap-6">
              <div className="flex justify-between items-center border-b border-white/5 pb-3">
                <h3 className="font-extrabold text-white text-base">Match Roster Request</h3>
                <button onClick={() => setSelectedMatchForRequests(null)} className="text-[#71717a] hover:text-white transition-colors"><X className="w-4.5 h-4.5" /></button>
              </div>
              <div className="flex flex-col gap-4">
                {joinRequests.length > 0 ? joinRequests.map(req => (
                  <div key={req.id} className="flex justify-between items-center p-4 bg-[#1e1e26]/30 border border-white/5 rounded-xl text-xs text-[#a1a1aa]">
                    <div>
                      <h4 className="font-bold text-white text-xs">{req.playerName || 'Player'}</h4>
                      <span className="text-[10px] text-[#71717a] block mt-0.5">Status: {req.status}</span>
                    </div>
                    {req.status === 'Pending' && (
                      <div className="flex gap-2">
                        <button onClick={() => handleRequestDecision(req.id, 'accept')} className="w-8 h-8 rounded-lg bg-[#10b981]/15 hover:bg-[#10b981]/30 text-[#10b981] flex items-center justify-center transition-all"><Check className="w-4 h-4" /></button>
                        <button onClick={() => handleRequestDecision(req.id, 'reject')} className="w-8 h-8 rounded-lg bg-[#ef4444]/15 hover:bg-[#ef4444]/30 text-[#ef4444] flex items-center justify-center transition-all"><X className="w-4 h-4" /></button>
                      </div>
                    )}
                  </div>
                )) : (
                  <p className="text-xs text-[#71717a] py-4 text-center">No active roster applications.</p>
                )}
              </div>
            </div>
          ) : (
            <div className="bg-[#121216]/50 border border-white/5 rounded-2xl p-8 text-center text-[#71717a] py-12 flex flex-col items-center justify-center gap-3">
              <Users className="w-10 h-10 text-[#84cc16]/55 animate-pulse" />
              <p className="text-xs max-w-[220px] leading-relaxed">Select "Manage Requests" on one of your hosted friendly matches to approve or reject player applications.</p>
            </div>
          )}
        </div>
      </div>

      {/* Host Match Modal Popup with enforced location selectors */}
      {showMatchModal && (
        <div className="fixed inset-0 z-50 bg-[#000]/70 backdrop-blur-sm flex items-center justify-center p-4">
          <div className="bg-[#121216] border border-white/5 rounded-2xl w-full max-w-[500px] p-8 shadow-2xl relative animate-scale-up">
            <button onClick={() => setShowMatchModal(false)} className="absolute top-6 right-6 text-[#71717a] hover:text-white transition-colors">
              <X className="w-5 h-5" />
            </button>
            <h3 className="text-xl font-bold mb-6 text-white">Host Friendly Match</h3>
            
            <form onSubmit={createFriendlyMatchSubmit} className="flex flex-col gap-4">
              
              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Club Complex</label>
                  <select required value={selectedClubId} onChange={(e) => handleClubChange(e.target.value)} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]">
                    <option value="">-- Choose Club --</option>
                    {clubsList.map(c => (
                      <option key={c.id} value={c.id}>{c.name}</option>
                    ))}
                  </select>
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Select Court</label>
                  <select required value={newMatch.courtId} onChange={(e) => setNewMatch({...newMatch, courtId: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" disabled={!selectedClubId}>
                    <option value="">-- Choose Court --</option>
                    {courtsList.map(crt => (
                      <option key={crt.id} value={crt.id}>{crt.name} ({crt.sportType})</option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Match Date</label>
                  <input type="date" required value={newMatch.date} onChange={(e) => setNewMatch({...newMatch, date: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Required Players</label>
                  <input type="number" required min="2" max="22" value={newMatch.requiredPlayers} onChange={(e) => setNewMatch({...newMatch, requiredPlayers: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Start Time</label>
                  <input type="time" required value={newMatch.startTime} onChange={(e) => setNewMatch({...newMatch, startTime: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">End Time</label>
                  <input type="time" required value={newMatch.endTime} onChange={(e) => setNewMatch({...newMatch, endTime: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Organizer Notes</label>
                <textarea value={newMatch.note} onChange={(e) => setNewMatch({...newMatch, note: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl p-4 text-xs text-white outline-none resize-none h-[80px]" placeholder="Add game instructions..." />
              </div>

              <button type="submit" className="w-full py-4 rounded-xl bg-[#84cc16] text-black font-extrabold text-xs hover:bg-[#65a30d] transition-colors shadow-lg mt-3">Host Match</button>
            </form>
          </div>
        </div>
      )}

    </section>
  );
}
