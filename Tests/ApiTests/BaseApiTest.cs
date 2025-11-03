using NUnit.Framework;
using OnlineShoppingTests.Utils;
using AventStack.ExtentReports;

namespace OnlineShoppingTests.Tests.ApiTests
{
    public class BaseApiTest
    {
        protected ApiClient ApiClient;

        [SetUp]
        public void SetUp()
        {
            var baseUrl = ConfigManager.GetApiBaseUrl();
            ApiClient = new ApiClient(baseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            ApiClient?.Dispose();
        }
    }
}