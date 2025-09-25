using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BudgetTracker.Tests.Controllers;

public class RecordsControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateRecord_WithValidData_ReturnsCreated()
    {
        var request = new CreateRecordRequest
        {
            Amount = -50.00m,
            CategoryId = 2,
            SubcategoryId = 5,
            DateTime = DateTime.UtcNow,
            Description = "Integration test - grocery shopping",
            Currency = "USD"
        };

        var response = await Client.PostAsJsonAsync("/api/records", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateRecordResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeNullOrEmpty();
        TrackRecord(result.Id);
    }

    [Fact]
    public async Task GetRecords_ReturnsUserRecords()
    {
        var createRequest = new CreateRecordRequest
        {
            Amount = -30.00m,
            CategoryId = 2,
            SubcategoryId = 6,
            DateTime = DateTime.UtcNow,
            Description = "Integration test - transport",
            Currency = "USD"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/records", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateRecordResponse>();
        TrackRecord(created!.Id);

        var now = DateTime.UtcNow;

        var response = await Client.GetAsync($"/api/records?year={now.Year}&month={now.Month}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var records = await response.Content.ReadFromJsonAsync<List<RecordDto>>();
        records.Should().NotBeNull();
        records.Should().Contain(r => r.Description == "Integration test - transport");
    }

    [Fact]
    public async Task UpdateRecord_WithValidData_ReturnsNoContent()
    {
        var createRequest = new CreateRecordRequest
        {
            Amount = -25.00m,
            CategoryId = 2,
            SubcategoryId = 7,
            DateTime = DateTime.UtcNow,
            Description = "Original description",
            Currency = "USD"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/records", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateRecordResponse>();
        TrackRecord(created!.Id);

        var updateRequest = new UpdateRecordRequest
        {
            Amount = -35.00m,
            Description = "Updated description"
        };

        var response = await Client.PutAsJsonAsync($"/api/records/{created.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteRecord_WithValidId_ReturnsNoContent()
    {
        var createRequest = new CreateRecordRequest
        {
            Amount = -15.00m,
            CategoryId = 2,
            SubcategoryId = 8,
            DateTime = DateTime.UtcNow,
            Description = "To be deleted",
            Currency = "USD"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/records", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateRecordResponse>();

        var response = await Client.DeleteAsync($"/api/records/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetRecords_FilteredBySubcategory_ReturnsFilteredRecords()
    {
        var createRequest = new CreateRecordRequest
        {
            Amount = -45.00m,
            CategoryId = 2,
            SubcategoryId = 5,
            DateTime = DateTime.UtcNow,
            Description = "Food expense for filtering test",
            Currency = "USD"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/records", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateRecordResponse>();
        TrackRecord(created!.Id);

        var now = DateTime.UtcNow;

        var response = await Client.GetAsync($"/api/records?year={now.Year}&month={now.Month}&subcategoryId=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var records = await response.Content.ReadFromJsonAsync<List<RecordDto>>();
        records.Should().NotBeNull();
        records.Should().OnlyContain(r => r.SubcategoryName == "Food");
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

    private class UpdateRecordRequest
    {
        public decimal? Amount { get; set; }
        public string? Description { get; set; }
    }

    private class CreateRecordResponse
    {
        public string Id { get; set; } = string.Empty;
    }

    private class RecordDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SubcategoryName { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}