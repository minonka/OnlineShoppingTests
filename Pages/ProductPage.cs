using OpenQA.Selenium;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Pages
{
    public class ProductPage
    {
        private readonly IWebDriver driver;
        private readonly By quantityInput = By.XPath("//input[@id='quantity']");
        private readonly By addToCartButton = By.XPath("//button[contains(., 'Add to cart')]");
        private readonly By viewCartButton = By.XPath("//u[text()='View Cart']");
        private readonly By continueShoppingButton = By.XPath("//button[text()='Continue Shopping']");

        public ProductPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void AddToCart(int quantity)
        {
        if (quantity < 1)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be at least 1");

        // Wait for quantity input and set value
        var input = WaitHelper.WaitForElementVisible(driver, quantityInput);
        input.Clear();
        input.SendKeys(quantity.ToString());

        // Wait for Add to Cart button and click once
        var button = WaitHelper.WaitForElementClickable(driver, addToCartButton);
        button.Click();
        }

        public void ViewCart()
        {
            var button = WaitHelper.WaitForElementClickable(driver, viewCartButton);
            button.Click();
        }

        public void ContinueShopping()
        {
            var button = WaitHelper.WaitForElementClickable(driver, continueShoppingButton);
            button.Click();
        }
    }
}