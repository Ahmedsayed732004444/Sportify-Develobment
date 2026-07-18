import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Trophy, Plus, MapPin, X, Users, Calendar, Clock, Award, Shield } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function TournamentsView({ addToast }) {
  const navigate = useNavigate();
  const [tournaments, setTournaments] = useState([]);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [selectedTournament, setSelectedTournament] = useState(null); // for registering team
  const [teamName, setTeamName] = useState('');

  // Location selector lists
  const [clubsList, setClubsList] = useState([]);
  const [courtsList, setCourtsList] = useState([]);
  const [selectedClubId, setSelectedClubId] = useState('');

  // Form states for creation
  const [newTournament, setNewTournament] = useState({
    name: '',
    description: '',
    sportType: 'Football',
    startDate: '',
    endDate: '',
    maxParticipants: 8,
    type: 'Knockout',
    courtId: '',
    deadline: '',
    rules: '',
    imageUrl: ''
  });

  // Check role to expose creation button
  const [currentUserRole, setCurrentUserRole] = useState('Player');

  useEffect(() => {
    loadTournaments();
    loadRole();
  }, []);

  const loadRole = () => {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        let role = payload.role || 'Player';
        if (payload.roles) {
          try {
            const rolesArray = typeof payload.roles === 'string' ? JSON.parse(payload.roles) : payload.roles;
            if (Array.isArray(rolesArray) && rolesArray.length > 0) role = rolesArray[0];
          } catch (e) {}
        }
        if (role === 'Member') role = 'Player';
        setCurrentUserRole(role);
      } catch (e) {}
    }
  };

  const loadTournaments = async () => {
    try {
      const res = await apiFetch('/tournaments');
      if (!res.ok) throw new Error();
      const data = await res.json();
      setTournaments(data.items || []);
    } catch (e) {
      addToast('Failed to load tournaments.', 'error');
    }
  };

  const handleOpenCreateModal = async () => {
    setShowCreateModal(true);
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
    setNewTournament(prev => ({ ...prev, courtId: '' }));
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

  const handleCreateSubmit = async (e) => {
    e.preventDefault();
    if (!newTournament.courtId) {
      addToast('Court location selection is required.', 'error');
      return;
    }

    const selectedClub = clubsList.find(c => c.id === selectedClubId);
    const selectedCourt = courtsList.find(c => c.id === newTournament.courtId);

    // Pack extra parameters inside the description field
    const packedDescription = JSON.stringify({
      description: newTournament.description,
      type: newTournament.type,
      clubName: selectedClub?.name || 'Local Complex',
      courtName: selectedCourt?.name || 'Court Slot',
      deadline: newTournament.deadline,
      rules: newTournament.rules,
      imageUrl: newTournament.imageUrl
    });

    const payload = {
      name: newTournament.name,
      description: packedDescription,
      sportType: newTournament.sportType,
      startDate: newTournament.startDate,
      endDate: newTournament.endDate,
      maxParticipants: parseInt(newTournament.maxParticipants),
      courtId: newTournament.courtId
    };

    try {
      const res = await apiFetch('/tournaments', {
        method: 'POST',
        body: JSON.stringify(payload)
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Could not launch tournament.');
      }

      addToast('Tournament created successfully!', 'success');
      setShowCreateModal(false);
      // Reset form
      setNewTournament({
        name: '',
        description: '',
        sportType: 'Football',
        startDate: '',
        endDate: '',
        maxParticipants: 8,
        type: 'Knockout',
        courtId: '',
        deadline: '',
        rules: '',
        imageUrl: ''
      });
      setSelectedClubId('');
      setCourtsList([]);
      loadTournaments();
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const handleRegisterTeamSubmit = async (e) => {
    e.preventDefault();
    if (!teamName.trim() || !selectedTournament) return;

    try {
      const res = await apiFetch(`/tournaments/${selectedTournament.tournamentId}/register`, {
        method: 'POST',
        body: JSON.stringify({ teamName: teamName.trim() })
      });

      if (res.ok) {
        addToast('Team registered successfully to tournament!', 'success');
        setSelectedTournament(null);
        setTeamName('');
        loadTournaments();
      } else {
        const err = await res.json();
        throw new Error(err.detail || 'Could not complete team registration.');
      }
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const parseDescription = (desc) => {
    try {
      return JSON.parse(desc);
    } catch (e) {
      return { description: desc };
    }
  };

  const openRegisterModal = (t, e) => {
    e.stopPropagation(); // prevent card click
    setSelectedTournament(t);
  };

  return (
    <section className="animate-fade-in flex flex-col gap-8 text-xs text-[#a1a1aa]">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-3xl font-extrabold text-white">Tournaments Championship</h2>
          <p className="text-[#a1a1aa] text-sm mt-1 font-medium">Discover upcoming competitive events and leagues</p>
        </div>
        {(currentUserRole === 'Admin' || currentUserRole === 'Owner') && (
          <button onClick={handleOpenCreateModal} className="flex items-center gap-2 px-5 py-3 rounded-xl bg-[#84cc16] text-black text-sm font-extrabold hover:bg-[#65a30d] transition-colors shadow-lg">
            <Plus className="w-4.5 h-4.5" /> Host Tournament
          </button>
        )}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
        {tournaments.length === 0 ? (
          <p className="text-xs text-[#71717a] py-6 text-center col-span-3 bg-[#121216]/20 border border-white/5 rounded-2xl">No tournaments hosted yet.</p>
        ) : (
          tournaments.map(t => {
            const meta = parseDescription(t.description);
            return (
              <div key={t.tournamentId} onClick={() => navigate(`/tournament/${t.tournamentId}`)} className="bg-[#121216]/50 border border-white/5 rounded-3xl overflow-hidden hover:border-[#84cc16]/30 transition-all duration-300 flex flex-col justify-between cursor-pointer group relative">
                
                {/* Banner Image */}
                <div className="h-48 w-full bg-cover bg-center relative" style={{ backgroundImage: `url(${meta.imageUrl || 'https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=500&q=80'})` }}>
                  <span className="absolute top-4 right-4 text-xs px-2.5 py-1 rounded-md bg-[#84cc16]/90 text-black font-extrabold uppercase tracking-wider">{t.status}</span>
                </div>

                <div className="p-6 flex flex-col gap-4 flex-1">
                  <div>
                    <h3 className="font-extrabold text-white text-lg leading-tight">{t.name}</h3>
                    <p className="text-xs text-[#84cc16] font-semibold mt-1">Type: {meta.type || 'Knockout'} Bracket</p>
                  </div>

                  <p className="text-xs text-[#a1a1aa] leading-relaxed">{meta.description || 'Championship Tournament'}</p>

                  <div className="grid grid-cols-2 gap-3 bg-[#1e1e26]/30 border border-white/5 p-4 rounded-2xl text-[11px] text-[#a1a1aa]">
                    <span className="flex items-center gap-1.5"><MapPin className="w-3.5 h-3.5 text-[#84cc16]" /> {meta.clubName || 'Complex'} - {meta.courtName || 'Court'}</span>
                    <span className="flex items-center gap-1.5"><Calendar className="w-3.5 h-3.5 text-[#84cc16]" /> Start: {t.startDate}</span>
                    <span className="flex items-center gap-1.5"><Users className="w-3.5 h-3.5 text-[#84cc16]" /> Teams: {t.participantsCount} / {t.maxParticipants}</span>
                    {meta.deadline && (
                      <span className="flex items-center gap-1.5 text-rose-400 font-semibold"><Clock className="w-3.5 h-3.5" /> Deadline: {meta.deadline}</span>
                    )}
                  </div>

                  <div className="flex justify-between items-center text-xs text-[#71717a] mt-1 pt-3 border-t border-white/5">
                    <span>Organizer: <b className="text-white">{t.organizer?.name || 'Admin'}</b></span>
                  </div>

                  {t.canJoin && (
                    <button onClick={(e) => openRegisterModal(t, e)} className="w-full mt-2 py-3 rounded-xl bg-[#84cc16] hover:bg-[#65a30d] text-black text-xs font-bold transition-colors">
                      Register Team
                    </button>
                  )}
                </div>
              </div>
            );
          })
        )}
      </div>

      {/* Host Tournament Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 z-50 bg-[#000]/70 backdrop-blur-sm flex items-center justify-center p-4 overflow-y-auto">
          <div className="bg-[#121216] border border-white/5 rounded-3xl w-full max-w-[550px] p-8 shadow-2xl relative my-8 animate-scale-up">
            <button onClick={() => setShowCreateModal(false)} className="absolute top-6 right-6 text-[#71717a] hover:text-white transition-colors">
              <X className="w-5 h-5" />
            </button>
            <h3 className="text-xl font-bold mb-6 text-white">Host Tournament</h3>
            
            <form onSubmit={handleCreateSubmit} className="flex flex-col gap-4">
              
              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Tournament Name</label>
                  <input type="text" required value={newTournament.name} onChange={(e) => setNewTournament({...newTournament, name: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" placeholder="e.g. Cairo Summer Cup" />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Tournament Type</label>
                  <select value={newTournament.type} onChange={(e) => setNewTournament({...newTournament, type: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]">
                    <option value="Knockout">Knockout</option>
                    <option value="League">League</option>
                    <option value="Groups + Playoff">Groups + Playoff</option>
                  </select>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Club Complex</label>
                  <select required value={selectedClubId} onChange={(e) => handleClubChange(e.target.value)} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]">
                    <option value="">-- Choose Club --</option>
                    {clubsList.map(c => (
                      <option key={c.id} value={c.id}>{c.name}</option>
                    ))}
                  </select>
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Court Selection</label>
                  <select required value={newTournament.courtId} onChange={(e) => setNewTournament({...newTournament, courtId: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" disabled={!selectedClubId}>
                    <option value="">-- Choose Court --</option>
                    {courtsList.map(crt => (
                      <option key={crt.id} value={crt.id}>{crt.name} ({crt.sportType})</option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Sport Type</label>
                  <select value={newTournament.sportType} onChange={(e) => setNewTournament({...newTournament, sportType: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]">
                    <option value="Football">Football</option>
                    <option value="Padel">Padel</option>
                    <option value="Tennis">Tennis</option>
                    <option value="Basketball">Basketball</option>
                  </select>
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Max Participants (Teams)</label>
                  <input type="number" required min="4" max="64" value={newTournament.maxParticipants} onChange={(e) => setNewTournament({...newTournament, maxParticipants: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Start Date</label>
                  <input type="date" required value={newTournament.startDate} onChange={(e) => setNewTournament({...newTournament, startDate: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">End Date</label>
                  <input type="date" required value={newTournament.endDate} onChange={(e) => setNewTournament({...newTournament, endDate: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Reg Deadline</label>
                  <input type="date" required value={newTournament.deadline} onChange={(e) => setNewTournament({...newTournament, deadline: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Cover Image URL</label>
                  <input type="url" value={newTournament.imageUrl} onChange={(e) => setNewTournament({...newTournament, imageUrl: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" placeholder="https://example.com/image.jpg" />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Tournament Rules</label>
                <textarea value={newTournament.rules} onChange={(e) => setNewTournament({...newTournament, rules: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl p-4 text-xs text-white outline-none resize-none h-[80px]" placeholder="Rules guidelines..." />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Description</label>
                <textarea value={newTournament.description} onChange={(e) => setNewTournament({...newTournament, description: e.target.value})} className="bg-[#1e1e26]/50 border border-white/5 rounded-xl p-4 text-xs text-white outline-none resize-none h-[60px]" placeholder="Brief context..." />
              </div>

              <button type="submit" className="w-full py-4 rounded-xl bg-[#84cc16] text-black font-extrabold text-xs hover:bg-[#65a30d] transition-colors shadow-lg mt-2">Host Tournament</button>
            </form>
          </div>
        </div>
      )}

      {/* Register Team Modal */}
      {selectedTournament && (
        <div className="fixed inset-0 z-50 bg-[#000]/60 backdrop-blur-sm flex items-center justify-center p-4">
          <div className="bg-[#121216] border border-white/5 rounded-2xl w-full max-w-[450px] p-8 shadow-2xl relative animate-scale-up">
            <button onClick={() => setSelectedTournament(null)} className="absolute top-6 right-6 text-[#71717a] hover:text-white transition-colors">
              <X className="w-5 h-5" />
            </button>
            <h3 className="text-lg font-bold mb-1 text-white">Register Team</h3>
            <p className="text-xs text-[#a1a1aa] mb-6">Enter team details for {selectedTournament.name}</p>

            <form onSubmit={handleRegisterTeamSubmit} className="flex flex-col gap-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase tracking-wider">Team Name</label>
                <input type="text" required value={teamName} onChange={(e) => setTeamName(e.target.value)} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-5 py-3.5 text-sm outline-none focus:border-[#84cc16] text-white" placeholder="e.g. Cairo United" />
              </div>

              <button type="submit" className="w-full py-4 rounded-xl bg-[#84cc16] text-black font-bold hover:bg-[#65a30d] transition-colors shadow-lg">Submit Application</button>
            </form>
          </div>
        </div>
      )}

    </section>
  );
}
