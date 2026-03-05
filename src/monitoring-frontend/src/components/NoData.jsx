export function NoData({ message = 'No data for this period' }) {
  return (
    <div style={{
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      height: 120, color: '#6b7280', fontSize: 14, fontStyle: 'italic'
    }}>
      {message}
    </div>
  );
}