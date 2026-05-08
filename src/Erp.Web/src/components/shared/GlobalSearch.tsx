import { useState, useRef, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSearch } from '../../hooks/useParties';

export default function GlobalSearch() {
  const [query, setQuery] = useState('');
  const [isOpen, setIsOpen] = useState(false);
  const navigate = useNavigate();
  const ref = useRef<HTMLDivElement>(null);

  const { data: results, isLoading } = useSearch(query);

  // Sluit dropdown bij klik buiten het component
  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleSelect = (entityType: string, id: string) => {
    setQuery('');
    setIsOpen(false);
    // Later uitbreiden per entityType
    if (entityType === 'party') navigate(`/parties/${id}`);
  };

  const entityTypeLabel: Record<string, string> = {
    party: 'Relatie',
    order: 'Order',
    article: 'Artikel',
  };

  return (
    <div ref={ref} className="relative w-full">
      <input
        type="search"
        value={query}
        onChange={(e) => {
          setQuery(e.target.value);
          setIsOpen(true);
        }}
        onFocus={() => query.length >= 2 && setIsOpen(true)}
        placeholder="Zoek op naam, nummer, stad..."
        className="w-full px-4 py-2 text-sm border border-gray-300 rounded-lg bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
      />

      {isOpen && query.length >= 2 && (
        <div className="absolute top-full left-0 right-0 mt-1 bg-white border border-gray-200 rounded-lg shadow-lg z-50 max-h-80 overflow-auto">
          {isLoading && (
            <div className="px-4 py-3 text-sm text-gray-500">Zoeken...</div>
          )}

          {!isLoading && results?.length === 0 && (
            <div className="px-4 py-3 text-sm text-gray-500">
              Geen resultaten voor "{query}"
            </div>
          )}

          {!isLoading && results && results.length > 0 && (
            <ul>
              {results.map((result) => (
                <li key={result.id}>
                  <button
                    onClick={() => handleSelect(result.entityType, result.id)}
                    className="w-full text-left px-4 py-3 hover:bg-gray-50 flex items-center gap-3 border-b border-gray-100 last:border-0"
                  >
                    <span className="text-xs font-medium text-blue-600 bg-blue-50 px-2 py-0.5 rounded w-20 text-center shrink-0">
                      {entityTypeLabel[result.entityType] ?? result.entityType}
                    </span>
                    <div className="min-w-0">
                      <p className="text-sm font-medium text-gray-900 truncate">
                        {result.displayLabel}
                      </p>
                      {result.subtitle && (
                        <p className="text-xs text-gray-500 truncate">{result.subtitle}</p>
                      )}
                    </div>
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}
    </div>
  );
}
