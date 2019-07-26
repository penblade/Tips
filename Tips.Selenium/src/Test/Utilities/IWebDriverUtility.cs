using OpenQA.Selenium;

namespace Tips.Selenium.Test.Utilities
{
    internal interface IWebDriverUtility
    {
        IWebDriver Create(BrowserType browserType, string seleniumWebDriversPath);
        void Quit(BrowserType browserType, IWebDriver driver);
    }
}