import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useParty, usePartyHistory, useDeactivateParty, useUpdateOrganization, useUpdatePerson } from '../../hooks/useParties';

type Tab = 'details' | 'history';

export default function PartyDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [tab, setTab] = useState<Tab>('details');
  const [isEditing, setIsEditing] = useState(false);

  const { data: party, isLoading, isError } = useParty(id!);
  const { data: history } = usePartyHistory(id!);
  const deactivate = useDeactivateParty();
  const updateOrg = useUpdateOrganization(id!);
  const updatePerson = useUpdatePerson(id!);

  const [editForm, setEditForm] = useState<Record<string, string>>({});

  const startEditing = () => {
    if (!party) return;
    if (party.partyType === 'Organization' && party.organizationDetails) {
      setEditForm({
        name: party.name,
        vatNumber: party.organizationDetails.vatNumber ?? '',
        chamberOfCommerceNumber: party.organizationDetails.chamberOfCommerceNumber ?? '',
      });
    } else if (party.partyType === 'Person' && party.personDetails) {
      setEditForm({
        firstName: party.personDetails.firstName,
        middleName: party.personDetails.middleName ?? '',
        lastName: party.personDetails.lastName,
        initials: party.personDetails.initials ?? '',
      });
    }
    setIsEditing(true);
  };

  const handleSave = async () => {
    if (!party) return;
    if (party.partyType === 'Organization') {
      await updateOrg.mutateAsync({
        name: editForm.name,
        vatNumber: editForm.vatNumber || null,
        chamberOfCommerceNumber: editForm.chamberOfCommerceNumber || null,
      });
    } else {
      await updatePerson.mutateAsync({
        firstName: editForm.firstName,
        middleName: editForm.middleName || null,
        lastName: editForm.lastName,
        initials: editForm.initials || null,
      });
    }
    setIsEditing(false);
  };

  const handleDeactivate = async () => {
    if (!confirm(`${party?.name} deactiveren?`)) return;
    await deactivate.mutateAsync(id!);
    navigate('/parties');
  };

  if (isLoading) return <div className="text-sm text-gray-500">Laden...</div>;
  if (isError || !party) return <div className="text-sm text-red-500">Relatie niet gevonden.</div>;

  const isPerson = party.partyType === 'Person';

  return (
    <div className="space-y-6 max-w-4xl">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div>
          <div className="flex items-center gap-2 text-sm text-gray-500 mb-1">
            <Link to="/parties" className="hover:text-gray-700">Relaties</Link>
            <span>/</span>
            <span>{party.name}</span>
          </div>
          <h1 className="text-xl font-semibold text-gray-900">{party.name}</h1>
          <div className="flex gap-2 mt-2">
            <span className="text-xs px-2 py-0.5 rounded-full bg-gray-100 text-gray-600">
              {isPerson ? 'Persoon' : 'Organisatie'}
            </span>
            {party.isCustomer && <span className="text-xs px-2 py-0.5 rounded-full bg-blue-100 text-blue-700">Klant</span>}
            {party.isSupplier && <span className="text-xs px-2 py-0.5 rounded-full bg-green-100 text-green-700">Leverancier</span>}
            {!party.isActive && <span className="text-xs px-2 py-0.5 rounded-full bg-red-100 text-red-600">Inactief</span>}
          </div>
        </div>

        <div className="flex gap-2">
          {party.isActive && !isEditing && (
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
                disabled={updateOrg.isPending || updatePerson.isPending}
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
          {(['details', 'history'] as Tab[]).map(t => (
            <button
              key={t}
              onClick={() => setTab(t)}
              className={`pb-2 text-sm font-medium border-b-2 transition-colors ${
                tab === t
                  ? 'border-blue-600 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700'
              }`}
            >
              {t === 'details' ? 'Details' : 'Geschiedenis'}
              {t === 'history' && history && (
                <span className="ml-1.5 text-xs text-gray-400">({history.length})</span>
              )}
            </button>
          ))}
        </nav>
      </div>

      {/* Tab content */}
      {tab === 'details' && (
        <div className="grid grid-cols-2 gap-4">
          {/* Persoon of organisatie details */}
          {isPerson && party.personDetails && (
            <Section title="Persoonsgegevens">
              {isEditing ? (
                <div className="space-y-3">
                  <div className="grid grid-cols-2 gap-2">
                    <EditField label="Voornaam" field="firstName" form={editForm} onChange={setEditForm} />
                    <EditField label="Initialen" field="initials" form={editForm} onChange={setEditForm} />
                  </div>
                  <EditField label="Tussenvoegsel" field="middleName" form={editForm} onChange={setEditForm} />
                  <EditField label="Achternaam" field="lastName" form={editForm} onChange={setEditForm} />
                </div>
              ) : (
                <>
                  <Field label="Voornaam" value={party.personDetails.firstName} />
                  <Field label="Tussenvoegsel" value={party.personDetails.middleName} />
                  <Field label="Achternaam" value={party.personDetails.lastName} />
                  <Field label="Initialen" value={party.personDetails.initials} />
                </>
              )}
            </Section>
          )}

          {!isPerson && party.organizationDetails && (
            <Section title="Organisatiegegevens">
              {isEditing ? (
                <div className="space-y-3">
                  <EditField label="Naam" field="name" form={editForm} onChange={setEditForm} />
                  <EditField label="BTW-nummer" field="vatNumber" form={editForm} onChange={setEditForm} />
                  <EditField label="KVK-nummer" field="chamberOfCommerceNumber" form={editForm} onChange={setEditForm} />
                </div>
              ) : (
                <>
                  <Field label="BTW-nummer" value={party.organizationDetails.vatNumber} />
                  <Field label="KVK-nummer" value={party.organizationDetails.chamberOfCommerceNumber} />
                </>
              )}
            </Section>
          )}

          {party.customerRole && (
            <Section title="Klantgegevens">
              <Field label="Debiteurnummer" value={party.customerRole.debtorNumber.toString()} />
              <Field label="Korting" value={`${party.customerRole.discount}%`} />
              <Field label="Betalingstermijn" value={`${party.customerRole.paymentTermDays} dagen`} />
              <Field label="Kredietlimiet" value={
                party.customerRole.creditLimit != null
                  ? `€ ${party.customerRole.creditLimit.toLocaleString('nl-NL', { minimumFractionDigits: 2 })}`
                  : 'Geen'
              } />
              <Field label="BTW verlegd" value={party.customerRole.isVatShifted ? 'Ja' : 'Nee'} />
            </Section>
          )}

          {party.supplierRole && (
            <Section title="Leveranciersgegevens">
              <Field label="Leveranciersnummer" value={party.supplierRole.supplierNumber.toString()} />
              <Field label="Betalingstermijn" value={`${party.supplierRole.paymentTermDays} dagen`} />
              <Field label="BTW verlegd" value={party.supplierRole.isVatShifted ? 'Ja' : 'Nee'} />
            </Section>
          )}

          {party.addresses.length > 0 && (
            <Section title={`Adressen (${party.addresses.length})`}>
              {party.addresses.map((address, i) => (
                <div key={i} className={i > 0 ? 'mt-3 pt-3 border-t border-gray-100' : ''}>
                  <p className="text-xs font-medium text-gray-400 uppercase mb-1">
                    {addressTypeLabel[address.addressType]}{address.isDefault && ' · Standaard'}
                  </p>
                  {address.attention && <p className="text-sm text-gray-600">t.a.v. {address.attention}</p>}
                  <p className="text-sm text-gray-900">{address.street} {address.houseNumber}{address.houseNumberAddition}</p>
                  <p className="text-sm text-gray-900">{address.postalCode} {address.city}</p>
                  {address.countryCode !== 'NL' && <p className="text-sm text-gray-500">{address.countryCode}</p>}
                </div>
              ))}
            </Section>
          )}

          {party.contactMethods.length > 0 && (
            <Section title="Contactgegevens">
              {party.contactMethods.map((contact, i) => (
                <Field key={i} label={contactMethodLabel[contact.contactMethodType]} value={contact.value} />
              ))}
            </Section>
          )}

          {party.bankAccounts.length > 0 && (
            <Section title={`Bankrekeningen (${party.bankAccounts.length})`}>
              {party.bankAccounts.map((account, i) => (
                <div key={i} className={i > 0 ? 'mt-3 pt-3 border-t border-gray-100' : ''}>
                  <p className="text-sm font-mono text-gray-900">{account.iban}</p>
                  {account.bic && <p className="text-xs text-gray-500">{account.bic}</p>}
                  {account.accountHolder && <p className="text-xs text-gray-500">{account.accountHolder}</p>}
                  {account.isPrimary && <p className="text-xs text-blue-600 mt-0.5">Primair</p>}
                </div>
              ))}
            </Section>
          )}
        </div>
      )}

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

function Field({ label, value }: { label: string; value: string | null | undefined }) {
  return (
    <div className="flex justify-between text-sm">
      <span className="text-gray-500">{label}</span>
      <span className="text-gray-900 font-medium">{value ?? '—'}</span>
    </div>
  );
}

function EditField({
  label, field, form, onChange,
}: {
  label: string;
  field: string;
  form: Record<string, string>;
  onChange: (f: Record<string, string>) => void;
}) {
  return (
    <div className="space-y-1">
      <label className="text-xs text-gray-500">{label}</label>
      <input
        type="text"
        value={form[field] ?? ''}
        onChange={e => onChange({ ...form, [field]: e.target.value })}
        className="w-full px-3 py-1.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
      />
    </div>
  );
}

const addressTypeLabel: Record<string, string> = {
  Postal: 'Postadres', Delivery: 'Afleveradres', Invoice: 'Factuuradres',
};

const contactMethodLabel: Record<string, string> = {
  Phone: 'Telefoon', Email: 'E-mail', Mobile: 'Mobiel',
};

const eventTypeLabel: Record<string, string> = {
  PartyCreated: 'Aangemaakt',
  PartyUpdated: 'Bijgewerkt',
  PartyDeactivated: 'Gedeactiveerd',
};
