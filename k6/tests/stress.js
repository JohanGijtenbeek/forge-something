import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter, Rate } from 'k6/metrics';

const rateLimited = new Counter('rate_limited_requests');
const errorRate   = new Rate('error_rate');

export const options = {
  stages: [
    { duration: '30s', target: 50   },
    { duration: '30s', target: 100  },
    { duration: '30s', target: 200  },
    { duration: '30s', target: 300  },
    { duration: '1m',  target: 300  },
    { duration: '30s', target: 0    },
  ],
  thresholds: {
    http_req_failed:   ['rate<0.10'],
    http_req_duration: ['p(95)<2000'],
  },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5272';

export function setup() {
  const res = http.get(`${BASE_URL}/api/parties`);
  const parties = JSON.parse(res.body);
  return { partyIds: parties.slice(0, 20).map(p => p.id) };
}

export default function (data) {
  const partyIds = data.partyIds;

  const res = http.get(`${BASE_URL}/api/parties`);

  // 429 is geen fout - het is de limiter die correct werkt
  if (res.status === 429) {
    rateLimited.add(1);
  } else {
    check(res, { 'status 200': r => r.status === 200 });
    errorRate.add(res.status !== 200 && res.status !== 429);
  }

  if (partyIds.length > 0) {
    const id = partyIds[Math.floor(Math.random() * partyIds.length)];
    const detail = http.get(`${BASE_URL}/api/parties/${id}`);

    if (detail.status === 429) {
      rateLimited.add(1);
    } else {
      check(detail, { 'detail 200': r => r.status === 200 });
      errorRate.add(detail.status !== 200 && detail.status !== 429);
    }
  }

  sleep(0.5);
}

export function handleSummary(data) {
  const limited = data.metrics.rate_limited_requests?.values?.count ?? 0;
  const total   = data.metrics.http_reqs?.values?.count ?? 0;
  const errors  = data.metrics.error_rate?.values?.rate ?? 0;
  const dur     = data.metrics.http_req_duration;

  return {
    stdout: `
════════════════════════════════════════
  ERP Stress Test Resultaten
════════════════════════════════════════
  Totaal requests:      ${total}
  Rate limited (429):   ${limited} (${((limited/total)*100).toFixed(1)}%)
  Echte fouten:         ${(errors * 100).toFixed(2)}%

  Response times:
    Mediaan:  ${dur?.values?.med?.toFixed(0)}ms
    p90:      ${dur?.values['p(90)']?.toFixed(0)}ms
    p95:      ${dur?.values['p(95)']?.toFixed(0)}ms
    Max:      ${dur?.values?.max?.toFixed(0)}ms
════════════════════════════════════════
`
  };
}
