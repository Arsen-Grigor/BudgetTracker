using System.Security.Claims;
using BudgetTracker.Application.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetTrackerService _service;
    private readonly ILogger<BudgetsController> _logger;

    public BudgetsController(IBudgetTrackerService service, ILogger<BudgetsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) 
               ?? throw new UnauthorizedAccessException("User ID not found in token");
    }

    [HttpGet]
    public async Task<IActionResult> GetBudgets(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var query = new GetBudgetsQuery(userId, year, month);
        
        _logger.LogInformation("Fetching budgets for user {UserId}, {Year}-{Month}", userId, year, month);
        
        var budgets = await _service.GetBudgetsAsync(query, cancellationToken);
        return Ok(budgets);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBudget(
        [FromBody] CreateBudgetRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var command = new CreateBudgetCommand(
            userId,
            request.Year,
            request.Month,
            request.SubcategoryId,
            request.PlannedAmount,
            request.Currency ?? "USD"
        );

        _logger.LogInformation("Creating budget for user {UserId}, {Year}-{Month}, subcategory {SubcategoryId}", 
            userId, request.Year, request.Month, request.SubcategoryId);
        
        var budgetId = await _service.CreateBudgetAsync(command, cancellationToken);
        
        _logger.LogInformation("Budget {BudgetId} created successfully", budgetId);
        
        return CreatedAtAction(
            nameof(GetBudgets),
            new { year = request.Year, month = request.Month },
            new { id = budgetId }
        );
    }

    [HttpPut("{budgetId}")]
    public async Task<IActionResult> UpdateBudget(
        string budgetId,
        [FromBody] UpdateBudgetRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var command = new UpdateBudgetCommand(budgetId, userId, request.PlannedAmount);

        _logger.LogInformation("Updating budget {BudgetId} for user {UserId}", budgetId, userId);
        
        await _service.UpdateBudgetAsync(command, cancellationToken);
        
        _logger.LogInformation("Budget {BudgetId} updated successfully", budgetId);
        
        return NoContent();
    }

    [HttpDelete("{budgetId}")]
    public async Task<IActionResult> DeleteBudget(
        string budgetId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var command = new DeleteBudgetCommand(budgetId, userId);

        _logger.LogInformation("Deleting budget {BudgetId} for user {UserId}", budgetId, userId);
        
        await _service.DeleteBudgetAsync(command, cancellationToken);
        
        _logger.LogInformation("Budget {BudgetId} deleted successfully", budgetId);
        
        return NoContent();
    }
}

public class CreateBudgetRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int SubcategoryId { get; set; }
    public decimal PlannedAmount { get; set; }
    public string? Currency { get; set; }
}

public class UpdateBudgetRequest
{
    public decimal PlannedAmount { get; set; }
}