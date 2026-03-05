import { useState, useEffect } from 'react';
import { fetchMetrics, fetchRollup1M, fetchRollup1H } from '../api/metricsApi';

export const TIME_RANGES = {
  live: { label: 'Live', isLive: true },
  '1h': { label: '1H', offsetMs: 60 * 60 * 1000, source: 'rollup1m' },
  '1d': { label: '1D', offsetMs: 24 * 60 * 60 * 1000, source: 'rollup1m' },
  '1w': { label: '1W', offsetMs: 7 * 24 * 60 * 60 * 1000, source: 'rollup1m' },
  '1m': { label: '1M', offsetMs: 30 * 24 * 60 * 60 * 1000, source: 'rollup1h' },
  '1y': { label: '1Y', offsetMs: 365 * 24 * 60 * 60 * 1000, source: 'rollup1h' },
};

export function useMetricsData(rangeKey) {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const range = TIME_RANGES[rangeKey];
    if (!range || range.isLive) {
      setData([]);
      return;
    }

    let cancelled = false;

    async function load() {
      try {
        setLoading(true);
        setError(null);
        const to = new Date();
        const from = new Date(to.getTime() - range.offsetMs);

        let result;
        if (range.source === 'rollup1m') result = await fetchRollup1M(from, to);
        else if (range.source === 'rollup1h') result = await fetchRollup1H(from, to);
        else result = await fetchMetrics(from, to);

        if (!cancelled) setData(result);
      } catch (e) {
        if (!cancelled) setError(e.message);
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    load();
    return () => { cancelled = true; };
  }, [rangeKey]);

  return { data, loading, error };
}