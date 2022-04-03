using Demo2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo2.Controllers;

[ApiController]
[Route("[controller]")]
public class DebtController : ControllerBase
{
    private readonly ILogger<DebtController> _logger;
    private readonly DebtCalculatorService _debtCalculatorService;

    public DebtController(ILogger<DebtController> logger, DebtCalculatorService debtCalculatorService)
    {
        _logger = logger;
        _debtCalculatorService = debtCalculatorService;
    }

    [HttpGet]
    public async Task<ActionResult<DebtResponse>> Get([FromQuery] long applicationId, [FromQuery] decimal annualIncome)
    {
        if (annualIncome == 0)
            return BadRequest();

        var result = await _debtCalculatorService.CalculateDebtResponse(applicationId, annualIncome);
        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
