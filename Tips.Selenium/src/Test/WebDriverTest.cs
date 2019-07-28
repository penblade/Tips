using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Tips.Selenium.Test
{
    [TestClass]
    public class WebDriverTest
    {
        private const string SeleniumWebDriversPath = @"C:\Selenium.WebDrivers";
        private const string TestUrl = @"https://dogfoodcon.com/";

        [TestMethod]
        [DataRow(BrowserType.Chrome, SeleniumWebDriversPath)]
        [DataRow(BrowserType.Firefox, SeleniumWebDriversPath)]
        [DataRow(BrowserType.Edge, SeleniumWebDriversPath)]
        [DataRow(BrowserType.IE11, SeleniumWebDriversPath)]
        public void VerifyRemoteWebDriversAreSetup(BrowserType browserType, string seleniumWebDriversPath)
        {
            using (var driver = WebDriverFactory.Create(browserType, seleniumWebDriversPath))
            {
                driver.Navigate().GoToUrl(TestUrl);

                // DogFoodCon
                var pageLoadCheck = driver.WaitUntilInitialPageLoad("DogFoodCon");

                var sessionsLink = driver.WaitUntilFindElement(By.XPath("//a[@title='Sessions']"));
                sessionsLink.Click();

                // Sessions - DogFoodCon
                pageLoadCheck = driver.WaitUntilPageLoad("Sessions – DogFoodCon", pageLoadCheck);

                var session = driver.WaitUntilFindElement(By.XPath("//a[text()='Jeff McKenzie']"));

                // Scroll to the element, but don't verify it is visible to the user.
                // I did this step just so you can see the session appear on the screen.
                driver.ScrollIntoView(session);

                driver.Quit(browserType);
            }
        }
    }
}
