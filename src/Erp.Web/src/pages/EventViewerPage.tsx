import { useState } from 'react';
import { useEventStream } from '../hooks/useEventStream';
import type { LiveEvent } from '../types/events';

const categoryColor: Record<string, string> = {
  Created: 'bg-green-100 text-green-800',
  Updated: 'bg-blue-100 text-blue-800',
  Deactivated: 'bg-red-100 text-red-800',
  Removed: 'bg-red-100 text-red-800',
  PrimarySet: 'bg-amber-100 text-amber-800',
  DefaultSet: 'bg-amber-100 text-amber-800',
};

function badgeColor(eventType: string): string {
  for (const [key, cls] of Object.entries<string>(categoryColor)) {
    if (eventType.includes(key)) return cls;
  }
  return 'bg-gray-100 text-gray-800';
}

export default function EventViewerPage() {
  const { events, isConnected, clearEvents } = useEventStream();
  const [typeFilter, setTypeFilter] = useState('');
  const [aggregateFilter, setAggregateFilter] = useState('');
  const [expanded, setExpanded] = useState<Set<number>>(new Set());

  const filtered = events.filter((e: LiveEvent) =>
    e.eventType.toLowerCase().includes(typeFilter.toLowerCase()) &&
    e.aggregateType.toLowerCase().includes(aggregateFilter.toLowerCase())
  );

  function toggleExpanded(idx: number) {
    setExpanded(prev => {
      const next = new Set(prev);
      next.has(idx) ? next.delete(idx) : next.add(idx);
      return next;
    });
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <h1 className="text-xl font-semibold text-gray-900">Live Events</h1>
          <span className={`inline-flex items-center gap-1.5 text-xs font-medium px-2 py-1 rounded-full ${isConnected ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
            <span className={`w-1.5 h-1.5 rounded-full ${isConnected ? 'bg-green-500' : 'bg-red-500'}`} />
            {isConnected ? 'Connected' : 'Disconnected'}
          </span>
        </div>
        <button
          onClick={clearEvents}
          className="text-sm text-gray-500 hover:text-gray-700 transition-colors"
        >
          Clear
        </button>
      </div>

      <div className="flex gap-3">
        <input
          type="text"
          placeholder="Filter by event type..."
          value={typeFilter}
          onChange={e => setTypeFilter(e.target.value)}
          className="flex-1 border border-gray-300 rounded-md px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <input
          type="text"
          placeholder="Filter by aggregate type..."
          value={aggregateFilter}
          onChange={e => setAggregateFilter(e.target.value)}
          className="flex-1 border border-gray-300 rounded-md px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      {filtered.length === 0 ? (
        <p className="text-sm text-gray-400 py-8 text-center">
          {isConnected ? 'Waiting for events...' : 'Connecting to event stream...'}
        </p>
      ) : (
        <div className="space-y-2">
          {filtered.map((event: LiveEvent, idx: number) => (
            <div key={idx} className="bg-white border border-gray-200 rounded-lg overflow-hidden">
              <button
                onClick={() => toggleExpanded(idx)}
                className="w-full flex items-center gap-3 px-4 py-3 text-left hover:bg-gray-50 transition-colors"
              >
                <span className={`text-xs font-semibold px-2 py-0.5 rounded-full ${badgeColor(event.eventType)}`}>
                  {event.eventType}
                </span>
                <span className="text-sm text-gray-500">{event.aggregateType}</span>
                <span className="text-xs text-gray-400 font-mono">{event.aggregateId}</span>
                <span className="ml-auto text-xs text-gray-400">
                  {new Date(event.occurredAt).toLocaleTimeString()}
                </span>
                <span className="text-gray-400 text-xs">{expanded.has(idx) ? '▲' : '▼'}</span>
              </button>
              {expanded.has(idx) && (
                <div className="px-4 pb-3 border-t border-gray-100">
                  <pre className="text-xs text-gray-600 bg-gray-50 rounded p-3 overflow-auto mt-2">
                    {JSON.stringify(event.payload, null, 2)}
                  </pre>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
