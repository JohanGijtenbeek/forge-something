import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useOrder, useOrderHistory, useUpdateOrderStatus, useCancelOrder } from '../../hooks/useOrders';
import type { OrderStatus } from '../../types/api';

type Tab = 'details' | 'bom' | 'operations' | 'history';

const statusLabel: Record<OrderStatus, string> = {
  draft: 'Concept',
  released: 'Vrijgegeven',
  inprogress: 'In uitvoering',
  done: 'Gereed',
  cancelled: 'Geannuleerd',
};

const statusBadgeClass: Record<OrderStatus, string> = {
  draft: 'bg-gray-100 text-gray-600',
  released: 'bg-blue-100 text-blue-700',
  inprogress: 'bg-amber-100 text-amber-700',
  done: 'bg-green-100 text-green-700',
  cancelled: 'bg-red-100 text-red-600',
};

const eventTypeLabel: Record<string, string> = {
  OrderCreated: 'Aangemaakt',
  OrderStatusChanged: 'Status gewijzigd',
};

export default function OrderDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [tab, setTab] = useState<Tab>('details');

  const { data: order, isLoading, isError } = useOrder(id!);
  const { data: history } = useOrderHistory(id!);
  const updateStatus = useUpdateOrderStatus(id!);
  const cancel = useCancelOrder();

  const handleTransition = async (status: string) => {
    await updateStatus.mutateAsync({ status });
  };

  const handleCancel = async () => {
    if (!confirm(`Order #${order?.orderNumber} annuleren?`)) return;
    await cancel.mutateAsync(id!);
    navigate('/orders');
  };

  if (isLoading) return <div className="text-sm text-gray-500">Laden...</div>;
  if (isError || !order) return <div className="text-sm text-red-500">Order niet gevonden.</div>;

  const status = order.status as OrderStatus;
  const isTerminal = status === 'done' || status === 'cancelled';

  return (
    <div className="space-y-6 max-w-4xl">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div>
          <div className="flex items-center gap-2 text-sm text-gray-500 mb-1">
            <Link to="/orders" className="hover:text-gray-700">Productieorders</Link>
            <span>/</span>
            <span className="font-mono">#{order.orderNumber}</span>
          </div>
          <h1 className="text-xl font-semibold text-gray-900">{order.articleName}</h1>
          <div className="flex gap-2 mt-2">
            <span className="text-xs px-2 py-0.5 rounded-full bg-gray-100 text-gray-500 font-mono">
              #{order.orderNumber}
            </span>
            <span className={`text-xs px-2 py-0.5 rounded-full ${statusBadgeClass[status]}`}>
              {statusLabel[status]}
            </span>
            {order.articleRevision && (
              <span className="text-xs px-2 py-0.5 rounded-full bg-gray-100 text-gray-500 font-mono">
                rev {order.articleRevision}
              </span>
            )}
          </div>
        </div>

        {!isTerminal && (
          <div className="flex gap-2">
            {status === 'draft' && (
              <button
                onClick={() => handleTransition('released')}
                disabled={updateStatus.isPending}
                className="text-sm font-medium text-white bg-blue-600 px-3 py-1.5 rounded-lg hover:bg-blue-700 disabled:opacity-50"
              >
                Vrijgeven
              </button>
            )}
            {status === 'released' && (
              <button
                onClick={() => handleTransition('inprogress')}
                disabled={updateStatus.isPending}
                className="text-sm font-medium text-white bg-amber-600 px-3 py-1.5 rounded-lg hover:bg-amber-700 disabled:opacity-50"
              >
                Starten
              </button>
            )}
            {status === 'inprogress' && (
              <button
                onClick={() => handleTransition('done')}
                disabled={updateStatus.isPending}
                className="text-sm font-medium text-white bg-green-600 px-3 py-1.5 rounded-lg hover:bg-green-700 disabled:opacity-50"
              >
                Gereedmelden
              </button>
            )}
            <button
              onClick={handleCancel}
              disabled={cancel.isPending}
              className="text-sm text-red-500 hover:text-red-700 border border-red-200 px-3 py-1.5 rounded-lg hover:bg-red-50 disabled:opacity-50"
            >
              Annuleren
            </button>
          </div>
        )}
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200">
        <nav className="flex gap-4">
          {(['details', 'bom', 'operations', 'history'] as Tab[]).map(t => (
            <button
              key={t}
              onClick={() => setTab(t)}
              className={`pb-2 text-sm font-medium border-b-2 transition-colors ${
                tab === t
                  ? 'border-blue-600 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700'
              }`}
            >
              {t === 'details' ? 'Details'
                : t === 'bom' ? `Stuklijst${order.bomLines.length > 0 ? ` (${order.bomLines.length})` : ''}`
                : t === 'operations' ? `Bewerkingen${order.operations.length > 0 ? ` (${order.operations.length})` : ''}`
                : `Geschiedenis${history ? ` (${history.length})` : ''}`}
            </button>
          ))}
        </nav>
      </div>

      {/* Details tab */}
      {tab === 'details' && (
        <div className="grid grid-cols-2 gap-4">
          <Section title="Ordergegevens">
            <Field label="Ordernummer" value={`#${order.orderNumber}`} mono />
            <Field label="Artikel" value={`${order.articleCode} — ${order.articleName}`} />
            {order.articleRevision && <Field label="Revisie" value={order.articleRevision} mono />}
            <Field label="Klant" value={order.customerName} />
            <Field
              label="Aantal"
              value={`${order.quantity.toLocaleString('nl-NL')} ${order.unitOfMeasure}`}
            />
            <Field label="Status" value={statusLabel[status]} />
            <Field
              label="Deadline"
              value={order.dueDate ? new Date(order.dueDate).toLocaleDateString('nl-NL') : null}
            />
            {order.notes && (
              <div className="pt-2 border-t border-gray-100">
                <p className="text-xs text-gray-500 mb-1">Notities</p>
                <p className="text-sm text-gray-700">{order.notes}</p>
              </div>
            )}
          </Section>

          <Section title="Systeemgegevens">
            <Field label="Aangemaakt" value={new Date(order.createdAt).toLocaleString('nl-NL')} />
            <Field label="Bijgewerkt" value={new Date(order.updatedAt).toLocaleString('nl-NL')} />
            {order.quoteId && (
              <div className="flex justify-between text-sm">
                <span className="text-gray-500">Offerte</span>
                <Link
                  to={`/quotes/${order.quoteId}`}
                  className="text-blue-600 hover:text-blue-800 font-medium"
                >
                  Bekijk offerte →
                </Link>
              </div>
            )}
          </Section>
        </div>
      )}

      {/* BOM tab */}
      {tab === 'bom' && (
        <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="text-left px-4 py-3 font-medium text-gray-600">Code</th>
                <th className="text-left px-4 py-3 font-medium text-gray-600">Naam</th>
                <th className="text-right px-4 py-3 font-medium text-gray-600">Hoeveelheid</th>
                <th className="text-left px-4 py-3 font-medium text-gray-600">Eenheid</th>
                <th className="text-left px-4 py-3 font-medium text-gray-600">Notities</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {order.bomLines.map(line => (
                <tr key={line.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 font-mono text-gray-900">{line.componentCode}</td>
                  <td className="px-4 py-3 text-gray-700">{line.componentName}</td>
                  <td className="px-4 py-3 text-right font-mono text-gray-900">{line.quantity}</td>
                  <td className="px-4 py-3 text-gray-500">{line.unitOfMeasure}</td>
                  <td className="px-4 py-3 text-gray-500 text-xs">{line.notes ?? '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {order.bomLines.length === 0 && (
            <div className="px-4 py-8 text-center text-sm text-gray-500">
              Geen stuklijstregels (snapshot van het moment van aanmaken).
            </div>
          )}
        </div>
      )}

      {/* Operations tab */}
      {tab === 'operations' && (
        <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="text-left px-4 py-3 font-medium text-gray-600 w-16">Seq</th>
                <th className="text-left px-4 py-3 font-medium text-gray-600">Bewerkingstype</th>
                <th className="text-right px-4 py-3 font-medium text-gray-600">Min.</th>
                <th className="text-left px-4 py-3 font-medium text-gray-600">Notities</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {order.operations.map(op => (
                <tr key={op.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 font-mono text-gray-500">{op.sequenceNumber}</td>
                  <td className="px-4 py-3 text-gray-900">
                    <span>{op.operationTypeName}</span>
                    {op.isSubcontracted && (
                      <span className="ml-2 text-xs px-1.5 py-0.5 rounded bg-amber-100 text-amber-700">uitbesteed</span>
                    )}
                    {op.isConditional && (
                      <span className="ml-2 text-xs px-1.5 py-0.5 rounded bg-purple-100 text-purple-700">conditioneel</span>
                    )}
                  </td>
                  <td className="px-4 py-3 text-right font-mono text-gray-700">
                    {op.estimatedMinutes != null ? op.estimatedMinutes : '—'}
                  </td>
                  <td className="px-4 py-3 text-gray-500 text-xs">{op.notes ?? '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {order.operations.length === 0 && (
            <div className="px-4 py-8 text-center text-sm text-gray-500">
              Geen bewerkingen (snapshot van het moment van aanmaken).
            </div>
          )}
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
