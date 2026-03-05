import {
  ResponsiveContainer, LineChart, Line, AreaChart, Area,
  XAxis, YAxis, CartesianGrid, Tooltip, Legend
} from 'recharts';
import { useState, useMemo } from 'react';
import { formatTime, formatValue, formatBits } from '../utils/formatters';
import { getRawKey, getAvgKey, getMinKey, getMaxKey, isRollupPointEmpty, extractRollupValue, METRIC_LABELS, METRIC_UNITS } from '../utils/metricHelpers';
import { NoData } from './NoData';

const COLORS = { avg: '#3b82f6', min: '#34d399', max: '#f87171', value: '#3b82f6' };

function customTooltipFormatter(metricName) {
  return (value) => {
    if (metricName.includes('Net')) return [formatBits(value), ''];
    return [`${value != null ? Number(value).toFixed(2) : '—'} ${METRIC_UNITS[metricName] || ''}`, ''];
  };
}

export function MetricChart({ metricName, data, rangeKey, isLive }) {
  const [visibleStats, setVisibleStats] = useState({ avg: true, min: false, max: false });

  const isNet = metricName.includes('Net');
  const isRollup = !isLive;

  const chartData = useMemo(() => {
    if (isLive) {
      return data.map((point) => {
        const key = getRawKey(metricName);
        const value = key ? point[key] : null;
        return { ts: point.ts, value: value ?? null };
      });
    }

    return data
      .map((point) => {
        const tsKey = point.bucketTs ? 'bucketTs' : 'ts';
        if (!isNet && isRollupPointEmpty(point, metricName)) {
          return { ts: point[tsKey], avg: null, min: null, max: null };
        }
        return {
          ts: point[tsKey],
          avg: extractRollupValue(point, metricName, 'avg'),
          min: isNet ? null : extractRollupValue(point, metricName, 'min'),
          max: isNet ? null : extractRollupValue(point, metricName, 'max'),
        };
      });
  }, [data, metricName, isLive, isNet]);

  const hasData = chartData.some((p) =>
    isLive ? p.value != null : (p.avg != null || p.min != null || p.max != null)
  );

  if (!hasData) return (
    <div style={cardStyle}>
      <ChartTitle metricName={metricName} />
      <NoData />
    </div>
  );

  const yTickFormatter = (val) => {
    if (isNet) return formatBits(val);
    return `${Number(val).toFixed(1)}`;
  };

  const xTickFormatter = (ts) => formatTime(ts, rangeKey);

  const tooltipFormatter = customTooltipFormatter(metricName);

  return (
    <div style={cardStyle}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 8 }}>
        <ChartTitle metricName={metricName} />
        {isRollup && !isNet && (
          <div style={{ display: 'flex', gap: 6 }}>
            {['avg', 'min', 'max'].map((stat) => (
              <button
                key={stat}
                onClick={() => setVisibleStats((prev) => ({ ...prev, [stat]: !prev[stat] }))}
                style={{
                  padding: '3px 10px', borderRadius: 4, border: 'none', cursor: 'pointer',
                  fontSize: 12, fontWeight: 600, textTransform: 'uppercase',
                  background: visibleStats[stat] ? COLORS[stat] : '#1f2937',
                  color: visibleStats[stat] ? '#fff' : '#6b7280',
                }}
              >
                {stat}
              </button>
            ))}
          </div>
        )}
      </div>

      <ResponsiveContainer width="100%" height={200}>
        {isLive ? (
          <LineChart data={chartData} margin={{ top: 4, right: 16, left: 0, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="#1f2937" />
            <XAxis dataKey="ts" tickFormatter={xTickFormatter} tick={{ fill: '#6b7280', fontSize: 11 }} minTickGap={40} />
            <YAxis tickFormatter={yTickFormatter} tick={{ fill: '#6b7280', fontSize: 11 }} width={60} />
            <Tooltip formatter={tooltipFormatter} labelFormatter={(l) => formatTime(l, rangeKey)} contentStyle={tooltipStyle} />
            <Line type="monotone" dataKey="value" stroke={COLORS.value} dot={false} strokeWidth={2} connectNulls={false} isAnimationActive={false} />
          </LineChart>
        ) : (
          <AreaChart data={chartData} margin={{ top: 4, right: 16, left: 0, bottom: 0 }}>
            <defs>
              <linearGradient id={`grad-avg-${metricName}`} x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor={COLORS.avg} stopOpacity={0.3} />
                <stop offset="95%" stopColor={COLORS.avg} stopOpacity={0} />
              </linearGradient>
              <linearGradient id={`grad-min-${metricName}`} x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor={COLORS.min} stopOpacity={0.3} />
                <stop offset="95%" stopColor={COLORS.min} stopOpacity={0} />
              </linearGradient>
              <linearGradient id={`grad-max-${metricName}`} x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor={COLORS.max} stopOpacity={0.3} />
                <stop offset="95%" stopColor={COLORS.max} stopOpacity={0} />
              </linearGradient>
            </defs>
            <CartesianGrid strokeDasharray="3 3" stroke="#1f2937" />
            <XAxis dataKey="ts" tickFormatter={xTickFormatter} tick={{ fill: '#6b7280', fontSize: 11 }} minTickGap={40} />
            <YAxis tickFormatter={yTickFormatter} tick={{ fill: '#6b7280', fontSize: 11 }} width={60} />
            <Tooltip formatter={tooltipFormatter} labelFormatter={(l) => formatTime(l, rangeKey)} contentStyle={tooltipStyle} />
            {isNet ? (
              <Area type="monotone" dataKey="avg" stroke={COLORS.avg} fill={`url(#grad-avg-${metricName})`} dot={false} strokeWidth={2} connectNulls={false} isAnimationActive={false} name="avg" />
            ) : (
              <>
                {visibleStats.avg && <Area type="monotone" dataKey="avg" stroke={COLORS.avg} fill={`url(#grad-avg-${metricName})`} dot={false} strokeWidth={2} connectNulls={false} isAnimationActive={false} name="avg" />}
                {visibleStats.min && <Area type="monotone" dataKey="min" stroke={COLORS.min} fill={`url(#grad-min-${metricName})`} dot={false} strokeWidth={1.5} connectNulls={false} isAnimationActive={false} name="min" />}
                {visibleStats.max && <Area type="monotone" dataKey="max" stroke={COLORS.max} fill={`url(#grad-max-${metricName})`} dot={false} strokeWidth={1.5} connectNulls={false} isAnimationActive={false} name="max" />}
              </>
            )}
          </AreaChart>
        )}
      </ResponsiveContainer>
    </div>
  );
}

function ChartTitle({ metricName }) {
  return (
    <span style={{ color: '#e5e7eb', fontSize: 14, fontWeight: 600 }}>
      {METRIC_LABELS[metricName] || metricName}
      <span style={{ color: '#6b7280', fontSize: 12, marginLeft: 6 }}>
        ({METRIC_UNITS[metricName] || ''})
      </span>
    </span>
  );
}

const cardStyle = {
  background: '#111827', border: '1px solid #1f2937',
  borderRadius: 10, padding: '14px 16px', marginBottom: 16,
};

const tooltipStyle = {
  background: '#1f2937', border: '1px solid #374151', color: '#f9fafb', fontSize: 12,
};