using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OnlineShoppingTests.Utils;
using OnlineShoppingTests.TestData.Models;

namespace OnlineShoppingTests.Pages
{
    public class RegistrationPage
    {
        private readonly IWebDriver driver;
        private readonly HomePage homePage;
        private readonly By userNameFieldSignUp = By.XPath("//input[contains(@data-qa, 'signup-name')]");
        private readonly By emailFieldSignUp = By.XPath("//input[contains(@data-qa, 'signup-email')]");
        private readonly By signupButton = By.XPath("//button[contains(@data-qa, 'signup-button')]");
        private readonly By successMessageSignUp = By.XPath("//b[contains(text(), 'Enter Account Information')]");
        private readonly By passwordField = By.XPath("//input[@data-qa='password']");
        private readonly By firstNameField = By.XPath("//input[@data-qa='first_name']");
        private readonly By lastNameField = By.XPath("//input[@data-qa='last_name']");
        private readonly By addressField = By.XPath("//input[@data-qa='address']");
        private readonly By countryDropdown = By.XPath("//select[@data-qa='country']");
        private readonly By stateField = By.XPath("//input[@data-qa='state']");
        private readonly By cityField = By.XPath("//input[@data-qa='city']");
        private readonly By zipField = By.XPath("//input[@data-qa='zipcode']");
        private readonly By mobileField = By.XPath("//input[@data-qa='mobile_number']");
        private readonly By createAccountButton = By.XPath("//button[contains(text(), 'Create Account')]");
        private readonly By successMessageRegistration = By.XPath("//b[contains(text(), 'Account Created!')]");
        private readonly By viewCart = By.XPath("//a[@href='/view_cart']");

        public RegistrationPage(IWebDriver driver)
        {
            this.driver = driver;
            this.homePage = new HomePage(driver);
        }
        public void SignUp(User user)
        {
            WaitHelper.WaitForElementVisible(driver, userNameFieldSignUp).SendKeys(user.Username);
            driver.FindElement(emailFieldSignUp).SendKeys(user.Email);
            driver.FindElement(signupButton).Click();
        }
        public bool IsSignUpSuccessful()
        {
            try
            {
                var message = WaitHelper.WaitForElementVisible(driver, successMessageSignUp);
                return message.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public void SelectCountry(string country)
        {
            var dropdownElement = WaitHelper.WaitForElementClickable(driver, countryDropdown);
            var select = new SelectElement(dropdownElement);
            select.SelectByText(country);
        }
        public void Register(User user)
        {
            WaitHelper.WaitForElementVisible(driver, passwordField).SendKeys(user.Password);
            driver.FindElement(firstNameField).SendKeys(user.FirstName);
            driver.FindElement(lastNameField).SendKeys(user.LastName);
            driver.FindElement(addressField).SendKeys(user.StreetAddress);
            SelectCountry("Canada");
            driver.FindElement(stateField).SendKeys(user.State);
            driver.FindElement(cityField).SendKeys(user.City);
            driver.FindElement(zipField).SendKeys(user.ZipCode);
            driver.FindElement(mobileField).SendKeys(user.MobileNumber);
            var accountButton = driver.FindElement(createAccountButton);
            homePage.ScrollIntoView(accountButton).Click();
        }
        public bool IsRegistrationSuccessful()
        {
            try
            {
                var message = WaitHelper.WaitForElementVisible(driver, successMessageRegistration);
                return message.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public void ViewCart()
        {
            var button = WaitHelper.WaitForElementClickable(driver, viewCart);
            button.Click();
        }
    }
}