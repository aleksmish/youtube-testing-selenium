using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Interactions;
using BasePageObjectModel;
using static System.Net.WebRequestMethods;

namespace YoutubeTests
{
    public class VideoSearch
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();
            options.BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            driver = new ChromeDriver(@"D:\ChromeDriver\chromedriver_win32\chromedriver.exe", options);
            driver.Navigate().GoToUrl("https://www.youtube.com/");
        }

        [Test]
        public void doesSearchBoxExist()
        {
            var searchBox = driver.FindElement(By.CssSelector("input[id='search']"));
            searchBox.Click();

            Assert.IsTrue(searchBox.IsDisplayed());
        }

        [Test]
        public void doesSearchButtonWorkProperly()
        {
            var searchBox = driver.FindElement(By.CssSelector("input[id='search']"));
            searchBox.Click();
            searchBox.SendKeys("Blender tutorial");

            driver.FindElement(By.CssSelector("button[id='search-icon-legacy']")).Click();

            Assert.IsTrue(driver.Url == "https://www.youtube.com/results?search_query=Blender+tutorial");        
        }

        [Test]
        public void isSearchResultDisplayedProperly()
        {
            var searchBox = driver.FindElement(By.CssSelector("input[id='search']"));
            searchBox.Click();
            searchBox.SendKeys("Blender tutorial");
            driver.FindElement(By.CssSelector("button[id='search-icon-legacy']")).Click();

            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 2));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("ytd-video-renderer div#meta yt-formatted-string")));

            var videos = driver.FindElements(By.CssSelector("ytd-video-renderer div#meta yt-formatted-string"));
            var occurencesOfValidVideos = videos.Count() - videos.Count(v => v.Text.ToLower().Contains("blender") || v.Text.ToLower().Contains("tutorial"));
         
            // check that valid videos count are equal or more 65% of all videos on the page
            Assert.IsTrue(100 * occurencesOfValidVideos / videos.Count() >= 65);
        }

        [Test]
        public void doesSearchBoxAcceptSpecialCharacters()
        {
            var searchBox = driver.FindElement(By.CssSelector("input[id='search']"));
            searchBox.Click();
            string input = "1234567890 abcdefg ~`!@#$%^&*()-_+={}[]|/\\:;\"`<>,.?";
            searchBox.SendKeys(input);

            Assert.IsTrue(driver.FindElement(By.CssSelector("input[id='search']")).GetAttribute("value") == input);
        }

        [Test]
        public void blankSearch()
        {
            var searchBox = driver.FindElement(By.CssSelector("input[id='search']"));
            searchBox.Click();
            searchBox.SendKeys("          ");

            Assert.IsTrue(driver.Url == "https://www.youtube.com/");
        }

        [TearDown]
        public void Quit()
        {
            driver.Quit(); 
        }
    }

    public class HomePageBeforeSignIn
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();
            options.BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            driver = new ChromeDriver(@"D:\ChromeDriver\chromedriver_win32\chromedriver.exe", options);
            driver.Navigate().GoToUrl("https://www.youtube.com/");
        }

        [Test]
        public void isTheUserNotAbleToSeeTheSubscriptionList()
        {
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 2));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='Subscriptions']")));
            driver.FindElement(By.XPath("//span[text()='Subscriptions']")).Click();
            Thread.Sleep(1000);
            Assert.IsTrue(driver.FindElement(By.XPath("//yt-formatted-string[text()='Sign in to see updates from your favorite YouTube channels']")).Displayed);
        }

        [Test]
        public void isTheUserNotAbleToSeeTheLibrary()
        {
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 2));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='Library']")));
            driver.FindElement(By.XPath("//span[text()='Library']")).Click();
            Thread.Sleep(1000);
            Assert.IsTrue(driver.FindElement(By.XPath("//yt-formatted-string[text()='Sign in to access videos that you’ve liked or saved']")).Displayed);
        }

        [Test]
        public void isTheUserNotAbleToSeeHistory()
        {
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 2));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='History']")));
            driver.FindElement(By.XPath("//span[text()='History']")).Click();
            Thread.Sleep(1000);
            Assert.IsTrue(driver.FindElement(By.XPath("//span[text()=\"Watch history isn't viewable when signed out.\"]")).Displayed);
        }

        [TearDown]
        public void Quit()
        {
            driver.Quit(); 
        }
    }

    public class HomePageAfterSignIn
    {
        private IWebDriver driver;
        string gmail = "testcases61@gmail.com";
        string password = "Admin_12345";

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();
            options.BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            driver = new ChromeDriver(@"D:\ChromeDriver\chromedriver_win32\chromedriver.exe", options);
            driver.Navigate().GoToUrl("https://www.youtube.com/");

            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));

            driver.FindElement(By.XPath("//ytd-button-renderer[@button-renderer]/yt-button-shape/a/yt-touch-feedback-shape/div")).Click();
            driver.FindElement(By.CssSelector("input[type='email']")).SendKeys(gmail);
            driver.FindElement(By.XPath("//span[text()='Next']")).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='password']")));
            driver.FindElement(By.CssSelector("input[type='password']")).SendKeys(password);
            driver.FindElement(By.XPath("//span[text()='Next']")).Click();

            Thread.Sleep(4000);
        }

        [Test]
        public void isTheUserAbleToSeeTheSubscriptionList()
        {
            driver.FindElement(By.XPath("//span[text()='Subscriptions']")).Click();
            Thread.Sleep(1000);
            Assert.IsTrue(driver.FindElements(By.XPath("//yt-formatted-string[text()='Sign in to see updates from your favorite YouTube channels']")).Count() == 0);
        }

        [Test]
        public void isTheUserAbleToSeeTheLibrary()
        {
            driver.FindElement(By.XPath("//span[text()='Library']")).Click();
            Thread.Sleep(1000);
            Assert.IsTrue(driver.FindElements(By.XPath("//yt-formatted-string[text()='Sign in to access videos that you’ve liked or saved']")).Count() == 0);
        }

        [Test]
        public void isTheUserAbleToSeeHistory()
        {
            driver.FindElement(By.CssSelector("yt-icon-button[id='guide-button']")).Click();
            driver.FindElement(By.XPath("//yt-formatted-string[text()='History']")).Click();
            Thread.Sleep(1000);
            Assert.IsTrue(driver.FindElements(By.XPath("//span[text()=\"Watch history isn't viewable when signed out.\"]")).Count() == 0);
        }

        [TearDown]
        public void Quit()
        {
            driver.Quit(); 
        }
    }

    public class VideoViewsBeforeSignIn
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();
            options.BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            driver = new ChromeDriver(@"D:\ChromeDriver\chromedriver_win32\chromedriver.exe", options);
            driver.Navigate().GoToUrl("https://www.youtube.com/");
        }

        [Test]
        public void isUserNotAbleToLike()
        {
            driver.FindElement(By.CssSelector("div[id='meta']")).Click();
            Thread.Sleep(3000);
            driver.FindElement(By.CssSelector("div[id='segmented-like-button']")).Click();
            Assert.IsTrue(driver.FindElement(By.XPath("//yt-formatted-string[text()='Sign in to make your opinion count.']")).Displayed);
        }

        [TearDown]
        public void Quit()
        {
            driver.Quit(); 
        }
    }

    public class VideoViewsAfterSignIn
    {
        private IWebDriver driver;
        string gmail = "testcases61@gmail.com";
        string password = "Admin_12345";

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();
            options.BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            driver = new ChromeDriver(@"D:\ChromeDriver\chromedriver_win32\chromedriver.exe", options);
            driver.Navigate().GoToUrl("https://www.youtube.com/");

            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));

            driver.FindElement(By.XPath("//ytd-button-renderer[@button-renderer]/yt-button-shape/a/yt-touch-feedback-shape/div")).Click();
            driver.FindElement(By.CssSelector("input[type='email']")).SendKeys(gmail);
            driver.FindElement(By.XPath("//span[text()='Next']")).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='password']")));
            driver.FindElement(By.CssSelector("input[type='password']")).SendKeys(password);
            driver.FindElement(By.XPath("//span[text()='Next']")).Click();

            Thread.Sleep(4000);
        }

        [Test]
        public void isUserAbleToLike()
        {
            driver.FindElement(By.CssSelector("div[id='meta']")).Click();
            Thread.Sleep(3000);
            driver.FindElement(By.CssSelector("div[id='segmented-like-button']")).Click();
            Assert.IsTrue(driver.FindElements(By.XPath("//yt-formatted-string[text()='Sign in to make your opinion count.']")).Count() == 0);
        }

        [TearDown]
        public void Quit()
        {
            driver.Quit(); 
        }
    }
}
