# Online Shopping Automation Tests

Automated UI tests for an online shopping application using **C#**, **Selenium WebDriver**, and **NUnit**.
The framework follows the **Page Object Model (POM)** design pattern and supports configuration through `appsettings.json`.

---

## Features

* Built with **.NET 8**, **Selenium WebDriver**, and **NUnit**
* Uses **Page Object Model (POM)** for clean separation of logic
* **Config-driven** (base URL, browser, timeouts)
* **Explicit + Implicit waits** for test stability
* **Automatic screenshots** on failure
* **CI-ready** – easily runs in headless mode

---

## Project Structure

```
OnlineShoppingTests/
│
├─ Pages/                    → Page objects (HomePage, CartPage, etc.)
├─ Screenshots/              → Failure screenshots
├─ TestData/                 → Test data (users)
├─ Tests/                    → NUnit test classes
├─ Utils/                    → Helpers (ConfigManager, WaitHelper, UserDataManager)
├─ appsettings.json          → Configuration (URL, browser, waits)
├─ OnlineShoppingTests.csproj
└─ README.md                 → Project overview & instructions
```

---

## Setup Instructions

### 1. Prerequisites

* .NET SDK 8 or higher
* Chrome, Firefox or Edge browser
* VS Code with C# Dev Kit extension

### 2. Install Dependencies

```bash
dotnet restore
```

### 3. Configure

```json
{
  "baseUrl": "https://automationexercise.com/",
  "browser": "chrome",
  "headless": false,
  "timeout": 5,
  "screenshotsFolder": "Screenshots"
}
```

### 4. Run Tests

```bash
dotnet test
dotnet test --filter "TestName"
```

---

## Design Notes

* `BaseTest` handles browser setup, teardown, and screenshot capture.
* `WaitHelper` provides reusable explicit wait methods.
* Page classes encapsulate locators and UI actions.
* Tests remain short and business-focused.

---

## Reporting

On test failure, screenshots are automatically saved in `/Screenshots` and attached to NUnit results for review.

---

## Future Enhancements

* Add data management
* Integration into CI/CD (GitHub Actions, Azure Pipelines)

---

## Author

Monika Kupcekova – QA Automation Engineer

*This project was created as a demonstration of a scalable Selenium + NUnit automation framework.*
