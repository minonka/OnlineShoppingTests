using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using OnlineShoppingTests.Pages;
using OnlineShoppingTests.Utils;
using OnlineShoppingTests.TestData.Models;
using OnlineShoppingTests.TestReporting;

namespace OnlineShoppingTests.Tests
{
    public class BaseTest
    {
        private IWebDriver actualDriver;
        protected LoggingWebDriver driver;
        protected HtmlTestReport report = null!;
        public string TestName { get; private set; } = string.Empty;


        // Shared Page Object instances
        protected HomePage homePage;
        protected ProductPage productPage;
        protected CartPage cartPage;
        protected RegistrationPage registrationPage;
        protected CheckoutPage checkoutPage;
        protected PaymentPage paymentPage;

        protected static readonly IReadOnlyDictionary<string, User> Users = UserDataManager.GetAllUsers();
        
        [SetUp]
        public void SetUp()
        {
            var browser = ConfigManager.Browser;
            var baseUrl = ConfigManager.GetUiBaseUrl();

            actualDriver = StartBrowser(browser);
            actualDriver.Manage().Window.Maximize();
            actualDriver.Manage().Cookies.DeleteAllCookies();
            actualDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(ConfigManager.Timeout);
            HandleGdprConsent();

            // Initialize logging
            TestName = TestContext.CurrentContext.Test.Name;
            report = new HtmlTestReport(TestName);

            // Wrap driver with logging capability
            driver = new LoggingWebDriver(actualDriver, report);

            // Now use the wrapped driver
            driver.Navigate().GoToUrl(baseUrl);
            HandleGdprConsent();

            // Initialize all common page objects
            InitializePageObjects();
        }

        private IWebDriver StartBrowser(string browser)
        {
            switch (browser.ToLower())
            {
                case "edge":
                    new DriverManager().SetUpDriver(new EdgeConfig());
                    return new EdgeDriver();

                case "firefox":
                    new DriverManager().SetUpDriver(new FirefoxConfig());
                    return new FirefoxDriver();

                case "chrome":
                default:
                    new DriverManager().SetUpDriver(new ChromeConfig());
                    return new ChromeDriver();
            }
        }

        private void InitializePageObjects()
        {
            homePage = new HomePage(driver);
            productPage = new ProductPage(driver);
            cartPage = new CartPage(driver);
            registrationPage = new RegistrationPage(driver);
            checkoutPage = new CheckoutPage(driver);
            paymentPage = new PaymentPage(driver);
        }
        
        private void HandleGdprConsent()
        {
            try
            {
                var consentButton = driver.FindElement(By.CssSelector("button.fc-button.fc-cta-consent.fc-primary-button"));
                if (consentButton.Displayed && consentButton.Enabled)
                {
                    consentButton.Click();
                    TestContext.Out.WriteLine("GDPR consent accepted.");
                }
            }
            catch (NoSuchElementException)
            {
                // Popup not present; nothing to do
            }
            catch (Exception e)
            {
                TestContext.Out.WriteLine($"Error handling GDPR consent: {e.Message}");
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Take screenshot before disposing the driver
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                TakeScreenshot(TestContext.CurrentContext.Test.Name);
            }

            // Delete user if logged in
            DeleteUser();

            GenerateReportFile();

            // Always dispose driver(s)
            driver.Quit();
            driver.Dispose();
            actualDriver.Quit();
            actualDriver.Dispose();
        }

        private void TakeScreenshot(string testName)
        {
            try
            {
                if (driver is ITakesScreenshot screenshotDriver)
                {
                    // Ensure absolute folder path
                    var projectDir = Directory.GetParent(AppContext.BaseDirectory)
                                      ?.Parent?.Parent?.Parent?.FullName;
                    var dir = Path.Combine(projectDir!, ConfigManager.ScreenshotsFolder);
                    Directory.CreateDirectory(dir); // creates folder if it doesn't exist

                    // Create filename with timestamp
                    var fileName = $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    var filePath = Path.Combine(dir, fileName);

                    // Save screenshot
                    var screenshot = screenshotDriver.GetScreenshot();
                    screenshot.SaveAsFile(filePath);

                    // Attach to NUnit report
                    TestContext.AddTestAttachment(filePath, "Failure Screenshot");
                    TestContext.Out.WriteLine($"Screenshot saved to: {filePath}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Screenshot failed: {e.Message}");
            }
        }

        private void DeleteUser()
        {
            By deleteButtonLocator = By.XPath("//a[contains(@href, 'delete_account')]");
            if (driver.FindElements(deleteButtonLocator).Count > 0)
            {
                driver.FindElement(deleteButtonLocator).Click();
            }
        }

        private void GenerateReportFile()
        {
            // Ensure absolute folder path
            var projectDir = Directory.GetParent(AppContext.BaseDirectory)
                                ?.Parent?.Parent?.Parent?.FullName;
            var dir = Path.Combine(projectDir!, ConfigManager.TestReportsFolder);
            Directory.CreateDirectory(dir); // creates folder if it doesn't exist
            
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"{TestName}_{timestamp}.html";
            string filePath = Path.Combine(dir, fileName);
            
            report?.SaveReport(filePath);
            TestContext.Out.WriteLine($"Test report saved to: {filePath}");
        }
    }
}