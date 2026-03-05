import { useState, useEffect, useCallback } from 'react';
import { fetchMetricSettings, createMetricSetting, patchMetricSetting } from '../api/settingsApi';

export function useMetricSettings() {
  const [settings, setSettings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const load = useCallback(async () => {
    try {
      setLoading(true);
      const data = await fetchMetricSettings();
      setSettings(data);
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const create = useCallback(async (metricName, isEnabled, displayOrder) => {
    await createMetricSetting(metricName, isEnabled, displayOrder);
    await load();
  }, [load]);

  const patch = useCallback(async (metricName, patchData) => {
    const updated = await patchMetricSetting(metricName, patchData);
    setSettings((prev) =>
      prev.map((s) => s.metricName === metricName ? updated : s)
    );
  }, []);

  const enabledSettings = settings
    .filter((s) => s.isEnabled)
    .sort((a, b) => a.displayOrder - b.displayOrder);

  return { settings, enabledSettings, loading, error, create, patch, reload: load };
}