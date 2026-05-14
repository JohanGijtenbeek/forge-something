import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useArticles, useDeactivateArticle, useArticleCategories } from '../../hooks/useArticles';
import type { ArticleListResponse, ArticleType } from '../../types/api';

const PAGE_SIZE = 25;

const articleTypeLabel: Record<ArticleType, string> = {
  raw_material: 'Grondstof',
  manufactured: 'Geproduceerd',
  bought_out: 'Ingekocht',
  service: 'Dienst',
};

const articleTypeBadgeClass: Record<ArticleType, string> = {
  raw_material: 'bg-yellow-100 text-yellow-700',
  manufactured: 'bg-blue-100 text-blue-700',
  bought_out: 'bg-purple-100 text-purple-700',
  service: 'bg-gray-100 text-gray-600',
};

export default function ArticlesPage() {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [articleType, setArticleType] = useState('');
  const [includeInactive, setIncludeInactive] = useState(false);

  const { data, isLoading, isError } = useArticles({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    categoryId: categoryId || undefined,
    articleType: articleType || undefined,
    includeInactive,
  });

  const { data: categories } = useArticleCategories();
  const deactivate = useDeactivateArticle();

  const handleDeactivate = async (article: ArticleListResponse) => {
    if (!confirm(`${article.name} deactiveren?`)) return;
    await deactivate.mutateAsync(article.id);
  };

  const handleFilterChange = (fn: () => void) => {
    fn();
    setPage(1);
  };

  if (isLoading) return <div className="text-sm text-gray-500">Laden...</div>;
  if (isError) return <div className="text-sm text-red-500">Fout bij ophalen van artikelen.</div>;

  const totalPages = data?.totalPages ?? 1;
  const totalCount = data?.totalCount ?? 0;

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold text-gray-900">Artikelen</h1>
          <p className="text-sm text-gray-500 mt-0.5">{totalCount} resultaten</p>
        </div>
        <Link
          to="/articles/new"
          className="px-3 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700"
        >
          + Artikel
        </Link>
      </div>

      {/* Filters */}
      <div className="flex gap-3 items-center flex-wrap">
        <input
          type="search"
          value={search}
          onChange={e => handleFilterChange(() => setSearch(e.target.value))}
          placeholder="Zoeken op code of naam..."
          className="flex-1 min-w-48 max-w-sm px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <select
          value={articleType}
          onChange={e => handleFilterChange(() => setArticleType(e.target.value))}
          className="px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
        >
          <option value="">Alle types</option>
          <option value="raw_material">Grondstof</option>
          <option value="manufactured">Geproduceerd</option>
          <option value="bought_out">Ingekocht</option>
          <option value="service">Dienst</option>
        </select>
        <select
          value={categoryId}
          onChange={e => handleFilterChange(() => setCategoryId(e.target.value))}
          className="px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
        >
          <option value="">Alle categorieën</option>
          {categories?.map(c => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>
        <label className="flex items-center gap-2 text-sm text-gray-600 cursor-pointer">
          <input
            type="checkbox"
            checked={includeInactive}
            onChange={e => handleFilterChange(() => setIncludeInactive(e.target.checked))}
            className="rounded"
          />
          Inclusief inactief
        </label>
      </div>

      {/* Table */}
      <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 border-b border-gray-200">
            <tr>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Nr.</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Code</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Naam</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Type</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Categorie</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Eenheid</th>
              <th className="text-right px-4 py-3 font-medium text-gray-600">Prijs</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Status</th>
              <th className="px-4 py-3" />
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {data?.items.map(article => (
              <tr
                key={article.id}
                className="hover:bg-gray-50 cursor-pointer"
                onClick={() => navigate(`/articles/${article.id}`)}
              >
                <td className="px-4 py-3 text-gray-400 font-mono text-xs">{article.articleNumber}</td>
                <td className="px-4 py-3 font-mono font-medium text-gray-900">{article.code}</td>
                <td className="px-4 py-3 text-gray-900">{article.name}</td>
                <td className="px-4 py-3">
                  <span className={`text-xs px-2 py-0.5 rounded-full ${articleTypeBadgeClass[article.articleType]}`}>
                    {articleTypeLabel[article.articleType]}
                  </span>
                </td>
                <td className="px-4 py-3 text-gray-500">{article.category ?? '—'}</td>
                <td className="px-4 py-3 text-gray-500">{article.unitOfMeasure ?? '—'}</td>
                <td className="px-4 py-3 text-right text-gray-900">
                  {article.purchasePrice != null
                    ? `€ ${article.purchasePrice.toLocaleString('nl-NL', { minimumFractionDigits: 2, maximumFractionDigits: 4 })}`
                    : '—'}
                </td>
                <td className="px-4 py-3">
                  <span className={`text-xs px-2 py-0.5 rounded-full ${
                    article.isActive ? 'bg-gray-100 text-gray-600' : 'bg-red-100 text-red-600'
                  }`}>
                    {article.isActive ? 'Actief' : 'Inactief'}
                  </span>
                </td>
                <td className="px-4 py-3 text-right" onClick={e => e.stopPropagation()}>
                  {article.isActive && (
                    <button
                      onClick={() => handleDeactivate(article)}
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

        {data?.items.length === 0 && (
          <div className="px-4 py-8 text-center text-sm text-gray-500">
            Geen artikelen gevonden.
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
