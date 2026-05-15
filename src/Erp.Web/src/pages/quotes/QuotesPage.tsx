import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuotes, useCreateQuote } from '../../hooks/useQuotes';
import type { QuoteStatus, CreateQuoteRequest } from '../../types/api';

const PAGE_SIZE = 25;

const statusLabel: Record<QuoteStatus, string> = {
  draft: 'Concept',
  sent: 'Verstuurd',
  accepted: 'Geaccepteerd',
  rejected: 'Afgewezen',
};

const statusBadgeClass: Record<QuoteStatus, string> = {
  draft: 'bg-gray-100 text-gray-600',
  sent: 'bg-blue-100 text-blue-700',
  accepted: 'bg-green-100 text-green-700',
  rejected: 'bg-red-100 text-red-600',
};

const defaultForm: CreateQuoteRequest = {
  customerId: null,
  date: new Date().toISOString().split('T')[0],
  reference: null,
  contactPerson: null,
  deliveryTime: null,
  hourlyRate: 72,
  materialMargin: 115,
  standardMargin: 11,
  setupTime: 1,
};

export default function QuotesPage() {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState('');
  const [showCreate, setShowCreate] = useState(false);
  const [form, setForm] = useState<CreateQuoteRequest>(defaultForm);

  const { data, isLoading, isError } = useQuotes({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    status: status || undefined,
  });

  const createQuote = useCreateQuote();

  const handleFilterChange = (fn: () => void) => {
    fn();
    setPage(1);
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    const result = await createQuote.mutateAsync(form);
    setShowCreate(false);
    setForm(defaultForm);
    navigate(`/quotes/${result.id}`);
  };

  if (isLoading) return <div className="text-sm text-gray-500">Laden...</div>;
  if (isError) return <div className="text-sm text-red-500">Fout bij ophalen van offertes.</div>;

  const totalPages = data?.totalPages ?? 1;
  const totalCount = data?.totalCount ?? 0;

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold text-gray-900">Offertes</h1>
          <p className="text-sm text-gray-500 mt-0.5">{totalCount} resultaten</p>
        </div>
        <button
          onClick={() => setShowCreate(true)}
          className="px-3 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700"
        >
          + Offerte
        </button>
      </div>

      {/* Filters */}
      <div className="flex gap-3 items-center flex-wrap">
        <input
          type="search"
          value={search}
          onChange={e => handleFilterChange(() => setSearch(e.target.value))}
          placeholder="Zoeken op nummer, klant, onderdeel..."
          className="flex-1 min-w-48 max-w-sm px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <select
          value={status}
          onChange={e => handleFilterChange(() => setStatus(e.target.value))}
          className="px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
        >
          <option value="">Alle statussen</option>
          <option value="draft">Concept</option>
          <option value="sent">Verstuurd</option>
          <option value="accepted">Geaccepteerd</option>
          <option value="rejected">Afgewezen</option>
        </select>
      </div>

      {/* Table */}
      <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 border-b border-gray-200">
            <tr>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Offerte #</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Klant</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Datum</th>
              <th className="text-right px-4 py-3 font-medium text-gray-600">Regels</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Status</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Aangemaakt</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {data?.items.map(quote => (
              <tr
                key={quote.id}
                className="hover:bg-gray-50 cursor-pointer"
                onClick={() => navigate(`/quotes/${quote.id}`)}
              >
                <td className="px-4 py-3 font-mono font-medium text-gray-900">#{quote.quoteNumber}</td>
                <td className="px-4 py-3 text-gray-700">{quote.customerName ?? '—'}</td>
                <td className="px-4 py-3 text-gray-500">
                  {new Date(quote.date).toLocaleDateString('nl-NL')}
                </td>
                <td className="px-4 py-3 text-right font-mono text-gray-700">{quote.lineCount}</td>
                <td className="px-4 py-3">
                  <span className={`text-xs px-2 py-0.5 rounded-full ${statusBadgeClass[quote.status as QuoteStatus]}`}>
                    {statusLabel[quote.status as QuoteStatus] ?? quote.status}
                  </span>
                </td>
                <td className="px-4 py-3 text-gray-400 text-xs">
                  {new Date(quote.createdAt).toLocaleDateString('nl-NL')}
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {data?.items.length === 0 && (
          <div className="px-4 py-8 text-center text-sm text-gray-500">
            Geen offertes gevonden.
          </div>
        )}

        {totalPages > 1 && (
          <div className="flex items-center justify-between px-4 py-3 border-t border-gray-200 bg-gray-50">
            <p className="text-sm text-gray-500">
              Pagina {page} van {totalPages}
            </p>
            <div className="flex gap-2">
              <button
                onClick={() => setPage(p => Math.max(1, p - 1))}
                disabled={page === 1}
                className="px-3 py-1.5 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed"
              >
                Vorige
              </button>
              <button
                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
                className="px-3 py-1.5 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed"
              >
                Volgende
              </button>
            </div>
          </div>
        )}
      </div>

      {/* Create modal */}
      {showCreate && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6 space-y-4">
            <h2 className="text-lg font-semibold text-gray-900">Nieuwe offerte</h2>
            <form onSubmit={handleCreate} className="space-y-3">
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Datum</label>
                  <input
                    type="date"
                    value={form.date}
                    onChange={e => setForm(f => ({ ...f, date: e.target.value }))}
                    required
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Referentie</label>
                  <input
                    type="text"
                    value={form.reference ?? ''}
                    onChange={e => setForm(f => ({ ...f, reference: e.target.value || null }))}
                    placeholder="Optioneel"
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Contactpersoon</label>
                  <input
                    type="text"
                    value={form.contactPerson ?? ''}
                    onChange={e => setForm(f => ({ ...f, contactPerson: e.target.value || null }))}
                    placeholder="Optioneel"
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Levertijd</label>
                  <input
                    type="text"
                    value={form.deliveryTime ?? ''}
                    onChange={e => setForm(f => ({ ...f, deliveryTime: e.target.value || null }))}
                    placeholder="bv. 4 weken"
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Uurloon (€)</label>
                  <input
                    type="number"
                    step="0.01"
                    value={form.hourlyRate}
                    onChange={e => setForm(f => ({ ...f, hourlyRate: parseFloat(e.target.value) }))}
                    required
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Insteltijd (uur)</label>
                  <input
                    type="number"
                    step="0.25"
                    value={form.setupTime}
                    onChange={e => setForm(f => ({ ...f, setupTime: parseFloat(e.target.value) }))}
                    required
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Materiaalopslag (%)</label>
                  <input
                    type="number"
                    step="0.1"
                    value={form.materialMargin}
                    onChange={e => setForm(f => ({ ...f, materialMargin: parseFloat(e.target.value) }))}
                    required
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Standaard opslag (%)</label>
                  <input
                    type="number"
                    step="0.1"
                    value={form.standardMargin}
                    onChange={e => setForm(f => ({ ...f, standardMargin: parseFloat(e.target.value) }))}
                    required
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
              </div>
              <div className="flex justify-end gap-2 pt-2">
                <button
                  type="button"
                  onClick={() => { setShowCreate(false); setForm(defaultForm); }}
                  className="px-3 py-2 text-sm text-gray-600 hover:text-gray-900"
                >
                  Annuleren
                </button>
                <button
                  type="submit"
                  disabled={createQuote.isPending}
                  className="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50"
                >
                  {createQuote.isPending ? 'Opslaan...' : 'Aanmaken'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
