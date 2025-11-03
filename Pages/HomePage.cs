using OpenQA.Selenium;
using OnlineShoppingTests.Utils;

namespace OnlineShoppingTests.Pages
{
    public class HomePage
    {
        private readonly IWebDriver driver;
        private readonly By searchInput = By.XPath("//input[contains(@id, 'search_product')]");
        private readonly By searchButton = By.XPath("//button[contains(@id, 'submit_search')]");
        private readonly By navigationProducts = By.XPath("//a[@href='/products']");
        private readonly By viewFirstProductLocator = By.XPath("(//div[contains(@class,'choose')]//a)[1]");
        private readonly By firstProductNameLocator = By.XPath("(//div[contains(@class, 'productinfo')]//p)[1]");

        private readonly Dictionary<string, By> _categorySelectors = new()
        {
            {"Women", By.XPath("//a[@href='#Women']")},
            {"Men", By.XPath("//a[@href='#Men']")},
            {"Kids", By.XPath("//a[@href='#Kids']")}
        };

        private readonly Dictionary<string, Dictionary<string, By>> _subCategorySelectors = new()
        {
            {"Women", new Dictionary<string, By>
                {
                    {"Dress", By.XPath("//div[@id='Women']//a[text()='Dress ']")},
                    {"Tops", By.XPath("//a[text()='Tops ']")},
                    {"Saree", By.XPath("//a[text()='Saree ']")}
                }
            },
            {"Men", new Dictionary<string, By>
                {
                    {"Tshirts", By.XPath("//a[text()='Tshirts ']")},
                    {"Jeans", By.XPath("//a[text()='Jeans ']")}
                }
            },
            {"Kids", new Dictionary<string, By>
                {
                    {"Dress", By.XPath("//div[@id='Kids']//a[text()='Dress ']")},
                    {"Tops & Shirts", By.XPath("//div[@id='Kids']//a[text()='Tops & Shirts ']")}
                }
            }
        };        
        public HomePage(IWebDriver driver)
        {
            this.driver = driver;
        }
        public void BrowseToCategory(string category)
        {
            // Validate category exists
            if (!_categorySelectors.TryGetValue(category, out var categoryLocator))
                throw new ArgumentException($"Category '{category}' is not defined in HomePage.");

            // Click the category
            var categoryElement = WaitHelper.WaitForElementClickable(driver, categoryLocator);
            ScrollIntoView(categoryElement);
            categoryElement.Click();            
        }
        public void BrowseToCategory(string category, string subcategory)
        {
            // Validate category exists
            if (!_categorySelectors.TryGetValue(category, out var categoryLocator))
                throw new ArgumentException($"Category '{category}' not found");

            // Click category to reveal subcategories
            var categoryElement = WaitHelper.WaitForElementClickable(driver, categoryLocator)
                ?? throw new InvalidOperationException($"Category '{category}' is not clickable");
            ScrollIntoView(categoryElement);
            categoryElement.Click();

            // Validate subcategory exists
            if (!_subCategorySelectors.TryGetValue(category, out var subDict))
                throw new ArgumentException($"No subcategories defined for category '{category}'");

            if (!subDict.TryGetValue(subcategory, out var subLocator))
                throw new ArgumentException($"Subcategory '{subcategory}' not found under '{category}'");

            // Click the subcategory
            var subElement = WaitHelper.WaitForElementClickable(driver, subLocator)
                ?? throw new InvalidOperationException($"Subcategory '{subcategory}' is not clickable");
            ScrollIntoView(subElement);
            subElement.Click();
        }

        public void SearchProduct(string searchTerm)
        {
            // Navigate to Products
            var navProducts = WaitHelper.WaitForElementClickable(driver, navigationProducts);
            navProducts.Click();

            // Enter search term
            var searchElement = WaitHelper.WaitForElementVisible(driver, searchInput);
            searchElement.Clear();
            searchElement.SendKeys(searchTerm);

            // Click search button
            var searchBtn = WaitHelper.WaitForElementClickable(driver, searchButton);
            searchBtn.Click();
        }

        public void SelectProduct(string item)
        {
            var productLocator = By.XPath($"//p[text()='{item}']/..//..//..//div[@class='choose']//a");
            var product = WaitHelper.WaitForElementClickable(driver, productLocator) ?? throw new InvalidOperationException("Product not found on Home Page");
            ScrollIntoView(product);
            product.Click();
        }

        public string SelectFirstProduct()
        {
            var viewFirstProduct = WaitHelper.WaitForElementClickable(driver, viewFirstProductLocator)
                  ?? throw new InvalidOperationException("No products found on the page");
            ScrollIntoView(viewFirstProduct);
            
            var firstProduct = WaitHelper.WaitForElementClickable(driver, firstProductNameLocator)
                  ?? throw new InvalidOperationException("No products found on the page");

            string firstProductName = firstProduct.Text.Trim();
            viewFirstProduct.Click();
            return firstProductName;
        }
        public IWebElement ScrollIntoView(IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block:'center', inline:'center'});", element);
            return element;
        }
    }
}