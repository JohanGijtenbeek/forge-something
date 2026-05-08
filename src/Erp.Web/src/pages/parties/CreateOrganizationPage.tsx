import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useCreateOrganization } from '../../hooks/useParties';

export default function CreateOrganizationPage() {
  const navigate = useNavigate();
  const create = useCreateOrganization();

  const [form, setForm] = useState({
    name: '',
    vatNumber: '',
    chamberOfCommerceNumber: '',
    registerAsCustomer: false,
    registerAsSupplier: false,
  });

  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      const result = await create.mutateAsync({
        name: form.name,
        vatNumber: form.vatNumber || null,
        chamberOfCommerceNumber: form.chamberOfCommerceNumber || null,
        registerAsCustomer: form.registerAsCustomer,
        registerAsSupplier: form.registerAsSupplier,
      });
      navigate(`/parties/${result.id}`);
    } catch {
      setError('Er is een fout opgetreden bij het aanmaken.');
    }
  };

  return (
    <div className="max-w-lg space-y-6">
      <div>
        <div className="flex items-center gap-2 text-sm text-gray-500 mb-1">
          <Link to="/parties" className="hover:text-gray-700">Relaties</Link>
          <span>/</span>
          <span>Nieuwe organisatie</span>
        </div>
        <h1 className="text-xl font-semibold text-gray-900">Organisatie aanmaken</h1>
      </div>

      <form onSubmit={handleSubmit} className="bg-white rounded-lg border border-gray-200 p-5 space-y-4">
        <Field label="Naam *">
          <input
            type="text"
            required
            value={form.name}
            onChange={e => setForm(f => ({ ...f, name: e.target.value }))}
            className={inputClass}
            placeholder="Bedrijfsnaam B.V."
          />
        </Field>

        <Field label="BTW-nummer">
          <input
            type="text"
            value={form.vatNumber}
            onChange={e => setForm(f => ({ ...f, vatNumber: e.target.value }))}
            className={inputClass}
            placeholder="NL123456789B01"
          />
        </Field>

        <Field label="KVK-nummer">
          <input
            type="text"
            value={form.chamberOfCommerceNumber}
            onChange={e => setForm(f => ({ ...f, chamberOfCommerceNumber: e.target.value }))}
            className={inputClass}
            placeholder="12345678"
          />
        </Field>

        <div className="pt-2 border-t border-gray-100 space-y-2">
          <p className="text-sm font-medium text-gray-700">Rollen</p>
          <label className="flex items-center gap-2 text-sm text-gray-600 cursor-pointer">
            <input
              type="checkbox"
              checked={form.registerAsCustomer}
              onChange={e => setForm(f => ({ ...f, registerAsCustomer: e.target.checked }))}
              className="rounded"
            />
            Registreren als klant
          </label>
          <label className="flex items-center gap-2 text-sm text-gray-600 cursor-pointer">
            <input
              type="checkbox"
              checked={form.registerAsSupplier}
              onChange={e => setForm(f => ({ ...f, registerAsSupplier: e.target.checked }))}
              className="rounded"
            />
            Registreren als leverancier
          </label>
        </div>

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
            to="/parties"
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

const inputClass = "w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500";
