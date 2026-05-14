import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import {
  useArticle,
  useArticleHistory,
  useArticleBom,
  useArticleOperations,
  useOperationTypes,
  useDeactivateArticle,
  useUpdateArticle,
  useAddBomComponent,
  useRemoveBomComponent,
  useAddArticleOperation,
  useRemoveArticleOperation,
  useArticleCategories,
  useUnitsOfMeasure,
} from '../../hooks/useArticles';
import type { ArticleType, BomLineResponse, ArticleOperationResponse } from '../../types/api';

type Tab = 'details' | 'bom' | 'operations' | 'history';

const articleTypeLabel: Record<ArticleType, string> = {
  raw_material: 'Grondstof',
  manufactured: 'Geproduceerd',
  bought_out: 'Ingekocht',
  service: 'Dienst',
};

const eventTypeLabel: Record<string, string> = {
  ArticleCreated: 'Aangemaakt',
  ArticleUpdated: 'Bijgewerkt',
  ArticleDeactivated: 'Gedeactiveerd',
};

const inputClass = 'w-full px-3 py-1.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500';

export default function ArticleDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [tab, setTab] = useState<Tab>('details');
  const [isEditing, setIsEditing] = useState(false);
  const [showAddBom, setShowAddBom] = useState(false);
  const [showAddOp, setShowAddOp] = useState(false);

  const { data: article, isLoading, isError } = useArticle(id!);
  const { data: history } = useArticleHistory(id!);
  const { data: bom } = useArticleBom(id!);
  const { data: operations } = useArticleOperations(id!);
  const { data: operationTypes } = useOperationTypes();
  const { data: categories } = useArticleCategories();
  const { data: unitsOfMeasure } = useUnitsOfMeasure();

  const deactivate = useDeactivateArticle();
  const update = useUpdateArticle(id!);
  const addBom = useAddBomComponent(id!);
  const removeBom = useRemoveBomComponent(id!);
  const addOp = useAddArticleOperation(id!);
  const removeOp = useRemoveArticleOperation(id!);

  const [editForm, setEditForm] = useState<{
    code: string;
    name: string;
    articleType: ArticleType;
    description: string;
    categoryId: string;
    unitOfMeasureId: string;
    purchasePrice: string;
    revision: string;
  }>({
    code: '',
    name: '',
    articleType: 'raw_material',
    description: '',
    categoryId: '',
    unitOfMeasureId: '',
    purchasePrice: '',
    revision: '',
  });

  const [bomForm, setBomForm] = useState({
    childArticleId: '',
    quantity: '',
    unitOfMeasureId: '',
    sortOrder: '0',
  });

  const [opForm, setOpForm] = useState({
    sequenceNumber: '10',
    operationTypeId: '',
    estimatedMinutes: '',
    notes: '',
    isConditional: false,
  });

  const startEditing = () => {
    if (!article) return;
    setEditForm({
      code: article.code,
      name: article.name,
      articleType: article.articleType,
      description: article.description ?? '',
      categoryId: article.categoryId ?? '',
      unitOfMeasureId: article.unitOfMeasureId ?? '',
      purchasePrice: article.purchasePrice?.toString() ?? '',
      revision: article.revision ?? '',
    });
    setIsEditing(true);
  };

  const handleSave = async () => {
    await update.mutateAsync({
      code: editForm.code,
      name: editForm.name,
      articleType: editForm.articleType,
      description: editForm.description || null,
      categoryId: editForm.categoryId || null,
      unitOfMeasureId: editForm.unitOfMeasureId || null,
      purchasePrice: editForm.purchasePrice ? parseFloat(editForm.purchasePrice) : null,
      revision: editForm.revision || null,
    });
    setIsEditing(false);
  };

  const handleDeactivate = async () => {
    if (!confirm(`${article?.name} deactiveren?`)) return;
    await deactivate.mutateAsync(id!);
    navigate('/articles');
  };

  const handleAddBom = async (e: React.FormEvent) => {
    e.preventDefault();
    await addBom.mutateAsync({
      childArticleId: bomForm.childArticleId,
      quantity: parseFloat(bomForm.quantity),
      unitOfMeasureId: bomForm.unitOfMeasureId || null,
      sortOrder: parseInt(bomForm.sortOrder, 10),
    });
    setBomForm({ childArticleId: '', quantity: '', unitOfMeasureId: '', sortOrder: '0' });
    setShowAddBom(false);
  };

  const handleRemoveBom = async (line: BomLineResponse) => {
    if (!confirm(`${line.childName} verwijderen uit stuklijst?`)) return;
    await removeBom.mutateAsync(line.id);
  };

  const handleAddOp = async (e: React.FormEvent) => {
    e.preventDefault();
    await addOp.mutateAsync({
      sequenceNumber: parseInt(opForm.sequenceNumber, 10),
      operationTypeId: opForm.operationTypeId,
      estimatedMinutes: opForm.estimatedMinutes ? parseFloat(opForm.estimatedMinutes) : null,
      notes: opForm.notes || null,
      isConditional: opForm.isConditional,
    });
    setOpForm({ sequenceNumber: '10', operationTypeId: '', estimatedMinutes: '', notes: '', isConditional: false });
    setShowAddOp(false);
  };

  const handleRemoveOp = async (op: ArticleOperationResponse) => {
    if (!confirm(`Bewerking "${op.operationTypeName}" verwijderen?`)) return;
    await removeOp.mutateAsync(op.id);
  };

  if (isLoading) return <div className="text-sm text-gray-500">Laden...</div>;
  if (isError || !article) return <div className="text-sm text-red-500">Artikel niet gevonden.</div>;

  const isManufactured = article.articleType === 'manufactured';
  const availableTabs: Tab[] = isManufactured ? ['details', 'bom', 'operations', 'history'] : ['details', 'history'];

  return (
    <div className="space-y-6 max-w-4xl">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div>
          <div className="flex items-center gap-2 text-sm text-gray-500 mb-1">
            <Link to="/articles" className="hover:text-gray-700">Artikelen</Link>
            <span>/</span>
            <span className="font-mono">{article.code}</span>
          </div>
          <h1 className="text-xl font-semibold text-gray-900">{article.name}</h1>
          <div className="flex gap-2 mt-2">
            <span className="text-xs px-2 py-0.5 rounded-full bg-gray-100 text-gray-500 font-mono">
              #{article.articleNumber}
            </span>
            <span className="text-xs px-2 py-0.5 rounded-full bg-blue-50 text-blue-700">
              {articleTypeLabel[article.articleType]}
            </span>
            {!article.isActive && (
              <span className="text-xs px-2 py-0.5 rounded-full bg-red-100 text-red-600">Inactief</span>
            )}
          </div>
        </div>

        <div className="flex gap-2">
          {article.isActive && !isEditing && (
            <>
              <button
                onClick={startEditing}
                className="text-sm text-gray-700 border border-gray-300 px-3 py-1.5 rounded-lg hover:bg-gray-50"
              >
                Bewerken
              </button>
              <button
                onClick={handleDeactivate}
                className="text-sm text-red-500 hover:text-red-700 border border-red-200 px-3 py-1.5 rounded-lg hover:bg-red-50"
              >
                Deactiveer
              </button>
            </>
          )}
          {isEditing && (
            <>
              <button
                onClick={handleSave}
                disabled={update.isPending}
                className="text-sm text-white bg-blue-600 px-3 py-1.5 rounded-lg hover:bg-blue-700 disabled:opacity-50"
              >
                Opslaan
              </button>
              <button
                onClick={() => setIsEditing(false)}
                className="text-sm text-gray-700 border border-gray-300 px-3 py-1.5 rounded-lg hover:bg-gray-50"
              >
                Annuleren
              </button>
            </>
          )}
        </div>
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200">
        <nav className="flex gap-4">
          {availableTabs.map(t => (
            <button
              key={t}
              onClick={() => setTab(t)}
              className={`pb-2 text-sm font-medium border-b-2 transition-colors ${
                tab === t
                  ? 'border-blue-600 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700'
              }`}
            >
              {t === 'details' ? 'Details' : t === 'bom' ? 'Stuklijst' : t === 'operations' ? 'Bewerkingen' : 'Geschiedenis'}
              {t === 'history' && history && (
                <span className="ml-1.5 text-xs text-gray-400">({history.length})</span>
              )}
              {t === 'bom' && bom && (
                <span className="ml-1.5 text-xs text-gray-400">({bom.length})</span>
              )}
              {t === 'operations' && operations && (
                <span className="ml-1.5 text-xs text-gray-400">({operations.length})</span>
              )}
            </button>
          ))}
        </nav>
      </div>

      {/* Details tab */}
      {tab === 'details' && (
        <div className="grid grid-cols-2 gap-4">
          <Section title="Artikelgegevens">
            {isEditing ? (
              <div className="space-y-3">
                <EditField label="Code" value={editForm.code} onChange={v => setEditForm(f => ({ ...f, code: v }))} />
                <EditField label="Naam" value={editForm.name} onChange={v => setEditForm(f => ({ ...f, name: v }))} />
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Type</label>
                  <select
                    value={editForm.articleType}
                    onChange={e => setEditForm(f => ({ ...f, articleType: e.target.value as ArticleType }))}
                    className={inputClass + ' bg-white'}
                  >
                    <option value="raw_material">Grondstof</option>
                    <option value="manufactured">Geproduceerd</option>
                    <option value="bought_out">Ingekocht</option>
                    <option value="service">Dienst</option>
                  </select>
                </div>
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Categorie</label>
                  <select
                    value={editForm.categoryId}
                    onChange={e => setEditForm(f => ({ ...f, categoryId: e.target.value }))}
                    className={inputClass + ' bg-white'}
                  >
                    <option value="">— geen —</option>
                    {categories?.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                  </select>
                </div>
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Eenheid</label>
                  <select
                    value={editForm.unitOfMeasureId}
                    onChange={e => setEditForm(f => ({ ...f, unitOfMeasureId: e.target.value }))}
                    className={inputClass + ' bg-white'}
                  >
                    <option value="">— geen —</option>
                    {unitsOfMeasure?.map(u => <option key={u.id} value={u.id}>{u.name} ({u.abbreviation})</option>)}
                  </select>
                </div>
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Inkoopprijs</label>
                  <input
                    type="number"
                    min="0"
                    step="0.0001"
                    value={editForm.purchasePrice}
                    onChange={e => setEditForm(f => ({ ...f, purchasePrice: e.target.value }))}
                    className={inputClass}
                  />
                </div>
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Revisie</label>
                  <input
                    type="text"
                    maxLength={10}
                    value={editForm.revision}
                    onChange={e => setEditForm(f => ({ ...f, revision: e.target.value }))}
                    className={inputClass}
                    placeholder="A"
                  />
                </div>
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Omschrijving</label>
                  <textarea
                    value={editForm.description}
                    onChange={e => setEditForm(f => ({ ...f, description: e.target.value }))}
                    className={inputClass + ' resize-none'}
                    rows={3}
                  />
                </div>
              </div>
            ) : (
              <>
                <Field label="Code" value={article.code} mono />
                <Field label="Naam" value={article.name} />
                <Field label="Type" value={articleTypeLabel[article.articleType]} />
                <Field label="Categorie" value={article.category} />
                <Field label="Eenheid" value={article.unitOfMeasure} />
                {article.revision && <Field label="Revisie" value={article.revision} mono />}
                <Field
                  label="Inkoopprijs"
                  value={
                    article.purchasePrice != null
                      ? `€ ${article.purchasePrice.toLocaleString('nl-NL', { minimumFractionDigits: 2, maximumFractionDigits: 4 })}`
                      : null
                  }
                />
                {article.description && (
                  <div className="pt-2 border-t border-gray-100">
                    <p className="text-xs text-gray-500 mb-1">Omschrijving</p>
                    <p className="text-sm text-gray-700">{article.description}</p>
                  </div>
                )}
              </>
            )}
          </Section>

          <Section title="Systeemgegevens">
            <Field label="Artikelnummer" value={article.articleNumber.toString()} />
            <Field label="Aangemaakt" value={new Date(article.createdAt).toLocaleString('nl-NL')} />
            <Field label="Bijgewerkt" value={new Date(article.updatedAt).toLocaleString('nl-NL')} />
          </Section>
        </div>
      )}

      {/* BOM tab */}
      {tab === 'bom' && isManufactured && (
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <p className="text-sm text-gray-500">
              {bom?.length ?? 0} component{bom?.length !== 1 ? 'en' : ''}
            </p>
            {article.isActive && (
              <button
                onClick={() => setShowAddBom(v => !v)}
                className="text-sm font-medium text-blue-600 hover:text-blue-700"
              >
                {showAddBom ? 'Annuleren' : '+ Component toevoegen'}
              </button>
            )}
          </div>

          {showAddBom && (
            <form onSubmit={handleAddBom} className="bg-white rounded-lg border border-gray-200 p-4 space-y-3">
              <p className="text-sm font-medium text-gray-700">Component toevoegen</p>
              <div className="grid grid-cols-2 gap-3">
                <div className="space-y-1 col-span-2">
                  <label className="text-xs text-gray-500">Artikel-ID *</label>
                  <input
                    type="text"
                    required
                    value={bomForm.childArticleId}
                    onChange={e => setBomForm(f => ({ ...f, childArticleId: e.target.value }))}
                    className={inputClass}
                    placeholder="GUID van het artikel"
                  />
                </div>
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Hoeveelheid *</label>
                  <input
                    type="number"
                    required
                    min="0.0001"
                    step="0.0001"
                    value={bomForm.quantity}
                    onChange={e => setBomForm(f => ({ ...f, quantity: e.target.value }))}
                    className={inputClass}
                    placeholder="1.0000"
                  />
                </div>
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Eenheid</label>
                  <select
                    value={bomForm.unitOfMeasureId}
                    onChange={e => setBomForm(f => ({ ...f, unitOfMeasureId: e.target.value }))}
                    className={inputClass + ' bg-white'}
                  >
                    <option value="">— geen —</option>
                    {unitsOfMeasure?.map(u => <option key={u.id} value={u.id}>{u.name} ({u.abbreviation})</option>)}
                  </select>
                </div>
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Volgorde</label>
                  <input
                    type="number"
                    value={bomForm.sortOrder}
                    onChange={e => setBomForm(f => ({ ...f, sortOrder: e.target.value }))}
                    className={inputClass}
                  />
                </div>
              </div>
              <div className="flex gap-2">
                <button
                  type="submit"
                  disabled={addBom.isPending}
                  className="px-3 py-1.5 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50"
                >
                  {addBom.isPending ? 'Toevoegen...' : 'Toevoegen'}
                </button>
              </div>
            </form>
          )}

          <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Code</th>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Naam</th>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Type</th>
                  <th className="text-right px-4 py-3 font-medium text-gray-600">Hoeveelheid</th>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Eenheid</th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {bom?.map(line => (
                  <tr key={line.id} className="hover:bg-gray-50">
                    <td className="px-4 py-3 font-mono text-gray-900">
                      <Link to={`/articles/${line.childArticleId}`} className="hover:text-blue-600" onClick={e => e.stopPropagation()}>
                        {line.childCode}
                      </Link>
                    </td>
                    <td className="px-4 py-3 text-gray-700">{line.childName}</td>
                    <td className="px-4 py-3 text-gray-500 text-xs">{line.childArticleType}</td>
                    <td className="px-4 py-3 text-right text-gray-900 font-mono">{line.quantity}</td>
                    <td className="px-4 py-3 text-gray-500">{line.unitOfMeasure ?? '—'}</td>
                    <td className="px-4 py-3 text-right">
                      {article.isActive && (
                        <button
                          onClick={() => handleRemoveBom(line)}
                          className="text-xs text-red-500 hover:text-red-700"
                        >
                          Verwijder
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {(!bom || bom.length === 0) && (
              <div className="px-4 py-8 text-center text-sm text-gray-500">
                Geen componenten in stuklijst.
              </div>
            )}
          </div>
        </div>
      )}

      {/* Operations tab */}
      {tab === 'operations' && isManufactured && (
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <p className="text-sm text-gray-500">
              {operations?.length ?? 0} bewerking{operations?.length !== 1 ? 'en' : ''}
            </p>
            {article.isActive && (
              <button
                onClick={() => setShowAddOp(v => !v)}
                className="text-sm font-medium text-blue-600 hover:text-blue-700"
              >
                {showAddOp ? 'Annuleren' : '+ Bewerking toevoegen'}
              </button>
            )}
          </div>

          {showAddOp && (
            <form onSubmit={handleAddOp} className="bg-white rounded-lg border border-gray-200 p-4 space-y-3">
              <p className="text-sm font-medium text-gray-700">Bewerking toevoegen</p>
              <div className="grid grid-cols-2 gap-3">
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Volgordenummer *</label>
                  <input
                    type="number"
                    required
                    min="1"
                    value={opForm.sequenceNumber}
                    onChange={e => setOpForm(f => ({ ...f, sequenceNumber: e.target.value }))}
                    className={inputClass}
                  />
                </div>
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Bewerkingstype *</label>
                  <select
                    required
                    value={opForm.operationTypeId}
                    onChange={e => setOpForm(f => ({ ...f, operationTypeId: e.target.value }))}
                    className={inputClass + ' bg-white'}
                  >
                    <option value="">— selecteer —</option>
                    {operationTypes?.map(t => (
                      <option key={t.id} value={t.id}>
                        {t.name}{t.isSubcontracted ? ' (uitbesteed)' : ''}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="space-y-1">
                  <label className="text-xs text-gray-500">Geschatte minuten</label>
                  <input
                    type="number"
                    min="0"
                    step="0.01"
                    value={opForm.estimatedMinutes}
                    onChange={e => setOpForm(f => ({ ...f, estimatedMinutes: e.target.value }))}
                    className={inputClass}
                    placeholder="30"
                  />
                </div>
                <div className="space-y-1 flex items-end">
                  <label className="flex items-center gap-2 text-sm text-gray-700 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={opForm.isConditional}
                      onChange={e => setOpForm(f => ({ ...f, isConditional: e.target.checked }))}
                      className="rounded border-gray-300"
                    />
                    Conditioneel
                  </label>
                </div>
                <div className="space-y-1 col-span-2">
                  <label className="text-xs text-gray-500">Notities</label>
                  <input
                    type="text"
                    value={opForm.notes}
                    onChange={e => setOpForm(f => ({ ...f, notes: e.target.value }))}
                    className={inputClass}
                    placeholder="Optionele notities..."
                  />
                </div>
              </div>
              <div className="flex gap-2">
                <button
                  type="submit"
                  disabled={addOp.isPending}
                  className="px-3 py-1.5 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50"
                >
                  {addOp.isPending ? 'Toevoegen...' : 'Toevoegen'}
                </button>
              </div>
            </form>
          )}

          <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="text-left px-4 py-3 font-medium text-gray-600 w-16">Seq</th>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Bewerkingstype</th>
                  <th className="text-right px-4 py-3 font-medium text-gray-600">Min.</th>
                  <th className="text-left px-4 py-3 font-medium text-gray-600">Notities</th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {operations?.map(op => (
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
                    <td className="px-4 py-3 text-right">
                      {article.isActive && (
                        <button
                          onClick={() => handleRemoveOp(op)}
                          className="text-xs text-red-500 hover:text-red-700"
                        >
                          Verwijder
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {(!operations || operations.length === 0) && (
              <div className="px-4 py-8 text-center text-sm text-gray-500">
                Geen bewerkingen in routetemplate.
              </div>
            )}
          </div>
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

function EditField({
  label, value, onChange,
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
}) {
  return (
    <div className="space-y-1">
      <label className="text-xs text-gray-500">{label}</label>
      <input
        type="text"
        value={value}
        onChange={e => onChange(e.target.value)}
        className="w-full px-3 py-1.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
      />
    </div>
  );
}
