import { useState } from 'react';
import { ALL_METRIC_NAMES, METRIC_LABELS } from '../utils/metricHelpers';

export function ManageMetricsModal({ settings, onCreate, onPatch, onClose }) {
  const [creating, setCreating] = useState(null); // metricName
  const [form, setForm] = useState({ isEnabled: true, displayOrder: 1 });
  const [saving, setSaving] = useState(false);

  const existingNames = new Set(settings.map((s) => s.metricName));
  const notCreated = ALL_METRIC_NAMES.filter((n) => !existingNames.has(n));

  async function handleCreate() {
    if (!creating) return;
    setSaving(true);
    try {
      await onCreate(creating, form.isEnabled, Number(form.displayOrder));
      setCreating(null);
    } finally {
      setSaving(false);
    }
  }

  async function handleToggle(setting) {
    await onPatch(setting.metricName, { isEnabled: !setting.isEnabled });
  }

  async function handleOrderChange(setting, value) {
    const order = Number(value);
    if (!isNaN(order) && order > 0) {
      await onPatch(setting.metricName, { displayOrder: order });
    }
  }

  return (
    <div style={overlayStyle} onClick={(e) => e.target === e.currentTarget && onClose()}>
      <div style={modalStyle}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
          <h2 style={{ margin: 0, color: '#f9fafb', fontSize: 16 }}>Manage Metrics</h2>
          <button onClick={onClose} style={closeBtnStyle}>✕</button>
        </div>

        {/* Существующие настройки */}
        {settings.length > 0 && (
          <div style={{ marginBottom: 20 }}>
            <p style={{ color: '#6b7280', fontSize: 12, margin: '0 0 8px' }}>CONFIGURED</p>
            {settings
              .sort((a, b) => a.displayOrder - b.displayOrder)
              .map((s) => (
                <div key={s.metricName} style={settingRowStyle}>
                  <span style={{ color: '#e5e7eb', fontSize: 13, flex: 1 }}>
                    {METRIC_LABELS[s.metricName] || s.metricName}
                  </span>
                  <label style={{ display: 'flex', alignItems: 'center', gap: 6, cursor: 'pointer' }}>
                    <input
                      type="checkbox"
                      checked={s.isEnabled}
                      onChange={() => handleToggle(s)}
                      style={{ accentColor: '#3b82f6' }}
                    />
                    <span style={{ color: '#9ca3af', fontSize: 12 }}>Enabled</span>
                  </label>
                  <label style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                    <span style={{ color: '#9ca3af', fontSize: 12 }}>Order:</span>
                    <input
                      type="number"
                      defaultValue={s.displayOrder}
                      min={1}
                      onBlur={(e) => handleOrderChange(s, e.target.value)}
                      style={numberInputStyle}
                    />
                  </label>
                </div>
              ))}
          </div>
        )}

        {/* Добавить новую */}
        {notCreated.length > 0 && (
          <div>
            <p style={{ color: '#6b7280', fontSize: 12, margin: '0 0 8px' }}>ADD NEW</p>
            {notCreated.map((name) => (
              <div key={name}>
                {creating === name ? (
                  <div style={{ ...settingRowStyle, flexWrap: 'wrap', gap: 8 }}>
                    <span style={{ color: '#e5e7eb', fontSize: 13, flex: 1 }}>
                      {METRIC_LABELS[name] || name}
                    </span>
                    <label style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                      <input
                        type="checkbox"
                        checked={form.isEnabled}
                        onChange={(e) => setForm((f) => ({ ...f, isEnabled: e.target.checked }))}
                        style={{ accentColor: '#3b82f6' }}
                      />
                      <span style={{ color: '#9ca3af', fontSize: 12 }}>Enabled</span>
                    </label>
                    <label style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                      <span style={{ color: '#9ca3af', fontSize: 12 }}>Order:</span>
                      <input
                        type="number"
                        value={form.displayOrder}
                        min={1}
                        onChange={(e) => setForm((f) => ({ ...f, displayOrder: e.target.value }))}
                        style={numberInputStyle}
                      />
                    </label>
                    <button onClick={handleCreate} disabled={saving} style={addBtnStyle}>
                      {saving ? '...' : 'Save'}
                    </button>
                    <button onClick={() => setCreating(null)} style={cancelBtnStyle}>Cancel</button>
                  </div>
                ) : (
                  <div style={settingRowStyle}>
                    <span style={{ color: '#9ca3af', fontSize: 13, flex: 1 }}>
                      {METRIC_LABELS[name] || name}
                    </span>
                    <button onClick={() => { setCreating(name); setForm({ isEnabled: true, displayOrder: settings.length + 1 }); }} style={addBtnStyle}>
                      + Add
                    </button>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}

        {settings.length === 0 && notCreated.length === 0 && (
          <p style={{ color: '#6b7280', textAlign: 'center' }}>All metrics are configured.</p>
        )}
      </div>
    </div>
  );
}

const overlayStyle = {
  position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)',
  display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100,
};
const modalStyle = {
  background: '#1f2937', border: '1px solid #374151', borderRadius: 12,
  padding: 24, width: '100%', maxWidth: 520, maxHeight: '80vh', overflowY: 'auto',
};
const settingRowStyle = {
  display: 'flex', alignItems: 'center', gap: 12,
  padding: '8px 0', borderBottom: '1px solid #374151',
};
const closeBtnStyle = {
  background: 'none', border: 'none', color: '#9ca3af',
  fontSize: 18, cursor: 'pointer', lineHeight: 1,
};
const addBtnStyle = {
  padding: '4px 12px', borderRadius: 5, border: 'none',
  background: '#3b82f6', color: '#fff', fontSize: 12, cursor: 'pointer',
};
const cancelBtnStyle = {
  padding: '4px 10px', borderRadius: 5, border: 'none',
  background: '#374151', color: '#9ca3af', fontSize: 12, cursor: 'pointer',
};
const numberInputStyle = {
  width: 52, padding: '3px 6px', borderRadius: 4,
  border: '1px solid #374151', background: '#111827',
  color: '#f9fafb', fontSize: 12,
};