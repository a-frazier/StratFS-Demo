using Demo1.Models;
using Demo1.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Demo1.Controllers;

[ApiController]
[Route("[controller]")]
public class CreditDataController : ControllerBase
{
    private readonly ILogger<CreditDataController> _logger;
    private readonly CreditDataService _creditDataService;

    public CreditDataController(ILogger<CreditDataController> logger, CreditDataService creditDataService)
    {
        _logger = logger;
        _creditDataService = creditDataService;
    }

    [HttpGet]
    public IEnumerable<CreditDatum> Get([FromQuery] long? applicationId, [FromQuery] string? source, [FromQuery] string? bureau)
    {
        var result = _creditDataService.CreditData.Value.AsEnumerable();
        if (applicationId != null)
            result = result.Where(d => applicationId.Value == d.ApplicationId);
        if (source != null)
            result = result.Where(d => string.Equals(source, d.Source, StringComparison.OrdinalIgnoreCase));
        if (bureau != null)
            result = result.Where(d => string.Equals(bureau, d.Bureau, StringComparison.OrdinalIgnoreCase));

        return result;
    }
}
