// Sportify React Dashboard API service helper
const API_BASE = window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1'
    ? 'http://localhost:5250'
    : window.location.origin;

export const getApiBaseUrl = () => API_BASE;

function normalizeResponseData(data) {
    if (data === null || data === undefined) return data;

    if (Array.isArray(data)) {
        return data.map(normalizeResponseData);
    }

    if (typeof data === 'object') {
        const normalized = {};
        for (const key of Object.keys(data)) {
            normalized[key] = normalizeResponseData(data[key]);
        }

        // Set standard 'id' alias if entity-specific ID properties exist
        if (normalized.clubId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.clubId;
        }
        if (normalized.courtId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.courtId;
        }
        if (normalized.tournamentId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.tournamentId;
        }
        if (normalized.matchId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.matchId;
        }
        if (normalized.timeSlotId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.timeSlotId;
        }
        if (normalized.reviewId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.reviewId;
        }
        if (normalized.bookingId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.bookingId;
        }
        if (normalized.postId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.postId;
        }
        if (normalized.notificationId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.notificationId;
        }
        if (normalized.messageId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.messageId;
        }
        if (normalized.userId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.userId;
        }
        if (normalized.requestId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.requestId;
        }
        if (normalized.commentId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.commentId;
        }
        if (normalized.replyId !== undefined && normalized.id === undefined) {
            normalized.id = normalized.replyId;
        }

        // Notification body alias
        if (normalized.body !== undefined && normalized.message === undefined) {
            normalized.message = normalized.body;
        }

        // Booking properties mappings
        if (normalized.bookedBy !== undefined) {
            if (normalized.playerName === undefined) normalized.playerName = normalized.bookedBy.fullName;
            if (normalized.playerEmail === undefined) normalized.playerEmail = normalized.bookedBy.email;
        }
        if (normalized.court !== undefined) {
            if (normalized.courtName === undefined) normalized.courtName = normalized.court.name;
        }
        if (normalized.price !== undefined && normalized.totalPrice === undefined) {
            normalized.totalPrice = normalized.price;
        }
        if (normalized.timeSlot !== undefined) {
            if (normalized.date === undefined) normalized.date = normalized.timeSlot.day;
            if (normalized.startTime === undefined) normalized.startTime = normalized.timeSlot.startTime;
            if (normalized.endTime === undefined) normalized.endTime = normalized.timeSlot.endTime;
            if (normalized.timeSlotText === undefined) normalized.timeSlotText = `${normalized.timeSlot.startTime?.substring(0, 5)} - ${normalized.timeSlot.endTime?.substring(0, 5)}`;
        }

        // Friendly matches property mappings
        if (normalized.matchId !== undefined || normalized.requiredPlayers !== undefined) {
            if (normalized.organizer && normalized.creatorName === undefined) {
                normalized.creatorName = normalized.organizer.fullName;
            }
            if (normalized.acceptedPlayersCount !== undefined && normalized.currentPlayersCount === undefined) {
                normalized.currentPlayersCount = normalized.acceptedPlayersCount;
            }
            if (normalized.requiredPlayers !== undefined && normalized.maxPlayersCount === undefined) {
                normalized.maxPlayersCount = normalized.requiredPlayers;
            }
            if (normalized.court && normalized.court.pricePerHour !== undefined && normalized.pricePerPlayer === undefined) {
                normalized.pricePerPlayer = normalized.requiredPlayers > 0 
                    ? Math.round(normalized.court.pricePerHour / normalized.requiredPlayers)
                    : 0;
            }
        }

        // Reviews property mappings
        if (normalized.reviewId !== undefined && normalized.author !== undefined) {
            if (normalized.playerName === undefined) {
                normalized.playerName = normalized.author.fullName;
            }
        }

        // Tournaments property mappings
        if (normalized.tournamentId !== undefined || normalized.maxParticipants !== undefined) {
            if (normalized.participantsCount !== undefined && normalized.registeredTeamsCount === undefined) {
                normalized.registeredTeamsCount = normalized.participantsCount;
            }
            if (normalized.maxParticipants !== undefined && normalized.maxTeams === undefined) {
                normalized.maxTeams = normalized.maxParticipants;
            }
        }

        // Boolean mappings for friendly matches
        if (normalized.iApplied !== undefined && normalized.iapplied === undefined) {
            normalized.iapplied = normalized.iApplied;
        }
        if (normalized.iParticipating !== undefined && normalized.iparticipating === undefined) {
            normalized.iparticipating = normalized.iParticipating;
        }

        return normalized;
    }

    return data;
}

export async function apiFetch(endpoint, options = {}) {
    const token = localStorage.getItem('token');
    const headers = {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` }),
        ...options.headers
    };

    const res = await fetch(`${API_BASE}${endpoint}`, { ...options, headers });
    
    if (res.status === 401) {
        localStorage.clear();
        window.location.reload();
        throw new Error('Session expired. Please log in again.');
    }

    // Intercept res.json to normalize entity IDs
    const originalJson = res.json.bind(res);
    res.json = async () => {
        const data = await originalJson();
        return normalizeResponseData(data);
    };
    
    return res;
}

