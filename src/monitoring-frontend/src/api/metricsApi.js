const BASE = '';

function toIso(date) {
  return date.toISOString();
}

export async function fetchMetrics(from, to) {
  const params = new URLSearchParams({ from: toIso(from), to: toIso(to) });
  const res = await fetch(`${BASE}/api/metrics?${params}`);
  if (!res.ok) throw new Error('Failed to fetch metrics');
  return res.json();
}

export async function fetchRollup1M(from, to) {
  const params = new URLSearchParams({ from: toIso(from), to: toIso(to) });
  const res = await fetch(`${BASE}/api/metrics/rollup1m?${params}`);
  if (!res.ok) throw new Error('Failed to fetch rollup1m');
  return res.json();
}

export async function fetchRollup1H(from, to) {
  const params = new URLSearchParams({ from: toIso(from), to: toIso(to) });
  const res = await fetch(`${BASE}/api/metrics/rollup1h?${params}`);
  if (!res.ok) throw new Error('Failed to fetch rollup1h');
  return res.json();
}