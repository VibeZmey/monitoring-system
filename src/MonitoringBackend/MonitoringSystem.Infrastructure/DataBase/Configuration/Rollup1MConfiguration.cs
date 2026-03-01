using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonitoringSystem.Domain.Models;

namespace MonitoringSystem.Infrastructure.DataBase.Configuration;

public class Rollup1MConfiguration : IEntityTypeConfiguration<MetricRollup1M>
{
    public void Configure(EntityTypeBuilder<MetricRollup1M> builder)
    {
        builder.HasKey(m => new {m.BucketTs, m.Id});
        
        builder.Property(m => m.Id)
            .UseIdentityByDefaultColumn();
    }
}

