using NUnit.Framework;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Tests.ApiTests
{
    [TestFixture]
    [Category("API")]
    public class LoginApiTests : BaseApiTest
    {
        [Test]
        public async Task VerifyLogin_WithValidCredentials_ShouldReturnSuccessMessage()
        {
            // Arrange
            var body = new
            {
                email = "testuser@example.com",
                password = "123456"
            };

            // Act
            var response = await ApiClient.PostAsync("/api/verifyLogin", body);

            // Step 1: Transport-level check
            ApiAssertions.AssertHttpSuccess(response);

            // Step 2: Parse JSON
            var json = ApiAssertions.ParseJson(response);

            // Step 3: API-level and content checks
            ApiAssertions.AssertApiResponseCode(json, 200);
            ApiAssertions.AssertApiMessage(json, "User exists");
        }

        [Test]
        public async Task VerifyLogin_WithInvalidCredentials_ShouldReturnUserNotFound()
        {
            // Arrange
            var body = new
            {
                email = "nonexistent@example.com",
                password = "wrongpassword"
            };

            // Act
            var response = await ApiClient.PostAsync("/api/verifyLogin", body);

            // Step 1: HTTP check
            ApiAssertions.AssertHttpSuccess(response);

            // Step 2: Parse JSON
            var json = ApiAssertions.ParseJson(response);

            // Step 3: API-level validation
            ApiAssertions.AssertApiResponseCode(json, 404);
            ApiAssertions.AssertApiMessage(json, "not found");
        }
    }
}