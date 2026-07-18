import React, { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { getApiBaseUrl } from '../services/api';

const SocketContext = createContext(null);

export const useSocket = () => useContext(SocketContext);

export const SocketProvider = ({ children, token }) => {
    const [notificationHub, setNotificationHub] = useState(null);
    const [chatHub, setChatHub] = useState(null);
    const [notifConnected, setNotifConnected] = useState(false);
    const [chatConnected, setChatConnected] = useState(false);

    useEffect(() => {
        if (!token) {
            setNotificationHub(null);
            setChatHub(null);
            return;
        }

        const apiBase = getApiBaseUrl();
        const hubBase = `${apiBase}/hubs`;

        // 1. Notification Connection Setup
        const nHub = new signalR.HubConnectionBuilder()
            .withUrl(`${hubBase}/notifications`, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        nHub.start()
            .then(() => {
                console.log('Connected to Notification Hub');
                setNotifConnected(true);
            })
            .catch(err => {
                console.error('Notification Hub error:', err);
                setNotifConnected(false);
            });

        nHub.onclose(() => setNotifConnected(false));
        nHub.onreconnected(() => setNotifConnected(true));
        setNotificationHub(nHub);

        // 2. Chat Connection Setup
        const cHub = new signalR.HubConnectionBuilder()
            .withUrl(`${hubBase}/chat`, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        cHub.start()
            .then(() => {
                console.log('Connected to Chat Hub');
                setChatConnected(true);
            })
            .catch(err => {
                console.error('Chat Hub error:', err);
                setChatConnected(false);
            });

        cHub.onclose(() => setChatConnected(false));
        cHub.onreconnected(() => setChatConnected(true));
        setChatHub(cHub);

        return () => {
            nHub.stop();
            cHub.stop();
        };
    }, [token]);

    return (
        <SocketContext.Provider value={{ notificationHub, chatHub, notifConnected, chatConnected }}>
            {children}
        </SocketContext.Provider>
    );
};
