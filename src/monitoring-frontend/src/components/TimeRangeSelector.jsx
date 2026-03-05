import { TIME_RANGES } from '../hooks/useMetricsData';

export function TimeRangeSelector({ selected, onChange, onManage }) {
  return (
    <div style={{
      display: 'flex', alignItems: 'center', gap: 8,
      padding: '10px 24px', background: '#111827', borderBottom: '1px solid #1f2937', flexWrap: 'wrap'
    }}>
      <button
        onClick={onManage}
        style={{
          padding: '6px 14px', borderRadius: 6, border: '1px solid #374151',
          background: '#1f2937', color: '#d1d5db', cursor: 'pointer', fontSize: 13, marginRight: 8
        }}
      >
        ⚙ Manage Metrics
      </button>
      {Object.entries(TIME_RANGES).map(([key, range]) => (
        <button
          key={key}
          onClick={() => onChange(key)}
          style={{
            padding: '6px 14px', borderRadius: 6, border: 'none', cursor: 'pointer',
            fontSize: 13, fontWeight: selected === key ? 700 : 400,
            background: selected === key ? '#3b82f6' : '#1f2937',
            color: selected === key ? '#fff' : '#9ca3af',
            display: 'flex', alignItems: 'center', gap: 5,
          }}
        >
          {key === 'live' && (
            <span style={{
              width: 7, height: 7, borderRadius: '50%', background: '#4ade80',
              display: 'inline-block', animation: selected === 'live' ? 'pulse 1.5s infinite' : 'none'
            }} />
          )}
          {range.label}
        </button>
      ))}
    </div>
  );
}