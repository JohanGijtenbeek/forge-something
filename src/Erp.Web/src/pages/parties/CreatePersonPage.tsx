import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useCreatePerson } from '../../hooks/useParties';

export default function CreatePersonPage() {
  const navigate = useNavigate();
  const create = useCreatePerson();

  const [form, setForm] = useState({
    firstName: '',
    middleName: '',
    lastName: '',
    initials: '',
  });

  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      const result = await create.mutateAsync({
        firstName: form.firstName,
        middleName: form.middleName || null,
        lastName: form.lastName,
        initials: form.initials || null,
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
          <span>Nieuwe persoon</span>
        </div>
        <h1 className="text-xl font-semibold text-gray-900">Persoon aanmaken</h1>
      </div>

      <form onSubmit={handleSubmit} className="bg-white rounded-lg border border-gray-200 p-5 space-y-4">
        <div className="grid grid-cols-2 gap-3">
          <Field label="Voornaam *">
            <input
              type="text"
              required
              value={form.firstName}
              onChange={e => setForm(f => ({ ...f, firstName: e.target.value }))}
              className={inputClass}
            />
          </Field>
          <Field label="Initialen">
            <input
              type="text"
              value={form.initials}
              onChange={e => setForm(f => ({ ...f, initials: e.target.value }))}
              className={inputClass}
              placeholder="J.P."
            />
          </Field>
        </div>

        <Field label="Tussenvoegsel">
          <input
            type="text"
            value={form.middleName}
            onChange={e => setForm(f => ({ ...f, middleName: e.target.value }))}
            className={inputClass}
            placeholder="van der"
          />
        </Field>

        <Field label="Achternaam *">
          <input
            type="text"
            required
            value={form.lastName}
            onChange={e => setForm(f => ({ ...f, lastName: e.target.value }))}
            className={inputClass}
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
