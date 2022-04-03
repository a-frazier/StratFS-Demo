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
    public IEnumerable<CreditDatum> Get()
    {
        return _creditDataService.CreditData.Value;
    }
}
