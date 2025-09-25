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
public class RecordsController : ControllerBase
{
    private readonly IBudgetTrackerService _service;
    private readonly ILogger<RecordsController> _logger;

    public RecordsController(IBudgetTrackerService service, ILogger<RecordsController> logger)
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
    public async Task<IActionResult> GetRecords(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int? subcategoryId = null,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var query = new GetRecordsQuery(userId, year, month, subcategoryId);
        
        _logger.LogInformation("Fetching records for user {UserId}, {Year}-{Month}", userId, year, month);
        
        var records = await _service.GetRecordsAsync(query, cancellationToken);
        return Ok(records);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecord(
        [FromBody] CreateRecordRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var command = new CreateRecordCommand(
            userId,
            request.Amount,
            request.CategoryId,
            request.SubcategoryId,
            request.DateTime,
            request.Description,
            request.Currency ?? "USD"
        );

        _logger.LogInformation("Creating record for user {UserId}", userId);
        
        var recordId = await _service.CreateRecordAsync(command, cancellationToken);
        
        _logger.LogInformation("Record {RecordId} created successfully", recordId);
        
        return CreatedAtAction(
            nameof(GetRecords),
            new { year = request.DateTime.Year, month = request.DateTime.Month },
            new { id = recordId }
        );
    }

    [HttpPut("{recordId}")]
    public async Task<IActionResult> UpdateRecord(
        string recordId,
        [FromBody] UpdateRecordRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var command = new UpdateRecordCommand(recordId, userId, request.Amount, request.Description);

        _logger.LogInformation("Updating record {RecordId} for user {UserId}", recordId, userId);
        
        await _service.UpdateRecordAsync(command, cancellationToken);
        
        _logger.LogInformation("Record {RecordId} updated successfully", recordId);
        
        return NoContent();
    }

    [HttpDelete("{recordId}")]
    public async Task<IActionResult> DeleteRecord(
        string recordId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var command = new DeleteRecordCommand(recordId, userId);

        _logger.LogInformation("Deleting record {RecordId} for user {UserId}", recordId, userId);
        
        await _service.DeleteRecordAsync(command, cancellationToken);
        
        _logger.LogInformation("Record {RecordId} deleted successfully", recordId);
        
        return NoContent();
    }
}

public class CreateRecordRequest
{
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
    public int SubcategoryId { get; set; }
    public DateTime DateTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Currency { get; set; }
}

public class UpdateRecordRequest
{
    public decimal? Amount { get; set; }
    public string? Description { get; set; }
}