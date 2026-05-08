import http from 'k6/http';
import { check, sleep } from 'k6';
import { Trend, Rate, Counter } from 'k6/metrics';

// Custom metrics
const partiesListDuration  = new Trend('parties_list_duration');
const partyDetailDuration  = new Trend('party_detail_duration');
const searchDuration       = new Trend('search_duration');
const errorRate            = new Rate('error_rate');
const totalRequests        = new Counter('total_requests');

export const options = {
  // Ramp up naar 50 gebruikers, houd vast, ramp down
  stages: [
    { duration: '30s', target: 10  },  // Langzaam opbouwen
    { duration: '1m',  target: 50  },  // Opbouwen naar piek
    { duration: '2m',  target: 50  },  // Piek vasthouden
    { duration: '30s', target: 0   },  // Afbouwen
  ],
  thresholds: {
    http_req_failed:        ['rate<0.01'],    // minder dan 1% fouten
    http_req_duration:      ['p(95)<500'],    // 95% onder 500ms
    'http_req_duration{type:list}':   ['p(95)<300'],
    'http_req_duration{type:detail}': ['p(95)<500'],
    'http_req_duration{type:search}': ['p(95)<200'],
    error_rate:             ['rate<0.01'],
  },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5272';

// Vaste party ID's om detail op te halen — vervang met echte ID's na seeden
const SAMPLE_QUERY = ['bout', 'staal', 'metaal', 'tilburg', 'amsterdam', 'jan'];

export function setup() {
  // Haal een lijst van party ID's op voor de detail test
  const res = http.get(`${BASE_URL}/api/parties`);
  const parties = JSON.parse(res.body);
  return { partyIds: parties.slice(0, 10).map(p => p.id) };
}

export default function (data) {
  totalRequests.add(1);
  const partyIds = data.partyIds;

  // Scenario 1: lijst ophalen (meest voorkomend)
  const listRes = http.get(`${BASE_URL}/api/parties`, {
    tags: { type: 'list' },
  });
  check(listRes, { 'list 200': r => r.status === 200 });
  partiesListDuration.add(listRes.timings.duration);
  errorRate.add(listRes.status !== 200);

  sleep(0.5);

  // Scenario 2: detail ophalen
  if (partyIds.length > 0) {
    const id = partyIds[Math.floor(Math.random() * partyIds.length)];
    const detailRes = http.get(`${BASE_URL}/api/parties/${id}`, {
      tags: { type: 'detail' },
    });
    check(detailRes, { 'detail 200': r => r.status === 200 });
    partyDetailDuration.add(detailRes.timings.duration);
    errorRate.add(detailRes.status !== 200);
  }

  sleep(0.5);

  // Scenario 3: zoeken
  const query = SAMPLE_QUERY[Math.floor(Math.random() * SAMPLE_QUERY.length)];
  const searchRes = http.get(`${BASE_URL}/api/search?q=${query}`, {
    tags: { type: 'search' },
  });
  check(searchRes, { 'search 200': r => r.status === 200 });
  searchDuration.add(searchRes.timings.duration);
  errorRate.add(searchRes.status !== 200);

  sleep(1);
}

export function handleSummary(data) {
  return {
    'stdout': summaryOutput(data),
  };
}

function summaryOutput(data) {
  const dur = data.metrics.http_req_duration;
  return `
════════════════════════════════════════
  ERP Load Test Resultaten
════════════════════════════════════════
  Totaal requests:  ${data.metrics.total_requests?.values?.count ?? 'n/a'}
  Foutpercentage:   ${(data.metrics.http_req_failed?.values?.rate * 100).toFixed(2)}%

  Response times (alle requests):
    Mediaan:  ${dur?.values?.med?.toFixed(0)}ms
    p90:      ${dur?.values['p(90)']?.toFixed(0)}ms
    p95:      ${dur?.values['p(95)']?.toFixed(0)}ms
    p99:      ${dur?.values['p(99)']?.toFixed(0)}ms
    Max:      ${dur?.values?.max?.toFixed(0)}ms

  Parties lijst:   mediaan ${data.metrics.parties_list_duration?.values?.med?.toFixed(0)}ms
  Party detail:    mediaan ${data.metrics.party_detail_duration?.values?.med?.toFixed(0)}ms
  Zoeken:          mediaan ${data.metrics.search_duration?.values?.med?.toFixed(0)}ms
════════════════════════════════════════
`;
}
