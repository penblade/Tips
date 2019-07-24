using System;
using System.Diagnostics;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace Tips.Selenium.Test.Utilities
{
    internal class RemoteWebDriverUtility : IRemoteWebDriverUtility
    {
        public RemoteWebDriver Create(BrowserType browserType, string seleniumWebDriversPath)
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

        public void Quit(BrowserType browserType, RemoteWebDriver driver)
        {
            driver.Quit();
            if (browserType != BrowserType.IE11) return;

            EndProcessTree("IEDriverServer.exe");
            EndProcessTree("iexplore.exe");
        }

        private static void EndProcessTree(string imageName)
        {
            // Inspiration
            // https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/taskkill
            // https://stackoverflow.com/questions/5901679/kill-process-tree-programmatically-in-c-sharp
            // https://stackoverflow.com/questions/36729512/internet-explorer-11-does-not-close-after-selenium-test

            // /f - force process to terminate
            // /fi <Filter> - /fi \"pid gt 0 \" - select all processes
            // /im <ImageName> - select only processes with this image name
            Process.Start(new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = $"/f /fi \"pid gt 0\" /im {imageName}",
                CreateNoWindow = true,
                UseShellExecute = false
            })?.WaitForExit();
        }
    }
}
