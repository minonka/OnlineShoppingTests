using OpenQA.Selenium;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Pages
{
    public class CartPage
    {
        private readonly IWebDriver driver;
        private readonly By cartItems = By.XPath("//td[@class='cart_product']");
        private readonly By proceedToCheckoutButton = By.XPath("//a[contains(text(), 'Proceed To Checkout')]");
        private readonly By registerLoginButton = By.XPath("//u[contains(text(), 'Register / Login')]");
        private readonly By emptyCart = By.XPath("//b[text()='Cart is empty!']");

        public CartPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public bool IsProductInCart(string productName)
        {
            WaitHelper.WaitForElementVisible(driver, cartItems);
            var normalizedProductName = productName.Trim().ToLowerInvariant();

            var items = driver.FindElements(cartItems);
            foreach (var item in items)
            {
                // Go up to the parent <tr>
                var productRow = item.FindElement(By.XPath("./ancestor::tr"));

                // Find the link inside the description cell
                var productDescription = productRow.FindElement(By.XPath(".//td[@class='cart_description']//a"));

                // Get the text value
                string text = productDescription.Text.Trim().ToLowerInvariant();

                // Check if text value contains the product name
                if (!string.IsNullOrEmpty(text) && text.Equals(normalizedProductName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        
        public List<string> GetCartProductNames()
        {
            var items = driver.FindElements(cartItems);
            var productNames = new List<string>();

            foreach (var item in items)
            {
                var productRow = item.FindElement(By.XPath("./ancestor::tr"));
                var productDescription = productRow.FindElement(By.XPath(".//td[@class='cart_description']//a"));
                string name = productDescription.Text.Trim();
                if(string.IsNullOrEmpty(name))
                throw new InvalidOperationException("Product name is missing or empty");
                productNames.Add(name.ToLowerInvariant());
            }
            return productNames;
        }

        public int GetProductQuantity(string productName)
        {
            var rows = driver.FindElements(By.XPath(".//tr[contains(@id, 'product-')]"));
            foreach (var row in rows)
            {
                var nameCell = row.FindElement(By.XPath(".//td[@class='cart_description']//a"));
                if (nameCell.Text.Equals(productName, StringComparison.OrdinalIgnoreCase))
                {
                    var qtyElement = row.FindElement(By.XPath(".//td[@class='cart_quantity']//button"));
                    string qtyText = qtyElement.Text != "" ? qtyElement.Text : throw new InvalidOperationException("Quantity element is empty.");
                    if (int.TryParse(qtyText, out int qty))
                        return qty;
                }
            }
            throw new InvalidOperationException($"Product '{productName}' not found in cart.");
        }

        public void ProceedToCheckout()
        {
            var button = WaitHelper.WaitForElementClickable(driver, proceedToCheckoutButton);
            button.Click();
        }

        public void ProceedToCheckoutModal()
        {
            var button = WaitHelper.WaitForElementClickable(driver, registerLoginButton);
            button.Click();
        }
        
        public bool IsCartEmptyAfterOrder()
        {
            try
                {
                    var message = WaitHelper.WaitForElementVisible(driver, emptyCart);
                    return message.Displayed;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
        }
    }
}