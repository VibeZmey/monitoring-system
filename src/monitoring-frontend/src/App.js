import { useState } from 'react';
import { Header } from './components/Header';
import { TimeRangeSelector } from './components/TimeRangeSelector';
import { MetricChart } from './components/MetricChart';
import { ManageMetricsModal } from './components/ManageMetricsModal';
import { NoData } from './components/NoData';
import { useSignalR } from './hooks/useSignalR';
import { useMetricSettings } from './hooks/useMetricSettings';
import { useMetricsData, TIME_RANGES } from './hooks/useMetricsData';

export default function App() {
  const [rangeKey, setRangeKey] = useState('live');
  const [showModal, setShowModal] = useState(false);

  const isLive = rangeKey === 'live';

  const { settings, enabledSettings, loading: settingsLoading, create, patch } = useMetricSettings();
  const { liveData, connectionState } = useSignalR(isLive);
  const { data: historicalData, loading: dataLoading } = useMetricsData(rangeKey);

  const data = isLive ? liveData : historicalData;
  const loading = settingsLoading || (!isLive && dataLoading);

  return (
    <div style={{ minHeight: '100vh', background: '#0f172a', fontFamily: 'Inter, system-ui, sans-serif' }}>
      <style>{`
        * { box-sizing: border-box; }
        @keyframes pulse {
          0%, 100% { opacity: 1; }
          50% { opacity: 0.3; }
        }
        ::-webkit-scrollbar { width: 6px; }
        ::-webkit-scrollbar-track { background: #0f172a; }
        ::-webkit-scrollbar-thumb { background: #374151; border-radius: 3px; }
      `}</style>

      <Header connectionState={connectionState} />
      <TimeRangeSelector selected={rangeKey} onChange={setRangeKey} onManage={() => setShowModal(true)} />

      <main style={{ maxWidth: 960, margin: '0 auto', padding: '20px 16px' }}>
        {settingsLoading ? (
          <div style={{ color: '#6b7280', textAlign: 'center', padding: 40 }}>Loading settings...</div>
        ) : enabledSettings.length === 0 ? (
          <div style={{
            textAlign: 'center', padding: 60, color: '#6b7280',
            border: '1px dashed #374151', borderRadius: 12, marginTop: 20
          }}>
            <div style={{ fontSize: 32, marginBottom: 12 }}>📊</div>
            <div style={{ fontSize: 16, marginBottom: 8, color: '#9ca3af' }}>No metrics configured</div>
            <div style={{ fontSize: 13, marginBottom: 20 }}>Click "Manage Metrics" to add metrics to your dashboard</div>
            <button
              onClick={() => setShowModal(true)}
              style={{ padding: '8px 20px', borderRadius: 6, border: 'none', background: '#3b82f6', color: '#fff', cursor: 'pointer', fontSize: 14 }}
            >
              ⚙ Manage Metrics
            </button>
          </div>
        ) : loading ? (
          <div style={{ color: '#6b7280', textAlign: 'center', padding: 40 }}>Loading data...</div>
        ) : (
          enabledSettings.map((setting) => (
            <MetricChart
              key={setting.metricName}
              metricName={setting.metricName}
              data={data}
              rangeKey={rangeKey}
              isLive={isLive}
            />
          ))
        )}
      </main>

      {showModal && (
        <ManageMetricsModal
          settings={settings}
          onCreate={create}
          onPatch={patch}
          onClose={() => setShowModal(false)}
        />
      )}
    </div>
  );
}