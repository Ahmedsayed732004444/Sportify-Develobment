import React, { useState } from 'react';
import { Shield, User, Clock, Camera, Lock, Bell, Terminal, Eye, Award, Server } from 'lucide-react';

export default function AdminProfileView({ user, addToast }) {
  const [activeSubTab, setActiveSubTab] = useState('identity'); // identity, activity, security, settings

  // Security password state
  const [passwordState, setPasswordState] = useState({
    current: '',
    newPass: '',
    confirm: ''
  });

  // Settings preferences
  const [preferences, setPreferences] = useState({
    serverAlerts: true,
    weeklyReportLogs: true,
    theme: 'Dark'
  });

  // Simulated activity log for the admin
  const [activities] = useState([
    { id: 1, action: 'Approved Complex "Padel Zone"', time: 'Today, 2:14 PM', category: 'Approvals' },
    { id: 2, action: 'Banned User "Sayed Kotb"', time: 'Yesterday, 11:30 AM', category: 'Security' },
    { id: 3, action: 'Updated subscription limits for "Pro Complex"', time: '2 days ago', category: 'Billing' },
    { id: 4, action: 'Audited platform latency indexes', time: '3 days ago', category: 'System' }
  ]);

  const handlePasswordSubmit = (e) => {
    e.preventDefault();
    if (passwordState.newPass !== passwordState.confirm) {
      addToast('New password confirmation does not match.', 'error');
      return;
    }
    addToast('Security credentials updated successfully!', 'success');
    setPasswordState({ current: '', newPass: '', confirm: '' });
  };

  const handlePrefsSubmit = (e) => {
    e.preventDefault();
    addToast('Preferences and notifications saved.', 'success');
  };

  return (
    <div className="max-w-[850px] mx-auto flex flex-col gap-8 animate-fade-in text-xs text-[#a1a1aa]">
      {/* Cover Header */}
      <div className="bg-[#121216]/50 border border-white/5 rounded-3xl overflow-hidden shadow-xl relative">
        <div className="h-44 bg-gradient-to-r from-red-500/10 via-[#84cc16]/5 to-transparent relative">
          <div className="absolute top-4 right-4 flex gap-2">
            <span className="bg-red-500/10 border border-red-500/30 text-red-400 px-3.5 py-1.5 rounded-xl font-bold uppercase tracking-wider text-[9px] flex items-center gap-1">
              <Shield className="w-3.5 h-3.5" /> Root System Security
            </span>
          </div>
          <div className="absolute -bottom-10 left-8">
            <div className="relative group">
              <div className="w-24 h-24 rounded-2xl border-4 border-[#0c0c0f] bg-red-500/10 flex items-center justify-center text-red-400 font-bold text-3xl">
                {user?.name?.charAt(0) || 'A'}
              </div>
              <button className="absolute inset-0 bg-black/60 rounded-2xl opacity-0 group-hover:opacity-100 flex items-center justify-center text-white transition-opacity">
                <Camera className="w-5 h-5" />
              </button>
            </div>
          </div>
        </div>

        <div className="pt-14 pb-6 px-8 flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
          <div>
            <h2 className="text-xl font-bold text-white flex items-center gap-2">
              System Administrator
            </h2>
            <p className="text-xs text-[#a1a1aa] mt-1">Full architectural console control over Sportify platform</p>
          </div>
          <div className="flex flex-col gap-1 items-end">
            <span className="text-[10px] text-[#71717a] font-bold uppercase">Admin Console</span>
            <span className="text-xs text-red-400 font-extrabold">System Operator Core</span>
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex gap-2 border-b border-white/5 pb-2 overflow-x-auto">
        <button onClick={() => setActiveSubTab('identity')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'identity' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          Identity & Role
        </button>
        <button onClick={() => setActiveSubTab('activity')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'activity' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          Admin Activity Log
        </button>
        <button onClick={() => setActiveSubTab('security')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'security' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          Security Settings
        </button>
        <button onClick={() => setActiveSubTab('settings')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'settings' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          Account Preferences
        </button>
      </div>

      {/* Content panel */}
      <div className="bg-[#121216]/50 border border-white/5 p-6 md:p-8 rounded-3xl shadow-2xl">
        
        {/* PANEL 1: Identity & Credentials */}
        {activeSubTab === 'identity' && (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8 animate-fade-in">
            {/* Contact Info */}
            <div className="flex flex-col gap-4 bg-[#1e1e26]/10 border border-white/5 p-6 rounded-2xl">
              <h3 className="text-sm font-bold text-white flex items-center gap-2">
                <Terminal className="w-4 h-4 text-red-400" /> Administrative Identity
              </h3>
              <div className="flex flex-col gap-3 text-xs">
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Admin Name</span>
                  <span className="text-white font-semibold">{user?.name || 'Root Administrator'}</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">System Email</span>
                  <span className="text-white font-semibold">{user?.email || 'admin@sportify.com'}</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Access Scope</span>
                  <span className="text-white font-semibold">Full Platform Domain Control</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Verification Badge</span>
                  <span className="text-red-400 font-bold uppercase tracking-wider text-[9px]">Root Operator</span>
                </div>
              </div>
            </div>

            {/* Platform credentials */}
            <div className="flex flex-col gap-4 bg-[#1e1e26]/10 border border-white/5 p-6 rounded-2xl">
              <h3 className="text-sm font-bold text-white flex items-center gap-2">
                <Server className="w-4 h-4 text-red-400" /> Platform Access Parameters
              </h3>
              <div className="flex flex-col gap-3 text-xs">
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Access Level</span>
                  <span className="text-white font-semibold">Super Administrator (Level 3)</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Two-Factor Authorization</span>
                  <span className="text-[#84cc16] font-bold">ENABLED</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Session Expiration</span>
                  <span className="text-white font-semibold">12 Hours</span>
                </div>
                <div className="flex justify-between border-b border-white/5 pb-2">
                  <span className="text-[#71717a]">Last Login Session</span>
                  <span className="text-white font-semibold">{new Date().toLocaleString()}</span>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* PANEL 2: Activity Audit Log */}
        {activeSubTab === 'activity' && (
          <div className="flex flex-col gap-4 animate-fade-in">
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-sm font-bold text-white flex items-center gap-2">
                <Clock className="w-4.5 h-4.5 text-red-400" /> Platform Activity Logs
              </h3>
              <span className="text-[10px] text-[#71717a] font-bold uppercase">System Operator Audit</span>
            </div>
            
            <div className="flex flex-col gap-3">
              {activities.map(act => (
                <div key={act.id} className="bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl flex justify-between items-center hover:border-white/10 transition-colors">
                  <div className="flex flex-col gap-1">
                    <span className="text-white font-semibold">{act.action}</span>
                    <span className="text-[10px] text-[#71717a]">{act.time}</span>
                  </div>
                  <span className="px-2.5 py-1 bg-red-500/10 border border-red-500/20 text-red-400 font-bold uppercase text-[9px] rounded">
                    {act.category}
                  </span>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* PANEL 3: Password settings */}
        {activeSubTab === 'security' && (
          <form onSubmit={handlePasswordSubmit} className="flex flex-col gap-5 max-w-[450px] mx-auto animate-fade-in py-6">
            <h3 className="font-bold text-white text-sm flex items-center gap-2 mb-2">
              <Lock className="w-4.5 h-4.5 text-red-400" /> Password Configurations
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

        {/* PANEL 4: Account Preferences */}
        {activeSubTab === 'settings' && (
          <form onSubmit={handlePrefsSubmit} className="flex flex-col gap-6 animate-fade-in">
            {/* Preferences */}
            <div className="flex flex-col gap-4">
              <h3 className="font-bold text-white text-sm flex items-center gap-2">
                <Bell className="w-4.5 h-4.5 text-red-400" /> Notification Settings
              </h3>
              <div className="flex flex-col gap-3">
                <label className="flex items-center gap-3 bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer hover:border-white/10 transition-colors">
                  <input type="checkbox" checked={preferences.serverAlerts} onChange={() => setPreferences({...preferences, serverAlerts: !preferences.serverAlerts})} className="w-4 h-4 accent-[#84cc16]" />
                  <div>
                    <h5 className="font-bold text-white text-xs">Real-Time Server Alerts</h5>
                    <span className="text-[#71717a] text-[10px]">Notify me immediately via push notification for any system exceptions or latency spikes.</span>
                  </div>
                </label>
                <label className="flex items-center gap-3 bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer hover:border-white/10 transition-colors">
                  <input type="checkbox" checked={preferences.weeklyReportLogs} onChange={() => setPreferences({...preferences, weeklyReportLogs: !preferences.weeklyReportLogs})} className="w-4 h-4 accent-[#84cc16]" />
                  <div>
                    <h5 className="font-bold text-white text-xs">Weekly Operations Report</h5>
                    <span className="text-[#71717a] text-[10px]">Send email summaries of platform bookings, revenue averages, and membership upgrades.</span>
                  </div>
                </label>
              </div>
            </div>

            {/* Account Preferences */}
            <div className="flex flex-col gap-4 border-t border-white/5 pt-6">
              <h3 className="font-bold text-white text-sm flex items-center gap-2">
                <Eye className="w-4.5 h-4.5 text-red-400" /> Account Preferences
              </h3>
              <div className="flex flex-col gap-3">
                <div className="flex items-center justify-between bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl">
                  <div>
                    <h5 className="font-bold text-white text-xs">System Theme Preferences</h5>
                    <span className="text-[#71717a] text-[10px]">Toggle color theme parameters of admin control panels.</span>
                  </div>
                  <select value={preferences.theme} onChange={(e) => setPreferences({...preferences, theme: e.target.value})} className="bg-[#121216]/50 border border-white/5 rounded-lg px-3 py-1.5 text-white outline-none cursor-pointer">
                    <option value="Dark">Dark Mode (Default)</option>
                    <option value="Light">Light Mode</option>
                  </select>
                </div>
              </div>
            </div>

            <button type="submit" className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all cursor-pointer">Save Settings</button>
          </form>
        )}

      </div>
    </div>
  );
}
