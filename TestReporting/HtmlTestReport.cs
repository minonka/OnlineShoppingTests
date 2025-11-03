using System.Text;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace OnlineShoppingTests.TestReporting
{
    public class HtmlTestReport
    {
        private readonly List<TestStep> _steps = new List<TestStep>();
        private string _testName;
        private DateTime _startTime;
        private DateTime _endTime;
        private TestStatus _overallStatus = TestStatus.Passed;
        private TestStep? _currentStep;

        public HtmlTestReport(string testName)
        {
            _testName = testName;
            _startTime = DateTime.Now;
        }

        public void StartStep(string stepName)
        {
            _currentStep = new TestStep
            {
                Name = stepName,
                Status = TestStatus.Passed,
                Timestamp = DateTime.Now,
                Logs = new List<LogEntry>()
            };
            _steps.Add(_currentStep);
        }

        internal void LogAction(string action, bool success = true, Exception? ex = null)
        {
            if (_currentStep != null)
            {
                _currentStep.Logs.Add(new LogEntry
                {
                    Message = action,
                    Timestamp = DateTime.Now,
                    IsError = !success,
                    Exception = ex
                });

                if (!success)
                {
                    _currentStep.Status = TestStatus.Failed;
                    _overallStatus = TestStatus.Failed;
                }
            }
        }

        public void SaveReport(string filePath)
        {
            _endTime = DateTime.Now;
            var html = GenerateHtml();
            File.WriteAllText(filePath, html);
        }

        private string GenerateHtml()
        {
            var duration = (_endTime - _startTime).TotalSeconds;
            var passedCount = _steps.Count(s => s.Status == TestStatus.Passed);
            var failedCount = _steps.Count(s => s.Status == TestStatus.Failed);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en'>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset='UTF-8'>");
            sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            sb.AppendLine($"    <title>Test Report - {_testName}</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine(@"
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f5f5f5; padding: 20px; }
        .container { max-width: 1400px; margin: 0 auto; background: white; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; border-radius: 8px 8px 0 0; }
        .header h1 { margin-bottom: 10px; font-size: 28px; }
        .header .meta { opacity: 0.9; font-size: 14px; }
        .summary { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; padding: 30px; background: #fafafa; border-bottom: 1px solid #e0e0e0; }
        .summary-item { text-align: center; }
        .summary-item .label { color: #666; font-size: 12px; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 5px; }
        .summary-item .value { font-size: 32px; font-weight: bold; }
        .summary-item.passed .value { color: #4caf50; }
        .summary-item.failed .value { color: #f44336; }
        .summary-item.duration .value { color: #2196f3; font-size: 24px; }
        .steps { padding: 30px; }
        .step { margin-bottom: 15px; border: 1px solid #e0e0e0; border-radius: 6px; overflow: hidden; }
        .step-header { padding: 15px 20px; cursor: pointer; display: flex; align-items: center; justify-content: space-between; transition: background 0.2s; }
        .step-header:hover { background: #f9f9f9; }
        .step-header.passed { background: #e8f5e9; border-left: 4px solid #4caf50; }
        .step-header.failed { background: #ffebee; border-left: 4px solid #f44336; }
        .step-info { flex: 1; }
        .step-name { font-weight: 600; font-size: 16px; margin-bottom: 5px; }
        .step-meta { color: #666; font-size: 12px; }
        .step-status { display: flex; align-items: center; gap: 10px; }
        .status-badge { padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: 600; text-transform: uppercase; }
        .status-badge.passed { background: #4caf50; color: white; }
        .status-badge.failed { background: #f44336; color: white; }
        .toggle-icon { transition: transform 0.3s; font-size: 20px; color: #666; }
        .toggle-icon.expanded { transform: rotate(180deg); }
        .step-content { max-height: 0; overflow: hidden; transition: max-height 0.3s ease; background: #fafafa; }
        .step-content.expanded { max-height: 5000px; border-top: 1px solid #e0e0e0; }
        .step-logs { padding: 20px; }
        .log-entry { padding: 10px 15px; margin-bottom: 8px; background: white; border-left: 3px solid #2196f3; border-radius: 3px; font-family: 'Consolas', 'Courier New', monospace; font-size: 13px; }
        .log-entry.error { border-left-color: #f44336; background: #fff5f5; }
        .log-time { color: #999; font-size: 11px; margin-bottom: 4px; }
        .log-message { color: #333; line-height: 1.6; }
        .log-exception { margin-top: 10px; padding: 10px; background: #fff; border: 1px solid #ffcdd2; border-radius: 3px; }
        .exception-type { color: #d32f2f; font-weight: bold; margin-bottom: 5px; }
        .exception-message { color: #c62828; margin-bottom: 10px; white-space: pre-wrap; }
        .exception-stack { color: #666; font-size: 11px; white-space: pre-wrap; max-height: 300px; overflow-y: auto; }
        .no-logs { color: #999; font-style: italic; text-align: center; padding: 20px; }
    ");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine($"            <h1>{System.Security.SecurityElement.Escape(_testName)}</h1>");
            sb.AppendLine($"            <div class='meta'>Started: {_startTime:yyyy-MM-dd HH:mm:ss} | Ended: {_endTime:yyyy-MM-dd HH:mm:ss}</div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='summary'>");
            sb.AppendLine("            <div class='summary-item passed'>");
            sb.AppendLine("                <div class='label'>Passed</div>");
            sb.AppendLine($"                <div class='value'>{passedCount}</div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class='summary-item failed'>");
            sb.AppendLine("                <div class='label'>Failed</div>");
            sb.AppendLine($"                <div class='value'>{failedCount}</div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class='summary-item duration'>");
            sb.AppendLine("                <div class='label'>Duration</div>");
            sb.AppendLine($"                <div class='value'>{duration:F2}s</div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='steps'>");

            for (int i = 0; i < _steps.Count; i++)
            {
                var step = _steps[i];
                var statusClass = step.Status == TestStatus.Passed ? "passed" : "failed";
                var statusText = step.Status == TestStatus.Passed ? "Passed" : "Failed";

                sb.AppendLine($"            <div class='step'>");
                sb.AppendLine($"                <div class='step-header {statusClass}' onclick='toggleStep({i})'>");
                sb.AppendLine("                    <div class='step-info'>");
                sb.AppendLine($"                        <div class='step-name'>{System.Security.SecurityElement.Escape(step.Name)}</div>");
                sb.AppendLine($"                        <div class='step-meta'>{step.Logs.Count} action(s) | Completed at {step.Timestamp:HH:mm:ss.fff}</div>");
                sb.AppendLine("                    </div>");
                sb.AppendLine("                    <div class='step-status'>");
                sb.AppendLine($"                        <span class='status-badge {statusClass}'>{statusText}</span>");
                sb.AppendLine($"                        <span class='toggle-icon' id='icon-{i}'>▼</span>");
                sb.AppendLine("                    </div>");
                sb.AppendLine("                </div>");
                sb.AppendLine($"                <div class='step-content' id='content-{i}'>");
                sb.AppendLine("                    <div class='step-logs'>");

                if (step.Logs.Count > 0)
                {
                    foreach (var log in step.Logs)
                    {
                        var errorClass = log.IsError ? " error" : "";
                        sb.AppendLine($"                        <div class='log-entry{errorClass}'>");
                        sb.AppendLine($"                            <div class='log-time'>{log.Timestamp:HH:mm:ss.fff}</div>");
                        sb.AppendLine($"                            <div class='log-message'>{System.Security.SecurityElement.Escape(log.Message)}</div>");
                        
                        if (log.Exception != null)
                        {
                            sb.AppendLine("                            <div class='log-exception'>");
                            sb.AppendLine($"                                <div class='exception-type'>{System.Security.SecurityElement.Escape(log.Exception.GetType().Name)}</div>");
                            sb.AppendLine($"                                <div class='exception-message'>{System.Security.SecurityElement.Escape(log.Exception.Message)}</div>");
                            if (!string.IsNullOrEmpty(log.Exception.StackTrace))
                            {
                                sb.AppendLine($"                                <div class='exception-stack'>{System.Security.SecurityElement.Escape(log.Exception.StackTrace)}</div>");
                            }
                            sb.AppendLine("                            </div>");
                        }
                        
                        sb.AppendLine("                        </div>");
                    }
                }
                else
                {
                    sb.AppendLine("                        <div class='no-logs'>No actions logged for this step</div>");
                }

                sb.AppendLine("                    </div>");
                sb.AppendLine("                </div>");
                sb.AppendLine("            </div>");
            }

            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <script>");
            sb.AppendLine(@"
        function toggleStep(index) {
            const content = document.getElementById('content-' + index);
            const icon = document.getElementById('icon-' + index);
            content.classList.toggle('expanded');
            icon.classList.toggle('expanded');
        }
    ");
            sb.AppendLine("    </script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }

    public class TestStep
    {
        public required string Name { get; set; }
        public TestStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public required List<LogEntry> Logs { get; set; }
    }

    public class LogEntry
    {
        public required string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsError { get; set; }
        public Exception? Exception { get; set; }
    }

    public enum TestStatus
    {
        Passed,
        Failed
    }

    // Wrapper for IWebDriver that logs all operations
    public class LoggingWebDriver : IWebDriver, IJavaScriptExecutor, ITakesScreenshot
    {
        private readonly IWebDriver _driver;
        private readonly HtmlTestReport _report;

        public LoggingWebDriver(IWebDriver driver, HtmlTestReport report)
        {
            _driver = driver;
            _report = report;
        }

        // IJavaScriptExecutor implementation
        public object? ExecuteScript(string script, params object?[] args)
        {
            try
            {
                _report.LogAction($"Executing JavaScript: {script}");

                // unwrap any LoggingWebElements into their underlying IWebElement
                var processedArgs = args
                    ?.Select(arg => arg is LoggingWebElement lwe ? lwe.Unwrap() : arg)
                    .ToArray();
            
                var result = ((IJavaScriptExecutor)_driver).ExecuteScript(script, processedArgs ?? Array.Empty<object>());
                _report.LogAction("JavaScript executed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to execute JavaScript: {script}", false, ex);
                throw;
            }
        }

        public object? ExecuteScript(PinnedScript script, params object?[] args)
        {
            try
            {
                _report.LogAction($"Executing pinned JavaScript: {script}");

                // unwrap any LoggingWebElements into their underlying IWebElement
                var processedArgs = args
                    ?.Select(arg => arg is LoggingWebElement lwe ? lwe.Unwrap() : arg)
                    .ToArray();

                var result = ((IJavaScriptExecutor)_driver).ExecuteScript(script, processedArgs ?? Array.Empty<object>());
                _report.LogAction("Pinned JavaScript executed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to execute pinned JavaScript", false, ex);
                throw;
            }
        }

        public object? ExecuteAsyncScript(string script, params object?[] args)
        {
            try
            {
                _report.LogAction($"Executing async JavaScript: {script}");

                // unwrap any LoggingWebElements into their underlying IWebElement
                var processedArgs = args
                    ?.Select(arg => arg is LoggingWebElement lwe ? lwe.Unwrap() : arg)
                    .ToArray();


                var result = ((IJavaScriptExecutor)_driver).ExecuteAsyncScript(script, processedArgs ?? Array.Empty<object>());
                _report.LogAction("Async JavaScript executed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to execute async JavaScript: {script}", false, ex);
                throw;
            }
        }

        // ITakesScreenshot implementation
        public Screenshot GetScreenshot()
        {
            try
            {
                _report.LogAction("Taking screenshot");
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                _report.LogAction("Screenshot captured successfully");
                return screenshot;
            }
            catch (Exception ex)
            {
                _report.LogAction("Failed to capture screenshot", false, ex);
                throw;
            }
        }

        public string Url
        {
            get => _driver.Url;
            set
            {
                try
                {
                    _report.LogAction($"Navigate to URL: {value}");
                    _driver.Url = value;
                    _report.LogAction($"Successfully navigated to: {value}");
                }
                catch (Exception ex)
                {
                    _report.LogAction($"Failed to navigate to: {value}", false, ex);
                    throw;
                }
            }
        }

        public string Title => _driver.Title;
        public string PageSource => _driver.PageSource;
        public string CurrentWindowHandle => _driver.CurrentWindowHandle;
        public System.Collections.ObjectModel.ReadOnlyCollection<string> WindowHandles => _driver.WindowHandles;

        public void Close() => _driver.Close();
        public void Quit() => _driver.Quit();

        public IOptions Manage() => _driver.Manage();
        public INavigation Navigate() => new LoggingNavigation(_driver.Navigate(), _report);
        public ITargetLocator SwitchTo() => _driver.SwitchTo();

        public IWebElement FindElement(By by)
        {
            try
            {
                _report.LogAction($"Finding element: {by}");
                var element = _driver.FindElement(by);
                _report.LogAction($"Element found: {by}");
                return new LoggingWebElement(element, _report, by.ToString());
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to find element: {by}", false, ex);
                throw;
            }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            try
            {
                _report.LogAction($"Finding elements: {by}");
                var elements = _driver.FindElements(by);
                _report.LogAction($"Found {elements.Count} element(s): {by}");
                return elements;
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to find elements: {by}", false, ex);
                throw;
            }
        }

        public void Dispose() => _driver.Dispose();
    }

    // Wrapper for IWebElement that logs all operations
    public class LoggingWebElement : IWebElement
    {
        private readonly IWebElement _element;
        private readonly HtmlTestReport _report;
        private readonly string _locator;

        public LoggingWebElement(IWebElement element, HtmlTestReport report, string locator)
        {
            _element = element;
            _report = report;
            _locator = locator;
        }

        public string TagName => _element.TagName;
        public string Text => _element.Text;
        public bool Enabled => _element.Enabled;
        public bool Selected => _element.Selected;
        public Point Location => _element.Location;
        public Size Size => _element.Size;
        public bool Displayed => _element.Displayed;
        public IWebElement Unwrap() => _element;

        public void Clear()
        {
            try
            {
                _report.LogAction($"Clearing element: {_locator}");
                _element.Clear();
                _report.LogAction($"Element cleared: {_locator}");
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to clear element: {_locator}", false, ex);
                throw;
            }
        }

        public void SendKeys(string text)
        {
            try
            {
                _report.LogAction($"Sending keys to element {_locator}: {text}");
                _element.SendKeys(text);
                _report.LogAction($"Keys sent successfully to: {_locator}");
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to send keys to: {_locator}", false, ex);
                throw;
            }
        }

        public void Submit()
        {
            try
            {
                _report.LogAction($"Submitting form element: {_locator}");
                _element.Submit();
                _report.LogAction($"Form submitted: {_locator}");
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to submit form: {_locator}", false, ex);
                throw;
            }
        }

        public void Click()
        {
            try
            {
                _report.LogAction($"Clicking element: {_locator}");
                _element.Click();
                _report.LogAction($"Element clicked successfully: {_locator}");
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to click element: {_locator}", false, ex);
                throw;
            }
        }

        public string? GetAttribute(string attributeName) => _element.GetAttribute(attributeName);
        public string? GetDomAttribute(string attributeName) => _element.GetDomAttribute(attributeName);
        public string? GetDomProperty(string propertyName) => _element.GetDomProperty(propertyName);
        public string? GetProperty(string propertyName) => _element.GetAttribute(propertyName); // Use GetAttribute as a .NET Framework fallback
        public string GetCssValue(string propertyName) => _element.GetCssValue(propertyName);
        public ISearchContext GetShadowRoot() => _element.GetShadowRoot();

        public IWebElement FindElement(By by)
        {
            try
            {
                _report.LogAction($"Finding child element {by} within {_locator}");
                var element = _element.FindElement(by);
                _report.LogAction($"Child element found: {by}");
                return new LoggingWebElement(element, _report, by.ToString());
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to find child element {by} within {_locator}", false, ex);
                throw;
            }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> FindElements(By by) => _element.FindElements(by);
    }

    // Wrapper for INavigation
    public class LoggingNavigation : INavigation
    {
        private readonly INavigation _navigation;
        private readonly HtmlTestReport _report;

        public LoggingNavigation(INavigation navigation, HtmlTestReport report)
        {
            _navigation = navigation;
            _report = report;
        }

        public void Back()
        {
            try
            {
                _report.LogAction("Navigating back");
                _navigation.Back();
                _report.LogAction("Navigated back successfully");
            }
            catch (Exception ex)
            {
                _report.LogAction("Failed to navigate back", false, ex);
                throw;
            }
        }

        public void Forward()
        {
            try
            {
                _report.LogAction("Navigating forward");
                _navigation.Forward();
                _report.LogAction("Navigated forward successfully");
            }
            catch (Exception ex)
            {
                _report.LogAction("Failed to navigate forward", false, ex);
                throw;
            }
        }

        public void GoToUrl(string url)
        {
            try
            {
                _report.LogAction($"Navigating to URL: {url}");
                _navigation.GoToUrl(url);
                _report.LogAction($"Successfully navigated to: {url}");
            }
            catch (Exception ex)
            {
                _report.LogAction($"Failed to navigate to: {url}", false, ex);
                throw;
            }
        }

        public void GoToUrl(Uri url) => GoToUrl(url.ToString());

        public void Refresh()
        {
            try
            {
                _report.LogAction("Refreshing page");
                _navigation.Refresh();
                _report.LogAction("Page refreshed successfully");
            }
            catch (Exception ex)
            {
                _report.LogAction("Failed to refresh page", false, ex);
                throw;
            }
        }

        // Async implementations
        public Task BackAsync()
        {
            Back();
            return Task.CompletedTask;
        }

        public Task ForwardAsync()
        {
            Forward();
            return Task.CompletedTask;
        }

        public Task GoToUrlAsync(string url)
        {
            GoToUrl(url);
            return Task.CompletedTask;
        }

        public Task GoToUrlAsync(Uri url)
        {
            GoToUrl(url);
            return Task.CompletedTask;
        }

        public Task RefreshAsync()
        {
            Refresh();
            return Task.CompletedTask;
        }
    }

    // Helper for WebDriverWait logging
    public static class LoggingWaitExtensions
    {
        public static IWebElement WaitAndFindElement(this LoggingWebDriver driver, HtmlTestReport report, By by, int timeoutSeconds = 10)
        {
            try
            {
                report.LogAction($"Waiting up to {timeoutSeconds}s for element to be present: {by}");
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                var element = wait.Until(drv => drv.FindElement(by));
                report.LogAction($"Element found after wait: {by}");
                return element;
            }
            catch (Exception ex)
            {
                report.LogAction($"Timeout waiting for element: {by}", false, ex);
                throw;
            }
        }

        public static IWebElement WaitUntilClickable(this LoggingWebDriver driver, HtmlTestReport report, By by, int timeoutSeconds = 10)
        {
            try
            {
                report.LogAction($"Waiting up to {timeoutSeconds}s for element to be clickable: {by}");
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                var element = wait.Until(driver =>
                    {
                        var el = driver.FindElement(by);
                        return (el.Displayed && el.Enabled) ? el : null;
                    });
                report.LogAction($"Element is clickable: {by}");
                return new LoggingWebElement(element, report, by.ToString());
            }
            catch (Exception ex)
            {
                report.LogAction($"Timeout waiting for element to be clickable: {by}", false, ex);
                throw;
            }
        }
    }
}