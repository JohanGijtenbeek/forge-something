import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useParties, useDeactivateParty } from '../../hooks/useParties';
import type { PartyListResponse } from '../../types/api';

export default function PartiesPage() {
  const navigate = useNavigate();
  const [includeInactive, setIncludeInactive] = useState(false);
  const [search, setSearch] = useState('');

  const { data: parties, isLoading, isError } = useParties(includeInactive);
  const deactivate = useDeactivateParty();

  const filtered = parties?.filter((p) =>
    p.name.toLowerCase().includes(search.toLowerCase()) ||
    p.city?.toLowerCase().includes(search.toLowerCase())
  );

  const handleDeactivate = async (party: PartyListResponse) => {
    if (!confirm(`${party.name} deactiveren?`)) return;
    await deactivate.mutateAsync(party.id);
  };

  if (isLoading) return <div className="text-sm text-gray-500">Laden...</div>;
  if (isError) return <div className="text-sm text-red-500">Fout bij ophalen van relaties.</div>;

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold text-gray-900">Relaties</h1>
          <p className="text-sm text-gray-500 mt-0.5">{filtered?.length ?? 0} resultaten</p>
        </div>
        <div className="flex gap-2">
          <Link
            to="/parties/new/organization"
            className="px-3 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700"
          >
            + Organisatie
          </Link>
          <Link
            to="/parties/new/person"
            className="px-3 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
          >
            + Persoon
          </Link>
        </div>
      </div>

      {/* Filters */}
      <div className="flex gap-3 items-center">
        <input
          type="search"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Filteren op naam of stad..."
          className="flex-1 max-w-sm px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <label className="flex items-center gap-2 text-sm text-gray-600 cursor-pointer">
          <input
            type="checkbox"
            checked={includeInactive}
            onChange={(e) => setIncludeInactive(e.target.checked)}
            className="rounded"
          />
          Inclusief inactief
        </label>
      </div>

      {/* Tabel */}
      <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 border-b border-gray-200">
            <tr>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Naam</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Type</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Stad</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Rollen</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Status</th>
              <th className="px-4 py-3" />
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {filtered?.map((party) => (
              <tr
                key={party.id}
                className="hover:bg-gray-50 cursor-pointer"
                onClick={() => navigate(`/parties/${party.id}`)}
              >
                <td className="px-4 py-3 font-medium text-gray-900">{party.name}</td>
                <td className="px-4 py-3 text-gray-500">
                  {party.partyType === 'Organization' ? 'Organisatie' : 'Persoon'}
                </td>
                <td className="px-4 py-3 text-gray-500">{party.city ?? '—'}</td>
                <td className="px-4 py-3">
                  <div className="flex gap-1">
                    {party.isCustomer && (
                      <span className="text-xs px-2 py-0.5 rounded-full bg-blue-100 text-blue-700">Klant</span>
                    )}
                    {party.isSupplier && (
                      <span className="text-xs px-2 py-0.5 rounded-full bg-green-100 text-green-700">Leverancier</span>
                    )}
                  </div>
                </td>
                <td className="px-4 py-3">
                  <span className={`text-xs px-2 py-0.5 rounded-full ${
                    party.isActive
                      ? 'bg-gray-100 text-gray-600'
                      : 'bg-red-100 text-red-600'
                  }`}>
                    {party.isActive ? 'Actief' : 'Inactief'}
                  </span>
                </td>
                <td className="px-4 py-3 text-right" onClick={(e) => e.stopPropagation()}>
                  {party.isActive && (
                    <button
                      onClick={() => handleDeactivate(party)}
                      className="text-xs text-red-500 hover:text-red-700"
                    >
                      Deactiveer
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {filtered?.length === 0 && (
          <div className="px-4 py-8 text-center text-sm text-gray-500">
            Geen relaties gevonden.
          </div>
        )}
      </div>
    </div>
  );
}
