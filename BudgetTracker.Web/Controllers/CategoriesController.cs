using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IBudgetTrackerService _service;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IBudgetTrackerService service, ILogger<CategoriesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all categories");
        
        var query = new GetCategoriesQuery();
        var categories = await _service.GetCategoriesAsync(query, cancellationToken);
        
        return Ok(categories);
    }
}