using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Tips.Selenium.Test.Utilities
{
    internal class WebElementUtility
    {
        // Consider storing the DefaultWaitTime in the web.config.
        private const int DefaultWaitTime = 10;
        // Create a default wait time span so we can reuse the most common time span.
        private readonly TimeSpan _defaultWaitTimeSpan = TimeSpan.FromSeconds(DefaultWaitTime);
        private readonly IWebDriver _driver;

        private IWait<IWebDriver> Wait() => Wait(_defaultWaitTimeSpan);
        private IWait<IWebDriver> Wait(int waitTime) => Wait(TimeSpan.FromSeconds(waitTime));
        private IWait<IWebDriver> Wait(TimeSpan waitTimeSpan) => new WebDriverWait(_driver, waitTimeSpan);

        public WebElementUtility(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement FindElement(By locator)
        {
            Wait().Until(condition => ExpectedConditions.ElementIsVisible(locator));
            return _driver.FindElement(locator);
        }

        public IWebElement FindElement(By locator, Func<IWebDriver, IWebElement> condition)
        {
            Wait().Until(condition);
            return _driver.FindElement(locator);
        }

        /// <summary>
        /// Wait for page load by waiting for the element on old page to become stale
        /// and title on new page is set to the expected title on new page.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="elementOnOldPage"></param>
        /// <param name="titleOnNewPage"></param>
        /// <returns></returns>
        public void WaitUntilPageLoad(IWebElement elementOnOldPage, string titleOnNewPage)
        {
            // Inspiration:
            // http://www.obeythetestinggoat.com/how-to-get-selenium-to-wait-for-page-load-after-a-click.html
            // https://stackoverflow.com/questions/49866334/c-sharp-selenium-expectedconditions-is-obsolete
            Wait().Until(ExpectedConditions.StalenessOf(elementOnOldPage));
            Wait().Until(ExpectedConditions.TitleIs(titleOnNewPage));
        }
    }
}
