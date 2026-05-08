import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import AppLayout from './layouts/AppLayout';
import DashboardPage from './pages/DashboardPage';
import PartiesPage from './pages/parties/PartiesPage';
import CustomersPage from './pages/parties/CustomersPage';
import SuppliersPage from './pages/parties/SuppliersPage';
import PartyDetailPage from './pages/parties/PartyDetailPage';
import CreateOrganizationPage from './pages/parties/CreateOrganizationPage';
import CreatePersonPage from './pages/parties/CreatePersonPage';

const router = createBrowserRouter([
  {
    path: '/',
    element: <AppLayout />,
    children: [
      { index: true, element: <DashboardPage /> },
      { path: 'parties', element: <PartiesPage /> },
      { path: 'parties/customers', element: <CustomersPage /> },
      { path: 'parties/suppliers', element: <SuppliersPage /> },
      { path: 'parties/new/organization', element: <CreateOrganizationPage /> },
      { path: 'parties/new/person', element: <CreatePersonPage /> },
      { path: 'parties/:id', element: <PartyDetailPage /> },
    ],
  },
]);

export default function App() {
  return <RouterProvider router={router} />;
}
