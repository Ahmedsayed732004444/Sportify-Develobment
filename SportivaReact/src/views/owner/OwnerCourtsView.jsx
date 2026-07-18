import React, { useState, useEffect } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import { apiFetch, getApiBaseUrl } from '../../services/api';
import { Plus, Settings, Check, Ban, Shield, Trash, Edit3, X, Calendar, DollarSign, Users, AlertCircle } from 'lucide-react';

export default function OwnerCourtsView({ addToast }) {
  const { selectedClub } = useOutletContext();
  const navigate = useNavigate();

  const [courts, setCourts] = useState([]);
  const [loading, setLoading] = useState(false);
  
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingCourt, setEditingCourt] = useState(null);

  // Form states
  const [courtName, setCourtName] = useState('');
  const [courtDesc, setCourtDesc] = useState('');
  const [sportType, setSportType] = useState('Football');
  const [maxCapacity, setMaxCapacity] = useState(10);
  const [pricePerHour, setPricePerHour] = useState(150);
  const [courtImageFile, setCourtImageFile] = useState(null);

  useEffect(() => {
    if (selectedClub) {
      loadClubCourts();
    }
  }, [selectedClub]);

  const loadClubCourts = async () => {
    setLoading(true);
    try {
      const res = await apiFetch(`/clubs/${selectedClub.id}/courts`);
      if (res.ok) {
        const data = await res.json();
        setCourts(data.items || data || []);
      }
    } catch (e) {
      addToast('Failed to load courts.', 'error');
    } finally {
      setLoading(false);
    }
  };

  const resetForm = () => {
    setCourtName('');
    setCourtDesc('');
    setSportType('Football');
    setMaxCapacity(10);
    setPricePerHour(150);
    setCourtImageFile(null);
  };

  const handleEditClick = (court) => {
    setEditingCourt(court);
    setCourtName(court.name || '');
    setCourtDesc(court.description || '');
    setSportType(court.sportType || 'Football');
    setMaxCapacity(court.maxCapacity || 10);
    setPricePerHour(court.pricePerHour || 150);
    setCourtImageFile(null);
    setIsEditModalOpen(true);
  };

  const handleCreateSubmit = async (e) => {
    e.preventDefault();
    if (!selectedClub) return;
    try {
      const formData = new FormData();
      formData.append('Name', courtName);
      formData.append('Description', courtDesc);
      formData.append('SportType', sportType);
      formData.append('MaxCapacity', maxCapacity);
      formData.append('PricePerHour', pricePerHour);
      if (courtImageFile) {
        formData.append('Image', courtImageFile);
      }

      const apiBase = getApiBaseUrl();
      const token = localStorage.getItem('token');
      const res = await fetch(`${apiBase}/clubs/${selectedClub.id}/courts`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        },
        body: formData
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Could not add court.');
      }

      addToast('Court added successfully!', 'success');
      resetForm();
      setIsCreateModalOpen(false);
      loadClubCourts();
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const handleEditSubmit = async (e) => {
    e.preventDefault();
    if (!selectedClub || !editingCourt) return;
    try {
      const formData = new FormData();
      formData.append('Name', courtName);
      formData.append('Description', courtDesc);
      formData.append('SportType', sportType);
      formData.append('MaxCapacity', maxCapacity);
      formData.append('PricePerHour', pricePerHour);
      if (courtImageFile) {
        formData.append('Image', courtImageFile);
      }

      const apiBase = getApiBaseUrl();
      const token = localStorage.getItem('token');
      const res = await fetch(`${apiBase}/clubs/${selectedClub.id}/courts/${editingCourt.id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`
        },
        body: formData
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Could not update court.');
      }

      addToast('Court updated successfully!', 'success');
      resetForm();
      setIsEditModalOpen(false);
      setEditingCourt(null);
      loadClubCourts();
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  const handleToggleStatus = async (courtId) => {
    try {
      const res = await apiFetch(`/clubs/${selectedClub.id}/courts/${courtId}/status`, {
        method: 'PATCH'
      });
      if (res.ok) {
        addToast('Court status updated successfully.', 'success');
        loadClubCourts();
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to update status.', 'error');
    }
  };

  const handleDeleteCourt = async (courtId) => {
    if (!window.confirm('Are you sure you want to delete this court? All time slots and bookings for this court may be affected.')) return;
    try {
      const res = await apiFetch(`/clubs/${selectedClub.id}/courts/${courtId}`, {
        method: 'DELETE'
      });
      if (res.ok) {
        addToast('Court deleted successfully.', 'success');
        loadClubCourts();
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to delete court.', 'error');
    }
  };

  if (!selectedClub) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-6 bg-[#121216]/40 border border-white/5 rounded-2xl shadow-lg">
        <AlertCircle className="w-12 h-12 text-[#a1a1aa] mb-4" />
        <h3 className="text-lg font-bold text-white mb-2">No active venue selected</h3>
        <p className="text-xs text-[#a1a1aa] max-w-sm">Please register or select a club from the sidebar before configuring courts.</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-8 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight text-white">Court Management</h2>
          <p className="text-[#a1a1aa] text-xs mt-1">Configure play courts, pricing per hour, capacity, and slot generation</p>
        </div>
        <button
          onClick={() => { resetForm(); setIsCreateModalOpen(true); }}
          className="flex items-center gap-1.5 px-5 py-2.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg self-start sm:self-center"
        >
          <Plus className="w-4 h-4" /> Add Court
        </button>
      </div>

      {/* Courts grid */}
      {loading ? (
        <div className="text-xs text-[#a1a1aa] py-20 text-center">Loading courts...</div>
      ) : courts.length === 0 ? (
        <div className="bg-[#121216]/40 border border-dashed border-white/10 rounded-2xl p-12 text-center flex flex-col items-center">
          <Shield className="w-12 h-12 text-[#71717a] mb-4" />
          <h3 className="text-sm font-bold text-white mb-1">No Courts Registered</h3>
          <p className="text-xs text-[#a1a1aa] max-w-sm mb-6">Start setting up your sports center by adding football, tennis, or padel courts.</p>
          <button
            onClick={() => setIsCreateModalOpen(true)}
            className="px-5 py-2.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-lg"
          >
            Add First Court
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {courts.map(court => (
            <div key={court.id} className="bg-[#121216]/60 border border-white/5 rounded-2xl overflow-hidden shadow-lg hover:border-white/10 transition-all flex flex-col justify-between group">
              
              <div>
                {/* Court Image Banner */}
                <div className="h-40 w-full relative bg-white/5">
                  {court.imageUrl ? (
                    <img src={court.imageUrl} alt="court" className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
                  ) : (
                    <div className="w-full h-full flex items-center justify-center bg-white/5 text-[#71717a] font-bold text-xs">
                      No Court Image
                    </div>
                  )}
                  <div className="absolute top-4 right-4">
                    <span className={`text-[9px] px-2.5 py-1 rounded-full font-bold uppercase tracking-wider ${
                      court.isActive !== false ? 'bg-[#84cc16] text-black shadow-lg shadow-[#84cc16]/10' : 'bg-red-500 text-white'
                    }`}>
                      {court.isActive !== false ? 'Active' : 'Offline'}
                    </span>
                  </div>
                </div>

                {/* Court Info */}
                <div className="p-6">
                  <h3 className="text-sm font-bold text-white mb-2">{court.name}</h3>
                  <p className="text-xs text-[#a1a1aa] line-clamp-2 min-h-[2rem] mb-6">{court.description || 'No description provided.'}</p>

                  <div className="grid grid-cols-3 gap-2 text-center">
                    <div className="bg-white/5 p-3 rounded-xl border border-white/5">
                      <span className="text-[9px] text-[#71717a] font-bold uppercase tracking-wider block">Sport</span>
                      <span className="text-xs text-white font-semibold mt-1 block">{court.sportType || 'Football'}</span>
                    </div>

                    <div className="bg-white/5 p-3 rounded-xl border border-white/5">
                      <span className="text-[9px] text-[#71717a] font-bold uppercase tracking-wider block">Capacity</span>
                      <span className="text-xs text-white font-semibold mt-1 block">{court.maxCapacity} players</span>
                    </div>

                    <div className="bg-white/5 p-3 rounded-xl border border-white/5">
                      <span className="text-[9px] text-[#71717a] font-bold uppercase tracking-wider block">Price/hr</span>
                      <span className="text-xs text-[#84cc16] font-extrabold mt-1 block">EGP {court.pricePerHour}</span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Action Bar */}
              <div className="bg-[#121216] border-t border-white/5 px-6 py-4 flex items-center justify-between gap-3">
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => handleEditClick(court)}
                    className="p-2 bg-white/5 hover:bg-white/10 text-white rounded-xl transition-all"
                    title="Edit Court"
                  >
                    <Edit3 className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => handleToggleStatus(court.id)}
                    className={`p-2 rounded-xl transition-all ${
                      court.isActive !== false ? 'bg-orange-500/10 text-orange-400 hover:bg-orange-500/20' : 'bg-[#84cc16]/10 text-[#84cc16] hover:bg-[#84cc16]/20'
                    }`}
                    title={court.isActive !== false ? 'Take Offline' : 'Set Active'}
                  >
                    {court.isActive !== false ? <Ban className="w-4 h-4" /> : <Check className="w-4 h-4" />}
                  </button>
                  <button
                    onClick={() => handleDeleteCourt(court.id)}
                    className="p-2 bg-red-500/10 hover:bg-red-500/20 text-red-400 rounded-xl transition-all"
                    title="Delete Court"
                  >
                    <Trash className="w-4 h-4" />
                  </button>
                </div>

                <button
                  onClick={() => navigate(`/owner/courts/${court.id}/schedule`)}
                  className="flex items-center gap-1.5 px-3 py-2 bg-[#84cc16]/10 hover:bg-[#84cc16]/20 text-[#84cc16] font-bold text-[10px] rounded-xl border border-[#84cc16]/20 transition-all cursor-pointer"
                >
                  <Calendar className="w-3.5 h-3.5" /> Manage Slots
                </button>
              </div>

            </div>
          ))}
        </div>
      )}

      {/* CREATE COURT MODAL */}
      {isCreateModalOpen && (
        <div className="fixed inset-0 bg-black/80 backdrop-blur-md z-50 flex items-center justify-center p-4">
          <div className="bg-[#121216] border border-white/5 rounded-2xl max-w-[500px] w-full p-6 shadow-2xl relative animate-fade-in max-h-[90vh] overflow-y-auto">
            <button onClick={() => setIsCreateModalOpen(false)} className="absolute top-6 right-6 text-[#71717a] hover:text-white transition-colors">
              <X className="w-5 h-5" />
            </button>

            <h3 className="text-lg font-bold text-white mb-2">Add Play Court</h3>
            <p className="text-xs text-[#a1a1aa] mb-6">Create a court profile, set pricing, capacity, and upload facility pictures.</p>

            <form onSubmit={handleCreateSubmit} className="flex flex-col gap-4">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Court Name</label>
                <input
                  required
                  type="text"
                  placeholder="e.g. Football pitch A (5v5)"
                  value={courtName}
                  onChange={(e) => setCourtName(e.target.value)}
                  className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all"
                />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Description</label>
                <textarea
                  placeholder="Pitch specifications, grass type, amenities..."
                  value={courtDesc}
                  onChange={(e) => setCourtDesc(e.target.value)}
                  rows="3"
                  className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white placeholder-white/20 focus:border-[#84cc16] outline-none transition-all resize-none"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Sport Type</label>
                  <select
                    value={sportType}
                    onChange={(e) => setSportType(e.target.value)}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white focus:border-[#84cc16] outline-none transition-all cursor-pointer"
                  >
                    <option value="Football" className="bg-[#121216]">Football</option>
                    <option value="Padel" className="bg-[#121216]">Padel</option>
                    <option value="Tennis" className="bg-[#121216]">Tennis</option>
                    <option value="Basketball" className="bg-[#121216]">Basketball</option>
                  </select>
                </div>

                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Max Capacity (players)</label>
                  <input
                    required
                    type="number"
                    min="1"
                    value={maxCapacity}
                    onChange={(e) => setMaxCapacity(parseInt(e.target.value))}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white focus:border-[#84cc16] outline-none transition-all"
                  />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Price per Hour (EGP)</label>
                <input
                  required
                  type="number"
                  min="0"
                  value={pricePerHour}
                  onChange={(e) => setPricePerHour(parseInt(e.target.value))}
                  className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white focus:border-[#84cc16] outline-none transition-all"
                />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Court Picture</label>
                <input
                  type="file"
                  accept="image/*"
                  onChange={(e) => setCourtImageFile(e.target.files[0])}
                  className="text-xs text-[#a1a1aa] file:mr-4 file:py-2.5 file:px-4 file:rounded-xl file:border-0 file:text-xs file:font-bold file:bg-[#84cc16]/10 file:text-[#84cc16] file:hover:bg-[#84cc16]/20 cursor-pointer"
                />
              </div>

              <button
                type="submit"
                className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl shadow-lg mt-4 transition-all"
              >
                Add Court
              </button>
            </form>
          </div>
        </div>
      )}

      {/* EDIT COURT MODAL */}
      {isEditModalOpen && (
        <div className="fixed inset-0 bg-black/80 backdrop-blur-md z-50 flex items-center justify-center p-4">
          <div className="bg-[#121216] border border-white/5 rounded-2xl max-w-[500px] w-full p-6 shadow-2xl relative animate-fade-in max-h-[90vh] overflow-y-auto">
            <button onClick={() => { setIsEditModalOpen(false); setEditingCourt(null); }} className="absolute top-6 right-6 text-[#71717a] hover:text-white transition-colors">
              <X className="w-5 h-5" />
            </button>

            <h3 className="text-lg font-bold text-white mb-2">Edit Court Profile</h3>
            <p className="text-xs text-[#a1a1aa] mb-6">Modify specifications, hourly booking rates, or replace court pictures.</p>

            <form onSubmit={handleEditSubmit} className="flex flex-col gap-4">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Court Name</label>
                <input
                  required
                  type="text"
                  value={courtName}
                  onChange={(e) => setCourtName(e.target.value)}
                  className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white focus:border-[#84cc16] outline-none transition-all"
                />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Description</label>
                <textarea
                  value={courtDesc}
                  onChange={(e) => setCourtDesc(e.target.value)}
                  rows="3"
                  className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white focus:border-[#84cc16] outline-none transition-all resize-none"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Sport Type</label>
                  <select
                    value={sportType}
                    onChange={(e) => setSportType(e.target.value)}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white focus:border-[#84cc16] outline-none transition-all cursor-pointer"
                  >
                    <option value="Football" className="bg-[#121216]">Football</option>
                    <option value="Padel" className="bg-[#121216]">Padel</option>
                    <option value="Tennis" className="bg-[#121216]">Tennis</option>
                    <option value="Basketball" className="bg-[#121216]">Basketball</option>
                  </select>
                </div>

                <div className="flex flex-col gap-1.5">
                  <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Max Capacity (players)</label>
                  <input
                    required
                    type="number"
                    min="1"
                    value={maxCapacity}
                    onChange={(e) => setMaxCapacity(parseInt(e.target.value))}
                    className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white focus:border-[#84cc16] outline-none transition-all"
                  />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Price per Hour (EGP)</label>
                <input
                  required
                  type="number"
                  min="0"
                  value={pricePerHour}
                  onChange={(e) => setPricePerHour(parseInt(e.target.value))}
                  className="px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-xs text-white focus:border-[#84cc16] outline-none transition-all"
                />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider">Change Picture (Optional)</label>
                <input
                  type="file"
                  accept="image/*"
                  onChange={(e) => setCourtImageFile(e.target.files[0])}
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
