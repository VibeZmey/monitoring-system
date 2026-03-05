using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MonitoringSystem.Application.UseCases.MetricSettings.SetMetricSettings;
using MonitoringSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MonitoringSystem.Application.UseCases.MetricSettings.GetEnabledMetrics;

namespace MonitoringSystem.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricSettingsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SetMetricSetting([FromBody] SetMetricSettingsCommand metricSetting)
    {
        await mediator.Send(metricSetting);
        return Ok(await mediator.Send(new GetEnabledMetricsQuery()));
    }
    
}