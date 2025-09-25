using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BudgetTracker.Tests.Controllers;


public class BudgetsControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateBudget_WithValidData_ReturnsCreated()
    {
        var now = DateTime.UtcNow;
        var request = new CreateBudgetRequest
        {
            Year = now.Year,
            Month = now.Month,
            SubcategoryId = 5,
            PlannedAmount = 500.00m,
            Currency = "USD"
        };
        
        var response = await Client.PostAsJsonAsync("/api/budgets", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeNullOrEmpty();
        TrackBudget(result.Id);
    }

    [Fact]
    public async Task GetBudgets_ReturnsUserBudgets()
    {
        var now = DateTime.UtcNow;
        var createRequest = new CreateBudgetRequest
        {
            Year = now.Year,
            Month = now.Month,
            SubcategoryId = 6,
            PlannedAmount = 300.00m,
            Currency = "USD"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/budgets", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        TrackBudget(created!.Id);

        var response = await Client.GetAsync($"/api/budgets?year={now.Year}&month={now.Month}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var budgets = await response.Content.ReadFromJsonAsync<List<BudgetDto>>();
        budgets.Should().NotBeNull();
        budgets.Should().Contain(b => b.SubcategoryName == "Transportation");
    }

    [Fact]
    public async Task CreateRecord_AutomaticallyUpdatesBudget()
    {
        var now = DateTime.UtcNow;
        var budgetRequest = new CreateBudgetRequest
        {
            Year = now.Year,
            Month = now.Month,
            SubcategoryId = 7,
            PlannedAmount = 200.00m,
            Currency = "USD"
        };
        var budgetResponse = await Client.PostAsJsonAsync("/api/budgets", budgetRequest);
        var budget = await budgetResponse.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        TrackBudget(budget!.Id);

        var recordRequest = new CreateRecordRequest
        {
            Amount = -75.00m,
            CategoryId = 2,
            SubcategoryId = 7,
            DateTime = now,
            Description = "Shopping expense to test budget update",
            Currency = "USD"
        };
        var recordResponse = await Client.PostAsJsonAsync("/api/records", recordRequest);
        var record = await recordResponse.Content.ReadFromJsonAsync<CreateRecordResponse>();
        TrackRecord(record!.Id);

        var getBudgetsResponse = await Client.GetAsync($"/api/budgets?year={now.Year}&month={now.Month}");
        var budgets = await getBudgetsResponse.Content.ReadFromJsonAsync<List<BudgetDto>>();
        
        var updatedBudget = budgets!.First(b => b.SubcategoryName == "Shopping");
        updatedBudget.ActualSpent.Should().Be(75.00m);
        updatedBudget.RemainingAmount.Should().Be(125.00m);
        updatedBudget.UtilizationPercentage.Should().BeApproximately(37.5m, 0.1m);
    }

    [Fact]
    public async Task UpdateBudget_WithNewAmount_UpdatesSuccessfully()
    {
        var now = DateTime.UtcNow;
        var createRequest = new CreateBudgetRequest
        {
            Year = now.Year,
            Month = now.Month,
            SubcategoryId = 8,
            PlannedAmount = 150.00m,
            Currency = "USD"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/budgets", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        TrackBudget(created!.Id);

        var updateRequest = new UpdateBudgetRequest { PlannedAmount = 250.00m };

        var response = await Client.PutAsJsonAsync($"/api/budgets/{created.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteBudget_WithValidId_ReturnsNoContent()
    {
        var now = DateTime.UtcNow;
        var createRequest = new CreateBudgetRequest
        {
            Year = now.Year,
            Month = now.Month,
            SubcategoryId = 9,
            PlannedAmount = 100.00m,
            Currency = "USD"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/budgets", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateBudgetResponse>();

        var response = await Client.DeleteAsync($"/api/budgets/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private class CreateBudgetRequest
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int SubcategoryId { get; set; }
        public decimal PlannedAmount { get; set; }
        public string Currency { get; set; } = "USD";
    }

    private class UpdateBudgetRequest
    {
        public decimal PlannedAmount { get; set; }
    }

    private class CreateBudgetResponse
    {
        public string Id { get; set; } = string.Empty;
    }

    private class CreateRecordRequest
    {
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
    }

    private class CreateRecordResponse
    {
        public string Id { get; set; } = string.Empty;
    }

    private class BudgetDto
    {
        public string Id { get; set; } = string.Empty;
        public string SubcategoryName { get; set; } = string.Empty;
        public decimal PlannedAmount { get; set; }
        public decimal ActualSpent { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal UtilizationPercentage { get; set; }
        public bool IsOverBudget { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}