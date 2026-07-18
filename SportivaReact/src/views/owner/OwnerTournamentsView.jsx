import React, { useState, useEffect } from 'react';
import { useOutletContext } from 'react-router-dom';
import { apiFetch } from '../../services/api';
import { Trophy, Calendar, Users, Shield, Clock, AlertCircle, RefreshCw, Plus } from 'lucide-react';

export default function OwnerTournamentsView({ addToast }) {
  const { selectedClub } = useOutletContext();
  const [tournaments, setTournaments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  const [newTournament, setNewTournament] = useState({
    name: '',
    description: '',
    sportType: 'Football',
    startDate: '',
    endDate: '',
    maxParticipants: 8
  });

  useEffect(() => {
    if (selectedClub) {
      loadClubTournaments();
    }
  }, [selectedClub]);

  const loadClubTournaments = async () => {
    setLoading(true);
    try {
      const res = await apiFetch('/tournaments/my');
      if (res.ok) {
        const data = await res.json();
        setTournaments(data.items || data || []);
      }
    } catch (e) {
      addToast('Failed to load tournaments.', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateTournament = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      const res = await apiFetch('/tournaments', {
        method: 'POST',
        body: JSON.stringify({
          name: newTournament.name,
          description: newTournament.description,
          sportType: newTournament.sportType,
          startDate: newTournament.startDate,
          endDate: newTournament.endDate,
          maxParticipants: parseInt(newTournament.maxParticipants)
        })
      });

      if (res.ok) {
        addToast('Tournament hosted successfully!', 'success');
        setIsCreateModalOpen(false);
        setNewTournament({
          name: '',
          description: '',
          sportType: 'Football',
          startDate: '',
          endDate: '',
          maxParticipants: 8
        });
        loadClubTournaments();
      } else {
        const err = await res.json();
        throw new Error(err.detail || 'Could not launch tournament.');
      }
    } catch (err) {
      addToast(err.message, 'error');
    } finally {
      setSubmitting(false);
    }
  };

  if (!selectedClub) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-6 bg-[#121216]/40 border border-white/5 rounded-2xl shadow-lg">
        <AlertCircle className="w-12 h-12 text-[#a1a1aa] mb-4" />
        <h3 className="text-lg font-bold text-white mb-2">No active venue selected</h3>
        <p className="text-xs text-[#a1a1aa] max-w-sm">Please register or select a club from the sidebar to monitor tournaments.</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-8 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight text-white">Tournament League Monitor</h2>
          <p className="text-[#a1a1aa] text-xs mt-1">Live tracking of ongoing and scheduled tournament events on your premises</p>
        </div>
        <div className="flex items-center gap-3">
          <button
            onClick={() => setIsCreateModalOpen(true)}
            className="flex items-center gap-1.5 px-5 py-2.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg cursor-pointer"
          >
            <Plus className="w-4 h-4" /> Host Tournament
          </button>
          <button
            onClick={loadClubTournaments}
            className="p-2.5 bg-white/5 hover:bg-white/10 text-white rounded-xl border border-white/5 transition-all cursor-pointer"
          >
            <RefreshCw className="w-4 h-4" />
          </button>
        </div>
      </div>

      {/* Tournaments Grid */}
      {loading ? (
        <div className="text-xs text-[#a1a1aa] py-20 text-center flex items-center justify-center gap-2">
          <RefreshCw className="w-4 h-4 animate-spin text-[#84cc16]" /> Loading tournaments...
        </div>
      ) : tournaments.length === 0 ? (
        <div className="bg-[#121216]/40 border border-dashed border-white/10 rounded-2xl p-12 text-center flex flex-col items-center">
          <Trophy className="w-12 h-12 text-[#71717a] mb-4" />
          <h3 className="text-sm font-bold text-white mb-1">No tournaments hosted</h3>
          <p className="text-xs text-[#a1a1aa] max-w-sm">
            There are no tournaments currently hosted at your sports center. League events will appear here.
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {tournaments.map(tournament => {
            const isFinished = tournament.status === 2 || tournament.status === 'Completed';
            const isLive = tournament.status === 1 || tournament.status === 'Live';

            return (
              <div key={tournament.id} className="bg-[#121216]/60 border border-white/5 rounded-2xl p-6 shadow-lg flex flex-col justify-between hover:border-white/10 transition-all gap-5">
                <div>
                  <div className="flex items-start justify-between gap-4 mb-4">
                    <div>
                      <h3 className="text-xs font-bold text-white uppercase tracking-wider">{tournament.name}</h3>
                      <span className="text-[10px] text-[#71717a] font-bold">{tournament.courtName || 'Complex pitch'}</span>
                    </div>

                    <span className={`text-[9px] px-2 py-0.5 rounded font-bold uppercase tracking-wider ${
                      isFinished 
                        ? 'bg-[#71717a]/15 text-[#71717a] border border-[#71717a]/20'
                        : isLive 
                        ? 'bg-[#84cc16]/10 text-[#84cc16] border border-[#84cc16]/20'
                        : 'bg-blue-500/10 text-blue-400 border border-blue-500/20'
                    }`}>
                      {isFinished ? 'Completed' : isLive ? 'Ongoing Live' : 'Open Registration'}
                    </span>
                  </div>

                  {/* Details */}
                  <div className="flex flex-col gap-2 text-xs text-[#a1a1aa]">
                    <div className="flex items-center gap-2">
                      <Calendar className="w-4 h-4 text-[#71717a] shrink-0" />
                      <span>Start: {tournament.startDate}</span>
                    </div>

                    <div className="flex items-center gap-2">
                      <Calendar className="w-4 h-4 text-[#71717a] shrink-0" />
                      <span>End: {tournament.endDate}</span>
                    </div>

                    <div className="flex items-center gap-2">
                      <Users className="w-4 h-4 text-[#71717a] shrink-0" />
                      <span>Teams: {tournament.registeredTeamsCount || 0} / {tournament.maxTeams || 8} Registered</span>
                    </div>

                    <div className="flex items-center gap-2">
                      <Shield className="w-4 h-4 text-[#71717a] shrink-0" />
                      <span>Sport Type: {tournament.sportType || 'Football'}</span>
                    </div>
                  </div>
                </div>

                {/* Bottom details */}
                <div className="border-t border-white/5 pt-4 flex items-center justify-between">
                  <div>
                    <span className="text-[10px] text-[#71717a] block">Registration Fee</span>
                    <span className="text-xs text-white font-extrabold block">EGP {tournament.registrationFee || 0}</span>
                  </div>

                  <div>
                    <span className="text-[10px] text-[#71717a] block text-right">Prize Pool</span>
                    <span className="text-xs text-[#84cc16] font-extrabold block text-right">EGP {tournament.prizePool || 0}</span>
                  </div>
                </div>

              </div>
            );
          })}
        </div>
      )}

      {/* Host Tournament Modal */}
      {isCreateModalOpen && (
        <div className="fixed inset-0 z-50 bg-[#000]/70 backdrop-blur-sm flex items-center justify-center p-4">
          <div className="bg-[#121216] border border-white/5 rounded-2xl w-full max-w-[500px] p-8 shadow-2xl relative animate-scale-up text-xs text-[#a1a1aa]">
            <button onClick={() => setIsCreateModalOpen(false)} className="absolute top-6 right-6 text-[#71717a] hover:text-white transition-colors">
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
            <h3 className="text-lg font-bold mb-6 text-white text-center">Host Tournament Championship</h3>
            
            <form onSubmit={handleCreateTournament} className="flex flex-col gap-4">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold uppercase tracking-wider text-[#71717a]">Championship Name</label>
                <input type="text" required value={newTournament.name} onChange={(e) => setNewTournament({...newTournament, name: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" placeholder="e.g. Summer Padel Tournament" />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] font-bold uppercase tracking-wider text-[#71717a]">Sport Type</label>
                  <select required value={newTournament.sportType} onChange={(e) => setNewTournament({...newTournament, sportType: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]">
                    <option value="Football">Football</option>
                    <option value="Padel">Padel</option>
                    <option value="Basketball">Basketball</option>
                    <option value="Tennis">Tennis</option>
                  </select>
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] font-bold uppercase tracking-wider text-[#71717a]">Max Squads/Participants</label>
                  <select required value={newTournament.maxParticipants} onChange={(e) => setNewTournament({...newTournament, maxParticipants: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]">
                    <option value="4">4 Teams</option>
                    <option value="8">8 Teams</option>
                    <option value="16">16 Teams</option>
                    <option value="32">32 Teams</option>
                  </select>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] font-bold uppercase tracking-wider text-[#71717a]">Start Date</label>
                  <input type="date" required value={newTournament.startDate} onChange={(e) => setNewTournament({...newTournament, startDate: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] font-bold uppercase tracking-wider text-[#71717a]">End Date</label>
                  <input type="date" required value={newTournament.endDate} onChange={(e) => setNewTournament({...newTournament, endDate: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold uppercase tracking-wider text-[#71717a]">Championship Rules & Description</label>
                <textarea required value={newTournament.description} onChange={(e) => setNewTournament({...newTournament, description: e.target.value})} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl p-4 text-xs text-white outline-none resize-none h-[100px] focus:border-[#84cc16]" placeholder="Specify knockout mechanics, prizes, rules..." />
              </div>

              <button type="submit" disabled={submitting} className="w-full py-4 rounded-xl bg-[#84cc16] text-black font-bold hover:bg-[#65a30d] disabled:opacity-50 transition-colors shadow-lg mt-2">
                {submitting ? 'Launching Tournament...' : 'Host Tournament'}
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
