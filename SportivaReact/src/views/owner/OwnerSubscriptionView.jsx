import React, { useState, useEffect } from 'react';
import { useOutletContext } from 'react-router-dom';
import { apiFetch } from '../../services/api';
import { CreditCard, Calendar, Check, Shield, AlertCircle, RefreshCw, Star, Info, Phone, MessageSquare } from 'lucide-react';

export default function OwnerSubscriptionView({ addToast }) {
  const { selectedClub } = useOutletContext();
  const [activeSub, setActiveSub] = useState(null);
  const [plans, setPlans] = useState([]);
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  // Modal State
  const [modalOpen, setModalOpen] = useState(false);
  const [requestType, setRequestType] = useState('Renew'); // 'Renew' or 'Upgrade'
  const [targetPlan, setTargetPlan] = useState(null);
  const [phone, setPhone] = useState('');
  const [note, setNote] = useState('');

  useEffect(() => {
    if (selectedClub) {
      loadSubscriptionDetails();
    }
  }, [selectedClub]);

  const loadSubscriptionDetails = async () => {
    setLoading(true);
    try {
      // 1. Fetch active subscription
      const subRes = await apiFetch(`/clubs/${selectedClub.id}/subscriptions/active`);
      if (subRes.ok) {
        const subData = await subRes.json();
        setActiveSub(subData);
      } else {
        setActiveSub(null);
      }

      // 2. Fetch plans
      const plansRes = await apiFetch('/subscription-plans');
      if (plansRes.ok) {
        const plansData = await plansRes.json();
        setPlans(plansData.items || plansData || []);
      }

      // 3. Fetch request history
      const reqsRes = await apiFetch(`/clubs/${selectedClub.id}/subscriptions/requests`);
      if (reqsRes.ok) {
        const reqsData = await reqsRes.json();
        setRequests(reqsData || []);
      }
    } catch (e) {
      console.error('Error fetching subscription details', e);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenRequestModal = (type, plan = null) => {
    setRequestType(type);
    setTargetPlan(plan);
    setPhone('');
    setNote('');
    setModalOpen(true);
  };

  const handleSubmitRequest = async (e) => {
    e.preventDefault();
    if (!phone.trim()) {
      addToast('Please enter your contact phone number.', 'error');
      return;
    }

    setSubmitting(true);
    try {
      const planId = requestType === 'Renew' ? (activeSub?.planId || plans[0]?.id) : targetPlan?.id;
      if (!planId) {
        addToast('No plan selected.', 'error');
        return;
      }

      const res = await apiFetch(`/clubs/${selectedClub.id}/subscriptions/requests`, {
        method: 'POST',
        body: JSON.stringify({
          planId,
          requestType: requestType === 'Renew' ? 0 : 1,
          phone,
          note
        })
      });

      if (res.ok) {
        addToast(`Subscription ${requestType} request submitted successfully! An Admin will review it shortly.`, 'success');
        setModalOpen(false);
        loadSubscriptionDetails();
      } else {
        const err = await res.json();
        throw new Error(err.detail || 'Request submission failed.');
      }
    } catch (e) {
      addToast(e.message || 'Request failed.', 'error');
    } finally {
      setSubmitting(false);
    }
  };

  if (!selectedClub) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-6 bg-[#121216]/40 border border-white/5 rounded-2xl shadow-lg">
        <AlertCircle className="w-12 h-12 text-[#a1a1aa] mb-4" />
        <h3 className="text-lg font-bold text-white mb-2">No active venue selected</h3>
        <p className="text-xs text-[#a1a1aa] max-w-sm">Please register or select a club from the sidebar to manage subscriptions.</p>
      </div>
    );
  }

  const getStatusBadge = (status) => {
    switch (status) {
      case 0:
      case 'Pending':
        return <span className="text-[10px] bg-yellow-500/10 border border-yellow-500/20 text-yellow-500 px-2 py-0.5 rounded font-bold uppercase">Pending Review</span>;
      case 1:
      case 'Approved':
        return <span className="text-[10px] bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#84cc16] px-2 py-0.5 rounded font-bold uppercase">Approved</span>;
      case 2:
      case 'Rejected':
        return <span className="text-[10px] bg-red-500/10 border border-red-500/20 text-red-500 px-2 py-0.5 rounded font-bold uppercase">Rejected</span>;
      default:
        return null;
    }
  };

  return (
    <div className="flex flex-col gap-8 animate-fade-in relative">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight text-white">Subscription Management</h2>
          <p className="text-[#a1a1aa] text-xs mt-1">View your current operations plan or request plan renewals and upgrades</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Left column: Active Plan details */}
        <div className="flex flex-col gap-6 lg:col-span-1">
          <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl shadow-lg">
            <h3 className="text-sm font-bold text-white mb-4">Current Active Plan</h3>

            {loading ? (
              <div className="text-xs text-[#a1a1aa] py-10 text-center flex items-center justify-center gap-2">
                <RefreshCw className="w-4 h-4 animate-spin text-[#84cc16]" /> Loading billing details...
              </div>
            ) : !activeSub ? (
              <div className="flex flex-col items-center text-center p-6 bg-white/5 border border-dashed border-white/10 rounded-xl">
                <AlertCircle className="w-10 h-10 text-[#a1a1aa] mb-3" />
                <h4 className="text-xs font-bold text-white mb-1">No Active Plan</h4>
                <p className="text-[10px] text-[#a1a1aa] mb-4">You currently do not have an active subscription for this club.</p>
              </div>
            ) : (
              <div className="flex flex-col gap-5">
                <div className="flex items-center gap-3 bg-white/5 p-4 rounded-xl border border-white/5">
                  <div className="w-10 h-10 rounded-lg bg-[#84cc16]/10 text-[#84cc16] flex items-center justify-center">
                    <Star className="w-5 h-5 fill-current" />
                  </div>
                  <div>
                    <h4 className="text-sm font-extrabold text-white">{activeSub.planName || 'Sportify Member'}</h4>
                    <span className="text-[10px] text-[#84cc16] font-bold uppercase tracking-wider">Active</span>
                  </div>
                </div>

                <div className="flex flex-col gap-3 text-xs text-[#a1a1aa]">
                  <div className="flex justify-between border-b border-white/5 pb-2">
                    <span>Started Date</span>
                    <span className="text-white font-bold">{activeSub.startDate || 'N/A'}</span>
                  </div>
                  <div className="flex justify-between border-b border-white/5 pb-2">
                    <span>Expiration Date</span>
                    <span className="text-white font-bold">{activeSub.endDate || 'N/A'}</span>
                  </div>
                  <div className="flex justify-between border-b border-white/5 pb-2">
                    <span>Price Tier</span>
                    <span className="text-white font-bold">EGP {activeSub.price || 0}</span>
                  </div>
                </div>

                <div className="flex flex-col gap-2 mt-2">
                  <button
                    onClick={() => handleOpenRequestModal('Renew')}
                    disabled={submitting}
                    className="w-full py-2.5 bg-[#84cc16] hover:bg-[#65a30d] disabled:opacity-50 text-black font-extrabold text-xs rounded-xl shadow-lg transition-all text-center cursor-pointer"
                  >
                    Request Renewal
                  </button>
                </div>
              </div>
            )}
          </div>

          {/* Submissions Request History */}
          <div className="bg-[#121216]/60 border border-white/5 p-6 rounded-2xl shadow-lg">
            <h3 className="text-sm font-bold text-white mb-4">Request Log</h3>

            {loading ? (
              <div className="text-xs text-[#a1a1aa] py-6 text-center">Loading requests...</div>
            ) : requests.length === 0 ? (
              <div className="text-xs text-[#a1a1aa] py-6 text-center">No past request entries.</div>
            ) : (
              <div className="flex flex-col gap-3 max-h-[260px] overflow-y-auto pr-1">
                {requests.map((r) => (
                  <div key={r.id} className="p-3 bg-white/5 rounded-xl border border-white/5 flex flex-col gap-2">
                    <div className="flex justify-between items-start">
                      <div>
                        <span className="text-xs font-bold text-white block">{r.plan?.name}</span>
                        <span className="text-[9px] text-[#71717a]">{r.requestType === 0 ? 'Renewal' : 'Upgrade'} request</span>
                      </div>
                      {getStatusBadge(r.status)}
                    </div>
                    {r.note && <p className="text-[10px] text-[#a1a1aa] italic">Notes: "{r.note}"</p>}
                    <span className="text-[9px] text-[#71717a] text-right font-medium">Requested: {new Date(r.requestedAt).toLocaleDateString()}</span>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Right column: Upgrade Options */}
        <div className="lg:col-span-2 flex flex-col gap-6">
          <div className="bg-[#121216]/40 border border-white/5 p-6 rounded-2xl shadow-lg">
            <h3 className="text-sm font-bold text-white mb-2">Request Plan Upgrade</h3>
            <p className="text-xs text-[#a1a1aa] mb-6">Select a plan tier below to request an upgrade from Sportify Administration.</p>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {plans.map(plan => {
                const isActivePlan = activeSub && activeSub.planId === plan.id;

                return (
                  <div key={plan.id} className={`bg-[#121216] border rounded-2xl p-6 flex flex-col justify-between gap-6 transition-all ${
                    isActivePlan ? 'border-[#84cc16]' : 'border-white/5'
                  }`}>
                    <div>
                      <div className="flex justify-between items-start gap-4 mb-2">
                        <h4 className="text-sm font-bold text-white uppercase tracking-wider">{plan.name}</h4>
                        {isActivePlan && (
                          <span className="text-[9px] bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#84cc16] px-2 py-0.5 rounded font-bold uppercase">
                            Active
                          </span>
                        )}
                      </div>

                      <div className="flex items-baseline gap-1 mt-2">
                        <span className="text-3xl font-extrabold text-white">EGP {plan.price}</span>
                        <span className="text-xs text-[#a1a1aa]">/ month</span>
                      </div>

                      <ul className="flex flex-col gap-2.5 text-xs text-[#a1a1aa] mt-6">
                        <li className="flex items-center gap-2">
                          <Check className="w-4 h-4 text-[#84cc16] shrink-0" />
                          <span>Max Clubs: {plan.name.toLowerCase().includes('basic') ? 1 : plan.name.toLowerCase().includes('premium') ? 2 : 5}</span>
                        </li>
                        <li className="flex items-center gap-2">
                          <Check className="w-4 h-4 text-[#84cc16] shrink-0" />
                          <span>Max Courts per Club: {plan.maxCourts || 3}</span>
                        </li>
                        <li className="flex items-center gap-2">
                          <Check className="w-4 h-4 text-[#84cc16] shrink-0" />
                          <span>Max Tournaments: {plan.name.toLowerCase().includes('basic') ? 1 : plan.name.toLowerCase().includes('premium') ? 3 : 10}</span>
                        </li>
                      </ul>
                    </div>

                    <button
                      onClick={() => handleOpenRequestModal('Upgrade', plan)}
                      disabled={isActivePlan}
                      className={`w-full py-2.5 rounded-xl font-bold text-xs transition-all text-center cursor-pointer ${
                        isActivePlan 
                          ? 'bg-white/5 border border-white/10 text-[#71717a] cursor-not-allowed'
                          : 'bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold shadow-lg'
                      }`}
                    >
                      {isActivePlan ? 'Current Active Plan' : 'Request Upgrade'}
                    </button>
                  </div>
                );
              })}
            </div>
          </div>
        </div>
      </div>

      {/* Subscription Request Modal Form */}
      {modalOpen && (
        <div className="fixed inset-0 bg-black/80 backdrop-blur-sm z-[110] flex items-center justify-center p-4">
          <div className="bg-[#121216] border border-white/10 rounded-2xl max-w-md w-full p-6 shadow-2xl relative">
            <button onClick={() => setModalOpen(false)} className="absolute top-4 right-4 text-[#71717a] hover:text-white transition-colors">✕</button>
            
            <h3 className="text-base font-bold text-white mb-2 flex items-center gap-2">
              <CreditCard className="w-5 h-5 text-[#84cc16]" />
              {requestType === 'Renew' ? 'Request Renewal' : `Request Upgrade to ${targetPlan?.name}`}
            </h3>
            <p className="text-xs text-[#a1a1aa] mb-4">
              {requestType === 'Renew' 
                ? 'Fill this form to notify the administrator that you wish to renew your current subscription.' 
                : `Fill this form to request access to the ${targetPlan?.name} features and limits.`}
            </p>

            <form onSubmit={handleSubmitRequest} className="flex flex-col gap-4">
              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold uppercase tracking-wider text-white flex items-center gap-1.5">
                  <Phone className="w-3.5 h-3.5 text-[#84cc16]" /> Contact Phone Number
                </label>
                <input
                  type="text"
                  placeholder="e.g. +20 123 456 7890"
                  required
                  value={phone}
                  onChange={(e) => setPhone(e.target.value)}
                  className="bg-white/5 border border-white/10 rounded-xl px-4 py-2.5 text-xs text-white outline-none focus:border-[#84cc16] transition-all"
                />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-[10px] font-bold uppercase tracking-wider text-white flex items-center gap-1.5">
                  <MessageSquare className="w-3.5 h-3.5 text-[#84cc16]" /> Additional Notes
                </label>
                <textarea
                  placeholder="Please include any transfer reference codes or billing specifications here."
                  rows={4}
                  value={note}
                  onChange={(e) => setNote(e.target.value)}
                  className="bg-white/5 border border-white/10 rounded-xl px-4 py-2.5 text-xs text-white outline-none focus:border-[#84cc16] transition-all resize-none"
                />
              </div>

              <button
                type="submit"
                disabled={submitting}
                className="w-full mt-2 py-3 bg-[#84cc16] hover:bg-[#65a30d] disabled:opacity-50 text-black font-extrabold text-xs rounded-xl shadow-lg transition-all text-center cursor-pointer"
              >
                {submitting ? 'Submitting Application...' : 'Submit Request'}
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
