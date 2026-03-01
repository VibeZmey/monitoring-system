using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonitoringSystem.Domain.Models;

namespace MonitoringSystem.Infrastructure.DataBase.Configuration;

public class MetricConfiguration : IEntityTypeConfiguration<Metric>
{
    public void Configure(EntityTypeBuilder<Metric> builder)
    {
        builder.HasKey(m => m.Ts);
        
        builder.Property(m => m.Id)
            .UseIdentityByDefaultColumn();
    }
}