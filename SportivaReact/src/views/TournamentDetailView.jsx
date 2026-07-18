import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Trophy, Calendar, Users, X, Info, Shield, Plus, Clock } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function TournamentDetailView({ user, addToast }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const [tournament, setTournament] = useState(null);
  const [showRegModal, setShowRegModal] = useState(false);
  const [teamName, setTeamName] = useState('');
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadTournamentDetails();
  }, [id]);

  const loadTournamentDetails = async () => {
    try {
      setIsLoading(true);
      const res = await apiFetch(`/tournaments/${id}`);
      if (res.ok) {
        const data = await res.json();
        setTournament(data);
      }
    } catch (e) {
      addToast('Failed to load tournament championship details.', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const handleJoinTournament = async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        addToast('Please login to register in tournaments.', 'info');
        return;
      }

      const res = await apiFetch(`/tournaments/${tournament.id}/join`, {
        method: 'POST'
      });

      if (res.ok) {
        addToast('Registered successfully to tournament championship!', 'success');
        loadTournamentDetails();
      } else {
        const err = await res.json();
        throw new Error(err.detail || 'Could not complete registration.');
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

  if (isLoading) {
    return (
      <div className="max-w-[700px] mx-auto flex flex-col gap-6 py-20 text-[#71717a] animate-pulse">
        <div className="h-44 bg-[#1e1e26]/30 rounded-3xl"></div>
        <div className="h-6 w-36 bg-[#1e1e26]/30 rounded"></div>
      </div>
    );
  }

  if (!tournament) {
    return (
      <div className="text-center py-20 text-[#71717a]">
        <h3 className="font-bold text-white text-lg">Tournament Not Found</h3>
        <p className="text-xs mt-1">This championship event is currently inactive or removed.</p>
      </div>
    );
  }

  const meta = parseDescription(tournament.description);

  return (
    <div className="max-w-[850px] mx-auto flex flex-col lg:flex-row gap-8 pb-20 animate-fade-in text-xs text-[#a1a1aa]">
      
      {/* LEFT COLUMN: Cover Banner, Rules and Info */}
      <div className="flex-1 flex flex-col gap-6">
        <div className="bg-[#121216]/50 border border-white/5 rounded-3xl overflow-hidden shadow-2xl relative">
          <div className="h-52 w-full bg-cover bg-center" style={{ backgroundImage: `url(${meta.imageUrl || 'https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=800&q=80'})` }}>
            <span className="absolute top-4 right-4 text-xs px-2.5 py-1 rounded bg-[#84cc16]/90 text-black font-extrabold uppercase tracking-wider">{tournament.status}</span>
          </div>
          <div className="p-8 flex flex-col gap-4">
            <h2 className="text-2xl font-extrabold text-white leading-tight">{tournament.name}</h2>
            <span className="text-[#84cc16] font-bold text-xs uppercase tracking-widest">{meta.type || 'Knockout'} Championship Bracket</span>
            <p className="text-xs text-[#a1a1aa] leading-relaxed mt-2">{meta.description || 'Championship Tournament'}</p>
          </div>
        </div>

        {/* Rules & Guidelines */}
        {meta.rules && (
          <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-3">
            <h3 className="font-bold text-white text-sm flex items-center gap-1.5"><Info className="w-4 h-4 text-[#84cc16]" /> Championship Rules</h3>
            <p className="text-white/95 leading-relaxed text-xs">{meta.rules}</p>
          </div>
        )}
      </div>

      {/* RIGHT COLUMN: Roster Enrollment */}
      <div className="w-full lg:w-[320px] flex flex-col gap-6 shrink-0">
        
        {/* Status widget */}
        <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4 shadow-xl">
          <h3 className="font-extrabold text-white text-sm flex items-center gap-1.5"><Shield className="w-4 h-4 text-[#84cc16]" /> Bracket Slots</h3>
          
          <div className="flex flex-col gap-3.5 bg-[#1e1e26]/30 border border-white/5 p-4 rounded-xl">
            <div className="flex justify-between items-center text-xs">
              <span className="text-[#a1a1aa]">Enrollment</span>
              <span className="text-white font-bold">{tournament.participantsCount} / {tournament.maxParticipants} Teams</span>
            </div>
            <div className="flex justify-between items-center text-xs">
              <span className="text-[#a1a1aa]">Start Date</span>
              <span className="text-white font-bold">{tournament.startDate}</span>
            </div>
            {meta.deadline && (
              <div className="flex justify-between items-center text-xs text-rose-400 font-semibold border-t border-white/5 pt-2">
                <span>Reg Deadline</span>
                <span>{meta.deadline}</span>
              </div>
            )}
          </div>

          <div className="border-t border-white/5 pt-4 mt-2">
            {tournament.canJoin && user?.role === 'Player' ? (
              <button onClick={handleJoinTournament} className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg flex items-center justify-center gap-1.5">
                <Plus className="w-4 h-4" /> Register Now
              </button>
            ) : tournament.canJoin ? (
              <span className="text-xs font-bold text-center block text-[#71717a] bg-white/5 p-3.5 border border-white/10 rounded-xl">Only players can register</span>
            ) : (
              <span className="text-xs font-bold text-center block text-[#71717a] bg-white/5 p-3.5 border border-white/10 rounded-xl">Registration is closed</span>
            )}
          </div>
        </div>
      </div>

    </div>
  );
}
