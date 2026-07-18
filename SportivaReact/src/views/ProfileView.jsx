import React, { useState, useEffect } from 'react';
import { User, Shield, Star, Camera, CheckCircle2, Award, Calendar, Settings, Lock, Bell, Eye, MessageSquare, MapPin, Activity } from 'lucide-react';
import { apiFetch } from '../services/api';
import { useLanguage } from '../contexts/LanguageContext';

export default function ProfileView({ user, addToast }) {
  const { t } = useLanguage();
  const [activeSubTab, setActiveSubTab] = useState('card'); // card, settings, security, privacy
  
  // Player Sports Identity Metadata
  const [profile, setProfile] = useState({
    firstName: '',
    lastName: '',
    username: '',
    bio: '',
    city: 'Cairo',
    preferredSport: 'Football',
    skillLevel: 'Intermediate',
    sportsmanshipRating: 4.9,
    attendanceRate: 98,
    matchCompletionRate: 100,
    cancellationRate: 2,
    totalMatches: 24,
    totalFriendly: 18,
    totalTournaments: 6
  });

  // Security password state
  const [passwordState, setPasswordState] = useState({
    current: '',
    newPass: '',
    confirm: ''
  });

  // Notification Preferences
  const [notifPreferences, setNotifPreferences] = useState({
    emailInvites: true,
    pushReminders: true,
    matchAlerts: false,
    marketingEmails: false
  });

  // Privacy Settings
  const [privacySettings, setPrivacySettings] = useState({
    publicProfile: true,
    showHistory: true,
    showEmail: false
  });

  useEffect(() => {
    if (user?.id) {
      loadUserProfile();
    }
  }, [user]);

  const loadUserProfile = async () => {
    try {
      const res = await apiFetch(`/profiles/${user.id}`);
      if (res.ok) {
        const data = await res.json();
        
        let sportText = 'Football';
        if (data.preferredSport === 1) sportText = 'Padel';
        if (data.preferredSport === 2) sportText = 'Tennis';
        if (data.preferredSport === 3) sportText = 'Basketball';

        setProfile(prev => ({
          ...prev,
          firstName: data.firstName || '',
          lastName: data.lastName || '',
          username: user.userName || user.email?.split('@')[0] || 'captain_player',
          bio: data.bio || 'Sportify athlete and community captain.',
          city: data.city || 'Cairo',
          preferredSport: sportText,
          preferredCity: data.preferredCity || '',
          totalMatches: data.totalMatches || 0,
          totalBookings: data.totalBookings || 0,
          totalTournaments: data.totalTournaments || 0,
          attendanceRate: data.attendanceRate ?? 98,
          cancellationRate: data.cancellationRate ?? 2
        }));
      }
    } catch (e) {
      console.error(e);
    }
  };

  const handleProfileSubmit = async (e) => {
    e.preventDefault();
    try {
      let sportVal = 0;
      if (profile.preferredSport === 'Padel') sportVal = 1;
      if (profile.preferredSport === 'Tennis') sportVal = 2;
      if (profile.preferredSport === 'Basketball') sportVal = 3;

      const payload = {
        FirstName: profile.firstName,
        LastName: profile.lastName,
        Bio: profile.bio,
        City: profile.city,
        Country: 'Egypt',
        PreferredSport: sportVal,
        PreferredCity: profile.city
      };

      const res = await apiFetch('/profiles/me/info', {
        method: 'PUT',
        body: JSON.stringify(payload)
      });

      if (!res.ok) throw new Error();
      addToast('Profile info updated successfully!', 'success');
      loadUserProfile();
    } catch (err) {
      addToast('Failed to save profile modifications.', 'error');
    }
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

  const handlePrefsSubmit = (e) => {
    e.preventDefault();
    addToast('Preferences and privacy options saved.', 'success');
  };

  return (
    <div className="max-w-[900px] mx-auto flex flex-col gap-8 animate-fade-in text-xs text-[#a1a1aa]">
      {/* Cover & Identity Header */}
      <div className="bg-[#121216]/50 border border-white/5 rounded-3xl overflow-hidden shadow-xl relative">
        <div className="h-44 bg-gradient-to-r from-[#84cc16]/20 via-[#65a30d]/5 to-transparent relative">
          <div className="absolute top-4 right-4 flex gap-2">
            <span className="bg-[#84cc16]/10 border border-[#84cc16]/30 text-[#84cc16] px-3.5 py-1.5 rounded-xl font-bold uppercase tracking-wider text-[9px] flex items-center gap-1.5">
              <Shield className="w-3.5 h-3.5" /> {t('verifiedCaptain')}
            </span>
          </div>
          <div className="absolute -bottom-10 left-8">
            <div className="w-24 h-24 rounded-2xl border-4 border-[#0c0c0f] bg-[#84cc16]/10 flex items-center justify-center text-[#84cc16] font-bold text-3xl relative group">
              {profile.firstName ? profile.firstName.charAt(0) : user?.name?.charAt(0) || 'P'}
              <button className="absolute inset-0 bg-black/60 rounded-xl opacity-0 group-hover:opacity-100 flex items-center justify-center text-white transition-opacity">
                <Camera className="w-5 h-5" />
              </button>
            </div>
          </div>
        </div>

        <div className="pt-14 pb-6 px-8 flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
          <div>
            <h2 className="text-xl font-bold text-white flex items-center gap-2">
              {profile.firstName} {profile.lastName}
              <CheckCircle2 className="w-5 h-5 text-[#84cc16] fill-[#84cc16]/10" />
            </h2>
            <p className="text-xs text-[#71717a] font-mono mt-0.5">@{profile.username}</p>
            <p className="text-xs text-[#a1a1aa] mt-2 max-w-lg leading-relaxed">{profile.bio}</p>
          </div>
          <div className="flex gap-4 text-xs text-[#71717a] border-t border-white/5 md:border-t-0 pt-4 md:pt-0">
            <div className="flex flex-col items-center">
              <span className="text-white font-extrabold text-lg">{profile.totalMatches}</span>
              <span>{t('matches')}</span>
            </div>
            <div className="flex flex-col items-center border-x border-white/5 px-4">
              <span className="text-[#84cc16] font-extrabold text-lg">{profile.sportsmanshipRating}</span>
              <span>{t('sportsmanship')}</span>
            </div>
            <div className="flex flex-col items-center">
              <span className="text-white font-extrabold text-lg">{profile.city}</span>
              <span>{t('cityLabel')}</span>
            </div>
          </div>
        </div>
      </div>

      {/* Tabs Menu */}
      <div className="flex gap-2 border-b border-white/5 pb-2 overflow-x-auto">
        <button onClick={() => setActiveSubTab('card')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'card' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          {t('identityReputation')}
        </button>
        <button onClick={() => setActiveSubTab('settings')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'settings' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          {t('accountSettings')}
        </button>
        <button onClick={() => setActiveSubTab('security')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'security' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          {t('passwordSecurity')}
        </button>
        <button onClick={() => setActiveSubTab('privacy')} className={`px-4 py-2.5 rounded-xl font-bold transition-all whitespace-nowrap cursor-pointer ${activeSubTab === 'privacy' ? 'bg-[#84cc16]/15 border border-[#84cc16]/20 text-[#84cc16]' : 'hover:bg-white/5 text-[#a1a1aa]'}`}>
          {t('privacyNotifications')}
        </button>
      </div>

      {/* Tab Panels */}
      <div className="bg-[#121216]/50 border border-white/5 rounded-3xl p-6 md:p-8 shadow-2xl">
        
        {/* PANEL 1: Card & Reputation */}
        {activeSubTab === 'card' && (
          <div className="flex flex-col gap-8 animate-fade-in">
            {/* Reputation Scorecard Grid */}
            <div>
              <h3 className="text-sm font-bold text-white mb-4 flex items-center gap-2 rtl:justify-start">
                <Activity className="w-4 h-4 text-[#84cc16]" /> {t('identityReputation')}
              </h3>
              <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 text-left rtl:text-right">
                <div className="bg-[#1e1e26]/30 border border-white/5 p-4 rounded-xl flex flex-col">
                  <span className="text-[#71717a] font-bold text-[9px] uppercase tracking-wider">{t('attendanceRate')}</span>
                  <span className="text-xl font-black text-white mt-1">{profile.attendanceRate}%</span>
                </div>
                <div className="bg-[#1e1e26]/30 border border-white/5 p-4 rounded-xl flex flex-col">
                  <span className="text-[#71717a] font-bold text-[9px] uppercase tracking-wider">{t('completionRate')}</span>
                  <span className="text-xl font-black text-white mt-1">{profile.matchCompletionRate}%</span>
                </div>
                <div className="bg-[#1e1e26]/30 border border-white/5 p-4 rounded-xl flex flex-col">
                  <span className="text-[#71717a] font-bold text-[9px] uppercase tracking-wider">{t('cancellationRate')}</span>
                  <span className="text-xl font-black text-red-400 mt-1">{profile.cancellationRate}%</span>
                </div>
                <div className="bg-[#1e1e26]/30 border border-white/5 p-4 rounded-xl flex flex-col">
                  <span className="text-[#71717a] font-bold text-[9px] uppercase tracking-wider">{t('sportsSkillLevel')}</span>
                  <span className="text-xl font-black text-[#84cc16] mt-1">{t(profile.skillLevel) || profile.skillLevel}</span>
                </div>
              </div>
            </div>

            {/* Statistics details */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 text-left rtl:text-right">
              <div className="bg-[#1e1e26]/20 border border-white/5 p-4 rounded-2xl flex flex-col justify-center">
                <span className="text-[#71717a] font-bold text-[9px] uppercase">{t('friendlyLobbies')}</span>
                <span className="text-lg font-bold text-white mt-0.5">{profile.totalFriendly} {t('matches')}</span>
              </div>
              <div className="bg-[#1e1e26]/20 border border-white/5 p-4 rounded-2xl flex flex-col justify-center">
                <span className="text-[#71717a] font-bold text-[9px] uppercase">{t('tournamentRoster')}</span>
                <span className="text-lg font-bold text-white mt-0.5">{profile.totalTournaments} {t('tournaments')}</span>
              </div>
              <div className="bg-[#1e1e26]/20 border border-white/5 p-4 rounded-2xl flex flex-col justify-center">
                <span className="text-[#71717a] font-bold text-[9px] uppercase">{t('preferredSport')}</span>
                <span className="text-lg font-bold text-white mt-0.5">{t(profile.preferredSport) || profile.preferredSport}</span>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mt-2 text-left rtl:text-right">
              {/* Badges & Achievements */}
              <div className="flex flex-col gap-4">
                <h4 className="font-bold text-white text-xs flex items-center gap-2">
                  <Award className="w-4 h-4 text-[#84cc16]" /> {t('earnedAchievements')}
                </h4>
                <div className="grid grid-cols-2 gap-3 text-[10px]">
                  <div className="flex items-center gap-2.5 bg-[#1e1e26]/20 border border-white/5 p-3 rounded-xl">
                    <span className="text-xl">🏆</span>
                    <div>
                      <h5 className="font-bold text-white">Roster Winner</h5>
                      <span className="text-[#71717a]">Win a tournament</span>
                    </div>
                  </div>
                  <div className="flex items-center gap-2.5 bg-[#1e1e26]/20 border border-white/5 p-3 rounded-xl">
                    <span className="text-xl">⚽</span>
                    <div>
                      <h5 className="font-bold text-white">Top Goalscorer</h5>
                      <span className="text-[#71717a]">Score 5+ goals</span>
                    </div>
                  </div>
                  <div className="flex items-center gap-2.5 bg-[#1e1e26]/20 border border-white/5 p-3 rounded-xl">
                    <span className="text-xl">🤝</span>
                    <div>
                      <h5 className="font-bold text-white">Fair Player</h5>
                      <span className="text-[#71717a]">5 clean sheets in a row</span>
                    </div>
                  </div>
                  <div className="flex items-center gap-2.5 bg-[#1e1e26]/20 border border-white/5 p-3 rounded-xl">
                    <span className="text-xl">📅</span>
                    <div>
                      <h5 className="font-bold text-white">Punctual Captain</h5>
                      <span className="text-[#71717a]">100% attendance rate</span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Player Reviews */}
              <div className="flex flex-col gap-4">
                <h4 className="font-bold text-white text-xs flex items-center gap-2">
                  <MessageSquare className="w-4 h-4 text-[#84cc16]" /> {t('captainReviews')}
                </h4>
                <div className="flex flex-col gap-3">
                  <div className="bg-[#1e1e26]/10 border border-white/5 p-3 rounded-xl leading-relaxed">
                    <div className="flex justify-between items-center mb-1">
                      <span className="font-bold text-white">Kareem H.</span>
                      <span className="flex items-center gap-0.5 text-amber-400 font-extrabold text-[9px]"><Star className="w-3 h-3 fill-amber-400" /> 5.0</span>
                    </div>
                    "Highly punctual player, very cooperative, and shows great sportsmanship on padel courts."
                  </div>
                  <div className="bg-[#1e1e26]/10 border border-white/5 p-3 rounded-xl leading-relaxed">
                    <div className="flex justify-between items-center mb-1">
                      <span className="font-bold text-white">Omar S.</span>
                      <span className="flex items-center gap-0.5 text-amber-400 font-extrabold text-[9px]"><Star className="w-3 h-3 fill-amber-400" /> 5.0</span>
                    </div>
                    "Excellent midfield teammate. Friendly attitude and very punctual with reservations."
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* PANEL 2: Account Settings */}
        {activeSubTab === 'settings' && (
          <form onSubmit={handleProfileSubmit} className="flex flex-col gap-6 animate-fade-in text-left rtl:text-right">
            <h3 className="font-bold text-white text-sm">{t('modifyIdentity')}</h3>
            <div className="grid grid-cols-2 gap-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">{t('firstName')}</label>
                <input type="text" value={profile.firstName} onChange={(e) => setProfile({...profile, firstName: e.target.value})} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">{t('lastName')}</label>
                <input type="text" value={profile.lastName} onChange={(e) => setProfile({...profile, lastName: e.target.value})} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">{t('usernameHandle')}</label>
                <input type="text" value={profile.username} onChange={(e) => setProfile({...profile, username: e.target.value})} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">{t('cityLocation')}</label>
                <input type="text" value={profile.city} onChange={(e) => setProfile({...profile, city: e.target.value})} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">{t('favoriteSports')}</label>
                <select value={profile.preferredSport} onChange={(e) => setProfile({...profile, preferredSport: e.target.value})} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16] cursor-pointer">
                  <option value="Football">{t('Football') || 'Football'}</option>
                  <option value="Padel">{t('Padel') || 'Padel'}</option>
                  <option value="Tennis">{t('Tennis') || 'Tennis'}</option>
                  <option value="Basketball">{t('Basketball') || 'Basketball'}</option>
                </select>
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">{t('skillClassification')}</label>
                <select value={profile.skillLevel} onChange={(e) => setProfile({...profile, skillLevel: e.target.value})} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16] cursor-pointer">
                  <option value="Beginner">{t('Beginner') || 'Beginner'}</option>
                  <option value="Intermediate">{t('Intermediate') || 'Intermediate'}</option>
                  <option value="Advanced">{t('Advanced') || 'Advanced'}</option>
                  <option value="Semi-Pro">{t('Semi-Pro') || 'Semi-Pro'}</option>
                  <option value="Pro">{t('Pro') || 'Pro'}</option>
                </select>
              </div>
            </div>

            <div className="flex flex-col gap-1.5">
              <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">{t('athleticBioSummary')}</label>
              <textarea value={profile.bio} onChange={(e) => setProfile({...profile, bio: e.target.value})} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl p-4 text-xs text-white outline-none resize-none h-[80px]" placeholder="Add athletic summary..." />
            </div>

            <button type="submit" className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all cursor-pointer">{t('saveChanges')}</button>
          </form>
        )}

        {/* PANEL 3: Password Settings */}
        {activeSubTab === 'security' && (
          <form onSubmit={handlePasswordSubmit} className="flex flex-col gap-5 max-w-[450px] mx-auto animate-fade-in py-6">
            <h3 className="font-bold text-white text-sm flex items-center gap-2 mb-2">
              <Lock className="w-4.5 h-4.5 text-[#84cc16]" /> Password Configurations
            </h3>
            <div className="flex flex-col gap-1.5">
              <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">Current Password</label>
              <input type="password" value={passwordState.current} onChange={(e) => setPasswordState({...passwordState, current: e.target.value})} required className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">New Password</label>
              <input type="password" value={passwordState.newPass} onChange={(e) => setPasswordState({...passwordState, newPass: e.target.value})} required className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">Confirm New Password</label>
              <input type="password" value={passwordState.confirm} onChange={(e) => setPasswordState({...passwordState, confirm: e.target.value})} required className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 text-xs text-white outline-none focus:border-[#84cc16]" />
            </div>
            <button type="submit" className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all mt-2 cursor-pointer">Update Passcode</button>
          </form>
        )}

        {/* PANEL 4: Privacy & Notifications */}
        {activeSubTab === 'privacy' && (
          <form onSubmit={handlePrefsSubmit} className="flex flex-col gap-8 animate-fade-in">
            {/* Notification preferences */}
            <div className="flex flex-col gap-4">
              <h3 className="font-bold text-white text-sm flex items-center gap-2">
                <Bell className="w-4.5 h-4.5 text-[#84cc16]" /> Notification Preferences
              </h3>
              <div className="flex flex-col gap-3">
                <label className="flex items-center gap-3 bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer hover:border-white/10 transition-colors">
                  <input type="checkbox" checked={notifPreferences.emailInvites} onChange={() => setNotifPreferences({...notifPreferences, emailInvites: !notifPreferences.emailInvites})} className="w-4 h-4 accent-[#84cc16]" />
                  <div>
                    <h5 className="font-bold text-white text-xs">Email Invitations</h5>
                    <span className="text-[#71717a] text-[10px]">Receive emails when captains invite you to friendly match lobbies.</span>
                  </div>
                </label>
                <label className="flex items-center gap-3 bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer hover:border-white/10 transition-colors">
                  <input type="checkbox" checked={notifPreferences.pushReminders} onChange={() => setNotifPreferences({...notifPreferences, pushReminders: !notifPreferences.pushReminders})} className="w-4 h-4 accent-[#84cc16]" />
                  <div>
                    <h5 className="font-bold text-white text-xs">Push Reminders</h5>
                    <span className="text-[#71717a] text-[10px]">Send notifications on booking statuses and schedule changes.</span>
                  </div>
                </label>
                <label className="flex items-center gap-3 bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer hover:border-white/10 transition-colors">
                  <input type="checkbox" checked={notifPreferences.matchAlerts} onChange={() => setNotifPreferences({...notifPreferences, matchAlerts: !notifPreferences.matchAlerts})} className="w-4 h-4 accent-[#84cc16]" />
                  <div>
                    <h5 className="font-bold text-white text-xs">Match Invites</h5>
                    <span className="text-[#71717a] text-[10px]">Notify me when public matches corresponding to my favorite sport are created nearby.</span>
                  </div>
                </label>
              </div>
            </div>

            {/* Privacy settings */}
            <div className="flex flex-col gap-4 border-t border-white/5 pt-6">
              <h3 className="font-bold text-white text-sm flex items-center gap-2">
                <Eye className="w-4.5 h-4.5 text-[#84cc16]" /> Profile Privacy Parameters
              </h3>
              <div className="flex flex-col gap-3">
                <label className="flex items-center justify-between bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer hover:border-white/10 transition-colors">
                  <div>
                    <h5 className="font-bold text-white text-xs">Public Search Directory</h5>
                    <span className="text-[#71717a] text-[10px]">Allow captains and complexes to search my profile handle.</span>
                  </div>
                  <input type="checkbox" checked={privacySettings.publicProfile} onChange={() => setPrivacySettings({...privacySettings, publicProfile: !privacySettings.publicProfile})} className="w-4 h-4 accent-[#84cc16]" />
                </label>
                <label className="flex items-center justify-between bg-[#1e1e26]/20 border border-white/5 p-4 rounded-xl cursor-pointer hover:border-white/10 transition-colors">
                  <div>
                    <h5 className="font-bold text-white text-xs">Expose Activity History</h5>
                    <span className="text-[#71717a] text-[10px]">Show my tournament medals and historical matches played on my public profile.</span>
                  </div>
                  <input type="checkbox" checked={privacySettings.showHistory} onChange={() => setPrivacySettings({...privacySettings, showHistory: !privacySettings.showHistory})} className="w-4 h-4 accent-[#84cc16]" />
                </label>
              </div>
            </div>

            <button type="submit" className="w-full py-3 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all cursor-pointer">Save Settings</button>
          </form>
        )}

      </div>
    </div>
  );
}
