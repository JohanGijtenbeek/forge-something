import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import {
  useQuote,
  useQuoteHistory,
  useUpdateQuoteStatus,
  useDeleteQuote,
  useAddQuoteLine,
  useUpdateQuoteLine,
  useRemoveQuoteLine,
  useAcceptQuoteLine,
  useConvertQuote,
} from '../../hooks/useQuotes';
import type { QuoteStatus, QuoteLineResponse, AddQuoteLineRequest } from '../../types/api';

type Tab = 'lines' | 'details' | 'history';

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

const eventTypeLabel: Record<string, string> = {
  QuoteCreated: 'Aangemaakt',
  QuoteStatusChanged: 'Status gewijzigd',
  QuoteConverted: 'Omgezet naar orders',
};

const emptyLine: AddQuoteLineRequest = {
  sortOrder: 0,
  partName: '',
  partNumber: '',
  quantity: 1,
  articleId: null,
  materialType: null,
  materialCode: null,
  materialCode2: null,
  materialGeometry: null,
  materialSizeMm: null,
  materialLengthMm: null,
  materialQuantity: null,
  materialPrice: null,
  materialSource: 'inclusive',
  operationCount: 0,
  operationTimeMinutes: 0,
  subcontractingCount: 0,
  subcontractingPrice: 0,
  isManualPrice: false,
  manualPrice: null,
  remarks: null,
};

export default function QuoteDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [tab, setTab] = useState<Tab>('lines');
  const [showAddLine, setShowAddLine] = useState(false);
  const [lineForm, setLineForm] = useState<AddQuoteLineRequest>(emptyLine);
  const [editingLine, setEditingLine] = useState<QuoteLineResponse | null>(null);
  const [convertResult, setConvertResult] = useState<string[] | null>(null);

  const { data: quote, isLoading, isError } = useQuote(id!);
  const { data: history } = useQuoteHistory(id!);

  const updateStatus = useUpdateQuoteStatus(id!);
  const deleteQuote = useDeleteQuote();
  const addLine = useAddQuoteLine(id!);
  const updateLine = useUpdateQuoteLine(id!);
  const removeLine = useRemoveQuoteLine(id!);
  const acceptLine = useAcceptQuoteLine(id!);
  const convertQuote = useConvertQuote(id!);

  const handleStatusTransition = async (status: string) => {
    await updateStatus.mutateAsync({ status });
  };

  const handleDelete = async () => {
    if (!confirm(`Offerte #${quote?.quoteNumber} afwijzen?`)) return;
    await deleteQuote.mutateAsync(id!);
    navigate('/quotes');
  };

  const handleAddLine = async (e: React.FormEvent) => {
    e.preventDefault();
    if (editingLine) {
      await updateLine.mutateAsync({ lineId: editingLine.id, request: lineForm });
    } else {
      await addLine.mutateAsync(lineForm);
    }
    setShowAddLine(false);
    setEditingLine(null);
    setLineForm(emptyLine);
  };

  const openEdit = (line: QuoteLineResponse) => {
    setEditingLine(line);
    setLineForm({
      sortOrder: line.sortOrder,
      partName: line.partName,
      partNumber: line.partNumber,
      quantity: line.quantity,
      articleId: line.articleId,
      materialType: line.materialType,
      materialCode: line.materialCode,
      materialCode2: line.materialCode2,
      materialGeometry: line.materialGeometry,
      materialSizeMm: line.materialSizeMm,
      materialLengthMm: line.materialLengthMm,
      materialQuantity: line.materialQuantity,
      materialPrice: line.materialPrice,
      materialSource: line.materialSource,
      operationCount: line.operationCount,
      operationTimeMinutes: line.operationTimeMinutes,
      subcontractingCount: line.subcontractingCount,
      subcontractingPrice: line.subcontractingPrice,
      isManualPrice: line.isManualPrice,
      manualPrice: line.manualPrice,
      remarks: line.remarks,
    });
    setShowAddLine(true);
  };

  const handleConvert = async () => {
    if (!confirm('Offerte omzetten naar productieorders?')) return;
    try {
      const result = await convertQuote.mutateAsync();
      setConvertResult(result.createdOrderIds);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Onbekende fout bij omzetten.';
      alert(message);
    }
  };

  if (isLoading) return <div className="text-sm text-gray-500">Laden...</div>;
  if (isError || !quote) return <div className="text-sm text-red-500">Offerte niet gevonden.</div>;

  const status = quote.status as QuoteStatus;
  const isTerminal = status === 'accepted' || status === 'rejected';
  const acceptedLines = quote.lines.filter(l => l.isAccepted);
  const canConvert = status === 'accepted' && acceptedLines.length > 0;

  return (
    <div className="space-y-6 max-w-5xl">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div>
          <div className="flex items-center gap-2 text-sm text-gray-500 mb-1">
            <Link to="/quotes" className="hover:text-gray-700">Offertes</Link>
            <span>/</span>
            <span className="font-mono">#{quote.quoteNumber}</span>
          </div>
          <h1 className="text-xl font-semibold text-gray-900">
            {quote.customerName ?? 'Geen klant'} — Offerte #{quote.quoteNumber}
          </h1>
          <div className="flex gap-2 mt-2">
            <span className={`text-xs px-2 py-0.5 rounded-full ${statusBadgeClass[status]}`}>
              {statusLabel[status]}
            </span>
            <span className="text-xs px-2 py-0.5 rounded-full bg-gray-100 text-gray-500">
              {quote.lines.length} regel{quote.lines.length !== 1 ? 's' : ''}
            </span>
          </div>
        </div>

        <div className="flex gap-2 flex-wrap justify-end">
          {status === 'draft' && (
            <button
              onClick={() => handleStatusTransition('sent')}
              disabled={updateStatus.isPending}
              className="text-sm font-medium text-white bg-blue-600 px-3 py-1.5 rounded-lg hover:bg-blue-700 disabled:opacity-50"
            >
              Versturen
            </button>
          )}
          {status === 'sent' && (
            <button
              onClick={() => handleStatusTransition('accepted')}
              disabled={updateStatus.isPending}
              className="text-sm font-medium text-white bg-green-600 px-3 py-1.5 rounded-lg hover:bg-green-700 disabled:opacity-50"
            >
              Accepteren
            </button>
          )}
          {canConvert && (
            <button
              onClick={handleConvert}
              disabled={convertQuote.isPending}
              className="text-sm font-medium text-white bg-purple-600 px-3 py-1.5 rounded-lg hover:bg-purple-700 disabled:opacity-50"
            >
              Omzetten naar orders
            </button>
          )}
          {!isTerminal && (
            <button
              onClick={handleDelete}
              disabled={deleteQuote.isPending}
              className="text-sm text-red-500 hover:text-red-700 border border-red-200 px-3 py-1.5 rounded-lg hover:bg-red-50 disabled:opacity-50"
            >
              Afwijzen
            </button>
          )}
        </div>
      </div>

      {/* Conversion result banner */}
      {convertResult && (
        <div className="bg-green-50 border border-green-200 rounded-lg p-4 text-sm text-green-800">
          <p className="font-medium mb-1">Offerte omgezet — {convertResult.length} order{convertResult.length !== 1 ? 's' : ''} aangemaakt.</p>
          <div className="flex gap-2 flex-wrap mt-2">
            {convertResult.map(orderId => (
              <Link
                key={orderId}
                to={`/orders/${orderId}`}
                className="text-xs px-2 py-0.5 rounded-full bg-green-100 text-green-700 hover:bg-green-200"
              >
                Order bekijken →
              </Link>
            ))}
          </div>
        </div>
      )}

      {/* Tabs */}
      <div className="border-b border-gray-200">
        <nav className="flex gap-4">
          {(['lines', 'details', 'history'] as Tab[]).map(t => (
            <button
              key={t}
              onClick={() => setTab(t)}
              className={`pb-2 text-sm font-medium border-b-2 transition-colors ${
                tab === t
                  ? 'border-blue-600 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700'
              }`}
            >
              {t === 'lines'
                ? `Regels${quote.lines.length > 0 ? ` (${quote.lines.length})` : ''}`
                : t === 'details'
                ? 'Details'
                : `Geschiedenis${history ? ` (${history.length})` : ''}`}
            </button>
          ))}
        </nav>
      </div>

      {/* Lines tab */}
      {tab === 'lines' && (
        <div className="space-y-3">
          {!isTerminal && (
            <div className="flex justify-end">
              <button
                onClick={() => { setEditingLine(null); setLineForm(emptyLine); setShowAddLine(true); }}
                className="px-3 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700"
              >
                + Regel toevoegen
              </button>
            </div>
          )}

          <div className="bg-white rounded-lg border border-gray-200 overflow-x-auto">
            <table className="w-full text-sm min-w-[900px]">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Onderdeel</th>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Onderdeelnr.</th>
                  <th className="text-right px-4 py-3 font-medium text-gray-600">Aantal</th>
                  <th className="text-right px-4 py-3 font-medium text-gray-600">Bew.</th>
                  <th className="text-right px-4 py-3 font-medium text-gray-600">Min.</th>
                  <th className="text-right px-4 py-3 font-medium text-gray-600">Prijs/st</th>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Geacc.</th>
                  {!isTerminal && <th className="px-4 py-3" />}
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {quote.lines
                  .slice()
                  .sort((a, b) => a.sortOrder - b.sortOrder)
                  .map(line => (
                    <tr key={line.id} className="hover:bg-gray-50">
                      <td className="px-4 py-3 text-gray-900 font-medium">{line.partName}</td>
                      <td className="px-4 py-3 font-mono text-gray-600">{line.partNumber}</td>
                      <td className="px-4 py-3 text-right font-mono text-gray-700">{line.quantity}</td>
                      <td className="px-4 py-3 text-right font-mono text-gray-700">{line.operationCount}</td>
                      <td className="px-4 py-3 text-right font-mono text-gray-700">{line.operationTimeMinutes}</td>
                      <td className="px-4 py-3 text-right font-mono text-gray-900">
                        {line.totalPricePerUnit != null
                          ? `€ ${line.totalPricePerUnit.toLocaleString('nl-NL', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
                          : '—'}
                        {line.isManualPrice && (
                          <span className="ml-1 text-xs text-amber-600">(hand)</span>
                        )}
                      </td>
                      <td className="px-4 py-3">
                        {line.isAccepted ? (
                          <span className="text-xs px-1.5 py-0.5 rounded-full bg-green-100 text-green-700">Ja</span>
                        ) : (
                          <span className="text-xs text-gray-400">—</span>
                        )}
                      </td>
                      {!isTerminal && (
                        <td className="px-4 py-3">
                          <div className="flex gap-2 justify-end">
                            {!line.isAccepted && (
                              <button
                                onClick={() => acceptLine.mutate(line.id)}
                                disabled={acceptLine.isPending}
                                className="text-xs text-green-600 hover:text-green-800 disabled:opacity-50"
                              >
                                Accepteren
                              </button>
                            )}
                            <button
                              onClick={() => openEdit(line)}
                              className="text-xs text-blue-600 hover:text-blue-800"
                            >
                              Bewerken
                            </button>
                            <button
                              onClick={() => {
                                if (confirm('Regel verwijderen?')) removeLine.mutate(line.id);
                              }}
                              disabled={removeLine.isPending}
                              className="text-xs text-red-500 hover:text-red-700 disabled:opacity-50"
                            >
                              Verwijderen
                            </button>
                          </div>
                        </td>
                      )}
                    </tr>
                  ))}
              </tbody>
            </table>
            {quote.lines.length === 0 && (
              <div className="px-4 py-8 text-center text-sm text-gray-500">
                Geen regels. Voeg een regel toe om te beginnen.
              </div>
            )}
          </div>
        </div>
      )}

      {/* Details tab */}
      {tab === 'details' && (
        <div className="grid grid-cols-2 gap-4">
          <Section title="Offertegegevens">
            <Field label="Offertenummer" value={`#${quote.quoteNumber}`} mono />
            <Field label="Klant" value={quote.customerName} />
            <Field label="Datum" value={new Date(quote.date).toLocaleDateString('nl-NL')} />
            <Field label="Referentie" value={quote.reference} />
            <Field label="Contactpersoon" value={quote.contactPerson} />
            <Field label="Levertijd" value={quote.deliveryTime} />
            <Field label="Status" value={statusLabel[status]} />
          </Section>
          <Section title="Calculatiegegevens">
            <Field label="Uurloon" value={`€ ${quote.hourlyRate.toFixed(2)}`} mono />
            <Field label="Materiaalopslag" value={`${quote.materialMargin}%`} mono />
            <Field label="Standaard opslag" value={`${quote.standardMargin}%`} mono />
            <Field label="Insteltijd" value={`${quote.setupTime} uur`} mono />
          </Section>
          {quote.remarks && (
            <div className="col-span-2 bg-white rounded-lg border border-gray-200 p-4">
              <h2 className="text-sm font-medium text-gray-700 mb-2">Opmerkingen</h2>
              <p className="text-sm text-gray-700">{quote.remarks}</p>
            </div>
          )}
          <Section title="Systeemgegevens">
            <Field label="Aangemaakt" value={new Date(quote.createdAt).toLocaleString('nl-NL')} />
            <Field label="Bijgewerkt" value={new Date(quote.updatedAt).toLocaleString('nl-NL')} />
          </Section>
        </div>
      )}

      {/* History tab */}
      {tab === 'history' && (
        <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
          {!history || history.length === 0 ? (
            <div className="px-4 py-8 text-center text-sm text-gray-500">Geen geschiedenis beschikbaar.</div>
          ) : (
            <table className="w-full text-sm">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Gebeurtenis</th>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Omschrijving</th>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Door</th>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Datum</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {history.map(entry => (
                  <tr key={entry.id} className="hover:bg-gray-50">
                    <td className="px-4 py-3">
                      <span className="text-xs font-medium px-2 py-0.5 rounded-full bg-gray-100 text-gray-600">
                        {eventTypeLabel[entry.eventType] ?? entry.eventType}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-gray-700">{entry.summary}</td>
                    <td className="px-4 py-3 text-gray-500">{entry.changedBy}</td>
                    <td className="px-4 py-3 text-gray-500">
                      {new Date(entry.changedAt).toLocaleString('nl-NL')}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      )}

      {/* Add/edit line modal */}
      {showAddLine && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-2xl p-6 space-y-4 max-h-[90vh] overflow-y-auto">
            <h2 className="text-lg font-semibold text-gray-900">
              {editingLine ? 'Regel bewerken' : 'Regel toevoegen'}
            </h2>
            <form onSubmit={handleAddLine} className="space-y-4">
              <div className="grid grid-cols-2 gap-3">
                <div className="col-span-2">
                  <label className="block text-xs font-medium text-gray-600 mb-1">Onderdeelnaam *</label>
                  <input
                    type="text"
                    value={lineForm.partName}
                    onChange={e => setLineForm(f => ({ ...f, partName: e.target.value }))}
                    required
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Onderdeelnummer</label>
                  <input
                    type="text"
                    value={lineForm.partNumber}
                    onChange={e => setLineForm(f => ({ ...f, partNumber: e.target.value }))}
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Aantal *</label>
                  <input
                    type="number"
                    step="0.0001"
                    value={lineForm.quantity}
                    onChange={e => setLineForm(f => ({ ...f, quantity: parseFloat(e.target.value) }))}
                    required
                    className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
              </div>

              <div className="border-t border-gray-100 pt-3">
                <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">Materiaal</p>
                <div className="grid grid-cols-3 gap-3">
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Soort</label>
                    <input
                      type="text"
                      value={lineForm.materialType ?? ''}
                      onChange={e => setLineForm(f => ({ ...f, materialType: e.target.value || null }))}
                      placeholder="bv. Staal"
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Code</label>
                    <input
                      type="text"
                      value={lineForm.materialCode ?? ''}
                      onChange={e => setLineForm(f => ({ ...f, materialCode: e.target.value || null }))}
                      placeholder="Werkstoffnummer"
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Code 2</label>
                    <input
                      type="text"
                      value={lineForm.materialCode2 ?? ''}
                      onChange={e => setLineForm(f => ({ ...f, materialCode2: e.target.value || null }))}
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Geometrie</label>
                    <input
                      type="text"
                      value={lineForm.materialGeometry ?? ''}
                      onChange={e => setLineForm(f => ({ ...f, materialGeometry: e.target.value || null }))}
                      placeholder="bv. Rnd, Buis"
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Maat (mm)</label>
                    <input
                      type="number"
                      step="0.001"
                      value={lineForm.materialSizeMm ?? ''}
                      onChange={e => setLineForm(f => ({ ...f, materialSizeMm: e.target.value ? parseFloat(e.target.value) : null }))}
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Lengte (mm)</label>
                    <input
                      type="number"
                      step="0.001"
                      value={lineForm.materialLengthMm ?? ''}
                      onChange={e => setLineForm(f => ({ ...f, materialLengthMm: e.target.value ? parseFloat(e.target.value) : null }))}
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Aantal mat.</label>
                    <input
                      type="number"
                      step="0.0001"
                      value={lineForm.materialQuantity ?? ''}
                      onChange={e => setLineForm(f => ({ ...f, materialQuantity: e.target.value ? parseFloat(e.target.value) : null }))}
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Mat. prijs (€)</label>
                    <input
                      type="number"
                      step="0.0001"
                      value={lineForm.materialPrice ?? ''}
                      onChange={e => setLineForm(f => ({ ...f, materialPrice: e.target.value ? parseFloat(e.target.value) : null }))}
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Materiaal</label>
                    <select
                      value={lineForm.materialSource}
                      onChange={e => setLineForm(f => ({ ...f, materialSource: e.target.value }))}
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                    >
                      <option value="inclusive">Inclusief</option>
                      <option value="customer">Klant levert</option>
                    </select>
                  </div>
                </div>
              </div>

              <div className="border-t border-gray-100 pt-3">
                <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">Bewerkingen</p>
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Aantal bew.</label>
                    <input
                      type="number"
                      step="1"
                      value={lineForm.operationCount}
                      onChange={e => setLineForm(f => ({ ...f, operationCount: parseInt(e.target.value) || 0 }))}
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Bewerkingstijd (min)</label>
                    <input
                      type="number"
                      step="0.01"
                      value={lineForm.operationTimeMinutes}
                      onChange={e => setLineForm(f => ({ ...f, operationTimeMinutes: parseFloat(e.target.value) || 0 }))}
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Buitenbew.</label>
                    <input
                      type="number"
                      step="1"
                      value={lineForm.subcontractingCount}
                      onChange={e => setLineForm(f => ({ ...f, subcontractingCount: parseInt(e.target.value) || 0 }))}
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Bui. prijs (€)</label>
                    <input
                      type="number"
                      step="0.0001"
                      value={lineForm.subcontractingPrice}
                      onChange={e => setLineForm(f => ({ ...f, subcontractingPrice: parseFloat(e.target.value) || 0 }))}
                      className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                </div>
              </div>

              <div className="border-t border-gray-100 pt-3">
                <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">Prijs</p>
                <div className="grid grid-cols-2 gap-3">
                  <div className="flex items-center gap-2">
                    <input
                      type="checkbox"
                      id="isManualPrice"
                      checked={lineForm.isManualPrice}
                      onChange={e => setLineForm(f => ({ ...f, isManualPrice: e.target.checked }))}
                      className="rounded border-gray-300"
                    />
                    <label htmlFor="isManualPrice" className="text-sm text-gray-700">Handmatige prijs</label>
                  </div>
                  {lineForm.isManualPrice && (
                    <div>
                      <label className="block text-xs font-medium text-gray-600 mb-1">Prijs p/st (€)</label>
                      <input
                        type="number"
                        step="0.0001"
                        value={lineForm.manualPrice ?? ''}
                        onChange={e => setLineForm(f => ({ ...f, manualPrice: e.target.value ? parseFloat(e.target.value) : null }))}
                        className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                      />
                    </div>
                  )}
                </div>
              </div>

              <div>
                <label className="block text-xs font-medium text-gray-600 mb-1">Opmerking</label>
                <textarea
                  value={lineForm.remarks ?? ''}
                  onChange={e => setLineForm(f => ({ ...f, remarks: e.target.value || null }))}
                  rows={2}
                  className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div className="flex justify-end gap-2 pt-2">
                <button
                  type="button"
                  onClick={() => { setShowAddLine(false); setEditingLine(null); setLineForm(emptyLine); }}
                  className="px-3 py-2 text-sm text-gray-600 hover:text-gray-900"
                >
                  Annuleren
                </button>
                <button
                  type="submit"
                  disabled={addLine.isPending || updateLine.isPending}
                  className="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50"
                >
                  {addLine.isPending || updateLine.isPending ? 'Opslaan...' : 'Opslaan'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="bg-white rounded-lg border border-gray-200 p-4">
      <h2 className="text-sm font-medium text-gray-700 mb-3">{title}</h2>
      <div className="space-y-2">{children}</div>
    </div>
  );
}

function Field({ label, value, mono }: { label: string; value: string | null | undefined; mono?: boolean }) {
  return (
    <div className="flex justify-between text-sm">
      <span className="text-gray-500">{label}</span>
      <span className={`text-gray-900 font-medium ${mono ? 'font-mono' : ''}`}>{value ?? '—'}</span>
    </div>
  );
}
