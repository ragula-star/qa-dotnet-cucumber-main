using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace qa_dotnet_cucumber.Pages
{
    public class CertificationsPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public CertificationsPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }


        private readonly By CertificationsTab = By.XPath("//a[@data-tab='fourth']");
        private readonly By AddNewCertificationBtn = By.XPath("//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[5]/div[1]/div[2]/div/table/thead/tr/th[4]/div");
        private readonly By CertificationName = By.Name("certificationName");
        private readonly By CertificationFrom = By.Name("certificationFrom");
        private readonly By CertificationYear = By.Name("certificationYear");
        private readonly By AddCertificationBtn = By.XPath("//input[@type='button' and @value='Add']");
        private readonly By CertificationRows = By.XPath("//div[@data-tab='fourth']//table//tbody/tr");
        private readonly By DuplicatePopup = By.XPath("//div[contains(@class,'toast') or contains(text(),'already exists')]");
        private readonly By ErrorMessage = By.XPath("//div[contains(@class,'toast') or contains(@class,'ui message')]");
        private readonly By ToastMessage = By.XPath("//div[contains(@class,'ns-box-inner')]");


        public void GoToCertificationsTab()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(CertificationsTab)).Click();
        }

        public void AddCertification(string certName, string certFrom, string certYear)
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(AddNewCertificationBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(CertificationName)).SendKeys(certName);
            _driver.FindElement(CertificationFrom).SendKeys(certFrom);

            if (!string.IsNullOrEmpty(certYear))
            {
                var yearSelect = new SelectElement(_driver.FindElement(CertificationYear));
                yearSelect.SelectByText(certYear);
            }

            _driver.FindElement(AddCertificationBtn).Click();
        }


        public void EditCertification(string oldName, string newName, string newFrom, string newYear)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            foreach (var row in _driver.FindElements(CertificationRows))
            {
                var nameCell = row.FindElement(By.XPath("./td[1]"));

                if (nameCell.Text.Equals(oldName, StringComparison.OrdinalIgnoreCase))
                {

                    var editButton = wait.Until(d => row.FindElement(By.XPath(".//i[contains(@class,'write')]")));
                    editButton.Click();

                    var nameField = wait.Until(d => row.FindElement(By.XPath(".//input[@name='certificateName']")));
                    nameField.Clear();
                    nameField.SendKeys(newName);

                    var fromField = row.FindElement(By.XPath(".//input[@name='certifiedFrom']"));
                    fromField.Clear();
                    fromField.SendKeys(newFrom);

                    var yearSelect = new SelectElement(row.FindElement(By.XPath(".//select[@name='year']")));
                    yearSelect.SelectByText(newYear);

                    var updateButton = row.FindElement(By.XPath(".//input[@value='Update']"));
                    updateButton.Click();
                    break;
                }
            }
        }

        public void DeleteCertificationByName(string certName)
        {
            foreach (var row in _driver.FindElements(CertificationRows))
            {
                try
                {
                    var nameCell = row.FindElement(By.XPath("./td[1]"));
                    if (nameCell.Text.Equals(certName, StringComparison.OrdinalIgnoreCase))
                    {
                        var deleteBtn = row.FindElement(By.XPath(".//i[contains(@class,'remove icon')]"));
                        _wait.Until(ExpectedConditions.ElementToBeClickable(deleteBtn)).Click();
                        break;
                    }
                }
                catch { continue; }
            }
        }

        public void DeleteAllCertifications()
        {
            By deleteButton = By.XPath(".//span[@class='button' and ./i[contains(@class,'remove icon')]]");

            while (_driver.FindElements(CertificationRows).Count > 0)
            {
                var rows = _driver.FindElements(CertificationRows);
                int initialCount = rows.Count;

                try
                {
                    
                    var firstRow = rows[0];
                    var btn = firstRow.FindElement(deleteButton);
                    btn.Click();


                    //((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn);
                    

                   WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                    wait.Until(d => d.FindElements(CertificationRows).Count < initialCount);
                }
                catch (StaleElementReferenceException)
                {
                    continue; 
                }
                catch (NoSuchElementException)
                {
                    continue; 
                }
                catch (ElementClickInterceptedException)
                {
                    Thread.Sleep(500);
                    continue; 
                }
            }
        }



        public List<string> GetAllCertifications()
        {
            var certs = new List<string>();
            foreach (var row in _driver.FindElements(CertificationRows))
            {
                try
                {
                    var cert = row.FindElement(By.XPath("./td[1]")).Text;
                    certs.Add(cert);
                }
                catch { continue; }
            }
            return certs;
        }

        public string GetDuplicateMessage()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(DuplicatePopup)).Text;
            }
            catch { return string.Empty; }
        }
        public string GetToastMessage()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                var toast = wait.Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(ToastMessage)
                );
                return toast.Text.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

       
        public string GetErrorMessage()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(ErrorMessage)).Text;
            }
            catch { return string.Empty; }
        }
    }
}