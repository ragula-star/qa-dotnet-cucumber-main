using OpenQA.Selenium;

namespace qa_dotnet_cucumber.Pages
{
    public class NavigationHelper
    {
        private readonly IWebDriver _driver;

        public NavigationHelper(IWebDriver driver)
        {
            _driver = driver;
        }

        public void GoToEducationPage()
        {
            _driver.Navigate().GoToUrl("http://localhost:5003/Home/Education");
        }

        public void GoToCertificationPage()
        {
            _driver.Navigate().GoToUrl("http://localhost:5003/Home/Certification");
        }
    }
}
