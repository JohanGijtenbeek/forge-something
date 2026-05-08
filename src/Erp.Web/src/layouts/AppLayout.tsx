import { NavLink, Outlet } from 'react-router-dom';
import GlobalSearch from '../components/shared/GlobalSearch';

const navigation = [
  { name: 'Dashboard', href: '/' },
  { name: 'Relaties', href: '/parties' },
  { name: 'Klanten', href: '/parties/customers' },
  { name: 'Leveranciers', href: '/parties/suppliers' },
];

export default function AppLayout() {
  return (
    <div className="flex h-screen bg-gray-50">
      {/* Sidebar */}
      <aside className="w-56 bg-gray-900 text-gray-100 flex flex-col">
        <div className="px-6 py-5 border-b border-gray-700">
          <h1 className="text-lg font-semibold tracking-tight">ERP</h1>
          <p className="text-xs text-gray-400 mt-0.5">Metaalbewerking</p>
        </div>

        <nav className="flex-1 px-3 py-4 space-y-1">
          {navigation.map((item) => (
            <NavLink
              key={item.href}
              to={item.href}
              end={item.href === '/'}
              className={({ isActive }) =>
                `block px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  isActive
                    ? 'bg-gray-700 text-white'
                    : 'text-gray-300 hover:bg-gray-800 hover:text-white'
                }`
              }
            >
              {item.name}
            </NavLink>
          ))}
        </nav>
      </aside>

      {/* Main content */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Topbar */}
        <header className="bg-white border-b border-gray-200 px-6 py-3 flex items-center gap-4">
          <div className="flex-1 max-w-xl">
            <GlobalSearch />
          </div>
        </header>

        {/* Page content */}
        <main className="flex-1 overflow-auto p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
