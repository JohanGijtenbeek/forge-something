import axios from 'axios';

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? '',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Response interceptor voor globale foutafhandeling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // Later: auth redirect, toast notificaties etc.
    console.error('API fout:', error.response?.data ?? error.message);
    return Promise.reject(error);
  }
);
