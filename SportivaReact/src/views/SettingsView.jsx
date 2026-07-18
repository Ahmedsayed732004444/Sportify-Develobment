import React, { useState } from 'react';
import { Settings, Lock, Globe, Bell, Shield, Check, Trash, User, AlertCircle } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';

export default function SettingsView({ addToast }) {
  const { language, setLanguage, t } = useLanguage();
  const [activeSection, setActiveSection] = useState('account'); // account, password, preferences, security, danger
  const [emailAlerts, setEmailAlerts] = useState(true);
  const [pushAlerts, setPushAlerts] = useState(false);
  const [profilePrivacy, setProfilePrivacy] = useState('Public');
  const [themeMode, setThemeMode] = useState('Dark');
  const [twoFactor, setTwoFactor] = useState(false);

  const [accountForm, setAccountForm] = useState({
    username: 'Cap_Player',
    email: 'captain@sportify.com'
  });

  const [passwordForm, setPasswordForm] = useState({ oldPassword: '', newPassword: '' });

  const handlePasswordSubmit = (e) => {
    e.preventDefault();
    if (!passwordForm.oldPassword || !passwordForm.newPassword) {
      addToast('Please fill all password fields.', 'error');
      return;
    }
    addToast('Security password successfully updated!', 'success');
    setPasswordForm({ oldPassword: '', newPassword: '' });
  };

  const handleAccountSubmit = (e) => {
    e.preventDefault();
    addToast('Account profile preferences saved!', 'success');
  };

  const handleDeleteAccount = () => {
    const confirmation = window.confirm('DANGER ZONE: Are you sure you want to permanently delete your Sportify account? This will erase all histories and bookings.');
    if (confirmation) {
      addToast('Account deletion request queued.', 'info');
    }
  };

  const sections = [
    { id: 'account', name: 'Account Settings', icon: User },
    { id: 'password', name: 'Change Password', icon: Lock },
    { id: 'preferences', name: 'Preferences & Language', icon: Globe },
    { id: 'security', name: 'Privacy & Security', icon: Shield },
    { id: 'danger', name: 'Danger Zone', icon: Trash }
  ];

  return (
    <div className="max-w-[850px] mx-auto flex flex-col md:flex-row gap-8 animate-fade-in text-xs text-[#a1a1aa] py-4">
      {/* Sidebar Preference Selector */}
      <div className="w-full md:w-[240px] flex flex-col gap-2 shrink-0">
        <h2 className="text-xl font-bold text-white px-3 mb-2 flex items-center gap-2">
          <Settings className="w-5 h-5 text-[#84cc16]" /> Configuration
        </h2>
        {sections.map(sec => {
          const Icon = sec.icon;
          const active = activeSection === sec.id;
          return (
            <button
              key={sec.id}
              onClick={() => setActiveSection(sec.id)}
              className={`flex items-center gap-3.5 px-4 py-3 rounded-xl font-semibold transition-all text-left ${
                active
                  ? 'bg-[#84cc16]/10 text-[#84cc16] border-l-4 border-[#84cc16]'
                  : 'text-[#a1a1aa] hover:bg-white/5 hover:text-white'
              }`}
            >
              <Icon className="w-4 h-4 shrink-0" />
              <span>{sec.name}</span>
            </button>
          );
        })}
      </div>

      {/* Configuration Viewport */}
      <div className="flex-1 bg-[#121216]/50 border border-white/5 rounded-3xl p-6 md:p-8 shadow-2xl flex flex-col gap-6">
        
        {/* Account Details Settings */}
        {activeSection === 'account' && (
          <form onSubmit={handleAccountSubmit} className="flex flex-col gap-6">
            <div>
              <h3 className="text-base font-bold text-white">Account Settings</h3>
              <p className="text-[10px] text-[#71717a] mt-1">Configure representative system username and primary communication email</p>
            </div>

            <div className="flex flex-col gap-4">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase">Username Preference</label>
                <input 
                  type="text" 
                  value={accountForm.username} 
                  onChange={(e) => setAccountForm({...accountForm, username: e.target.value})} 
                  className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" 
                />
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase">Primary Email</label>
                <input 
                  type="email" 
                  value={accountForm.email} 
                  onChange={(e) => setAccountForm({...accountForm, email: e.target.value})} 
                  className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" 
                />
              </div>
            </div>

            <button 
              type="submit" 
              className="w-fit px-5 py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-md cursor-pointer"
            >
              Save Preferences
            </button>
          </form>
        )}

        {/* Change Password Settings */}
        {activeSection === 'password' && (
          <form onSubmit={handlePasswordSubmit} className="flex flex-col gap-6">
            <div>
              <h3 className="text-base font-bold text-white">Change Password</h3>
              <p className="text-[10px] text-[#71717a] mt-1">Reset your login password to ensure security credentials stay protected</p>
            </div>

            <div className="flex flex-col gap-4">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase">Current Password</label>
                <input 
                  type="password" 
                  value={passwordForm.oldPassword} 
                  onChange={(e) => setPasswordForm({...passwordForm, oldPassword: e.target.value})} 
                  className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" 
                />
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase">New Password</label>
                <input 
                  type="password" 
                  value={passwordForm.newPassword} 
                  onChange={(e) => setPasswordForm({...passwordForm, newPassword: e.target.value})} 
                  className="bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" 
                />
              </div>
            </div>

            <button 
              type="submit" 
              className="w-fit px-5 py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all shadow-md cursor-pointer"
            >
              Update Password
            </button>
          </form>
        )}

        {/* Preferences & Language Settings */}
        {activeSection === 'preferences' && (
          <div className="flex flex-col gap-6">
            <div>
              <h3 className="text-base font-bold text-white">Preferences & Language Settings</h3>
              <p className="text-[10px] text-[#71717a] mt-1">Select language overrides and interface system themes</p>
            </div>

            <div className="flex flex-col gap-4">
              <div className="flex items-center justify-between bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl">
                <span>System Language</span>
                <select 
                  value={language} 
                  onChange={(e) => { setLanguage(e.target.value); addToast(`Language updated successfully.`, 'success'); }} 
                  className="bg-[#1e1e26]/50 border border-white/5 rounded-lg px-3 py-1.5 outline-none focus:border-[#84cc16] text-white text-xs cursor-pointer"
                >
                  <option value="en">English</option>
                  <option value="ar">العربية (Arabic)</option>
                </select>
              </div>

              <div className="flex items-center justify-between bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl">
                <span>Theme Mode</span>
                <select 
                  value={themeMode} 
                  onChange={(e) => { setThemeMode(e.target.value); addToast(`Theme set to ${e.target.value}`, 'success'); }} 
                  className="bg-[#1e1e26]/50 border border-white/5 rounded-lg px-3 py-1.5 outline-none focus:border-[#84cc16] text-white text-xs cursor-pointer"
                >
                  <option value="Dark">Sleek Dark Mode</option>
                  <option value="Light">Classic Light Mode</option>
                </select>
              </div>
            </div>
          </div>
        )}

        {/* Privacy & Security Settings */}
        {activeSection === 'security' && (
          <div className="flex flex-col gap-6">
            <div>
              <h3 className="text-base font-bold text-white">Privacy & Security settings</h3>
              <p className="text-[10px] text-[#71717a] mt-1">Manage visibility scores and two-factor system verification options</p>
            </div>

            <div className="flex flex-col gap-4">
              {/* Profile privacy */}
              <div className="flex items-center justify-between bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl">
                <span>Profile visibility status</span>
                <select 
                  value={profilePrivacy} 
                  onChange={(e) => { setProfilePrivacy(e.target.value); addToast(`Privacy changed to ${e.target.value}`, 'info'); }} 
                  className="bg-[#1e1e26]/50 border border-white/5 rounded-lg px-3 py-1.5 outline-none focus:border-[#84cc16] text-white text-xs cursor-pointer"
                >
                  <option value="Public">Public (Anyone can search or see rating)</option>
                  <option value="Followers">Followers Only</option>
                  <option value="Private">Fully Private</option>
                </select>
              </div>

              {/* Alert Toggle */}
              <label className="flex items-center justify-between bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer">
                <span>Receive email alert notifications for match bookings</span>
                <input 
                  type="checkbox" 
                  checked={emailAlerts} 
                  onChange={(e) => { setEmailAlerts(e.target.checked); addToast('Notification alerts updated', 'success'); }} 
                  className="w-4 h-4 accent-[#84cc16]" 
                />
              </label>

              {/* 2FA Toggle */}
              <label className="flex items-center justify-between bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer">
                <span>Enable two-factor SMS authentication on login</span>
                <input 
                  type="checkbox" 
                  checked={twoFactor} 
                  onChange={(e) => { setTwoFactor(e.target.checked); addToast('2FA configurations updated', 'success'); }} 
                  className="w-4 h-4 accent-[#84cc16]" 
                />
              </label>
            </div>
          </div>
        )}

        {/* Danger Zone Settings */}
        {activeSection === 'danger' && (
          <div className="flex flex-col gap-6">
            <div>
              <h3 className="text-base font-bold text-red-400">Danger Zone</h3>
              <p className="text-[10px] text-[#71717a] mt-1">Irreversible administrative actions regarding account deletion</p>
            </div>

            <div className="bg-red-500/5 border border-red-500/10 p-5 rounded-2xl flex gap-3 text-red-400 leading-relaxed">
              <AlertCircle className="w-5 h-5 shrink-0" />
              <div>
                <h4 className="font-bold text-xs">Permanent Account Deletion</h4>
                <p className="mt-1 text-[11px] text-[#a1a1aa] mb-4">Deleting your account removes all active bookings, matches history, and subscription access privileges permanently.</p>
                <button
                  onClick={handleDeleteAccount}
                  className="px-4 py-2.5 bg-red-500 hover:bg-red-650 text-white font-bold rounded-xl transition-all shadow-md cursor-pointer"
                >
                  Delete Account permanently
                </button>
              </div>
            </div>
          </div>
        )}

      </div>
    </div>
  );
}
