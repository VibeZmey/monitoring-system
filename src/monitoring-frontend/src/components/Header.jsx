export function Header({ connectionState }) {
  const stateColor = {
    Connected: '#4ade80',
    Reconnecting: '#facc15',
    Disconnected: '#f87171',
  }[connectionState] || '#f87171';

  return (
    <header style={{
      display: 'flex', alignItems: 'center', justifyContent: 'space-between',
      padding: '12px 24px', background: '#111827', borderBottom: '1px solid #1f2937'
    }}>
      <h1 style={{ margin: 0, fontSize: 18, fontWeight: 700, color: '#f9fafb', letterSpacing: 1 }}>
        System Monitor
      </h1>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
        <span style={{
          width: 10, height: 10, borderRadius: '50%',
          background: stateColor, display: 'inline-block',
          boxShadow: connectionState === 'Connected' ? `0 0 6px ${stateColor}` : 'none'
        }} />
        <span style={{ color: '#9ca3af', fontSize: 13 }}>{connectionState}</span>
      </div>
    </header>
  );
}