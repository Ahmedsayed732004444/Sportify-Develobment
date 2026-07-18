import React, { useState, useEffect } from 'react';
import { useOutletContext } from 'react-router-dom';
import { apiFetch } from '../../services/api';
import { User, Shield, Building, Award, Phone, Mail, Clock, Camera, Lock, Bell, AlertCircle } from 'lucide-react';

export default function OwnerProfileView({ user, addToast }) {
  const { selectedClub } = useOutletContext();
  const [activeSubTab, setActiveSubTab] = useState('identity'); // identity, settings, security, notifications
  
  const [activeSub, setActiveSub] = useState(null);
  const [loading, setLoading] = useState(false);

  // Editable Profile credentials
  const [repInfo, setRepInfo] = useState({
    name: user?.name || 'Partner Owner',
    phone: '',
    email: '',
    desc: 'Premium sports complex hosting Football and Padel matches.'
  });

  // Security password state
  const [passwordState, setPasswordState] = useState({
    current: '',
    newPass: '',
    confirm: ''
  });

  // Notification toggles
  const [notifications, setNotifications] = useState({
    bookingAlerts: true,
    subscriptionAlerts: true,
    reviewAlerts: false
  });

  useEffect(() => {
    if (selectedClub) {
      loadProfileDetails();
      setRepInfo(prev => ({
        ...prev,
        phone: selectedClub.phoneNumber || '',
        email: selectedClub.email || '',
        desc: selectedClub.description || prev.desc
      }));
    }
  }, [selectedClub]);

  const loadProfileDetails = async () => {
    setLoading(true);
    try {
      const subRes = await apiFetch(`/clubs/${selectedClub.id}/subscriptions/active`);
      if (subRes.ok) {
        const subData = await subRes.json();
        setActiveSub(subData);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  const handleSettingsSubmit = (e) => {
    e.preventDefault();
    addToast('Business settings saved successfully!', 'success');
  };

  const handlePasswordSubmit = (e) => {
    e.preventDefault();
    if (passwordState.newPass !== passwordState.confirm) {
      addToast('New password confirmation does not match.', 'error');
      return;
    }
    addToast('Security credentials updated successfully!', 'success');
    setPasswordState({ current: '', newPass: '', confirm: '' });
  };

  const handleNotifSubmit = (e) => {
    e.preventDefault();
    addToast('Notification preferences updated.', 'success');
  };

  if (!selectedClub) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-6 bg-[#121216]/40 border border-white/5 rounded-2xl shadow-lg">
        <AlertCircle className="w-12 h-12 text-[#a1a1aa] mb-4" />
        <h3 className="text-lg font-bold text-white mb-2">No active venue selected</h3>
        <p className="text-xs text-[#a1a1aa] max-w-sm">Please register or select a club from the sidebar to inspect business statistics.</p>
      </div>
    );
  }

  return (
    <div className="max-w-[850px] mx-auto flex flex-col gap-8 animate-fade-in text-xs text-[#a1a1aa]">
      {/* Cover & Business Identity */}
      <div className="bg-[#121216]/50 border border-white/5 rounded-3xl overflow-hidden shadow-xl relative">
        <div className="h-44 bg-gradient-to-r from-[#84cc16]/20 via-[#65a30d]/5 to-transparent relative">
          <div className="absolute top-4 right-4 flex gap-2">
            <span className="bg-[#84cc16]/10 border border-[#84cc16]/30 text-[#84cc16] px-3.5 py-1.5 rounded-xl font-bold uppercase tracking-wider text-[9px] flex items-center gap-1">
              <Award className="w-3.5 h-3.5" /> Verified Business Partner
            </span>
          </div>
          <div className="absolute -bottom-10 left-8">
            <div className="relative group">
              {selectedClub.logoUrl ? (
                <img className="w-24 h-24 rounded-2xl border-4 border-[#0c0c0f] object-cover bg-black" src={selectedClub.logoUrl} alt="club logo" />
              ) : (
                <div className="w-24 h-24 rounded-2xl border-4 border-[#0c0c0f] bg-[#84cc16]/10 flex items-center justify-center text-[#84cc16] font-bold text-3xl">
                  {selectedClub.name.charAt(0)}
                </div>
              )}
              <button className="absolute inset-0 bg-black/60 rounded-2xl opacity-0 group-hover:opacity-100 flex items-center justify-center text-white transition-opacity">
                <Camera className="w-5 h-5" />
              </button>
            </div>
          </div>
        </div>

        <div className="pt-14 pb-6 px-8 flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
          <div>
            <h2 className="text-xl font-bold text-white flex items-center gap-2">
              {selectedClub.name}
            </h2>
            <p className="text-xs text-[#a1a1aa] mt-1">{selectedClub.city}, {selectedClub.address}</p>
          </div>
          <div className="flex flex-col gap-1 items-end">
            <span className="text-[10px] text-[#71717a] font-bold uppercase">Active Tier</span>
            <span className="text-xs text-[#84cc16] font-extrabold">{activeSub?.planName || 'Sportify Premium Plan'}</span>
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex gap-2 border-b border-white/5 pb-2 overflow-x-auto">
        <button onClick={() => setActiveSubTab('identity')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'identity' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          Business Identity
        </button>
        <button onClick={() => setActiveSubTab('settings')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'settings' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          Account Settings
        </button>
        <button onClick={() => setActiveSubTab('security')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'security' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          Security Settings
        </button>
        <button onClick={() => setActiveSubTab('notifications')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'notifications' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          Notification Preferences
        </button>
      </div>

      {/* Content area */}
      <div className="bg-[#121216]/50 border border-white/5 p-6 md:p-8 rounded-3xl shadow-2xl">
        
        {/* PANEL 1: Business Identity & Info */}
        {activeSubTab === 'identity' && (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8 animate-fade-in">
            {/* Contact Info */}
            <div className="flex flex-col gap-4 bg-[#1e1e26]/10 border border-white/5 p-6 rounded-2xl">
              <h3 className="text-sm font-bold text-white flex items-center gap-2">
                <Phone className="w-4 h-4 text-[#84cc16]" /> Contact Information
              </h3>
              <div className="flex flex-col gap-3 text-xs">
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Telephone Contact</span>
                  <span className="text-white font-semibold">{selectedClub.phoneNumber || 'N/A'}</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Business Email</span>
                  <span className="text-white font-semibold truncate max-w-[180px]">{selectedClub.email || 'N/A'}</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Operating City</span>
                  <span className="text-white font-semibold">{selectedClub.city}</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Legal Representative</span>
                  <span className="text-white font-semibold">{repInfo.name}</span>
                </div>
              </div>
            </div>

            {/* Subscription Info */}
            <div className="flex flex-col gap-4 bg-[#1e1e26]/10 border border-white/5 p-6 rounded-2xl">
              <h3 className="text-sm font-bold text-white flex items-center gap-2">
                <Shield className="w-4 h-4 text-[#84cc16]" /> Subscription Plan Details
              </h3>
              <div className="flex flex-col gap-3 text-xs">
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Current Sub Plan</span>
                  <span className="text-white font-semibold">{activeSub?.planName || 'Premium Partner'}</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Billing Renewal Date</span>
                  <span className="text-white font-semibold">{activeSub?.endDate || 'End of Month'}</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Billing Price Rate</span>
                  <span className="text-[#84cc16] font-bold">EGP {activeSub?.price || 450} / mo</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Verification Badge</span>
                  <span className="text-white font-semibold">Active & Approved</span>
                </div>
              </div>
            </div>

            {/* Business Pitch */}
            <div className="md:col-span-2 flex flex-col gap-3 bg-[#1e1e26]/5 border border-white/5 p-5 rounded-2xl">
              <h4 className="font-bold text-white text-xs">Business Description</h4>
              <p className="leading-relaxed text-[#a1a1aa]">{repInfo.desc}</p>
            </div>
          </div>
        )}

        {/* PANEL 2: Account Settings */}
        {activeSubTab === 'settings' && (
          <form onSubmit={handleSettingsSubmit} className="flex flex-col gap-6 animate-fade-in">
            <h3 className="font-bold text-white text-sm">Account Settings</h3>
            <div className="grid grid-cols-2 gap-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase">Legal Representative Name</label>
                <input type="text" value={repInfo.name} onChange={(e) => setRepInfo({...repInfo, name: e.target.value})} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase">Contact Telephone</label>
                <input type="text" value={repInfo.phone} onChange={(e) => setRepInfo({...repInfo, phone: e.target.value})} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
              </div>
            </div>

            <div className="flex flex-col gap-1.5">
              <label className="text-[10px] font-bold text-[#71717a] uppercase">Business Description</label>
              <textarea value={repInfo.desc} onChange={(e) => setRepInfo({...repInfo, desc: e.target.value})} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl p-4 text-xs text-white outline-none resize-none h-[80px]" />
            </div>

            <button type="submit" className="w-full py-3.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all cursor-pointer">Save Settings</button>
          </form>
        )}

        {/* PANEL 3: Security Settings */}
        {activeSubTab === 'security' && (
          <form onSubmit={handlePasswordSubmit} className="flex flex-col gap-5 max-w-[450px] mx-auto animate-fade-in py-6">
            <h3 className="font-bold text-white text-sm flex items-center gap-2 mb-2">
              <Lock className="w-4.5 h-4.5 text-[#84cc16]" /> Password Configurations
            </h3>
            <div className="flex flex-col gap-1.5">
              <label className="text-[10px] font-bold text-[#71717a] uppercase">Current Password</label>
              <input type="password" value={passwordState.current} onChange={(e) => setPasswordState({...passwordState, current: e.target.value})} required className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-[10px] font-bold text-[#71717a] uppercase">New Password</label>
              <input type="password" value={passwordState.newPass} onChange={(e) => setPasswordState({...passwordState, newPass: e.target.value})} required className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-[10px] font-bold text-[#71717a] uppercase">Confirm New Password</label>
              <input type="password" value={passwordState.confirm} onChange={(e) => setPasswordState({...passwordState, confirm: e.target.value})} required className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
            </div>
            <button type="submit" className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all mt-2 cursor-pointer">Update Passcode</button>
          </form>
        )}

        {/* PANEL 4: Notification Preferences */}
        {activeSubTab === 'notifications' && (
          <form onSubmit={handleNotifSubmit} className="flex flex-col gap-6 animate-fade-in">
            <h3 className="font-bold text-white text-sm flex items-center gap-2">
              <Bell className="w-4.5 h-4.5 text-[#84cc16]" /> Business Alerts Options
            </h3>
            <div className="flex flex-col gap-3">
              <label className="flex items-center gap-3 bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer hover:border-white/10 transition-colors">
                <input type="checkbox" checked={notifications.bookingAlerts} onChange={() => setNotifications({...notifications, bookingAlerts: !notifications.bookingAlerts})} className="w-4 h-4 accent-[#84cc16]" />
                <div>
                  <h5 className="font-bold text-white text-xs">New Reservation Alerts</h5>
                  <span className="text-[#71717a] text-[10px]">Notify legal representative when players confirm new court slots.</span>
                </div>
              </label>
              <label className="flex items-center gap-3 bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer hover:border-white/10 transition-colors">
                <input type="checkbox" checked={notifications.subscriptionAlerts} onChange={() => setNotifications({...notifications, subscriptionAlerts: !notifications.subscriptionAlerts})} className="w-4 h-4 accent-[#84cc16]" />
                <div>
                  <h5 className="font-bold text-white text-xs">Subscription Billing Notices</h5>
                  <span className="text-[#71717a] text-[10px]">Email notices 3 days prior to active pricing tier renewals.</span>
                </div>
              </label>
            </div>

            <button type="submit" className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all cursor-pointer">Save Preferences</button>
          </form>
        )}

      </div>
    </div>
  );
}
