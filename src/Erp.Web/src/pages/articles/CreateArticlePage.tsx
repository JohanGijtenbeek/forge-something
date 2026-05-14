import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useCreateArticle, useArticleCategories, useUnitsOfMeasure } from '../../hooks/useArticles';
import type { ArticleType } from '../../types/api';

const inputClass = 'w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500';

export default function CreateArticlePage() {
  const navigate = useNavigate();
  const create = useCreateArticle();
  const { data: categories } = useArticleCategories();
  const { data: unitsOfMeasure } = useUnitsOfMeasure();

  const [form, setForm] = useState({
    code: '',
    name: '',
    articleType: 'raw_material' as ArticleType,
    revision: '',
    description: '',
    categoryId: '',
    unitOfMeasureId: '',
    purchasePrice: '',
  });

  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      const result = await create.mutateAsync({
        code: form.code,
        name: form.name,
        articleType: form.articleType,
        description: form.description || null,
        categoryId: form.categoryId || null,
        unitOfMeasureId: form.unitOfMeasureId || null,
        purchasePrice: form.purchasePrice ? parseFloat(form.purchasePrice) : null,
        revision: form.revision || null,
      });
      navigate(`/articles/${result.id}`);
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number } })?.response?.status;
      if (status === 409) {
        setError('Een artikel met deze code bestaat al.');
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
          <Link to="/articles" className="hover:text-gray-700">Artikelen</Link>
          <span>/</span>
          <span>Nieuw artikel</span>
        </div>
        <h1 className="text-xl font-semibold text-gray-900">Artikel aanmaken</h1>
      </div>

      <form onSubmit={handleSubmit} className="bg-white rounded-lg border border-gray-200 p-5 space-y-4">
        <Field label="Code *">
          <input
            type="text"
            required
            value={form.code}
            onChange={e => setForm(f => ({ ...f, code: e.target.value }))}
            className={inputClass}
            placeholder="S355J2H"
          />
        </Field>

        <Field label="Naam *">
          <input
            type="text"
            required
            value={form.name}
            onChange={e => setForm(f => ({ ...f, name: e.target.value }))}
            className={inputClass}
            placeholder="Constructiestaal S355"
          />
        </Field>

        <Field label="Type *">
          <select
            value={form.articleType}
            onChange={e => setForm(f => ({ ...f, articleType: e.target.value as ArticleType }))}
            className={inputClass + ' bg-white'}
            required
          >
            <option value="raw_material">Grondstof</option>
            <option value="manufactured">Geproduceerd</option>
            <option value="bought_out">Ingekocht</option>
            <option value="service">Dienst</option>
          </select>
        </Field>

        <Field label="Categorie">
          <select
            value={form.categoryId}
            onChange={e => setForm(f => ({ ...f, categoryId: e.target.value }))}
            className={inputClass + ' bg-white'}
          >
            <option value="">— geen —</option>
            {categories?.map(c => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </Field>

        <Field label="Eenheid">
          <select
            value={form.unitOfMeasureId}
            onChange={e => setForm(f => ({ ...f, unitOfMeasureId: e.target.value }))}
            className={inputClass + ' bg-white'}
          >
            <option value="">— geen —</option>
            {unitsOfMeasure?.map(u => (
              <option key={u.id} value={u.id}>{u.name} ({u.abbreviation})</option>
            ))}
          </select>
        </Field>

        <Field label="Inkoopprijs">
          <input
            type="number"
            min="0"
            step="0.0001"
            value={form.purchasePrice}
            onChange={e => setForm(f => ({ ...f, purchasePrice: e.target.value }))}
            className={inputClass}
            placeholder="0.0000"
          />
        </Field>

        <Field label="Revisie">
          <input
            type="text"
            maxLength={10}
            value={form.revision}
            onChange={e => setForm(f => ({ ...f, revision: e.target.value }))}
            className={inputClass}
            placeholder="A"
          />
        </Field>

        <Field label="Omschrijving">
          <textarea
            value={form.description}
            onChange={e => setForm(f => ({ ...f, description: e.target.value }))}
            className={inputClass + ' resize-none'}
            rows={3}
            placeholder="Optionele omschrijving..."
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
            to="/articles"
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
