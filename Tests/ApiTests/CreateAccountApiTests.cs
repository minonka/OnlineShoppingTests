using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Net;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Tests.ApiTests
{
    [TestFixture]
    [Category("API")]
    public class CreateAccountApiTests : BaseApiTest
    {
        [Test]
        public async Task CreateAccount_WithUniqueEmail_ShouldReturnCreated()
        {
            // Arrange
            var uniqueEmail = $"user_{Guid.NewGuid():N}@example.com";

            var body = new
            {
                name = "API Test User",
                email = uniqueEmail,
                password = "123456",
                title = "Mr",
                birth_date = "10",
                birth_month = "05",
                birth_year = "1990",
                firstname = "API",
                lastname = "Tester",
                company = "Automation Co",
                address1 = "123 Test Street",
                address2 = "Suite 100",
                country = "United States",
                zipcode = "12345",
                state = "CA",
                city = "Los Angeles",
                mobile_number = "1234567890"
            };

            // Act
            var response = await ApiClient.PostAsync("/api/createAccount", body);

            // Step 1: HTTP-level check
            ApiAssertions.AssertHttpSuccess(response);

            // Step 2: Parse JSON
            var json = ApiAssertions.ParseJson(response);

            // Step 3: API-level validation
            ApiAssertions.AssertApiResponseCode(json, 201);

            ApiAssertions.AssertApiMessage(json, "User created");
        }

        [Test]
        public async Task CreateAccount_WithExistingEmail_ShouldReturnError()
        {
            // Arrange
            var existingEmail = "testuser@example.com";

            var body = new
            {
                name = "Existing User",
                email = existingEmail,
                password = "123456",
                title = "Ms",
                birth_date = "01",
                birth_month = "01",
                birth_year = "1995",
                firstname = "Already",
                lastname = "Exists",
                company = "Automation Co",
                address1 = "123 Main St",
                address2 = "",
                country = "United States",
                zipcode = "54321",
                state = "NY",
                city = "New York",
                mobile_number = "9876543210"
            };

            // Act
            var response = await ApiClient.PostAsync("/api/createAccount", body);

            // Step 1: HTTP-level check
            ApiAssertions.AssertHttpSuccess(response);

            // Step 2: Parse JSON
            var json = ApiAssertions.ParseJson(response);

            // Step 3: API-level check
            ApiAssertions.AssertApiResponseCode(json, 400);
            ApiAssertions.AssertApiMessage(json, "Email already exists");
        }
    }
}