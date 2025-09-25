using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BudgetTracker.Tests.Controllers;


public class CategoriesControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task GetCategories_ReturnsAllCategoriesWithSubcategories()
    {
        var response = await Client.GetAsync("/api/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        
        categories.Should().NotBeNull();
        categories.Should().HaveCount(2);
        categories.Should().Contain(c => c.Name == "Income");
        categories.Should().Contain(c => c.Name == "Expenses");
        
        var expenseCategory = categories!.First(c => c.Name == "Expenses");
        expenseCategory.Subcategories.Should().NotBeEmpty();
        expenseCategory.Subcategories.Should().Contain(s => s.Name == "Food");
    }

    private class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SubcategoryDto> Subcategories { get; set; } = new();
    }

    private class SubcategoryDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}