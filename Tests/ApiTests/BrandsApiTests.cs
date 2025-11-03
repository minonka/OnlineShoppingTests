using NUnit.Framework;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Tests.ApiTests 
{ 
    [TestFixture]
    [Category("API")]
    public class BrandsApiTests : BaseApiTest 
    { 
        [Test]
        public async Task Get_AllBrands_ShouldReturnSuccessAndList() 
        {
            var response = await ApiClient.GetAsync("/api/brandsList");

            // Assert - HTTP level 
            ApiAssertions.AssertHttpSuccess(response);

            // Parse response
            var json = ApiAssertions.ParseJson(response);

            // Assert - API-level response code
            ApiAssertions.AssertApiResponseCode(json, 200);

            // Assert - brands structure
            var brands = json["brands"];
            Assert.That(brands, Is.Not.Null, "the response should contain a 'brands' field");
            Assert.That(brands!.HasValues, Is.True, "the brands list should contain at least one entry");
        } 
    } 
}