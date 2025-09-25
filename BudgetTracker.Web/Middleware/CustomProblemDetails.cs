using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Web.Middleware;

public class CustomProblemDetails : ProblemDetails
{
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}