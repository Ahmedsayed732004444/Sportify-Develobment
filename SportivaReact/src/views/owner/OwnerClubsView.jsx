import React, { useState, useEffect } from 'react';
import { useOutletContext } from 'react-router-dom';
import { Building, Plus, Settings, Check, Ban, MapPin, Mail, Phone, Users, Trash, Edit3, X } from 'lucide-react';
import { apiFetch, getApiBaseUrl } from '../../services/api';

export default function OwnerClubsView({ addToast }) {
  const { clubs, refreshClubs, selectedClub, setSelectedClub } = useOutletContext();
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingClub, setEditingClub] = useState(null);

  // Form states
  const [clubName, setClubName] = useState('');
  const [clubGov, setClubGov] = useState('Cairo');
  const [clubCity, setClubCity] = useState('');
  const [clubAddr, setClubAddr] = useState('');
  const [clubPhone, setClubPhone] = useState('');
  const [clubEmail, setClubEmail] = useState('');
  const [logoFile, setLogoFile] = useState(null);

  const resetForm = () => {
    setClubName('');
    setClubGov('Cairo');
    setClubCity('');
    setClubAddr('');
    setClubPhone('');
    setClubEmail('');
    setLogoFile(null);
  };

  const handleEditClick = (club) => {
    setEditingClub(club);
    setClubName(club.name || '');
    setClubGov(club.governorate || 'Cairo');
    setClubCity(club.city || '');
    setClubAddr(club.address || '');
    setClubPhone(club.phoneNumber || '');
    setClubEmail(club.email || '');
    setLogoFile(null);
    setIsEditModalOpen(true);
  };

  const handleCreateSubmit = async (e) => {
    e.preventDefault();
    try {
      const formData = new FormData();
      formData.append('Name', clubName);
      formData.append('Governorate', clubGov);
      formData.append('City', clubCity);
      formData.append('Address', clubAddr);
      formData.append('PhoneNumber', clubPhone);
      formData.append('Email', clubEmail);
      if (logoFile) {
        formData.append('Logo', logoFile);
      }

      const apiBase = getApiBaseUrl();
      const token = localStorage.getItem('token');
      const res = await fetch(`${apiBase}/clubs`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        },
        body: formData
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Could not register club.');
      }

      addToast('Club registered successfully!', 'success');
      resetForm();
      setIsCreateModalOpen(false);
      refreshClubs();
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const handleEditSubmit = async (e) => {
    e.preventDefault();
    if (!editingClub) return;
    try {
      const formData = new FormData();
      formData.append('Name', clubName);
      formData.append('Governorate', clubGov);
      formData.append('City', clubCity);
      formData.append('Address', clubAddr);
      formData.append('PhoneNumber', clubPhone);
      formData.append('Email', clubEmail);
      if (logoFile) {
        formData.append('Logo', logoFile);
      }

      const apiBase = getApiBaseUrl();
      const token = localStorage.getItem('token');
      const res = await fetch(`${apiBase}/clubs/${editingClub.id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`
        },
        body: formData
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Could not update club.');
      }

      addToast('Club updated successfully!', 'success');
      resetForm();
      setIsEditModalOpen(false);
      setEditingClub(null);
      refreshClubs();
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const handleToggleStatus = async (clubId) => {
    try {
      const res = await apiFetch(`/clubs/${clubId}/status`, {
        method: 'PATCH'
      });
      if (res.ok) {
        addToast('Club status updated successfully.', 'success');
        refreshClubs();
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to toggle status.', 'error');
    }
  };

  const handleDeleteClub = async (clubId) => {
    if (!window.confirm('Are you sure you want to delete this club? This will delete all its courts and slots.')) return;
    try {
      const res = await apiFetch(`/clubs/${clubId}`, {
        method: 'DELETE'
      });
      if (res.ok) {
        addToast('Club deleted successfully.', 'success');
        refreshClubs();
        if (selectedClub?.id === clubId) {
          setSelectedClub(null);
        }
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to delete club.', 'error');
    }
  };

  const governorates = [
    'Cairo', 'Giza', 'Alexandria', 'Qalyubia', 'Gharbia', 
    'Monufia', 'Sharqia', 'Dakahlia', 'Beheira', 'Fayoum', 
    'Beni Suef', 'Minya', 'Assiut', 'Sohag', 'Qena', 'Luxor', 'Aswan'
  ];

  return (
    <div className="flex flex-col gap-8 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight text-white">Complex Facilities</h2>
          <p className="text-[#a1a1aa] text-xs mt-1">Configure your sports clubs, complex branding, and location details</p>
        </div>
        <button
          onClick={() => { resetForm(); setIsCreateModalOpen(true); }}
          className="flex items-center gap-1.5 px-5 py-2.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg self-start sm:self-center"
        >
          <Plus className="w-4 h-4" /> Register Club
        </button>
      </div>

      {/* Clubs List */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {clubs.map(club => (
          <div key={club.id} className="bg-[#121216]/60 border border-white/5 rounded-2xl flex flex-col justify-between overflow-hidden shadow-lg transition-all hover:border-white/10 group">
            {/* Top Info Banner */}
            <div className="p-6">
              <div className="flex items-start justify-between gap-4 mb-4">
                <div className="flex items-center gap-3">
                  {club.logoUrl ? (
                    <img src={club.logoUrl} alt="club logo" className="w-12 h-12 rounded-xl object-cover bg-white/5" />
                  ) : (
                    <div className="w-12 h-12 rounded-xl bg-[#84cc16]/10 flex items-center justify-center text-[#84cc16] font-bold text-lg">
                      {club.name.charAt(0)}
                    </div>
                  )}
                  <div>
                    <h3 className="text-sm font-bold text-white group-hover:text-[#84cc16] transition-colors">{club.name}</h3>
                    <span className="text-[10px] text-[#71717a] font-bold">{club.governorate}, Egypt</span>
                  </div>
                </div>

                <span className={`text-[10px] px-2 py-0.5 rounded-full font-bold uppercase tracking-wider ${
                  club.isActive !== false ? 'bg-[#84cc16]/10 text-[#84cc16] border border-[#84cc16]/20' : 'bg-red-500/10 text-red-400 border border-red-500/20'
                }`}>
                  {club.isActive !== false ? 'Active' : 'Suspended'}
                </span>
              </div>

              {/* Club Details */}
              <div className="flex flex-col gap-2.5 text-xs text-[#a1a1aa] mt-6">
                <div className="flex items-center gap-2">
                  <MapPin className="w-4 h-4 text-[#71717a] shrink-0" />
                  <span className="truncate">{club.city}, {club.address}</span>
                </div>
                <div className="flex items-center gap-2">
                  <Phone className="w-4 h-4 text-[#71717a] shrink-0" />
                  <span>{club.phoneNumber || 'N/A'}</span>
                </div>
                <div className="flex items-center gap-2">
                  <Mail className="w-4 h-4 text-[#71717a] shrink-0" />
                  <span className="truncate">{club.email || 'N/A'}</span>
                </div>
              </div>
            </div>

            {/* Bottom Actions Bar */}
            <div className="bg-[#121216] border-t border-white/5 px-6 py-4 flex items-center justify-between gap-3">
              <div className="flex items-center gap-2">
                <button
                  onClick={() => handleEditClick(club)}
                  className="p-2 bg-white/5 hover:bg-white/10 text-white rounded-xl transition-all"
                  title="Edit Club Details"
                >
                  <Edit3 className="w-4 h-4" />
                </button>
                <button
                  onClick={() => handleToggleStatus(club.id)}
                  className={`p-2 rounded-xl transition-all ${
                    club.isActive !== false ? 'bg-orange-500/10 text-orange-400 hover:bg-orange-500/20' : 'bg-[#84cc16]/10 text-[#84cc16] hover:bg-[#84cc16]/20'
                  }`}
                  title={club.isActive !== false ? 'Deactivate Club' : 'Activate Club'}
                >
                  {club.isActive !== false ? <Ban className="w-4 h-4" /> : <Check className="w-4 h-4" />}
                </button>
                <button
                  onClick={() => handleDeleteClub(club.id)}
                  className="p-2 bg-red-500/10 hover:bg-red-500/20 text-red-400 rounded-xl transition-all"
                  title="Delete Club"
                >
                  <Trash className="w-4 h-4" />
                </button>
              </div>

              {selectedClub?.id === club.id ? (
                <span className="text-[10px] text-[#84cc16] font-bold bg-[#84cc16]/10 px-2.5 py-1 rounded-lg border border-[#84cc16]/20">Active Venue</span>
              ) : (
                <button
                  onClick={() => { setSelectedClub(club); localStorage.setItem('owner_selected_club_id', club.id); }}
                  className="px-3 py-1.5 bg-white/5 hover:bg-white/10 text-white font-bold text-[10px] rounded-lg border border-white/10 transition-all cursor-pointer"
                >
                  Select Venue
                </button>
              )}
            </div>
          </div>
        ))}

        {clubs.length === 0 && (
          <div className="col-span-full bg-[#121216]/40 border border-dashed border-white/10 rounded-2xl p-12 text-center flex flex-col items-center">
            <Building className="w-12 h-12 text-[#71717a] mb-4" />
            <h3 className="text-sm font-bold text-white mb-1">No Clubs Registered</h3>
            <p className="text-xs text-[#a1a1aa] max-w-sm mb-6">Create a club profile to specify locations and register football play courts.</p>
            <button
              onClick={() => setIsCreateModalOpen(true)}
              className="px-5 py-2.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg"
            >
              Register First Club
            </button>
          </div>
        )}
      </div>

      {/* CREATE MODAL */}
      {isCreateModalOpen && (
        <div className="fixed inset-0 bg-black/80 backdrop-blur-md z-50 flex items-center justify-center p-4">
          <div className="bg-[#121216] border border-white/5 rounded-2xl max-w-[550px] w-full p-6 shadow-2xl relative animate-fade-in max-h-[90vh] overflow-y-auto">
            <button onClick={() => setIsCreateModalOpen(false)} className="absolute top-6 right-6 text-[#71717a] hover:text-white transition-colors">
              <X className="w-5 h-5" />
            </button>

            <h3 className="text-lg font-bold text-white mb-2">Register Club / Complex</h3>
            <p className="text-xs text-[#a1a1aa] mb-6">Provide location, contact info, and branding logo for your facility.</p>

            <form onSubmit={handleCreateSubmit} className="flex flex-col gap-4">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Club Name</label>
                <input
                  required
                  type="text"
                  placeholder="e.g. Al-Ahly Club Court, Smash Academy"
                  value={clubName}
                  onChange={(e) => setClubName(e.target.value)}
                  className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Governorate</label>
                  <select
                    value={clubGov}
                    onChange={(e) => setClubGov(e.target.value)}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white focus:border-[#84cc16] outline-none transition-all cursor-pointer"
                  >
                    {governorates.map(gov => (
                      <option key={gov} value={gov} className="bg-[#121216]">{gov}</option>
                    ))}
                  </select>
                </div>

                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">City</label>
                  <input
                    required
                    type="text"
                    placeholder="e.g. Nasr City, Sheikh Zayed"
                    value={clubCity}
                    onChange={(e) => setClubCity(e.target.value)}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                  />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Address</label>
                <input
                  required
                  type="text"
                  placeholder="Street name, landmark details..."
                  value={clubAddr}
                  onChange={(e) => setClubAddr(e.target.value)}
                  className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Phone Number</label>
                  <input
                    required
                    type="tel"
                    placeholder="01xxxxxxxxx"
                    value={clubPhone}
                    onChange={(e) => setClubPhone(e.target.value)}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                  />
                </div>

                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Email Address</label>
                  <input
                    required
                    type="email"
                    placeholder="info@clubname.com"
                    value={clubEmail}
                    onChange={(e) => setClubEmail(e.target.value)}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                  />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Logo Image</label>
                <input
                  type="file"
                  accept="image/*"
                  onChange={(e) => setLogoFile(e.target.files[0])}
                  className="text-xs text-[#a1a1aa] file:mr-4 file:py-2.5 file:px-4 file:rounded-xl file:border-0 file:text-xs file:font-bold file:bg-[#84cc16]/10 file:text-[#84cc16] file:hover:bg-[#84cc16]/20 cursor-pointer"
                />
              </div>

              <button
                type="submit"
                className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl shadow-lg mt-4 transition-all"
              >
                Register Club Facility
              </button>
            </form>
          </div>
        </div>
      )}

      {/* EDIT MODAL */}
      {isEditModalOpen && (
        <div className="fixed inset-0 bg-black/80 backdrop-blur-md z-50 flex items-center justify-center p-4">
          <div className="bg-[#121216] border border-white/5 rounded-2xl max-w-[550px] w-full p-6 shadow-2xl relative animate-fade-in max-h-[90vh] overflow-y-auto">
            <button onClick={() => { setIsEditModalOpen(false); setEditingClub(null); }} className="absolute top-6 right-6 text-[#71717a] hover:text-white transition-colors">
              <X className="w-5 h-5" />
            </button>

            <h3 className="text-lg font-bold text-white mb-2">Edit Club Facility</h3>
            <p className="text-xs text-[#a1a1aa] mb-6">Modify details, address, contacts, or upload a new branding logo.</p>

            <form onSubmit={handleEditSubmit} className="flex flex-col gap-4">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Club Name</label>
                <input
                  required
                  type="text"
                  value={clubName}
                  onChange={(e) => setClubName(e.target.value)}
                  className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Governorate</label>
                  <select
                    value={clubGov}
                    onChange={(e) => setClubGov(e.target.value)}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white focus:border-[#84cc16] outline-none transition-all cursor-pointer"
                  >
                    {governorates.map(gov => (
                      <option key={gov} value={gov} className="bg-[#121216]">{gov}</option>
                    ))}
                  </select>
                </div>

                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">City</label>
                  <input
                    required
                    type="text"
                    value={clubCity}
                    onChange={(e) => setClubCity(e.target.value)}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                  />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Address</label>
                <input
                  required
                  type="text"
                  value={clubAddr}
                  onChange={(e) => setClubAddr(e.target.value)}
                  className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Phone Number</label>
                  <input
                    required
                    type="tel"
                    value={clubPhone}
                    onChange={(e) => setClubPhone(e.target.value)}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                  />
                </div>

                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Email Address</label>
                  <input
                    required
                    type="email"
                    value={clubEmail}
                    onChange={(e) => setClubEmail(e.target.value)}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                  />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Change Logo Image (Optional)</label>
                <input
                  type="file"
                  accept="image/*"
                  onChange={(e) => setLogoFile(e.target.files[0])}
                  className="text-xs text-[#a1a1aa] file:mr-4 file:py-2.5 file:px-4 file:rounded-xl file:border-0 file:text-xs file:font-bold file:bg-[#84cc16]/10 file:text-[#84cc16] file:hover:bg-[#84cc16]/20 cursor-pointer"
                />
              </div>

              <button
                type="submit"
                className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl shadow-lg mt-4 transition-all"
              >
                Save Changes
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
