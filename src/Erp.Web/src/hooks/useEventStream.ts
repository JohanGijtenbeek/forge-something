import { useEffect, useRef, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import type { LiveEvent } from '../types/events';

const MAX_EVENTS = 500;
const CONSUMER_URL = import.meta.env.VITE_CONSUMER_URL ?? 'http://localhost:5002';

export function useEventStream() {
  const [events, setEvents] = useState<LiveEvent[]>([]);
  const [isConnected, setIsConnected] = useState(false);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${CONSUMER_URL}/hubs/events`)
      .withAutomaticReconnect()
      .build();

    connection.on('EventReceived', (event: LiveEvent) => {
      setEvents(prev => [event, ...prev].slice(0, MAX_EVENTS));
    });

    connection.onclose(() => setIsConnected(false));
    connection.onreconnecting(() => setIsConnected(false));
    connection.onreconnected(() => setIsConnected(true));

    connection.start()
      .then(() => setIsConnected(true))
      .catch(err => console.error('SignalR connection failed:', err));

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, []);

  const clearEvents = () => setEvents([]);

  return { events, isConnected, clearEvents };
}
