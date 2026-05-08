import { useParties, useCustomers, useSuppliers } from '../hooks/useParties';

export default function DashboardPage() {
  const { data: parties } = useParties();
  const { data: customers } = useCustomers();
  const { data: suppliers } = useSuppliers();

  const stats = [
    { label: 'Relaties', value: parties?.length ?? '—' },
    { label: 'Klanten', value: customers?.length ?? '—' },
    { label: 'Leveranciers', value: suppliers?.length ?? '—' },
  ];

  return (
    <div className="space-y-6">
      <h1 className="text-xl font-semibold text-gray-900">Dashboard</h1>

      <div className="grid grid-cols-3 gap-4">
        {stats.map((stat) => (
          <div key={stat.label} className="bg-white rounded-lg border border-gray-200 px-5 py-4">
            <p className="text-sm text-gray-500">{stat.label}</p>
            <p className="text-2xl font-semibold text-gray-900 mt-1">{stat.value}</p>
          </div>
        ))}
      </div>
    </div>
  );
}
