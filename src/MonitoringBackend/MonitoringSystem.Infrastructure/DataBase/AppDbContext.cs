using Microsoft.EntityFrameworkCore;
using MonitoringSystem.Application.Interfaces;
using MonitoringSystem.Domain.Models;
using MonitoringSystem.Infrastructure.DataBase.Configuration;

namespace MonitoringSystem.Infrastructure.DataBase;

public class AppDbContext(DbContextOptions options) 
    : DbContext(options), IAppDbContext
{
    public DbSet<Metric> Metrics { get; set; }
    public DbSet<MetricSetting> MetricSettings { get; set; }
    public DbSet<MetricRollup1H> MetricRollup1H { get; set; }
    public DbSet<MetricRollup1M> MetricRollup1M { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MetricConfiguration());
        modelBuilder.ApplyConfiguration(new MetricSettingConfiguration());
        modelBuilder.ApplyConfiguration(new Rollup1HConfiguration());
        modelBuilder.ApplyConfiguration(new Rollup1MConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}