using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using Tips.Selenium.Test.Utilities;

namespace Tips.Selenium.Test
{
    [TestClass]
    public class WebDriverTest
    {
        private readonly IWebDriverUtility _webDriverUtility;
        private const string SeleniumWebDriversPath = @"C:\Selenium.WebDrivers";
        private const string TestUrl = @"https://dogfoodcon.com/";

        public WebDriverTest()
        {
            _webDriverUtility = new WebDriverUtility();
        }

        private static WebElementUtility CreateWebElementUtility(IWebDriver driver) => new WebElementUtility(driver);

        [TestMethod]
        [DataRow(BrowserType.Chrome, SeleniumWebDriversPath)]
        [DataRow(BrowserType.Firefox, SeleniumWebDriversPath)]
        [DataRow(BrowserType.Edge, SeleniumWebDriversPath)]
        [DataRow(BrowserType.IE11, SeleniumWebDriversPath)]
        public void VerifyRemoteWebDriversAreSetup(BrowserType browserType, string seleniumWebDriversPath)
        {
            using (var driver = _webDriverUtility.Create(browserType, seleniumWebDriversPath))
            {
                var webElementUtility = CreateWebElementUtility(driver);

                driver.Navigate().GoToUrl(TestUrl);
                var htmlOnPage1 = webElementUtility.FindElement(By.XPath("html"));

                var sessionsLink = webElementUtility.FindElement(By.XPath("//a[@title='Sessions']"));
                sessionsLink.Click();
                webElementUtility.WaitUntilPageLoad(htmlOnPage1, "Sessions – DogFoodCon");
                _webDriverUtility.Quit(browserType, driver);
            }
        }
    }
}
