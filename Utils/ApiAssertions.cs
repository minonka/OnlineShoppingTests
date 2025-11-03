using NUnit.Framework;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace OnlineShoppingTests.Utils
{
    public static class ApiAssertions
    {
        /// <summary>
        /// Asserts that the HTTP response is successful (status 200) and not empty.
        /// </summary>
        public static void AssertHttpSuccess(RestResponse response)
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                $"Expected HTTP 200 OK, but got {(int)response.StatusCode} {response.StatusCode}");

            Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                "Response content should not be null or empty");
        }

        /// <summary>
        /// Parses JSON and returns it as a JObject, ensuring it's valid JSON.
        /// </summary>
        public static JObject ParseJson(RestResponse response)
        {
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                "Response must contain JSON content to parse");
            return JObject.Parse(response.Content!);
        }

        /// <summary>
        /// Asserts that the API’s internal responseCode equals the expected value.
        /// </summary>
        public static void AssertApiResponseCode(JObject json, int expectedCode)
        {
            var actualCode = (int)json["responseCode"]!;
            Assert.That(actualCode, Is.EqualTo(expectedCode),
                $"Expected API responseCode {expectedCode}, but got {actualCode}");
        }

        /// <summary>
        /// Asserts that the JSON contains a message field with the expected text (case-insensitive partial match).
        /// </summary>
        public static void AssertApiMessage(JObject json, string expectedMessagePart)
        {
            var message = json["message"]?.ToString();
            Assert.That(message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"Expected message to contain '{expectedMessagePart}', but got '{message}'");
        }
    }
}