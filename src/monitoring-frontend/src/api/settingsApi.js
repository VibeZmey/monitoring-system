const BASE = '';

export async function fetchMetricSettings() {
  const res = await fetch(`${BASE}/api/metric-settings`);
  if (!res.ok) throw new Error('Failed to fetch metric settings');
  return res.json();
}

export async function createMetricSetting(metricName, isEnabled, displayOrder) {
  const res = await fetch(`${BASE}/api/metric-settings`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ metricName, isEnabled, displayOrder }),
  });
  if (!res.ok) throw new Error('Failed to create metric setting');
}

export async function patchMetricSetting(metricName, patch) {
  const res = await fetch(`${BASE}/api/metric-settings/${metricName}`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(patch),
  });
  if (!res.ok) throw new Error('Failed to patch metric setting');
  return res.json();
}