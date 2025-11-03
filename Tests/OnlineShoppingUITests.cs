using NUnit.Framework;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Tests
{
    [TestFixture]
    public class OnlineShoppingUITests : BaseTest
    {
        // Categories
        private const string MenCategory = "Men";
        private const string KidsCategory = "Kids";

        // Subcategories
        private const string JeansSubcategory = "Jeans";
        private const string TopsAndShirtsSubcategory = "Tops & Shirts";

        // Products
        private const string SoftStretchJeans = "Soft Stretch Jeans";
        private const string FrozenTopsForKids = "Frozen Tops For Kids";

        private void BrowseAndAdd(string category, string subcategory, string product, int quantity = 1)
        {
            homePage.BrowseToCategory(category, subcategory);
            homePage.SelectProduct(product);
            productPage.AddToCart(quantity);
        }

        private string SearchAndAdd(string searchTerm, int quantity = 1)
        {
            homePage.SearchProduct(searchTerm);
            string firstProductName = homePage.SelectFirstProduct();
            productPage.AddToCart(quantity);
            return firstProductName;
        }

        [Test]
        public void BrowseAndAddItemToCart()
        {
            report.StartStep("1. Browse to 'Soft Stretch Jeans' product");
            BrowseAndAdd(MenCategory, JeansSubcategory, SoftStretchJeans, 1);

            report.StartStep("2. Navigate to cart");
            productPage.ViewCart();

            report.StartStep("3. Verify Product in cart");
            Assert.That(cartPage.IsProductInCart(SoftStretchJeans),
                $"Expected '{SoftStretchJeans}' to be in the cart.");
        }

        [Test]
        public void SearchAndAddItemToCart()
        {
            string firstProductName = SearchAndAdd("tshirt", 1);
            productPage.ViewCart();

            Assert.That(cartPage.IsProductInCart(firstProductName),
                $"Expected '{firstProductName}' to be in the cart.");
        }

        [Test]
        public void ViewCartAfterAddingItems()
        {
            // Add first item
            BrowseAndAdd(MenCategory, JeansSubcategory, SoftStretchJeans, 1);
            productPage.ContinueShopping();

            // Add second item
            BrowseAndAdd(KidsCategory, TopsAndShirtsSubcategory, FrozenTopsForKids, 1);
            productPage.ViewCart();

            // Assertions
            var productNames = cartPage.GetCartProductNames().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(productNames.Any(p => p.Equals(SoftStretchJeans, StringComparison.OrdinalIgnoreCase)),
                    $"Expected '{SoftStretchJeans}' in cart.");

                Assert.That(productNames.Any(p => p.Equals(FrozenTopsForKids, StringComparison.OrdinalIgnoreCase)),
                    $"Expected '{FrozenTopsForKids}' in cart.");

                int jeansQty = cartPage.GetProductQuantity(SoftStretchJeans);
                int topsQty = cartPage.GetProductQuantity(FrozenTopsForKids);

                Assert.That(jeansQty, Is.EqualTo(1), "Jeans quantity mismatch.");
                Assert.That(topsQty, Is.EqualTo(1), "Tops quantity mismatch.");
            });
        }

        [Test]
        public void CheckoutWithNewUserRegistration()
        {
            var user = UserDataManager.GetUser("validUser1");
            
            // Add an item
            BrowseAndAdd(MenCategory, JeansSubcategory, SoftStretchJeans, 1);
            productPage.ViewCart();

            // Proceed to checkout
            cartPage.ProceedToCheckout();
            cartPage.ProceedToCheckoutModal(); // register/login or continue shopping modal

            // Sign up
            registrationPage.SignUp(user);
            Assert.That(registrationPage.IsSignUpSuccessful(), "User signup failed.");

            // Register
            registrationPage.Register(user);
            Assert.That(registrationPage.IsRegistrationSuccessful(), "User registration failed.");
        }

        [Test]
        public void SuccessfulPayment()
        {
            var user = UserDataManager.GetUser("validUser2");

            // Add first item twice
            BrowseAndAdd(MenCategory, JeansSubcategory, SoftStretchJeans, 2);
            productPage.ContinueShopping();

            // Add second item
            BrowseAndAdd(KidsCategory, TopsAndShirtsSubcategory, FrozenTopsForKids, 1);
            productPage.ViewCart();

            cartPage.ProceedToCheckout();
            cartPage.ProceedToCheckoutModal(); // register/login or continue shopping modal
            registrationPage.SignUp(user);
            registrationPage.Register(user); // actual registration
            registrationPage.ViewCart();
            cartPage.ProceedToCheckout();

            // Verify delivery address
            Assert.That(checkoutPage.VerifyDeliveryAddress(user),
                "Delivery address does not match user data");

            // Verify total amount
            var displayedTotal = checkoutPage.GetCheckoutTotal();
            var calculatedTotal = checkoutPage.CalculateExpectedCartTotal();
            Assert.That(calculatedTotal, Is.EqualTo(displayedTotal),
                "Calculated total does not match displayed total");

            checkoutPage.ProceedToPayment();

            // Fill payment and confirm order
            paymentPage.FillPaymentDetails(user);
            paymentPage.ConfirmOrder();

            // Verify order placed            
            Assert.That(paymentPage.IsOrderConfirmationDisplayed(), "Order was not placed successfully");

            // Verify cart is empty after order
            registrationPage.ViewCart();
            Assert.That(cartPage.IsCartEmptyAfterOrder(), "Cart is not empty after checkout");
        }
    }
}