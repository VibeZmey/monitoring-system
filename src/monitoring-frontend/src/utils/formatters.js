export function formatBits(bitsPerSec) {
  if (bitsPerSec == null) return '—';
  if (bitsPerSec < 1_000) return `${bitsPerSec.toFixed(0)} bit/s`;
  if (bitsPerSec < 1_000_000) return `${(bitsPerSec / 1_000).toFixed(1)} Kbit/s`;
  return `${(bitsPerSec / 1_000_000).toFixed(2)} Mbit/s`;
}

export function formatValue(metricName, value) {
  if (value == null) return '—';
  if (metricName.includes('Pct')) return `${value.toFixed(1)}%`;
  if (metricName.includes('TempC') || metricName.includes('Temp')) return `${value.toFixed(1)}°C`;
  if (metricName.includes('Mb')) return `${value} MB`;
  if (metricName.includes('Net')) return formatBits(value);
  return value.toFixed(2);
}

export function formatTime(isoString, rangeKey) {
  const date = new Date(isoString);
  if (rangeKey === 'live' || rangeKey === '1h') {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' });
  }
  if (rangeKey === '1d' || rangeKey === '1w') {
    return date.toLocaleString([], { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' });
  }
  return date.toLocaleString([], { month: 'short', day: 'numeric', hour: '2-digit' });
}