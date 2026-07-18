import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { Sparkles, ArrowRight, ArrowLeft, Hourglass, Check, CheckCircle2, Trophy, HelpCircle, Shield, Building2 } from 'lucide-react';
import { apiFetch } from '../services/api';
import { useLanguage } from '../contexts/LanguageContext';

export default function BecomeOwnerView({ addToast, onStatusChange, initialStatus, user, onTriggerAuth }) {
  const navigate = useNavigate();
  const { t } = useLanguage();
  const [status, setStatus] = useState(initialStatus);
  const [selectedPlan, setSelectedPlan] = useState('');
  const [currentStep, setCurrentStep] = useState(1); // 1 = pricing & details, 2 = wizard form
  const [substep, setSubstep] = useState(1); // 1 to 4 form pages
  const [searchParams, setSearchParams] = useSearchParams();
  const planParam = searchParams.get('plan');

  // Dynamic Plans State
  const [plans, setPlans] = useState([]);
  const [plansLoading, setPlansLoading] = useState(true);

  // Representative Info
  const [repName, setRepName] = useState('');
  const [repPhone, setRepPhone] = useState('');
  const [repEmail, setRepEmail] = useState('');
  const [nationalId, setNationalId] = useState('');

  // Complex Info
  const [clubName, setClubName] = useState('');
  const [clubGov, setClubGov] = useState('Cairo');
  const [clubCity, setClubCity] = useState('');
  const [clubMaps, setClubMaps] = useState('');
  const [clubAddress, setClubAddress] = useState('');

  // Sports & Capacity
  const [sports, setSports] = useState({
    Football: false,
    Padel: false,
    Tennis: false,
    Basketball: false
  });
  const [estimatedCourts, setEstimatedCourts] = useState(4);

  // Business Media & Notes
  const [clubDesc, setClubDesc] = useState('');
  const [clubNotes, setClubNotes] = useState('');

  // Attached files
  const [clubLogoName, setClubLogoName] = useState('');
  const [docNationalIdName, setDocNationalIdName] = useState('');
  const [docTaxCertName, setDocTaxCertName] = useState('');

  useEffect(() => {
    loadPlans();
    if (user) {
      fetchStatus();
    }
  }, [user]);

  useEffect(() => {
    if (planParam) {
      setSelectedPlan(planParam);
      if (!user) {
        onTriggerAuth(`/become-owner?plan=${encodeURIComponent(planParam)}`);
      } else {
        setCurrentStep(2);
        setSubstep(1);
      }
    }
  }, [planParam, user]);

  const loadPlans = async () => {
    try {
      setPlansLoading(true);
      const res = await apiFetch('/subscription-plans');
      if (res.ok) {
        const data = await res.json();
        const plansList = data.items || data || [];
        // Only active plans sorted by price ascending
        setPlans(plansList.filter(p => p.isActive).sort((a, b) => a.price - b.price));
      }
    } catch (e) {
      console.error('Error loading plans', e);
    } finally {
      setPlansLoading(false);
    }
  };

  const fetchStatus = async () => {
    try {
      const res = await apiFetch('/me/membership-request');
      if (res.ok) {
        const data = await res.json();
        setStatus(data.status);
        if (onStatusChange) onStatusChange(data.status);
      }
    } catch (e) {}
  };

  const handlePlanSelect = (planName) => {
    setSelectedPlan(planName);
    if (!user) {
      setSearchParams({ plan: planName });
      onTriggerAuth(`/become-owner?plan=${encodeURIComponent(planName)}`);
    } else {
      setCurrentStep(2);
      setSubstep(1);
    }
  };

  const toggleSport = (sport) => {
    setSports(prev => ({ ...prev, [sport]: !prev[sport] }));
  };

  const validateSubstep = (stepNum) => {
    if (stepNum === 1) {
      if (!repName.trim()) {
        addToast('Legal representative name is required', 'error');
        return false;
      }
      if (!nationalId.trim()) {
        addToast('National ID is required', 'error');
        return false;
      }
      if (nationalId.length !== 14 || isNaN(nationalId)) {
        addToast('National ID must be exactly 14 digits and numeric', 'error');
        return false;
      }
      if (!repPhone.trim()) {
        addToast('Phone number is required', 'error');
        return false;
      }
      if (!repEmail.trim() || !repEmail.includes('@')) {
        addToast('A valid email address is required', 'error');
        return false;
      }
      return true;
    }
    if (stepNum === 2) {
      if (!clubName.trim()) {
        addToast('Proposed club complex name is required', 'error');
        return false;
      }
      if (!clubGov.trim()) {
        addToast('Governorate is required', 'error');
        return false;
      }
      if (!clubCity.trim()) {
        addToast('City is required', 'error');
        return false;
      }
      if (!clubMaps.trim()) {
        addToast('Google Maps coordinates link is required', 'error');
        return false;
      }
      if (!clubAddress.trim()) {
        addToast('Physical address details are required', 'error');
        return false;
      }
      return true;
    }
    if (stepNum === 3) {
      const selectedSportsCount = Object.values(sports).filter(v => v).length;
      if (selectedSportsCount === 0) {
        addToast('Please select at least one sport type', 'error');
        return false;
      }
      if (estimatedCourts < 1) {
        addToast('Estimated courts capacity must be at least 1', 'error');
        return false;
      }
      return true;
    }
    if (stepNum === 4) {
      if (!clubDesc.trim()) {
        addToast('Business pitch description is required', 'error');
        return false;
      }
      return true;
    }
    return true;
  };

  const handleNext = () => {
    if (validateSubstep(substep)) {
      setSubstep(prev => prev + 1);
    }
  };

  const handleBack = () => {
    if (substep > 1) {
      setSubstep(prev => prev - 1);
    } else {
      setCurrentStep(1);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateSubstep(4)) return;

    const sportsList = Object.keys(sports).filter(k => sports[k]);

    // Packed detailed metadata inside notes
    const packedMetadata = JSON.stringify({
      representativeEmail: repEmail,
      nationalId: nationalId,
      governorate: clubGov,
      city: clubCity,
      estimatedCourts: estimatedCourts,
      clubDescription: clubDesc,
      sports: sportsList,
      subscriptionPlan: selectedPlan,
      notes: clubNotes,
      attachedDocuments: [
        docNationalIdName || 'national_id_photocopy.pdf',
        docTaxCertName || 'commercial_registry.pdf'
      ],
      clubImages: [
        clubLogoName || 'complex_main_facade.jpg'
      ]
    });

    try {
      const res = await apiFetch('/membership-requests', {
        method: 'POST',
        body: JSON.stringify({
          fullName: repName,
          phone: repPhone,
          isClubOwner: true,
          clubName: clubName,
          address: clubAddress,
          locationUrl: clubMaps,
          note: packedMetadata
        })
      });

      if (res.ok) {
        addToast('Owner onboarding request submitted successfully!', 'success');
        setStatus('Pending');
        if (onStatusChange) onStatusChange('Pending');
      } else {
        const err = await res.json();
        throw new Error(err.detail || 'Could not register request');
      }
    } catch (err) {
      addToast(err.message, 'error');
    }
  };

  // Case A: Pending review
  if (status === 'Pending') {
    return (
      <div className="max-w-[650px] mx-auto text-center flex flex-col items-center gap-6 py-20 animate-fade-in">
        <div className="w-20 h-20 rounded-full bg-amber-500/10 border border-amber-500/20 flex items-center justify-center text-amber-400 animate-pulse">
          <Hourglass className="w-10 h-10" />
        </div>
        <h2 className="text-3xl font-extrabold text-white">Application Under Review</h2>
        <p className="text-sm text-[#a1a1aa] leading-relaxed max-w-[500px]">
          Our onboarding managers are currently validating your legal representative name, National ID card details, and Google maps coordinates. You will receive an alert notification as soon as your Sportify Partner account is approved.
        </p>
      </div>
    );
  }

  // Case B: Step 1 - Pricing Plan Selection, FAQs & Benefits
  if (currentStep === 1) {
    return (
      <div className="max-w-[1100px] mx-auto flex flex-col gap-16 py-8 animate-fade-in text-xs text-[#a1a1aa] text-left rtl:text-right">
        {/* Header Block */}
        <div className="text-center max-w-[600px] mx-auto flex flex-col gap-4">
          <span className="px-3 py-1.5 bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#a3e635] rounded-full font-bold uppercase tracking-widest w-fit mx-auto flex items-center gap-1.5 animate-pulse">
            <Sparkles className="w-4 h-4" /> {t('becomeOwner')}
          </span>
          <h1 className="text-3xl md:text-5xl font-black tracking-tight text-white leading-tight">
            {t('choosePlan')}
          </h1>
          <p className="text-sm text-[#a1a1aa] leading-relaxed">
            {t('choosePlanDesc')}
          </p>
        </div>

        {/* Pricing Cards */}
        {plansLoading ? (
          <div className="text-xs text-[#a1a1aa] py-20 text-center flex flex-col items-center justify-center gap-3 bg-[#121216]/40 border border-white/5 rounded-3xl">
            <RefreshCw className="w-6 h-6 animate-spin text-[#84cc16]" />
            <span>Loading subscription plans...</span>
          </div>
        ) : plans.length === 0 ? (
          <div className="text-xs text-[#a1a1aa] py-20 text-center bg-[#121216]/40 border border-white/5 rounded-3xl">
            No subscription plans are currently active. Please contact administrator support.
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {plans.map((plan) => {
              const isRecommended = plan.name.toLowerCase().includes('pro') || plan.name.toLowerCase().includes('premium');
              return (
                <div 
                  key={plan.id} 
                  className={`bg-[#121216]/50 border rounded-3xl p-8 flex flex-col justify-between gap-8 hover:border-[#84cc16]/20 transition-all duration-300 relative group text-left rtl:text-right ${
                    isRecommended ? 'border-[#84cc16]/30 shadow-2xl shadow-[#84cc16]/5' : 'border-white/5'
                  }`}
                >
                  {isRecommended && (
                    <span className="absolute top-4 right-4 rtl:right-auto rtl:left-4 text-[9px] uppercase font-extrabold tracking-widest px-3 py-1 bg-[#84cc16]/10 text-[#a3e635] rounded-full border border-[#84cc16]/20">
                      {t('recommended')}
                    </span>
                  )}
                  <div className="flex flex-col gap-5">
                    <div className="w-12 h-12 rounded-2xl bg-[#84cc16]/10 flex items-center justify-center text-[#84cc16] font-bold text-lg">
                      {isRecommended ? <Trophy className="w-6 h-6" /> : plan.name.charAt(0)}
                    </div>
                    <div>
                      <h3 className="font-bold text-lg text-white">{plan.name}</h3>
                      <p className="text-xs text-[#71717a] mt-1">{plan.description || 'Flexible management plan'}</p>
                    </div>
                    <div className="text-3xl font-extrabold text-[#84cc16] mt-2">
                      {plan.price > 0 ? `${plan.price} EGP` : 'Free'}
                      {plan.price > 0 && <span className="text-xs text-[#71717a] font-normal">{t('perMonth')}</span>}
                      <span className="text-xs text-[#71717a] font-normal block mt-1">{t('cancelAnytime')}</span>
                    </div>
                    <ul className="text-left rtl:text-right text-xs text-[#a1a1aa] flex flex-col gap-3.5 border-t border-white/5 pt-5">
                      <li className="flex items-center gap-2.5">
                        <Check className="w-4 h-4 text-[#84cc16] shrink-0" />
                        <span>{t('maxClubsLabel')}: {plan.name.toLowerCase().includes('basic') ? 1 : plan.name.toLowerCase().includes('premium') ? 2 : 5}</span>
                      </li>
                      <li className="flex items-center gap-2.5">
                        <Check className="w-4 h-4 text-[#84cc16] shrink-0" />
                        <span>{t('maxCourtsLabel')}: {plan.maxCourts} {t('activeCourts')}</span>
                      </li>
                      <li className="flex items-center gap-2.5">
                        <Check className="w-4 h-4 text-[#84cc16] shrink-0" />
                        <span>{t('timeSlotGen')}</span>
                      </li>
                      <li className="flex items-center gap-2.5">
                        <Check className="w-4 h-4 text-[#84cc16] shrink-0" />
                        <span>{t('peakRates')}</span>
                      </li>
                    </ul>
                  </div>
                  <button 
                    onClick={() => handlePlanSelect(plan.name)} 
                    className={`w-full py-4 rounded-2xl text-xs font-bold transition-all flex items-center justify-center gap-2 cursor-pointer ${
                      isRecommended 
                        ? 'bg-[#84cc16] hover:bg-[#65a30d] text-black shadow-lg shadow-[#84cc16]/15' 
                        : 'bg-white/5 hover:bg-white/10 border border-white/10 text-white'
                    }`}
                  >
                    {t('selectPlanBtn')} {plan.name} <ArrowRight className="w-4 h-4 rtl:rotate-180" />
                  </button>
                </div>
              );
            })}
          </div>
        )}

        {/* Benefits & Value Proposition Section */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8 mt-6 text-left rtl:text-right">
          <div className="bg-[#121216]/30 border border-white/5 rounded-2xl p-6 flex flex-col gap-3">
            <Building2 className="w-8 h-8 text-[#84cc16] self-start" />
            <h4 className="text-sm font-bold text-white">{t('fullVenueOwnership')}</h4>
            <p className="leading-relaxed text-[#71717a]">
              {t('fullVenueOwnershipDesc')}
            </p>
          </div>
          <div className="bg-[#121216]/30 border border-white/5 rounded-2xl p-6 flex flex-col gap-3">
            <Shield className="w-8 h-8 text-[#84cc16] self-start" />
            <h4 className="text-sm font-bold text-white">{t('overlappingPrevention')}</h4>
            <p className="leading-relaxed text-[#71717a]">
              {t('overlappingPreventionDesc')}
            </p>
          </div>
          <div className="bg-[#121216]/30 border border-white/5 rounded-2xl p-6 flex flex-col gap-3">
            <CheckCircle2 className="w-8 h-8 text-[#84cc16] self-start" />
            <h4 className="text-sm font-bold text-white">{t('automatedSchedules')}</h4>
            <p className="leading-relaxed text-[#71717a]">
              {t('automatedSchedulesDesc')}
            </p>
          </div>
        </div>

        {/* Frequently Asked Questions */}
        <div className="bg-[#121216]/30 border border-white/5 rounded-3xl p-8 md:p-10 text-left rtl:text-right">
          <h3 className="text-lg font-bold text-white mb-6 flex items-center gap-2">
            <HelpCircle className="w-5 h-5 text-[#84cc16]" /> {t('faqTitle')}
          </h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8 text-xs leading-relaxed text-[#71717a]">
            <div>
              <h4 className="font-bold text-white mb-1.5">{t('faqQ1')}</h4>
              <p>
                {t('faqA1')}
              </p>
            </div>
            <div>
              <h4 className="font-bold text-white mb-1.5">{t('faqQ2')}</h4>
              <p>
                {t('faqA2')}
              </p>
            </div>
            <div>
              <h4 className="font-bold text-white mb-1.5">{t('faqQ3')}</h4>
              <p>
                {t('faqA3')}
              </p>
            </div>
            <div>
              <h4 className="font-bold text-white mb-1.5">{t('faqQ4')}</h4>
              <p>
                {t('faqA4')}
              </p>
            </div>
          </div>
        </div>
      </div>
    );
  }

  // Case C: Step 2 - Form Wizard stepper
  return (
    <div className="max-w-[750px] mx-auto flex flex-col gap-8 animate-fade-in text-xs text-[#a1a1aa]">
      <div className="flex justify-between items-center bg-[#121216]/50 border border-white/5 p-5 rounded-2xl">
        <div>
          <span className="text-[10px] font-bold text-[#84cc16] uppercase tracking-widest">Sportify Partner Registration</span>
          <h3 className="text-lg font-bold text-white mt-0.5">Onboarding Application</h3>
        </div>
        <span className="text-xs px-3.5 py-1.5 bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#a3e635] rounded-xl font-bold uppercase tracking-wider">{selectedPlan}</span>
      </div>

      {/* Progress indicators */}
      <div className="bg-[#121216]/30 border border-white/5 rounded-2xl p-4 flex justify-between text-xs font-semibold text-[#71717a] gap-2">
        <span className={substep >= 1 ? 'text-[#84cc16] font-bold' : ''}>1. Representative</span>
        <span className={substep >= 2 ? 'text-[#84cc16] font-bold' : ''}>2. Club Location</span>
        <span className={substep >= 3 ? 'text-[#84cc16] font-bold' : ''}>3. Sports Types</span>
        <span className={substep >= 4 ? 'text-[#84cc16] font-bold' : ''}>4. Review Info</span>
      </div>

      <div className="bg-[#121216]/50 border border-white/5 rounded-2xl p-8 shadow-2xl">
        <form onSubmit={handleSubmit} className="flex flex-col gap-6">
          
          {/* Form Step 1: Representative details */}
          {substep === 1 && (
            <div className="flex flex-col gap-5 animate-fade-in">
              <h4 className="font-bold text-white text-base">Representative Credentials</h4>
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Full Legal Name</label>
                <input type="text" value={repName} onChange={(e) => setRepName(e.target.value)} required className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-sm text-white outline-none focus:border-[#84cc16]" placeholder="e.g. Sayed Kotb" />
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase">National ID (الرقم القومي - 14 رقم)</label>
                <input type="text" maxLength={14} value={nationalId} onChange={(e) => setNationalId(e.target.value)} required className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-sm text-white outline-none focus:border-[#84cc16]" placeholder="e.g. 29901011234567" />
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Phone Number</label>
                  <input type="text" value={repPhone} onChange={(e) => setRepPhone(e.target.value)} required className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-sm text-white outline-none focus:border-[#84cc16]" placeholder="e.g. +201123456789" />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Business Email</label>
                  <input type="email" value={repEmail} onChange={(e) => setRepEmail(e.target.value)} required className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-sm text-white outline-none focus:border-[#84cc16]" placeholder="e.g. owner@complex.com" />
                </div>
              </div>
            </div>
          )}

          {/* Form Step 2: Club Location Details */}
          {substep === 2 && (
            <div className="flex flex-col gap-5 animate-fade-in">
              <h4 className="font-bold text-white text-base">Club Complex Address</h4>
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Proposed Club Name</label>
                <input type="text" value={clubName} onChange={(e) => setClubName(e.target.value)} required className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-sm text-white outline-none focus:border-[#84cc16]" placeholder="e.g. Pyramids Padel Complex" />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Governorate</label>
                  <input type="text" value={clubGov} onChange={(e) => setClubGov(e.target.value)} required className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-sm text-white outline-none focus:border-[#84cc16]" />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase">City</label>
                  <input type="text" value={clubCity} onChange={(e) => setClubCity(e.target.value)} required className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-sm text-white outline-none focus:border-[#84cc16]" placeholder="e.g. Maadi" />
                </div>
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Google Maps Coordinates URL</label>
                <input type="url" value={clubMaps} onChange={(e) => setClubMaps(e.target.value)} required className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-sm text-white outline-none focus:border-[#84cc16]" placeholder="https://maps.google.com/..." />
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Physical Address</label>
                <input type="text" value={clubAddress} onChange={(e) => setClubAddress(e.target.value)} required className="bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-sm text-white outline-none focus:border-[#84cc16]" placeholder="e.g. Street 15, Near Metro station, Maadi" />
              </div>
            </div>
          )}

          {/* Form Step 3: Sports available */}
          {substep === 3 && (
            <div className="flex flex-col gap-5 animate-fade-in">
              <h4 className="font-bold text-white text-base">Sports Types & Capacity</h4>
              <div className="flex flex-col gap-2">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Available Sports (Select all that apply)</label>
                <div className="grid grid-cols-2 gap-3 mt-1">
                  {Object.keys(sports).map(sport => (
                    <label key={sport} className={`flex items-center gap-3 p-4 rounded-xl border cursor-pointer hover:border-[#84cc16]/50 transition-colors ${sports[sport] ? 'bg-[#84cc16]/15 border-[#84cc16]/50 text-white' : 'bg-[#1e1e26]/30 border-white/5 text-[#a1a1aa]'}`}>
                      <input type="checkbox" checked={sports[sport]} onChange={() => toggleSport(sport)} className="w-4 h-4 accent-[#84cc16]" />
                      <span>{sport}</span>
                    </label>
                  ))}
                </div>
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Estimated Number of Courts</label>
                <input type="number" min={1} value={estimatedCourts} onChange={(e) => setEstimatedCourts(parseInt(e.target.value))} required className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl px-4 py-3 text-sm text-white outline-none focus:border-[#84cc16]" />
              </div>
            </div>
          )}

          {/* Form Step 4: Business Pitch & notes */}
          {substep === 4 && (
            <div className="flex flex-col gap-5 animate-fade-in">
              <h4 className="font-bold text-white text-base">Business Pitch & Notes</h4>
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Club Description / Facility details</label>
                <textarea value={clubDesc} onChange={(e) => setClubDesc(e.target.value)} required className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl p-4 text-sm text-white outline-none resize-none h-[100px]" placeholder="Describe courts quality, locker rooms, lightning..." />
              </div>
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold text-[#a1a1aa] uppercase">Additional Comments (Optional)</label>
                <textarea value={clubNotes} onChange={(e) => setClubNotes(e.target.value)} className="w-full bg-[#1e1e26]/50 border border-white/5 rounded-xl p-4 text-sm text-white outline-none resize-none h-[100px]" placeholder="Review requests or onboarding feedback..." />
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-2">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase text-[10px]">Club Cover / Facade Image</label>
                  <div className="relative flex items-center justify-center border border-dashed border-white/10 hover:border-[#84cc16]/50 transition-all rounded-xl p-4 bg-[#1e1e26]/30 cursor-pointer">
                    <input type="file" onChange={(e) => setClubLogoName(e.target.files[0]?.name || '')} className="absolute inset-0 opacity-0 cursor-pointer" accept="image/*" />
                    <span className="text-[11px] text-[#fafafa] truncate">{clubLogoName || 'Select Complex Photo'}</span>
                  </div>
                </div>

                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-semibold text-[#a1a1aa] uppercase text-[10px]">Documents Verification Attachments</label>
                  <div className="flex gap-2">
                    <div className="flex-1 relative flex items-center justify-center border border-dashed border-white/10 hover:border-[#84cc16]/50 transition-all rounded-xl p-3 bg-[#1e1e26]/30 cursor-pointer">
                      <input type="file" onChange={(e) => setDocNationalIdName(e.target.files[0]?.name || '')} className="absolute inset-0 opacity-0 cursor-pointer" accept=".pdf,image/*" />
                      <span className="text-[10px] text-[#fafafa] truncate">{docNationalIdName || 'National ID'}</span>
                    </div>
                    <div className="flex-1 relative flex items-center justify-center border border-dashed border-white/10 hover:border-[#84cc16]/50 transition-all rounded-xl p-3 bg-[#1e1e26]/30 cursor-pointer">
                      <input type="file" onChange={(e) => setDocTaxCertName(e.target.files[0]?.name || '')} className="absolute inset-0 opacity-0 cursor-pointer" accept=".pdf,image/*" />
                      <span className="text-[10px] text-[#fafafa] truncate">{docTaxCertName || 'Business License'}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Navigation Controls */}
          <div className="flex justify-between items-center border-t border-white/5 pt-6 mt-2 gap-4">
            <button type="button" onClick={handleBack} className="px-6 py-3.5 bg-white/5 hover:bg-white/10 border border-white/10 text-white font-bold text-xs rounded-xl transition-all flex items-center gap-2">
              <ArrowLeft className="w-4 h-4" /> Back
            </button>

            {substep < 4 ? (
              <button type="button" onClick={handleNext} className="px-6 py-3.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all ml-auto flex items-center gap-2">
                Next Step <ArrowRight className="w-4 h-4" />
              </button>
            ) : (
              <button type="submit" className="px-6 py-3.5 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-xl transition-all ml-auto shadow-lg shadow-[#84cc16]/15">
                Submit Business Application
              </button>
            )}
          </div>

        </form>
      </div>
    </div>
  );
}
