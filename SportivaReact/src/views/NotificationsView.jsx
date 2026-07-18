import React, { useState, useEffect } from 'react';
import { Bell } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function NotificationsView({ addToast }) {
  const [notifications, setNotifications] = useState([]);

  useEffect(() => {
    loadNotifications();
  }, []);

  const loadNotifications = async () => {
    try {
      const res = await apiFetch('/notifications');
      if (res.ok) {
        const data = await res.json();
        setNotifications(data.items || []);
      }
    } catch (e) {}
  };

  const markAllNotifications = async () => {
    try {
      const res = await apiFetch('/notifications/read-all', { method: 'PUT' });
      if (res.ok) {
        setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
        addToast('All alerts read', 'info');
      }
    } catch (e) {}
  };

  return (
    <section className="animate-fade-in flex flex-col gap-6">
      <div className="flex justify-between items-center mb-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight">Alert Notifications</h2>
          <p className="text-[#a1a1aa] text-sm">Manage system alerts and confirmations</p>
        </div>
        <button onClick={markAllNotifications} className="px-5 py-2.5 rounded-xl border border-[#ffffff08] bg-white/5 text-sm font-semibold hover:bg-white/10 transition-colors">Mark all read</button>
      </div>
      <div className="flex flex-col gap-4">
        {notifications.length > 0 ? notifications.map(n => (
          <div key={n.id} className={`flex items-start gap-4 p-5 rounded-xl border ${
            !n.isRead ? 'border-l-4 border-l-[#84cc16] border-white/10 bg-[#84cc16]/3' : 'border-white/10 bg-[#16161c]/45'
          }`}>
            <Bell className="w-5 h-5 text-[#84cc16] mt-0.5" />
            <div>
              <h4 className="font-semibold text-sm">{n.title}</h4>
              <p className="text-xs text-[#a1a1aa] mt-0.5">{n.message}</p>
              <span className="text-[10px] text-[#71717a] block mt-1">{new Date(n.createdAt).toLocaleString()}</span>
            </div>
          </div>
        )) : (
          <p className="text-sm text-[#71717a] text-center py-8">Inbox is clear.</p>
        )}
      </div>
    </section>
  );
}
