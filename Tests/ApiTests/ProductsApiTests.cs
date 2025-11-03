using NUnit.Framework;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Tests.ApiTests
{
    [TestFixture]
    [Category("API")]
    public class ProductsApiTests : BaseApiTest
    {
        [Test]
        public async Task Get_AllProducts_ShouldReturnSuccessAndList()
        {
            // Act
            var response = await ApiClient.GetAsync("/api/productsList");

            // Step 1: Transport-level validation
            ApiAssertions.AssertHttpSuccess(response);

            // Step 2: Parse JSON
            var json = ApiAssertions.ParseJson(response);

            // Step 3: API-level response validation
            ApiAssertions.AssertApiResponseCode(json, 200);

            // Step 4: Verify response structure
            var products = json["products"];
            Assert.That(products, Is.Not.Null, "The response should include a 'products' field");
            Assert.That(products!.HasValues, Is.True, "The products list should contain at least one item");
        }
    }
}