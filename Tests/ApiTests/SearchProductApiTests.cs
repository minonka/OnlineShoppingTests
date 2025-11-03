using NUnit.Framework;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Tests.ApiTests
{
    [TestFixture]
    [Category("API")]
    public class SearchProductApiTests : BaseApiTest
    {
        [Test]
        public async Task SearchProduct_WithValidKeyword_ShouldReturnMatchingProducts()
        {
            // Arrange
            var searchBody = new { search_product = "top" };

            // Act
            var response = await ApiClient.PostAsync("/api/searchProduct", searchBody);

            // Step 1: Transport-level validation
            ApiAssertions.AssertHttpSuccess(response);

            // Step 2: Parse JSON
            var json = ApiAssertions.ParseJson(response);

            // Step 3: API-level validation
            ApiAssertions.AssertApiResponseCode(json, 200);

            var products = json["products"];
            Assert.That(products, Is.Not.Null, "the response should contain a 'products' field");
            Assert.That(products!.HasValues, Is.True, "the products list should contain at least one entry");

            // Step 4: Optional business rule check — verify product names contain the keyword
            foreach (var product in products!)
            {
                var name = product["name"]?.ToString()?.ToLowerInvariant();
                Assert.That(name, Does.Contain("top"),
                    $"Product name '{name}' should contain the search keyword 'top'");
                Console.Out.WriteLine(name);
            }
        }
    }
}