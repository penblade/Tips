using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace OpenQA.Selenium
{
    internal static class WebDriverFactory
    {
        public static IWebDriver Create(BrowserType browserType, string seleniumWebDriversPath)
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    return new ChromeDriver(seleniumWebDriversPath);
                case BrowserType.Firefox:
                    return new FirefoxDriver(seleniumWebDriversPath);
                case BrowserType.Edge:
                    // Edge 18 or greater is installed via command line.  See docs for more info.
                    return new EdgeDriver();
                case BrowserType.IE11:
                    return new InternetExplorerDriver(seleniumWebDriversPath);
                default:
                    throw new ArgumentOutOfRangeException(nameof(browserType), browserType, null);
            }
        }
    }
}
