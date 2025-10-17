using OpenQA.Selenium;
using OnlineShoppingTests.TestData.Models;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Pages
{
    public class CheckoutPage
    {
        private readonly IWebDriver driver;
        private readonly CartPage cartPage;
        private readonly HomePage homePage;

        public CheckoutPage(IWebDriver driver)
        {
            this.driver = driver;
            this.cartPage = new CartPage(driver);
            this.homePage = new HomePage(driver);
        }

        // Locators
        private readonly By deliveryAddressBox = By.XPath("//ul[@id='address_delivery']");
        private readonly By totalAmountLabel = By.XPath("(//p[@class='cart_total_price'])[last()]");
        private readonly By placeOrderButton = By.XPath("//a[contains(text(), 'Place Order')]");

        // Verify the delivery address text matches the user's data
        public bool VerifyDeliveryAddress(User user)
        {
            var addressText = WaitHelper.WaitForElementVisible(driver, deliveryAddressBox).Text;

            return addressText.Contains(user.FirstName, StringComparison.OrdinalIgnoreCase)
                && addressText.Contains(user.LastName, StringComparison.OrdinalIgnoreCase)
                && addressText.Contains(user.StreetAddress, StringComparison.OrdinalIgnoreCase)
                && addressText.Contains(user.City, StringComparison.OrdinalIgnoreCase)
                && addressText.Contains(user.State, StringComparison.OrdinalIgnoreCase)
                && addressText.Contains(user.Country, StringComparison.OrdinalIgnoreCase)
                && addressText.Contains(user.ZipCode, StringComparison.OrdinalIgnoreCase)
                && addressText.Contains(user.MobileNumber, StringComparison.OrdinalIgnoreCase);
        }

        // Get total price displayed at checkout
        public decimal GetCheckoutTotal()
        {
            var text = WaitHelper.WaitForElementVisible(driver, totalAmountLabel).Text.Replace("Rs. ", "").Trim();
            if (!decimal.TryParse(text, out decimal total))
            throw new InvalidOperationException($"Could not parse checkout total: {text}");
            return total;
        }

        // Count total price of all cart items
        public decimal CalculateExpectedCartTotal()
        {
            decimal total = 0m;
            var rows = driver.FindElements(By.XPath(".//tr[contains(@id, 'product-')]"));

            foreach (var row in rows)
            {
                // Product name
                var nameCell = row.FindElement(By.XPath(".//td[@class='cart_description']//a"));
                string productName = nameCell.Text.Trim();

                // Product unit price
                var priceCell = row.FindElement(By.XPath(".//td[@class='cart_price']//p[1]"));
                string priceText = priceCell.Text.Replace("Rs. ", "").Trim();
                if (!decimal.TryParse(priceText, out decimal unitPrice))
                    throw new InvalidOperationException($"Could not parse price for product '{productName}'");

                // Quantity
                int quantity = cartPage.GetProductQuantity(productName);

                // Add subtotal
                decimal subtotal = unitPrice * quantity;
                total += subtotal;

                Console.WriteLine($"Product '{productName}': {quantity} × {unitPrice:C} = {subtotal:C}");
            }

            Console.WriteLine($"Calculated expected cart total: {total:C}");
            return total;
        }

        // Proceed to payment
        public void ProceedToPayment()
        {
            var button = WaitHelper.WaitForElementClickable(driver, placeOrderButton);
            homePage.ScrollIntoView(button).Click();
        }
    }
}