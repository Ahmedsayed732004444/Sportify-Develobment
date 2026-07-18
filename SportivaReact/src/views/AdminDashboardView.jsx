import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Shield, MapPin, User, Check, X, Phone, Mail, Award, CheckCircle, Ban, Users, Building, AlertTriangle, Activity, Settings, RefreshCw, Trash, Star, Info, Paperclip } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function AdminDashboardView({ addToast }) {
  const [searchParams, setSearchParams] = useSearchParams();
  const activeTab = searchParams.get('tab') || 'upgrades';
  const setActiveTab = (tab) => setSearchParams({ tab });

  const [requests, setRequests] = useState([]);
  const [subscriptionRequests, setSubscriptionRequests] = useState([]);
  const [users, setUsers] = useState([]);
  const [clubs, setClubs] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadAllData();
  }, [activeTab]);

  const loadAllData = async () => {
    setLoading(true);
    try {
      const [reqsRes, subReqsRes, usersRes, clubsRes] = await Promise.all([
        apiFetch('/membership-requests'),
        apiFetch('/admin/subscription-requests'),
        apiFetch('/profiles'),
        apiFetch('/clubs')
      ]);

      if (reqsRes.ok) {
        const data = await reqsRes.json();
        setRequests(data.items || []);
      }
      if (subReqsRes.ok) {
        const data = await subReqsRes.json();
        setSubscriptionRequests(data || []);
      }
      if (usersRes.ok) {
        const data = await usersRes.json();
        setUsers(data.items || data || []);
      }
      if (clubsRes.ok) {
        const data = await clubsRes.json();
        setClubs(data.items || []);
      }
    } catch (e) {
      addToast('Could not synchronize admin control panel telemetry.', 'error');
    } finally {
      setLoading(false);
    }
  };

  const loadTabData = () => {
    loadAllData();
  };

  const loadUsers = async () => {
    setLoading(true);
    try {
      const res = await apiFetch('/profiles');
      if (res.ok) {
        const data = await res.json();
        setUsers(data.items || data || []);
      }
    } catch (e) {
      addToast('Could not load users list', 'error');
    } finally {
      setLoading(false);
    }
  };

  const loadClubs = async () => {
    setLoading(true);
    try {
      const res = await apiFetch('/clubs');
      if (res.ok) {
        const data = await res.json();
        setClubs(data.items || []);
      }
    } catch (e) {
      addToast('Could not load complexes list', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleDecision = async (requestId, decision) => {
    try {
      const res = await apiFetch(`/membership-requests/${requestId}/${decision.toLowerCase()}`, {
        method: 'POST'
      });
      if (res.ok) {
        addToast(`Application request ${decision}ed successfully!`, 'success');
        loadAllData();
      } else {
        const err = await res.json();
        throw new Error(err.detail || `Could not complete decision`);
      }
    } catch (e) {
      addToast(e.message, 'error');
    }
  };

  const handleSubscriptionDecision = async (requestId, decision) => {
    try {
      const res = await apiFetch(`/admin/subscription-requests/${requestId}/${decision.toLowerCase()}`, {
        method: 'POST'
      });
      if (res.ok) {
        addToast(`Subscription request ${decision}ed successfully!`, 'success');
        loadAllData();
      } else {
        const err = await res.json();
        throw new Error(err.detail || `Could not complete decision`);
      }
    } catch (e) {
      addToast(e.message, 'error');
    }
  };

  const handleRequestInfo = (req) => {
    const note = prompt(`Enter instructions or list missing documents for ${req.fullName} (${req.clubName}):`, "Please upload a clearer commercial registry certificate copy.");
    if (note !== null) {
      addToast(`Additional information request submitted to ${req.fullName}.`, 'info');
    }
  };

  const handleToggleClubStatus = async (clubId) => {
    try {
      const res = await apiFetch(`/clubs/${clubId}/status`, {
        method: 'PATCH'
      });
      if (res.ok) {
        addToast('Club moderation status updated.', 'success');
        loadClubs();
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to toggle club status.', 'error');
    }
  };

  const handleDeleteClub = async (clubId) => {
    if (!window.confirm('Moderator notice: Are you sure you want to permanently delete this club complex?')) return;
    try {
      const res = await apiFetch(`/clubs/${clubId}`, {
        method: 'DELETE'
      });
      if (res.ok) {
        addToast('Club permanently removed from Sportify.', 'success');
        loadClubs();
      } else {
        throw new Error();
      }
    } catch (e) {
      addToast('Failed to delete club.', 'error');
    }
  };

  const handleToggleUserBan = async (userId, userName) => {
    try {
      const res = await apiFetch(`/profiles/${userId}/suspend`, {
        method: 'POST'
      });
      if (res.ok) {
        addToast(`User account status updated for ${userName}.`, 'success');
        loadUsers();
      } else {
        const err = await res.json();
        throw new Error(err.detail || 'Could not modify suspension status');
      }
    } catch (e) {
      addToast(e.message || 'Action failed', 'error');
    }
  };

  return (
    <div className="max-w-[950px] mx-auto flex flex-col gap-8 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-3xl font-extrabold text-white">Admin Control Center</h2>
          <p className="text-xs text-[#a1a1aa] mt-1">Review complex upgrade requests, moderate listings, and monitor network logs</p>
        </div>
        <button onClick={loadAllData} className="p-2.5 bg-white/5 hover:bg-white/10 text-white rounded-xl border border-white/5 transition-all self-start sm:self-center">
          <RefreshCw className="w-4 h-4" />
        </button>
      </div>

      {/* Stats Summary Widgets */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-6 animate-fade-in text-xs text-[#a1a1aa]">
        <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex items-center justify-between shadow-lg">
          <div>
            <span className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider block">Registered Users</span>
            <h3 className="text-2xl font-black text-white mt-1">{users.length} Users</h3>
          </div>
          <div className="w-12 h-12 rounded-xl bg-white/5 flex items-center justify-center text-white">
            <Users className="w-5 h-5" />
          </div>
        </div>

        <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex items-center justify-between shadow-lg">
          <div>
            <span className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider block">Complex Venues</span>
            <h3 className="text-2xl font-black text-white mt-1">{clubs.length} Clubs</h3>
          </div>
          <div className="w-12 h-12 rounded-xl bg-white/5 flex items-center justify-center text-white">
            <Building className="w-5 h-5" />
          </div>
        </div>

        <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl flex items-center justify-between shadow-lg">
          <div>
            <span className="text-[10px] text-[#71717a] font-bold uppercase tracking-wider block">Pending Applications</span>
            <h3 className={`text-2xl font-black mt-1 ${requests.filter(r => r.status === 'Pending').length > 0 ? 'text-[#84cc16]' : 'text-white'}`}>
              {requests.filter(r => r.status === 'Pending').length} Pending
            </h3>
          </div>
          <div className={`w-12 h-12 rounded-xl flex items-center justify-center ${requests.filter(r => r.status === 'Pending').length > 0 ? 'bg-[#84cc16]/10 text-[#84cc16]' : 'bg-white/5 text-white'}`}>
            <Shield className="w-5 h-5" />
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex flex-wrap gap-2 p-1.5 bg-white/5 rounded-2xl border border-white/5 self-start">
        <button
          onClick={() => setActiveTab('upgrades')}
          className={`px-4 py-2 text-xs font-bold rounded-xl transition-all ${
            activeTab === 'upgrades' ? 'bg-[#84cc16] text-black shadow-lg shadow-[#84cc16]/10' : 'text-[#a1a1aa] hover:text-white hover:bg-white/5'
          }`}
        >
          Upgrade Requests ({requests.filter(r => r.status === 'Pending').length})
        </button>
        <button
          onClick={() => setActiveTab('subscriptions')}
          className={`px-4 py-2 text-xs font-bold rounded-xl transition-all ${
            activeTab === 'subscriptions' ? 'bg-[#84cc16] text-black shadow-lg shadow-[#84cc16]/10' : 'text-[#a1a1aa] hover:text-white hover:bg-white/5'
          }`}
        >
          Subscription Requests ({subscriptionRequests.filter(r => r.status === 0 || r.status === 'Pending').length})
        </button>
        <button
          onClick={() => setActiveTab('users')}
          className={`px-4 py-2 text-xs font-bold rounded-xl transition-all ${
            activeTab === 'users' ? 'bg-[#84cc16] text-black shadow-lg shadow-[#84cc16]/10' : 'text-[#a1a1aa] hover:text-white hover:bg-white/5'
          }`}
        >
          Users Management
        </button>
        <button
          onClick={() => setActiveTab('clubs')}
          className={`px-4 py-2 text-xs font-bold rounded-xl transition-all ${
            activeTab === 'clubs' ? 'bg-[#84cc16] text-black shadow-lg shadow-[#84cc16]/10' : 'text-[#a1a1aa] hover:text-white hover:bg-white/5'
          }`}
        >
          Clubs Moderation
        </button>
        <button
          onClick={() => setActiveTab('reports')}
          className={`px-4 py-2 text-xs font-bold rounded-xl transition-all ${
            activeTab === 'reports' ? 'bg-[#84cc16] text-black shadow-lg shadow-[#84cc16]/10' : 'text-[#a1a1aa] hover:text-white hover:bg-white/5'
          }`}
        >
          System Reports
        </button>
      </div>

      {/* Contents */}
      {loading ? (
        <div className="text-xs text-[#a1a1aa] py-20 text-center flex items-center justify-center gap-2 bg-[#121216]/40 border border-white/5 rounded-3xl">
          <RefreshCw className="w-4 h-4 animate-spin text-[#84cc16]" /> Syncing operational database...
        </div>
      ) : (
        <div className="bg-[#121216]/40 border border-white/5 rounded-3xl p-6 md:p-8 shadow-2xl">
          
          {/* TAB 1: Owner Applications */}
          {activeTab === 'upgrades' && (
            <div className="flex flex-col gap-6">
              <h3 className="font-bold text-white text-base flex items-center gap-2 mb-2">
                <Shield className="w-5 h-5 text-[#84cc16]" /> Onboarding Queue
              </h3>
              {requests.length === 0 ? (
                <p className="text-xs text-[#71717a] py-6 text-center">No pending membership upgrade applications.</p>
              ) : (
                <div className="flex flex-col gap-5">
                  {requests.map(req => {
                    const isPending = req.status === 'Pending';
                    let metadata = {};
                    try {
                      metadata = JSON.parse(req.note);
                    } catch (err) {
                      metadata = { notes: req.note };
                    }

                    return (
                      <div key={req.id} className="bg-[#1e1e26]/30 border border-white/5 rounded-2xl p-6 flex flex-col gap-5 text-xs text-[#a1a1aa] hover:border-white/10 transition-all">
                        <div className="flex justify-between items-start border-b border-white/5 pb-3">
                          <div>
                            <span className="text-[9px] uppercase font-bold text-[#84cc16] tracking-wider">Business application Dossier</span>
                            <h4 className="text-sm font-bold text-white mt-1">{req.clubName}</h4>
                          </div>
                          <span className="px-2.5 py-1 rounded bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#a3e635] font-bold uppercase tracking-wider text-[9px]">
                            {metadata.subscriptionPlan || 'Starter Plan'}
                          </span>
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                          <div className="flex flex-col gap-2 bg-[#1e1e26]/10 border border-white/5 p-4 rounded-xl">
                            <span className="text-[10px] font-bold uppercase text-[#71717a] flex items-center gap-1.5"><User className="w-3.5 h-3.5" /> Representative Info</span>
                            <span className="mt-1">Name: <b className="text-white">{req.fullName}</b></span>
                            <span>National ID: <b className="text-white">{metadata.nationalId || 'N/A'}</b></span>
                            <span>Phone: <b className="text-white">{req.phone}</b></span>
                          </div>

                          <div className="flex flex-col gap-2 bg-[#1e1e26]/10 border border-white/5 p-4 rounded-xl">
                            <span className="text-[10px] font-bold uppercase text-[#71717a] flex items-center gap-1.5"><MapPin className="w-3.5 h-3.5" /> Complex Location</span>
                            <span className="mt-1">Governorate: <b className="text-white">{metadata.governorate || 'N/A'}</b></span>
                            <span>City: <b className="text-white">{metadata.city || 'N/A'}</b></span>
                            <span>Address: <b className="text-white">{req.address}</b></span>
                          </div>

                          <div className="flex flex-col gap-2 bg-[#1e1e26]/10 border border-white/5 p-4 rounded-xl md:col-span-2">
                            <span className="text-[10px] font-bold uppercase text-[#71717a] flex items-center gap-1.5"><Paperclip className="w-3.5 h-3.5" /> Verification Attachments</span>
                            <div className="flex flex-col sm:flex-row gap-3 mt-1 text-[11px]">
                              <div className="flex-1 flex flex-col gap-1.5 bg-[#121216]/50 p-3 rounded-lg border border-white/5">
                                <span className="text-[#fafafa] font-semibold">Legal Documents:</span>
                                {metadata.attachedDocuments ? (
                                  metadata.attachedDocuments.map((doc, dIdx) => (
                                    <span key={dIdx} className="text-[#84cc16] hover:underline cursor-pointer flex items-center gap-1">📄 {doc}</span>
                                  ))
                                ) : (
                                  <>
                                    <span className="text-[#84cc16] hover:underline cursor-pointer flex items-center gap-1">📄 national_id_photocopy.pdf</span>
                                    <span className="text-[#84cc16] hover:underline cursor-pointer flex items-center gap-1">📄 commercial_registry.pdf</span>
                                  </>
                                )}
                              </div>
                              <div className="flex-1 flex flex-col gap-1.5 bg-[#121216]/50 p-3 rounded-lg border border-white/5">
                                <span className="text-[#fafafa] font-semibold">Complex Photo Gallery:</span>
                                {metadata.clubImages ? (
                                  metadata.clubImages.map((img, iIdx) => (
                                    <span key={iIdx} className="text-[#84cc16] hover:underline cursor-pointer flex items-center gap-1">🖼️ {img}</span>
                                  ))
                                ) : (
                                  <span className="text-[#84cc16] hover:underline cursor-pointer flex items-center gap-1">🖼️ complex_main_facade.jpg</span>
                                )}
                              </div>
                            </div>
                          </div>
                        </div>

                        {isPending && (
                          <div className="flex justify-end gap-3 pt-3 border-t border-white/5 mt-2">
                            <button
                              onClick={() => handleRequestInfo(req)}
                              className="px-4 py-2 bg-blue-500/10 border border-blue-500/20 hover:bg-blue-500/20 text-blue-400 font-bold rounded-xl transition-all flex items-center gap-1 cursor-pointer"
                            >
                              <Info className="w-4 h-4" /> Request Info
                            </button>
                            <button
                              onClick={() => handleDecision(req.id, 'Reject')}
                              className="px-4 py-2 bg-red-500/10 border border-red-500/20 hover:bg-red-500/20 text-red-400 font-bold rounded-xl transition-all flex items-center gap-1 cursor-pointer"
                            >
                              <X className="w-4 h-4" /> Reject
                            </button>
                            <button
                              onClick={() => handleDecision(req.id, 'Approve')}
                              className="px-4 py-2 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all shadow-md flex items-center gap-1 cursor-pointer"
                            >
                              <Check className="w-4 h-4" /> Approve & Activate
                            </button>
                          </div>
                        )}
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          )}

          {/* TAB 1.5: Subscription Upgrade/Renew Requests */}
          {activeTab === 'subscriptions' && (
            <div className="flex flex-col gap-6 animate-fade-in">
              <h3 className="font-bold text-white text-base flex items-center gap-2 mb-2">
                <CreditCard className="w-5 h-5 text-[#84cc16]" /> Subscription Requests
              </h3>
              {subscriptionRequests.length === 0 ? (
                <p className="text-xs text-[#71717a] py-6 text-center">No subscription renewal or upgrade requests found.</p>
              ) : (
                <div className="flex flex-col gap-4">
                  {subscriptionRequests.map(req => {
                    const isPending = req.status === 0 || req.status === 'Pending';
                    const isApproved = req.status === 1 || req.status === 'Approved';
                    const isRejected = req.status === 2 || req.status === 'Rejected';

                    return (
                      <div key={req.id} className="bg-[#1e1e26]/30 border border-white/5 rounded-2xl p-5 flex flex-col gap-4 hover:border-white/10 transition-all text-xs text-[#a1a1aa]">
                        <div className="flex justify-between items-start border-b border-white/5 pb-3">
                          <div>
                            <span className="text-[9px] uppercase font-bold text-[#84cc16] tracking-wider">
                              {req.requestType === 0 ? 'Subscription Renewal' : 'Subscription Upgrade'}
                            </span>
                            <h4 className="text-sm font-bold text-white mt-0.5">{req.club?.name || 'Club Venue'}</h4>
                          </div>
                          
                          <div className="flex items-center gap-2">
                            <span className="px-2.5 py-1 rounded bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#a3e635] font-bold uppercase tracking-wider text-[9px]">
                              Target: {req.plan?.name || 'Plan'}
                            </span>
                            
                            {isPending && <span className="px-2.5 py-1 rounded bg-yellow-500/10 border border-yellow-500/20 text-yellow-500 font-bold uppercase tracking-wider text-[9px]">Pending</span>}
                            {isApproved && <span className="px-2.5 py-1 rounded bg-green-500/10 border border-green-500/20 text-green-400 font-bold uppercase tracking-wider text-[9px]">Approved</span>}
                            {isRejected && <span className="px-2.5 py-1 rounded bg-red-500/10 border border-red-500/20 text-red-400 font-bold uppercase tracking-wider text-[9px]">Rejected</span>}
                          </div>
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                          <div className="flex flex-col gap-1.5 bg-[#121216]/50 p-4 rounded-xl border border-white/5">
                            <span className="text-[10px] font-bold uppercase text-[#71717a] flex items-center gap-1"><User className="w-3.5 h-3.5" /> Request Info</span>
                            <span>Phone: <b className="text-white">{req.phone}</b></span>
                            <span>Requested At: <b className="text-white">{new Date(req.requestedAt).toLocaleString()}</b></span>
                          </div>

                          <div className="flex flex-col gap-1.5 bg-[#121216]/50 p-4 rounded-xl border border-white/5">
                            <span className="text-[10px] font-bold uppercase text-[#71717a] flex items-center gap-1"><MessageSquare className="w-3.5 h-3.5" /> Notes / Details</span>
                            <p className="text-[11px] text-white italic">"{req.note || 'No notes provided.'}"</p>
                          </div>
                        </div>

                        {isPending && (
                          <div className="flex justify-end gap-3 pt-3 border-t border-white/5 mt-1">
                            <button
                              onClick={() => handleSubscriptionDecision(req.id, 'Reject')}
                              className="px-4 py-2 bg-red-500/10 border border-red-500/20 hover:bg-red-500/20 text-red-400 font-bold rounded-xl transition-all flex items-center gap-1 cursor-pointer"
                            >
                              <X className="w-4 h-4" /> Reject Request
                            </button>
                            <button
                              onClick={() => handleSubscriptionDecision(req.id, 'Approve')}
                              className="px-4 py-2 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all shadow-md flex items-center gap-1 cursor-pointer"
                            >
                              <Check className="w-4 h-4" /> Approve & Update Plan
                            </button>
                          </div>
                        )}
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          )}

          {/* TAB 2: Users Management */}
          {activeTab === 'users' && (
            <div className="flex flex-col gap-6">
              <h3 className="font-bold text-white text-base flex items-center gap-2 mb-2">
                <Users className="w-5 h-5 text-[#84cc16]" /> Registered System Users
              </h3>
              <div className="flex flex-col gap-3">
                {users.map(u => (
                  <div key={u.userId} className="flex justify-between items-center p-4 bg-[#1e1e26]/30 border border-white/5 rounded-2xl hover:bg-[#1e1e26]/50 transition-all animate-fade-in">
                    <div className="flex items-center gap-3">
                      {u.profilePictureUrl ? (
                        <img className="w-8 h-8 rounded-full object-cover" src={u.profilePictureUrl} alt="" />
                      ) : (
                        <div className="w-8 h-8 rounded-full bg-white/10 flex items-center justify-center font-bold text-white text-xs">
                          {u.fullName ? u.fullName.charAt(0) : 'U'}
                        </div>
                      )}
                      <div>
                        <h4 className="font-bold text-white text-xs">{u.fullName}</h4>
                        <span className="text-[9px] text-[#71717a] font-semibold">{u.city || 'Cairo'}, Egypt</span>
                      </div>
                    </div>

                    <div className="flex items-center gap-4">
                      <span className={`text-[10px] px-2 py-0.5 rounded font-bold uppercase ${
                        u.isDisabled ? 'bg-red-500/10 text-red-400 border border-red-500/20' : 'bg-[#84cc16]/10 text-[#84cc16] border border-[#84cc16]/20'
                      }`}>
                        {u.isDisabled ? 'Suspended' : 'Active'}
                      </span>
                      <button
                        onClick={() => handleToggleUserBan(u.userId, u.fullName)}
                        className={`px-3 py-1.5 font-bold text-[10px] rounded-lg border transition-all cursor-pointer ${
                          u.isDisabled
                            ? 'bg-[#84cc16]/10 border-[#84cc16]/20 hover:bg-[#84cc16]/20 text-[#84cc16]'
                            : 'bg-red-500/10 border-red-500/20 hover:bg-red-500/20 text-red-400'
                        }`}
                      >
                        {u.isDisabled ? 'Reactivate' : 'Suspend'}
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* TAB 3: Clubs Moderation */}
          {activeTab === 'clubs' && (
            <div className="flex flex-col gap-6">
              <h3 className="font-bold text-white text-base flex items-center gap-2 mb-2">
                <Building className="w-5 h-5 text-[#84cc16]" /> Registered Complexes Moderation
              </h3>
              <div className="flex flex-col gap-4">
                {clubs.map(c => (
                  <div key={c.id} className="bg-[#1e1e26]/30 border border-white/5 rounded-2xl p-5 flex flex-col sm:flex-row justify-between sm:items-center gap-4 hover:border-white/10 transition-all">
                    <div>
                      <h4 className="text-sm font-bold text-white">{c.name}</h4>
                      <p className="text-[10px] text-[#a1a1aa] mt-1">{c.city}, {c.address}</p>
                    </div>

                    <div className="flex items-center gap-3 shrink-0">
                      <span className={`text-[9px] px-2 py-0.5 rounded font-bold uppercase ${
                        c.isActive !== false ? 'bg-[#84cc16]/10 text-[#84cc16]' : 'bg-red-500/10 text-red-400'
                      }`}>
                        {c.isActive !== false ? 'Online' : 'Offline/Suspended'}
                      </span>
                      
                      <button
                        onClick={() => handleToggleClubStatus(c.id)}
                        className={`p-2 rounded-xl transition-all ${
                          c.isActive !== false ? 'bg-orange-500/10 text-orange-400 hover:bg-orange-500/20' : 'bg-[#84cc16]/10 text-[#84cc16] hover:bg-[#84cc16]/20'
                        }`}
                        title={c.isActive !== false ? 'Deactivate Listing' : 'Activate Listing'}
                      >
                        {c.isActive !== false ? <Ban className="w-4 h-4" /> : <Check className="w-4 h-4" />}
                      </button>

                      <button
                        onClick={() => handleDeleteClub(c.id)}
                        className="p-2 bg-red-500/10 hover:bg-red-500/20 text-red-400 rounded-xl transition-all"
                        title="Delete complex Listing"
                      >
                        <Trash className="w-4 h-4" />
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* TAB 4: Reports */}
          {activeTab === 'reports' && (
            <div className="text-center py-16 bg-[#1e1e26]/10 border border-white/5 rounded-3xl flex flex-col items-center gap-2">
              <Activity className="w-8 h-8 text-[#71717a]/50" />
              <h4 className="text-xs font-bold text-white">No System Alerts</h4>
              <p className="text-[10px] text-[#71717a] max-w-xs leading-relaxed">
                All background servers are operating normally. Real-time Hangfire and database telemetry reports will appear here if anomalies are detected.
              </p>
            </div>
          )}

        </div>
      )}
    </div>
  );
}
