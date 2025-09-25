using System.Net.Http.Json;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetTracker.Tests;

public class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly WebAppFactory Factory;
    protected readonly IConfiguration Configuration;
    protected string? AuthToken;
    protected string? UserId;
    private readonly List<string> _recordIds = new();
    private readonly List<string> _budgetIds = new();

    protected IntegrationTestBase()
    {
        Factory = new WebAppFactory();
        Client = Factory.CreateClient();
        
        var scope = Factory.Services.CreateScope();
        Configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    }

    public virtual async Task InitializeAsync()
    {
        await AuthenticateAsync();
    }

    public virtual async Task DisposeAsync()
    {
        await CleanupTestDataAsync();
        Client.Dispose();
        await Factory.DisposeAsync();
    }

    protected async Task AuthenticateAsync()
    {
        var loginRequest = new
        {
            Username = Configuration["Auth:Username"],
            Password = Configuration["Auth:Password"]
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        AuthToken = loginResponse?.Token;
        UserId = loginResponse?.UserId;

        Client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthToken);
    }

    protected void TrackRecord(string recordId) => _recordIds.Add(recordId);
    protected void TrackBudget(string budgetId) => _budgetIds.Add(budgetId);

    private async Task CleanupTestDataAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BudgetTrackerDbContext>();

        foreach (var recordId in _recordIds)
        {
            if (Guid.TryParse(recordId, out var guid))
            {
                var recordIdTyped = new BudgetTracker.Domain.Entities.Records.RecordId(guid);
                var record = await context.Records
                    .FirstOrDefaultAsync(r => r.Id == recordIdTyped);
                if (record != null) context.Records.Remove(record);
            }
        }

        foreach (var budgetId in _budgetIds)
        {
            if (Guid.TryParse(budgetId, out var guid))
            {
                var budgetIdTyped = new BudgetTracker.Domain.Entities.Budgets.MonthCategoryBudgetId(guid);
                var budget = await context.Budgets
                    .FirstOrDefaultAsync(b => b.Id == budgetIdTyped);
                if (budget != null) context.Budgets.Remove(budget);
            }
        }

        await context.SaveChangesAsync();
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }
}