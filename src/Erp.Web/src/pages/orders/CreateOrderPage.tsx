import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useCreateOrder } from '../../hooks/useOrders';
import { useArticles } from '../../hooks/useArticles';
import { useCustomers } from '../../hooks/useParties';

const inputClass = 'w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500';

export default function CreateOrderPage() {
  const navigate = useNavigate();
  const create = useCreateOrder();

  const { data: articlesData } = useArticles({ articleType: 'manufactured', includeInactive: false, pageSize: 500 });
  const { data: customers } = useCustomers(false);

  const [form, setForm] = useState({
    articleId: '',
    customerId: '',
    quantity: '',
    unitOfMeasure: '',
    dueDate: '',
    notes: '',
  });

  const [error, setError] = useState<string | null>(null);

  const handleArticleChange = (articleId: string) => {
    const article = articlesData?.items.find(a => a.id === articleId);
    setForm(f => ({
      ...f,
      articleId,
      unitOfMeasure: article?.unitOfMeasure ?? f.unitOfMeasure,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      const result = await create.mutateAsync({
        articleId: form.articleId,
        customerId: form.customerId || null,
        quantity: parseFloat(form.quantity),
        unitOfMeasure: form.unitOfMeasure,
        dueDate: form.dueDate || null,
        notes: form.notes || null,
      });
      navigate(`/orders/${result.id}`);
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number } })?.response?.status;
      if (status === 404) {
        setError('Artikel of klant niet gevonden.');
      } else if (status === 400) {
        setError('Controleer de invoer en probeer opnieuw.');
      } else {
        setError('Er is een fout opgetreden bij het aanmaken.');
      }
    }
  };

  return (
    <div className="max-w-lg space-y-6">
      <div>
        <div className="flex items-center gap-2 text-sm text-gray-500 mb-1">
          <Link to="/orders" className="hover:text-gray-700">Productieorders</Link>
          <span>/</span>
          <span>Nieuwe order</span>
        </div>
        <h1 className="text-xl font-semibold text-gray-900">Order aanmaken</h1>
      </div>

      <form onSubmit={handleSubmit} className="bg-white rounded-lg border border-gray-200 p-5 space-y-4">
        <Field label="Artikel *">
          <select
            required
            value={form.articleId}
            onChange={e => handleArticleChange(e.target.value)}
            className={inputClass + ' bg-white'}
          >
            <option value="">— selecteer artikel —</option>
            {articlesData?.items.map(a => (
              <option key={a.id} value={a.id}>
                {a.code} — {a.name}
              </option>
            ))}
          </select>
        </Field>

        <Field label="Klant">
          <select
            value={form.customerId}
            onChange={e => setForm(f => ({ ...f, customerId: e.target.value }))}
            className={inputClass + ' bg-white'}
          >
            <option value="">— geen —</option>
            {customers?.map(c => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </Field>

        <div className="grid grid-cols-2 gap-3">
          <Field label="Aantal *">
            <input
              type="number"
              required
              min="0.0001"
              step="0.0001"
              value={form.quantity}
              onChange={e => setForm(f => ({ ...f, quantity: e.target.value }))}
              className={inputClass}
              placeholder="1.0000"
            />
          </Field>

          <Field label="Eenheid *">
            <input
              type="text"
              required
              value={form.unitOfMeasure}
              onChange={e => setForm(f => ({ ...f, unitOfMeasure: e.target.value }))}
              className={inputClass}
              placeholder="st"
            />
          </Field>
        </div>

        <Field label="Deadline">
          <input
            type="date"
            value={form.dueDate}
            onChange={e => setForm(f => ({ ...f, dueDate: e.target.value }))}
            className={inputClass}
          />
        </Field>

        <Field label="Notities">
          <textarea
            value={form.notes}
            onChange={e => setForm(f => ({ ...f, notes: e.target.value }))}
            className={inputClass + ' resize-none'}
            rows={3}
            placeholder="Optionele notities..."
          />
        </Field>

        {error && <p className="text-sm text-red-500">{error}</p>}

        <div className="flex gap-2 pt-2">
          <button
            type="submit"
            disabled={create.isPending}
            className="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50"
          >
            {create.isPending ? 'Aanmaken...' : 'Aanmaken'}
          </button>
          <Link
            to="/orders"
            className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
          >
            Annuleren
          </Link>
        </div>
      </form>
    </div>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="space-y-1">
      <label className="text-sm font-medium text-gray-700">{label}</label>
      {children}
    </div>
  );
}
