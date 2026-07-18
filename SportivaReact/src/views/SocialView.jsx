import React, { useState, useEffect } from 'react';
import { Image, ThumbsUp, MessageCircle, Trophy, Users, Star, Flame, Sparkles, Send } from 'lucide-react';
import { apiFetch } from '../services/api';

export default function SocialView({ addToast }) {
  const [posts, setPosts] = useState([]);
  const [matches, setMatches] = useState([]);
  const [activeTab, setActiveTab] = useState('feed'); // feed, invites, discussions
  
  const [newPostContent, setNewPostContent] = useState('');
  const [commentsInputs, setCommentsInputs] = useState({});

  const [activePlayers, setActivePlayers] = useState([]);
  const [featuredClubs, setFeaturedClubs] = useState([]);

  useEffect(() => {
    loadSocialFeed();
    loadFriendlyMatches();
    loadActivePlayers();
    loadFeaturedClubs();
  }, []);

  const loadActivePlayers = async () => {
    try {
      const res = await apiFetch('/profiles?pageSize=3');
      if (res.ok) {
        const data = await res.json();
        setActivePlayers(data.items || []);
      }
    } catch (e) {}
  };

  const loadFeaturedClubs = async () => {
    try {
      const res = await apiFetch('/clubs?pageSize=3');
      if (res.ok) {
        const data = await res.json();
        setFeaturedClubs(data.items || []);
      }
    } catch (e) {}
  };

  const loadSocialFeed = async () => {
    try {
      const res = await apiFetch('/posts');
      if (res.ok) {
        const data = await res.json();
        const postsList = data.items || [];
        const postsWithComments = await Promise.all(postsList.map(async (post) => {
          try {
            const commentsRes = await apiFetch(`/posts/${post.id}/comments`);
            if (commentsRes.ok) {
              const commentsData = await commentsRes.json();
              return { ...post, comments: commentsData.items || [] };
            }
          } catch (err) {
            console.error('Error loading comments for post ' + post.id, err);
          }
          return { ...post, comments: [] };
        }));
        setPosts(postsWithComments);
      }
    } catch (e) {}
  };

  const loadFriendlyMatches = async () => {
    try {
      const res = await apiFetch('/friendly-matches');
      if (res.ok) {
        const data = await res.json();
        setMatches(data.items || []);
      }
    } catch (e) {}
  };

  const submitNewPost = async (e) => {
    e.preventDefault();
    if (!newPostContent.trim()) return;
    try {
      const res = await apiFetch('/posts', {
        method: 'POST',
        body: JSON.stringify({ content: newPostContent })
      });
      if (res.ok) {
        addToast('Post published to feed!', 'success');
        setNewPostContent('');
        loadSocialFeed();
      }
    } catch (e) {}
  };

  const toggleLikePost = async (postId) => {
    try {
      const res = await apiFetch(`/posts/${postId}/likes`, { method: 'POST' });
      if (res.ok) {
        loadSocialFeed();
      }
    } catch (e) {}
  };

  const submitComment = async (postId) => {
    const txt = commentsInputs[postId];
    if (!txt || !txt.trim()) return;
    try {
      const res = await apiFetch(`/posts/${postId}/comments`, {
        method: 'POST',
        body: JSON.stringify({ content: txt })
      });
      if (res.ok) {
        setCommentsInputs(prev => ({ ...prev, [postId]: '' }));
        loadSocialFeed();
      }
    } catch (e) {}
  };

  const requestJoinMatch = async (matchId) => {
    try {
      const res = await apiFetch(`/friendly-matches/${matchId}/join-requests`, {
        method: 'POST',
        body: JSON.stringify({ notes: 'Requesting roster join via feed' })
      });
      if (res.ok) {
        addToast('Match join request submitted successfully!', 'success');
        loadFriendlyMatches();
      } else {
        const err = await res.json();
        throw new Error(err.detail || 'Could not join');
      }
    } catch (e) {
      addToast(e.message, 'error');
    }
  };

  return (
    <div className="flex flex-col lg:flex-row gap-8 max-w-[1200px] mx-auto animate-fade-in">
      
      {/* LEFT COLUMN: Post Publishing and Feed items */}
      <div className="flex-1 flex flex-col gap-6">
        <div>
          <h2 className="text-3xl font-extrabold text-white">Community Workspace</h2>
          <p className="text-sm text-[#a1a1aa] mt-1">Interact with local players, share activities, and post ratings</p>
        </div>

        {/* Tab Filters */}
        <div className="bg-[#121216]/50 border border-white/5 p-1.5 rounded-xl flex gap-2 w-fit">
          <button onClick={() => setActiveTab('feed')} className={`px-4 py-2 text-xs font-semibold rounded-lg transition-all ${activeTab === 'feed' ? 'bg-[#84cc16] text-black' : 'text-[#a1a1aa] hover:text-white'}`}>Public Feed</button>
          <button onClick={() => setActiveTab('invites')} className={`px-4 py-2 text-xs font-semibold rounded-lg transition-all ${activeTab === 'invites' ? 'bg-[#84cc16] text-black' : 'text-[#a1a1aa] hover:text-white'}`}>Match Invitations</button>
          <button onClick={() => setActiveTab('discussions')} className={`px-4 py-2 text-xs font-semibold rounded-lg transition-all ${activeTab === 'discussions' ? 'bg-[#84cc16] text-black' : 'text-[#a1a1aa] hover:text-white'}`}>Sports Discussions</button>
        </div>

        {activeTab === 'feed' && (
          <>
            {/* Publish Post Form */}
            <form onSubmit={submitNewPost} className="bg-[#121216]/50 border border-white/5 rounded-2xl p-6 flex flex-col gap-4">
              <textarea value={newPostContent} onChange={(e) => setNewPostContent(e.target.value)} placeholder="Share your latest match details or facility feedback..." className="w-full bg-[#1e1e26]/30 border border-white/5 rounded-xl p-4 text-xs text-white outline-none resize-none h-[90px] focus:border-[#84cc16]" />
              <div className="flex justify-between items-center">
                <span className="text-xs text-[#71717a] flex items-center gap-1.5"><Image className="w-4 h-4" /> Share Photo</span>
                <button type="submit" className="px-5 py-2.5 rounded-xl bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs transition-colors shadow-lg shadow-[#84cc16]/10">Post Feed</button>
              </div>
            </form>

            {/* Social Posts Renders */}
            <div className="flex flex-col gap-6">
              {posts.length === 0 ? (
                <p className="text-xs text-[#71717a] text-center py-6">No community posts yet. Be the first to publish!</p>
              ) : (
                posts.map(post => (
                  <div key={post.id} className="bg-[#121216]/40 border border-white/5 rounded-2xl p-6 flex flex-col gap-4">
                    <div className="flex items-center gap-3">
                      {post.author?.profilePictureUrl ? (
                        <img className="w-10 h-10 rounded-full border border-white/10 object-cover" src={post.author.profilePictureUrl} alt="" />
                      ) : (
                        <div className="w-10 h-10 rounded-full border border-white/10 bg-[#84cc16]/10 flex items-center justify-center font-bold text-[#84cc16] text-sm">
                          {post.author?.fullName ? post.author.fullName.charAt(0) : 'P'}
                        </div>
                      )}
                      <div>
                        <h4 className="text-xs font-bold text-white">{post.author?.fullName || 'Sportify Player'}</h4>
                        <span className="text-[10px] text-[#71717a]">{new Date(post.createdAt).toLocaleDateString()}</span>
                      </div>
                    </div>

                    <p className="text-xs leading-relaxed text-white/90">{post.content}</p>

                    <div className="flex items-center gap-6 border-y border-white/5 py-3 text-[#a1a1aa] text-[10px] uppercase font-bold tracking-wider">
                      <button onClick={() => toggleLikePost(post.id)} className="hover:text-white flex items-center gap-1.5 transition-all"><ThumbsUp className={`w-3.5 h-3.5 ${post.isLiked ? 'text-[#84cc16] fill-[#84cc16]' : ''}`} /> {post.likesCount || 0} Likes</button>
                      <span className="flex items-center gap-1.5"><MessageCircle className="w-3.5 h-3.5" /> {post.comments?.length || 0} Comments</span>
                    </div>

                    {/* Comments list */}
                    <div className="flex flex-col gap-3 pl-4 border-l border-white/5">
                      {post.comments?.map(c => (
                        <div key={c.id} className="text-xs bg-[#1e1e26]/20 p-3 rounded-xl border border-white/5">
                          <span className="font-bold text-[#84cc16] block mb-0.5">{c.author?.fullName || 'Sportify Member'}</span>
                          <p className="text-[#a1a1aa]">{c.content}</p>
                        </div>
                      ))}
                      <div className="flex gap-3 mt-1">
                        <input type="text" value={commentsInputs[post.id] || ''} onChange={(e) => setCommentsInputs({...commentsInputs, [post.id]: e.target.value})} onKeyPress={(e) => e.key === 'Enter' && submitComment(post.id)} className="flex-1 bg-[#1e1e26]/30 border border-white/5 rounded-xl px-4 py-2.5 text-xs outline-none text-white focus:border-[#84cc16]" placeholder="Write a comment..." />
                        <button onClick={() => submitComment(post.id)} className="px-4 bg-[#84cc16] hover:bg-[#65a30d] text-black font-bold text-xs rounded-xl transition-all">Comment</button>
                      </div>
                    </div>
                  </div>
                ))
              )}
            </div>
          </>
        )}

        {activeTab === 'invites' && (
          <div className="flex flex-col gap-6">
            {matches.length === 0 ? (
              <p className="text-xs text-[#71717a] text-center py-6">No match invitations active right now.</p>
            ) : (
              matches.map(m => (
                <div key={m.matchId} className="bg-[#121216]/50 border border-white/5 rounded-2xl p-6 flex flex-col justify-between gap-4">
                  <div className="flex justify-between items-start">
                    <div>
                      <span className="px-2.5 py-0.5 bg-[#84cc16]/10 border border-[#84cc16]/20 text-[#a3e635] rounded text-[10px] font-bold uppercase">{m.sportType}</span>
                      <h4 className="text-sm font-bold text-white mt-2">Inviting Extra Players for {m.sportType} Roster</h4>
                      <p className="text-xs text-[#a1a1aa] mt-1">Play Date: {m.date} | Slots Remaining: <b className="text-[#84cc16]">{m.slotsRemaining}</b></p>
                    </div>
                    <img className="w-10 h-10 rounded-full border border-white/10" src="https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=80&q=80" alt="avatar" />
                  </div>
                  <div className="flex justify-between items-center border-t border-white/5 pt-3">
                    <span className="text-xs text-[#71717a]">Hosted by <b className="text-white">{m.organizer?.name || 'Player'}</b></span>
                    <button onClick={() => requestJoinMatch(m.matchId)} className="px-4 py-2 bg-[#84cc16] hover:bg-[#65a30d] text-black font-extrabold text-xs rounded-lg transition-colors">Join Roster</button>
                  </div>
                </div>
              ))
            )}
          </div>
        )}

        {activeTab === 'discussions' && (
          <div className="text-center py-12 bg-[#121216]/40 border border-white/5 rounded-2xl flex flex-col items-center gap-2">
            <MessageCircle className="w-8 h-8 text-[#71717a]/50" />
            <h4 className="text-xs font-bold text-white">Discussions Feed coming soon</h4>
            <p className="text-[10px] text-[#71717a] max-w-xs">Community chat rooms and sports forums will be enabled in a future release.</p>
          </div>
        )}
      </div>

      {/* RIGHT COLUMN: Sidebar Panels */}
      <div className="w-full lg:w-[320px] flex flex-col gap-6 shrink-0">
        
        {/* Trending Players */}
        <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4">
          <h3 className="font-extrabold text-white text-sm flex items-center gap-2">
            <Flame className="w-4 h-4 text-[#84cc16] animate-pulse" />
            <span>Active Players</span>
          </h3>
          <div className="flex flex-col gap-3.5">
            {activePlayers.length === 0 ? (
              <p className="text-xs text-[#71717a]">No active players found.</p>
            ) : (
              activePlayers.map((p) => (
                <div key={p.id} className="flex items-center gap-3 bg-[#1e1e26]/30 border border-white/5 p-3 rounded-xl">
                  {p.profilePictureUrl ? (
                    <img className="w-8 h-8 rounded-full object-cover" src={p.profilePictureUrl} alt="" />
                  ) : (
                    <div className="w-8 h-8 rounded-full bg-[#84cc16]/10 flex items-center justify-center font-bold text-[#84cc16] text-xs">
                      {p.fullName ? p.fullName.charAt(0) : 'P'}
                    </div>
                  )}
                  <div className="flex-1 min-w-0">
                    <h4 className="text-xs font-bold text-white truncate">{p.fullName}</h4>
                    <p className="text-[9px] text-[#71717a] truncate mt-0.5">{p.city || 'Egypt'}</p>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        {/* Featured Clubs */}
        <div className="bg-[#121216]/50 border border-white/5 p-6 rounded-2xl flex flex-col gap-4">
          <h3 className="font-extrabold text-white text-sm flex items-center gap-2">
            <Trophy className="w-4 h-4 text-[#84cc16]" />
            <span>Featured Complexes</span>
          </h3>
          <div className="flex flex-col gap-3.5">
            {featuredClubs.length === 0 ? (
              <p className="text-xs text-[#71717a]">No complexes registered.</p>
            ) : (
              featuredClubs.map((c) => (
                <div key={c.id} className="flex flex-col gap-1 bg-[#1e1e26]/30 border border-white/5 p-3.5 rounded-xl">
                  <h4 className="text-xs font-bold text-white">{c.name}</h4>
                  <span className="text-[9px] text-[#71717a]">{c.city}, {c.governorate}</span>
                </div>
              ))
            )}
          </div>
        </div>

      </div>

    </div>
  );
}
