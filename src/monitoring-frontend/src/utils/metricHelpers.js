const INT_MAX = 2147483647;

// Sentinel значения из дефолтного конструктора rollup
export function isRollupPointEmpty(point, metricName) {
  const name = metricName.toLowerCase();

  if (name.includes('net')) return false; // 0 валидно для сети

  const minKey = getMinKey(metricName);
  const maxKey = getMaxKey(metricName);

  if (!minKey || !maxKey) return false;

  const min = point[minKey];
  const max = point[maxKey];

  if (min == null || max == null) return true;

  // Usage % sentinel: min >= 101, max < 0
  if (name.includes('usage')) return min >= 101 && max < 0;

  // Temp sentinel: min >= 150, max < 0
  if (name.includes('temp')) return min >= 150 && max < 0;

  // UsedMb sentinel: min >= INT_MAX, max < 0
  if (name.includes('mb')) return min >= INT_MAX && max < 0;

  return false;
}

export function getMinKey(metricName) {
  const map = {
    CpuUsagePct: 'cpuUsageMin',
    CpuTempC: 'cpuTempMin',
    GpuUsagePct: 'gpuUsageMin',
    GpuTempC: 'gpuTempMin',
    RamUsagePct: 'ramUsageMin',
    RamUsedMb: 'ramUsedMbMin',
    DiskUsagePct: 'diskUsageMin',
    DiskUsedMb: 'diskUsedMbMin',
  };
  return map[metricName] || null;
}

export function getMaxKey(metricName) {
  const map = {
    CpuUsagePct: 'cpuUsageMax',
    CpuTempC: 'cpuTempMax',
    GpuUsagePct: 'gpuUsageMax',
    GpuTempC: 'gpuTempMax',
    RamUsagePct: 'ramUsageMax',
    RamUsedMb: 'ramUsedMbMax',
    DiskUsagePct: 'diskUsageMax',
    DiskUsedMb: 'diskUsedMbMax',
  };
  return map[metricName] || null;
}

export function getAvgKey(metricName) {
  const map = {
    CpuUsagePct: 'cpuUsageAvg',
    CpuTempC: 'cpuTempAvg',
    GpuUsagePct: 'gpuUsageAvg',
    GpuTempC: 'gpuTempAvg',
    RamUsagePct: 'ramUsageAvg',
    RamUsedMb: 'ramUsedMbAvg',
    DiskUsagePct: 'diskUsageAvg',
    DiskUsedMb: 'diskUsedMbAvg',
    NetBytesSentTotal: 'netSentBytesPerSecAvg',
    NetBytesRecvTotal: 'netRecvBytesPerSecAvg',
  };
  return map[metricName] || null;
}

export function getRawKey(metricName) {
  const map = {
    CpuUsagePct: 'cpuUsagePct',
    CpuTempC: 'cpuTempC',
    GpuUsagePct: 'gpuUsagePct',
    GpuTempC: 'gpuTempC',
    RamUsagePct: 'ramUsagePct',
    RamUsedMb: 'ramUsedMb',
    RamTotalMb: 'ramTotalMb',
    DiskUsagePct: 'diskUsagePct',
    DiskUsedMb: 'diskUsedMb',
    DiskTotalMb: 'diskTotalMb',
    NetBytesSentTotal: 'netBytesSentTotal',
    NetBytesRecvTotal: 'netBytesRecvTotal',
  };
  return map[metricName] || null;
}

export const METRIC_LABELS = {
  CpuUsagePct: 'CPU Usage',
  CpuTempC: 'CPU Temperature',
  GpuUsagePct: 'GPU Usage',
  GpuTempC: 'GPU Temperature',
  RamUsagePct: 'RAM Usage',
  RamUsedMb: 'RAM Used',
  RamTotalMb: 'RAM Total',
  DiskUsagePct: 'Disk Usage',
  DiskUsedMb: 'Disk Used',
  DiskTotalMb: 'Disk Total',
  NetBytesSentTotal: 'Network Sent',
  NetBytesRecvTotal: 'Network Received',
};

export const METRIC_UNITS = {
  CpuUsagePct: '%',
  CpuTempC: '°C',
  GpuUsagePct: '%',
  GpuTempC: '°C',
  RamUsagePct: '%',
  RamUsedMb: 'MB',
  RamTotalMb: 'MB',
  DiskUsagePct: '%',
  DiskUsedMb: 'MB',
  DiskTotalMb: 'MB',
  NetBytesSentTotal: 'bit/s',
  NetBytesRecvTotal: 'bit/s',
};

export const ALL_METRIC_NAMES = [
  'CpuUsagePct',
  'CpuTempC',
  'GpuUsagePct',
  'GpuTempC',
  'RamUsagePct',
  'RamUsedMb',
  'RamTotalMb',
  'DiskUsagePct',
  'DiskUsedMb',
  'DiskTotalMb',
  'NetBytesSentTotal',
  'NetBytesRecvTotal',
];

// Конвертация rollup network: bytes/sec → bits/sec
export function rollupNetToBits(value) {
  if (value == null) return null;
  return value * 8;
}

export function extractRollupValue(point, metricName, statKey) {
  const isNet = metricName.includes('Net');

  if (isNet) {
    const avgKey = getAvgKey(metricName);
    const val = avgKey ? point[avgKey] : null;
    return rollupNetToBits(val);
  }

  if (statKey === 'avg') {
    const key = getAvgKey(metricName);
    return key ? point[key] : null;
  }
  if (statKey === 'min') {
    const key = getMinKey(metricName);
    return key ? point[key] : null;
  }
  if (statKey === 'max') {
    const key = getMaxKey(metricName);
    return key ? point[key] : null;
  }
  return null;
}