using System.Text.RegularExpressions;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.Repositories;

public abstract class BaseSecureRepository
{
    protected readonly BudgetTrackerDbContext _context;
    protected readonly ILogger _logger;

    protected BaseSecureRepository(BudgetTrackerDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    protected static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be null or empty", nameof(userId));
        
        if (userId.Length > 450)
            throw new ArgumentException("UserId too long", nameof(userId));
        
        // this will allow only alphanumeric,hyphens,underscores
        if (!Regex.IsMatch(userId, @"^[a-zA-Z0-9\-_@.]+$"))
            throw new ArgumentException("Invalid UserId format", nameof(userId));
    }

    protected static Guid ValidateAndParseGuid(string guidString, string paramName)
    {
        if (string.IsNullOrWhiteSpace(guidString))
            throw new ArgumentException($"{paramName} cannot be null or empty", paramName);

        if (!Guid.TryParse(guidString, out var guid))
            throw new ArgumentException($"Invalid {paramName} format", paramName);

        return guid;
    }

    protected static string SanitizeString(string input, int maxLength = 500)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
            
        var sanitized = Regex.Replace(input, @"[<>""'%;()&+]", "");
        
        return sanitized.Length > maxLength ? sanitized.Substring(0, maxLength) : sanitized;
    }

    protected void LogSuspiciousActivity(string action, string userId, string details)
    {
        _logger.LogWarning("Suspicious activity detected. Action: {Action}, UserId: {UserId}, Details: {Details}", 
            action, userId, details);
    }
}