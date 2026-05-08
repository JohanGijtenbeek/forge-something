import http from 'k6/http';
import { check, sleep } from 'k6';

// Smoke test - minimale load om te verifiëren dat alles werkt
// Gebruik dit eerst voordat je zwaardere tests draait
export const options = {
  vus: 1,
  duration: '10s',
  thresholds: {
    http_req_failed:   ['rate<0.01'],      // minder dan 1% fouten
    http_req_duration: ['p(95)<1000'],     // 95% onder 1 seconde
  },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5272';

export default function () {
  // Parties ophalen
  const parties = http.get(`${BASE_URL}/api/parties`);
  check(parties, {
    'parties status 200':    r => r.status === 200,
    'parties heeft data':    r => JSON.parse(r.body).length > 0,
    'parties < 500ms':       r => r.timings.duration < 500,
  });

  sleep(1);

  // Klanten ophalen
  const customers = http.get(`${BASE_URL}/api/parties/customers`);
  check(customers, {
    'customers status 200':  r => r.status === 200,
    'customers < 500ms':     r => r.timings.duration < 500,
  });

  sleep(1);

  // Zoeken
  const search = http.get(`${BASE_URL}/api/search?q=bout`);
  check(search, {
    'search status 200':     r => r.status === 200,
    'search < 300ms':        r => r.timings.duration < 300,
  });

  sleep(1);
}
