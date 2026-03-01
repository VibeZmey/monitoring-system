using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonitoringSystem.Domain.Models;

namespace MonitoringSystem.Infrastructure.DataBase.Configuration;

public class MetricSettingConfiguration : IEntityTypeConfiguration<MetricSetting>
{
    public void Configure(EntityTypeBuilder<MetricSetting> builder)
    {
        builder.HasKey(s => s.MetricName);
    }
}