import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCustomers } from '../../hooks/useParties';

export default function CustomersPage() {
  const navigate = useNavigate();
  const [includeInactive, setIncludeInactive] = useState(false);
  const [search, setSearch] = useState('');
  const { data: customers, isLoading } = useCustomers(includeInactive);

  const filtered = customers?.filter(p =>
    p.name.toLowerCase().includes(search.toLowerCase()) ||
    p.city?.toLowerCase().includes(search.toLowerCase())
  );

  if (isLoading) return <div className="text-sm text-gray-500">Laden...</div>;

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold text-gray-900">Klanten</h1>
          <p className="text-sm text-gray-500 mt-0.5">{filtered?.length ?? 0} klanten</p>
        </div>
      </div>

      <div className="flex gap-3 items-center">
        <input
          type="search"
          value={search}
          onChange={e => setSearch(e.target.value)}
          placeholder="Filteren op naam of stad..."
          className="flex-1 max-w-sm px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <label className="flex items-center gap-2 text-sm text-gray-600 cursor-pointer">
          <input type="checkbox" checked={includeInactive} onChange={e => setIncludeInactive(e.target.checked)} className="rounded" />
          Inclusief inactief
        </label>
      </div>

      <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 border-b border-gray-200">
            <tr>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Naam</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Stad</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Status</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {filtered?.map(party => (
              <tr key={party.id} className="hover:bg-gray-50 cursor-pointer" onClick={() => navigate(`/parties/${party.id}`)}>
                <td className="px-4 py-3 font-medium text-gray-900">{party.name}</td>
                <td className="px-4 py-3 text-gray-500">{party.city ?? '—'}</td>
                <td className="px-4 py-3">
                  <span className={`text-xs px-2 py-0.5 rounded-full ${party.isActive ? 'bg-gray-100 text-gray-600' : 'bg-red-100 text-red-600'}`}>
                    {party.isActive ? 'Actief' : 'Inactief'}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {filtered?.length === 0 && (
          <div className="px-4 py-8 text-center text-sm text-gray-500">Geen klanten gevonden.</div>
        )}
      </div>
    </div>
  );
}
