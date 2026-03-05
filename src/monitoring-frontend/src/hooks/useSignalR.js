import { useEffect, useRef, useState } from 'react';
import * as signalR from '@microsoft/signalr';

const HUB_URL = '/metrics';
const LIVE_WINDOW_MS = 5 * 60 * 1000; // 5 минут

export function useSignalR(isLive) {
  const [liveData, setLiveData] = useState([]);
  const [connectionState, setConnectionState] = useState('Disconnected');
  const connectionRef = useRef(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect()
      .build();

    connectionRef.current = connection;

    connection.onreconnecting(() => setConnectionState('Reconnecting'));
    connection.onreconnected(() => setConnectionState('Connected'));
    connection.onclose(() => setConnectionState('Disconnected'));

    connection.on('ReceiveMetric', (metric) => {
      setLiveData((prev) => {
        const now = Date.now();
        const updated = [...prev, metric].filter(
          (m) => now - new Date(m.ts).getTime() <= LIVE_WINDOW_MS
        );
        return updated;
      });
    });

    connection
      .start()
      .then(async () => {
        setConnectionState('Connected');
        await connection.invoke('JoinHub');
      })
      .catch(() => setConnectionState('Disconnected'));

    return () => {
      connection.invoke('LeaveHub').catch(() => {}).finally(() => connection.stop());
    };
  }, []);

  // Очищаем live данные когда уходим с live режима
  useEffect(() => {
    if (!isLive) setLiveData([]);
  }, [isLive]);

  return { liveData, connectionState };
}