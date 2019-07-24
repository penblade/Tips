using OpenQA.Selenium.Remote;

namespace Tips.Selenium.Test.Utilities
{
    internal interface IRemoteWebDriverUtility
    {
        RemoteWebDriver Create(BrowserType browserType, string seleniumWebDriversPath);
        void Quit(BrowserType browserType, RemoteWebDriver driver);
    }
}