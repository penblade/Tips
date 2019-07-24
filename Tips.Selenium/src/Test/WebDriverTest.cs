using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Selenium.Test.Utilities;

namespace Tips.Selenium.Test
{
    [TestClass]
    public class WebDriverTest
    {
        private readonly IRemoteWebDriverUtility _remoteWebDriverUtility;
        private const string SeleniumWebDriversPath = @"C:\Selenium.WebDrivers";
        private const string TestUrl = @"https://dogfoodcon.com/";

        public WebDriverTest()
        {
            _remoteWebDriverUtility = new RemoteWebDriverUtility();
        }

        [TestMethod]
        [DataRow(BrowserType.Chrome, SeleniumWebDriversPath)]
        [DataRow(BrowserType.Firefox, SeleniumWebDriversPath)]
        [DataRow(BrowserType.Edge, SeleniumWebDriversPath)]
        [DataRow(BrowserType.IE11, SeleniumWebDriversPath)]
        public void VerifyRemoteWebDriversAreSetup(BrowserType browserType, string seleniumWebDriversPath)
        {
            using (var driver = _remoteWebDriverUtility.Create(browserType, seleniumWebDriversPath))
            {
                driver.Navigate().GoToUrl(TestUrl);
                _remoteWebDriverUtility.Quit(browserType, driver);
            }
        }
    }
}
