import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useOrders } from '../../hooks/useOrders';
import type { OrderStatus } from '../../types/api';

const PAGE_SIZE = 25;

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

export default function OrdersPage() {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState('');

  const { data, isLoading, isError } = useOrders({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    status: status || undefined,
  });

  const handleFilterChange = (fn: () => void) => {
    fn();
    setPage(1);
  };

  if (isLoading) return <div className="text-sm text-gray-500">Laden...</div>;
  if (isError) return <div className="text-sm text-red-500">Fout bij ophalen van orders.</div>;

  const totalPages = data?.totalPages ?? 1;
  const totalCount = data?.totalCount ?? 0;

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold text-gray-900">Productieorders</h1>
          <p className="text-sm text-gray-500 mt-0.5">{totalCount} resultaten</p>
        </div>
        <Link
          to="/orders/create"
          className="px-3 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700"
        >
          + Order
        </Link>
      </div>

      {/* Filters */}
      <div className="flex gap-3 items-center flex-wrap">
        <input
          type="search"
          value={search}
          onChange={e => handleFilterChange(() => setSearch(e.target.value))}
          placeholder="Zoeken op ordernummer, artikel, klant..."
          className="flex-1 min-w-48 max-w-sm px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <select
          value={status}
          onChange={e => handleFilterChange(() => setStatus(e.target.value))}
          className="px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
        >
          <option value="">Alle statussen</option>
          <option value="draft">Concept</option>
          <option value="released">Vrijgegeven</option>
          <option value="inprogress">In uitvoering</option>
          <option value="done">Gereed</option>
          <option value="cancelled">Geannuleerd</option>
        </select>
      </div>

      {/* Table */}
      <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 border-b border-gray-200">
            <tr>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Order #</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Artikel</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Klant</th>
              <th className="text-right px-4 py-3 font-medium text-gray-600">Aantal</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Status</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Deadline</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Aangemaakt</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {data?.items.map(order => (
              <tr
                key={order.id}
                className="hover:bg-gray-50 cursor-pointer"
                onClick={() => navigate(`/orders/${order.id}`)}
              >
                <td className="px-4 py-3 font-mono font-medium text-gray-900">#{order.orderNumber}</td>
                <td className="px-4 py-3">
                  <span className="font-mono text-gray-700">{order.articleCode}</span>
                  <span className="text-gray-500 ml-2 text-xs">{order.articleName}</span>
                </td>
                <td className="px-4 py-3 text-gray-500">{order.customerName ?? '—'}</td>
                <td className="px-4 py-3 text-right text-gray-900 font-mono">
                  {order.quantity} {order.unitOfMeasure}
                </td>
                <td className="px-4 py-3">
                  <span className={`text-xs px-2 py-0.5 rounded-full ${statusBadgeClass[order.status as OrderStatus]}`}>
                    {statusLabel[order.status as OrderStatus] ?? order.status}
                  </span>
                </td>
                <td className="px-4 py-3 text-gray-500">
                  {order.dueDate ? new Date(order.dueDate).toLocaleDateString('nl-NL') : '—'}
                </td>
                <td className="px-4 py-3 text-gray-400 text-xs">
                  {new Date(order.createdAt).toLocaleDateString('nl-NL')}
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {data?.items.length === 0 && (
          <div className="px-4 py-8 text-center text-sm text-gray-500">
            Geen orders gevonden.
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
    </div>
  );
}
