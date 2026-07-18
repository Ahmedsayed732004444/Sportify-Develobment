import React, { useState } from 'react';
import { Sparkles, Mail, Lock, User, Phone, CheckCircle, ArrowRight, UserPlus, LogIn } from 'lucide-react';
import { getApiBaseUrl } from '../services/api';

export default function LoginView({ onLoginSuccess }) {
  const [isRegisterMode, setIsRegisterMode] = useState(false);

  // Forms Input States
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [userName, setUserName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [gender, setGender] = useState('Male');

  const handleLoginSubmit = async (e) => {
    e.preventDefault();
    try {
      const apiBase = getApiBaseUrl();
      const res = await fetch(`${apiBase}/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Authentication failed. Please verify credentials.');
      }

      const data = await res.json();
      localStorage.setItem('token', data.token);
      localStorage.setItem('userName', `${data.firstName} ${data.lastName}`);
      localStorage.setItem('userId', data.id);
      
      onLoginSuccess(data.token);
    } catch (err) {
      alert(err.message);
    }
  };

  const handleRegisterSubmit = async (e) => {
    e.preventDefault();
    try {
      const apiBase = getApiBaseUrl();
      const res = await fetch(`${apiBase}/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ 
          email, 
          password, 
          firstName, 
          lastName, 
          userName, 
          phoneNumber, 
          gender 
        })
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.detail || 'Registration failed. Check parameters.');
      }

      alert('Registration successful! Please sign in.');
      setIsRegisterMode(false);
    } catch (err) {
      alert(err.message);
    }
  };

  return (
    <div className="bg-[#121216]/95 border border-white/5 rounded-3xl p-8 backdrop-blur-xl shadow-2xl shadow-[#84cc16]/5 flex flex-col gap-6 w-full animate-fade-in text-xs text-[#a1a1aa]">
      
      <div className="text-center">
        <div className="w-12 h-12 rounded-2xl bg-[#84cc16] flex items-center justify-center text-black font-extrabold text-xl mx-auto mb-3 shadow-lg shadow-[#84cc16]/25">S</div>
        <h2 className="text-xl font-extrabold text-white">
          {isRegisterMode ? 'Create Sportify Account' : 'Welcome to Sportify'}
        </h2>
        <p className="text-[11px] text-[#71717a] mt-1">
          {isRegisterMode ? 'Join our sports venue booking network' : 'Access your courts and booking history'}
        </p>
      </div>

      {!isRegisterMode ? (
        /* SIGN IN FORM */
        <form onSubmit={handleLoginSubmit} className="flex flex-col gap-4">
          <div className="flex flex-col gap-1.5">
            <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">Email Address</label>
            <div className="flex items-center gap-3 bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 focus-within:border-[#84cc16] transition-all">
              <Mail className="w-4 h-4 text-[#71717a]" />
              <input type="email" required value={email} onChange={(e) => setEmail(e.target.value)} className="bg-transparent border-none outline-none text-white text-xs w-full" placeholder="name@domain.com" />
            </div>
          </div>

          <div className="flex flex-col gap-1.5">
            <label className="text-[10px] font-bold text-[#71717a] uppercase tracking-wider">Password</label>
            <div className="flex items-center gap-3 bg-[#1e1e26]/40 border border-white/5 rounded-xl px-4 py-3 focus-within:border-[#84cc16] transition-all">
              <Lock className="w-4 h-4 text-[#71717a]" />
              <input type="password" required value={password} onChange={(e) => setPassword(e.target.value)} className="bg-transparent border-none outline-none text-white text-xs w-full" placeholder="••••••••" />
            </div>
          </div>

          <button type="submit" className="w-full py-3.5 mt-2 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all shadow-lg flex items-center justify-center gap-2">
            <LogIn className="w-4 h-4" /> Sign In
          </button>
          
          <p className="text-center text-[10px] text-[#71717a] mt-2">
            Don't have an account?{' '}
            <button type="button" onClick={() => setIsRegisterMode(true)} className="text-[#84cc16] font-bold hover:underline">Register Now</button>
          </p>
        </form>
      ) : (
        /* REGISTER FORM */
        <form onSubmit={handleRegisterSubmit} className="flex flex-col gap-3.5 max-h-[420px] overflow-y-auto pr-1">
          <div className="grid grid-cols-2 gap-3">
            <div className="flex flex-col gap-1">
              <label className="text-[9px] font-bold text-[#71717a] uppercase tracking-wider">First Name</label>
              <input type="text" required value={firstName} onChange={(e) => setFirstName(e.target.value)} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-3.5 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]" placeholder="Ahmed" />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-[9px] font-bold text-[#71717a] uppercase tracking-wider">Last Name</label>
              <input type="text" required value={lastName} onChange={(e) => setLastName(e.target.value)} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-3.5 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]" placeholder="Salem" />
            </div>
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-[9px] font-bold text-[#71717a] uppercase tracking-wider">Username</label>
            <input type="text" required value={userName} onChange={(e) => setUserName(e.target.value)} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-3.5 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]" placeholder="ahmed_salem" />
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-[9px] font-bold text-[#71717a] uppercase tracking-wider">Email Address</label>
            <input type="email" required value={email} onChange={(e) => setEmail(e.target.value)} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-3.5 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]" placeholder="name@domain.com" />
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-[9px] font-bold text-[#71717a] uppercase tracking-wider">Password</label>
            <input type="password" required value={password} onChange={(e) => setPassword(e.target.value)} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-3.5 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]" placeholder="••••••••" />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div className="flex flex-col gap-1">
              <label className="text-[9px] font-bold text-[#71717a] uppercase tracking-wider">Phone</label>
              <input type="text" required value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-3.5 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]" placeholder="01XXXXXXXXX" />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-[9px] font-bold text-[#71717a] uppercase tracking-wider">Gender</label>
              <select value={gender} onChange={(e) => setGender(e.target.value)} className="w-full bg-[#1e1e26]/40 border border-white/5 rounded-xl px-3.5 py-2.5 text-xs text-white outline-none focus:border-[#84cc16]">
                <option value="Male">Male</option>
                <option value="Female">Female</option>
              </select>
            </div>
          </div>

          <button type="submit" className="w-full py-3.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold rounded-xl transition-all shadow-lg mt-2 flex items-center justify-center gap-1.5">
            <UserPlus className="w-4 h-4" /> Create Account
          </button>
          
          <p className="text-center text-[10px] text-[#71717a] mt-1">
            Already registered?{' '}
            <button type="button" onClick={() => setIsRegisterMode(false)} className="text-[#84cc16] font-bold hover:underline">Log In</button>
          </p>
        </form>
      )}

    </div>
  );
}
