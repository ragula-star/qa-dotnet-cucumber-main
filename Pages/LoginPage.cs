using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

using SeleniumExtras.WaitHelpers;

namespace qa_dotnet_cucumber.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        public IWebDriver Driver => _driver;


        private readonly By SignInButton = By.XPath("//a[text()='Sign In']");
        private readonly By UsernameField = By.XPath("//input[@placeholder='Email address']");
        private readonly By PasswordField = By.XPath("//input[@placeholder='Password']");
        private readonly By LoginButton = By.XPath("//button[text()='Login']");
        private readonly By SuccessMessage = By.XPath("//a[text()='Mars Logo']");
        private readonly By EmailErrorAlert = By.XPath("//div[text()='Please enter a valid email address']");
        private readonly By PasswordErrorAlert = By.XPath("//div[text()='Password must be at least 6 characters']");

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public void signIn()
        {
            _wait.Until(Driver => _driver.FindElement(SignInButton)).Click();
        }

        public void Login(string username, string password)
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(UsernameField)).SendKeys(username);
            _wait.Until(d => d.FindElement(PasswordField)).SendKeys(password);
            _wait.Until(ExpectedConditions.ElementToBeClickable(LoginButton)).Click();
        }


        public void LoginWithValidUser()
        {

            //_driver.Navigate().GoToUrl("http://localhost:5000");
            signIn();
            Login("ragulau4@gmail.com", "Ragula123456@");
        }


        public bool IsAtLoginPage()
        {
            return _driver.Url.Contains("/Home");
        }

        public string GetEmailMessage()
        {
            return _wait.Until(d => d.FindElement(EmailErrorAlert)).Text;
        }

        public string GetEmailAlert()
        {
            return _wait.Until(d => d.FindElement(EmailErrorAlert)).Text;
        }

        public string GetPasswordAlert()
        {
            return _wait.Until(d => d.FindElement(PasswordErrorAlert)).Text;
        }

       
        public string GetSuccessMessage()
        {
            return _wait.Until(d => d.FindElement(SuccessMessage)).Text;
        }
    }
}