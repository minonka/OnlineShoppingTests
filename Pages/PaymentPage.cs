using OpenQA.Selenium;
using OnlineShoppingTests.TestData.Models;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Pages;
public class PaymentPage
{
    private readonly IWebDriver driver;
    private readonly HomePage homePage;
    private readonly By nameOnCardField = By.XPath("//input[@data-qa='name-on-card']");
    private readonly By cardNumberField = By.XPath("//input[@data-qa='card-number']");
    private readonly By cvcField = By.XPath("//input[@data-qa='cvc']");
    private readonly By expiryMonthField = By.XPath("//input[@data-qa='expiry-month']");
    private readonly By expiryYearField = By.XPath("//input[@data-qa='expiry-year']");
    private readonly By payAndConfirmButton = By.XPath("//button[@data-qa='pay-button']");
    private readonly By orderPlaced = By.XPath("//b[text()='Order Placed!']");

    public PaymentPage(IWebDriver driver)
    {
        this.driver = driver;
        this.homePage = new HomePage(driver);
    }

    public void FillPaymentDetails(User user)
    {
        var payment = user.Payment;

        driver.FindElement(nameOnCardField).SendKeys(payment.CardName);
        driver.FindElement(cardNumberField).SendKeys(payment.CardNumber);
        driver.FindElement(cvcField).SendKeys(payment.Cvc);
        driver.FindElement(expiryMonthField).SendKeys(payment.ExpiryMonth);
        driver.FindElement(expiryYearField).SendKeys(payment.ExpiryYear);
    }

    public void ConfirmOrder()
    {
        var button = driver.FindElement(payAndConfirmButton);
        homePage.ScrollIntoView(button).Click();
    }

    public bool IsOrderConfirmationDisplayed()
    {
        try
        {
            var message = WaitHelper.WaitForElementVisible(driver, orderPlaced);
            return message.Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }
}