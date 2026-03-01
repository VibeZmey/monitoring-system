using Microsoft.EntityFrameworkCore;
using MonitoringSystem.Domain.Models;

namespace MonitoringSystem.Application.Interfaces;

public interface IAppDbContex
{
    DbSet<Metric> Metrics { get; set; }
    DbSet<MetricSetting> MetricSettings { get; set; }
    DbSet<MetricRollup1H> MetricRollup1H { get; set; }
    DbSet<MetricRollup1M> MetricRollup1M { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}