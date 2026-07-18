import React, { useState, useEffect, useRef } from 'react';
import { MessageSquare, Send, Check, CheckCheck } from 'lucide-react';
import { useSocket } from '../contexts/SocketContext';
import { apiFetch } from '../services/api';

export default function ChatView({ user, addToast }) {
  const { chatHub } = useSocket();
  const [conversations, setConversations] = useState([]);
  const [activeChat, setActiveChat] = useState(null);
  const [messages, setMessages] = useState([]);
  const [chatInput, setChatInput] = useState('');
  const [isTyping, setIsTyping] = useState(false);
  const chatLogRef = useRef(null);

  useEffect(() => {
    loadConversations();
  }, []);

  useEffect(() => {
    if (!chatHub) return;

    chatHub.on('ReceiveMessage', (msg) => {
      // If msg.sender.id is our active partner, or we are the sender and the receiver is our partner
      const isFromCurrentPartner = activeChat && (
        msg.sender?.id === activeChat.otherParty?.id || 
        (msg.isMine && activeChat.otherParty?.id)
      );
      
      if (isFromCurrentPartner) {
        setMessages(prev => [...prev, msg]);
        setIsTyping(false); // reset typing simulation
        setTimeout(scrollChatToBottom, 100);
        
        if (msg.sender?.id === activeChat.otherParty?.id) {
          apiFetch(`/messages/${activeChat.otherParty.id}/read`, { method: 'PUT' }).then(() => {
            loadConversations();
          });
        }
      } else {
        loadConversations();
      }
    });

    return () => {
      chatHub.off('ReceiveMessage');
    };
  }, [chatHub, activeChat]);

  const scrollChatToBottom = () => {
    if (chatLogRef.current) {
      chatLogRef.current.scrollTop = chatLogRef.current.scrollHeight;
    }
  };

  const loadConversations = async () => {
    try {
      const res = await apiFetch('/messages/conversations');
      if (res.ok) {
        const data = await res.json();
        setConversations(data.items || []);
      }
    } catch (e) {}
  };

  const selectConversation = async (convo) => {
    setActiveChat(convo);
    try {
      const res = await apiFetch(`/messages/${convo.otherParty.id}`);
      if (res.ok) {
        const data = await res.json();
        // The API returns messages in chronological order or reverse.
        // Let's sort them by SentAt.
        const sorted = (data.items || []).sort((a, b) => new Date(a.sentAt) - new Date(b.sentAt));
        setMessages(sorted);
        setTimeout(scrollChatToBottom, 100);
        
        // Mark as read
        await apiFetch(`/messages/${convo.otherParty.id}/read`, { method: 'PUT' });
        loadConversations();
      }
    } catch (e) {}
  };

  const sendChatMessage = async () => {
    if (!chatInput.trim() || !activeChat) return;
    try {
      const res = await apiFetch('/messages', {
        method: 'POST',
        body: JSON.stringify({ 
          receiverId: activeChat.otherParty.id, 
          content: chatInput 
        })
      });
      if (res.ok) {
        const msg = await res.json();
        setMessages(prev => [...prev, msg]);
        setChatInput('');
        setTimeout(scrollChatToBottom, 100);
        loadConversations();
      }
    } catch (e) {
      addToast('Failed to deliver message.', 'error');
    }
  };

  // Sort conversations by last message timestamp
  const sortedConversations = [...conversations].sort((a, b) => new Date(b.lastMessageAt) - new Date(a.lastMessageAt));

  return (
    <section className="flex-1 min-h-[500px] h-[calc(100vh-220px)] border border-[#ffffff08] bg-[#16161c]/45 rounded-2xl flex overflow-hidden">
      
      {/* 1. Conversations Sidebar list */}
      <div className="w-[320px] border-r border-[#ffffff08] flex flex-col p-6 animate-fade-in shrink-0">
        <h3 className="font-bold text-lg mb-4 text-white">Conversations</h3>
        <div className="flex-1 overflow-y-auto flex flex-col gap-2">
          {sortedConversations.length === 0 ? (
            <p className="text-xs text-[#71717a] py-6 text-center">No active chats.</p>
          ) : (
            sortedConversations.map(c => (
              <button key={c.otherParty.id} onClick={() => selectConversation(c)} className={`flex items-center gap-3.5 p-3 rounded-xl transition-all text-left ${
                activeChat?.otherParty?.id === c.otherParty.id ? 'bg-white/5' : 'hover:bg-white/5'
              }`}>
                <div className="relative">
                  <img className="w-11 h-11 rounded-full object-cover border border-white/10" src={c.otherParty.avatarUrl || "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=80&q=80"} alt={c.otherParty.name} />
                  {/* Active Online Status indicator mockup */}
                  <span className="absolute bottom-0 right-0 w-3 h-3 bg-[#10b981] rounded-full border-2 border-[#16161c]"></span>
                </div>
                <div className="flex-1 min-w-0">
                  <div className="flex justify-between items-baseline">
                    <span className="text-sm font-semibold truncate text-white">{c.otherParty.name}</span>
                  </div>
                  <span className="text-xs text-[#a1a1aa] truncate block mt-0.5">{c.lastMessagePreview || 'No messages'}</span>
                </div>
                {c.unreadCount > 0 && <span className="px-2 py-0.5 text-[10px] font-bold bg-[#ef4444] text-white rounded-full">{c.unreadCount}</span>}
              </button>
            ))
          )}
        </div>
      </div>

      {/* 2. Chat Log View */}
      <div className="flex-1 flex flex-col bg-[#0a0a0f]/20 min-w-0">
        {activeChat ? (
          <>
            <div className="p-6 border-b border-[#ffffff08] flex justify-between items-center shrink-0 bg-[#16161c]/25">
              <div>
                <h4 className="font-bold text-white text-base">{activeChat.otherParty.name}</h4>
                <span className="text-xs text-[#71717a]">SignalR Live Sockets Sync</span>
              </div>
            </div>

            <div ref={chatLogRef} className="flex-1 p-6 overflow-y-auto flex flex-col gap-4">
              {messages.map((m, idx) => {
                const isMine = m.isMine;
                return (
                  <div key={idx} className={`max-w-[65%] flex flex-col ${isMine ? 'ml-auto items-end' : 'mr-auto items-start'}`}>
                    <div className={`px-5 py-3 rounded-2xl text-xs leading-relaxed ${
                      isMine ? 'bg-[#84cc16] text-black rounded-br-none font-semibold' : 'bg-[#1e1e26]/40 border border-white/5 text-white rounded-bl-none'
                    }`}>
                      {m.content}
                    </div>
                    <div className="flex items-center gap-1 mt-1 text-[9px] text-[#71717a]">
                      <span>{new Date(m.sentAt).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}</span>
                      {isMine && (
                        <span>{m.isRead ? <CheckCheck className="w-3.5 h-3.5 text-[#84cc16]" /> : <Check className="w-3.5 h-3.5" />}</span>
                      )}
                    </div>
                  </div>
                );
              })}

              {isTyping && (
                <div className="mr-auto items-start max-w-[65%] flex flex-col">
                  <div className="bg-[#1e1e26]/40 border border-white/5 text-[#71717a] px-5 py-3 rounded-2xl text-xs rounded-bl-none italic animate-pulse">
                    Typing...
                  </div>
                </div>
              )}
            </div>

            <div className="p-6 border-t border-[#ffffff08] flex gap-4 shrink-0 bg-[#16161c]/10">
              <input type="text" value={chatInput} onChange={(e) => setChatInput(e.target.value)} onKeyPress={(e) => e.key === 'Enter' && sendChatMessage()} className="flex-1 bg-[#1e1e26]/40 border border-[#ffffff08] rounded-full px-5 py-3 text-xs text-white outline-none focus:border-[#84cc16]" placeholder="Type a message..." />
              <button onClick={sendChatMessage} className="w-12 h-12 rounded-full bg-[#84cc16] hover:bg-[#65a30d] flex items-center justify-center text-black font-bold transition-all shrink-0"><Send className="w-4 h-4" /></button>
            </div>
          </>
        ) : (
          <div className="flex-1 flex flex-col items-center justify-center text-[#71717a] gap-2">
            <MessageSquare className="w-12 h-12 text-[#84cc16]/50 animate-pulse" />
            <p className="text-xs">Select a conversation thread to start real-time chat.</p>
          </div>
        )}
      </div>

    </section>
  );
}
