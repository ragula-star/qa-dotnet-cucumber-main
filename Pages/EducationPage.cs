using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace qa_dotnet_cucumber.Pages
{
    public class EducationPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public EducationPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        private readonly By EducationTab = By.XPath("//a[text()='Education']");
        private readonly By AddNewBtn = By.XPath("/html/body/div[1]/div/section[2]/div/div/div/div[3]/form/div[4]/div/div[2]/div/table/thead/tr/th[6]/div");
        private readonly By UniversityName = By.XPath("//input[@placeholder='College/University Name']");
        private readonly By Countrycollege = By.Name("country");
        private readonly By Title = By.Name("title");
        private readonly By Degree = By.XPath("//input[@placeholder='Degree']");
        private readonly By YearGraduation = By.Name("yearOfGraduation");
        private readonly By AddEducationBtn = By.XPath("//input[@type='button' and @value='Add']");
        private readonly By DeleteEducationIcons = By.XPath("/div[@data-tab='third']//i[contains(@class,'remove icon')]");
        private readonly By DuplicatePopup = By.XPath("//div[contains(@class,'ns-box') or contains(@class,'toast') or contains(text(),'already exists')]");
        private readonly By EditButton = By.XPath("//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[4]/div/div[2]/div/table/tbody/tr/td[6]/span[1]/i");

        public void GoToEducationtab()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(EducationTab)).Click();
            //_wait.Until(ExpectedConditions.ElementToBeClickable(AddNewBtn)).Click();

        }

        public void AddEducation(string university, string countrycollege, string title, string degree, string yeargraduation)
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(AddNewBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(UniversityName));
            _driver.FindElement(UniversityName).Clear();
            _driver.FindElement(UniversityName).SendKeys(university);


            if (!string.IsNullOrEmpty(countrycollege))
            {
                var countrySelect = new SelectElement(_driver.FindElement(Countrycollege));
                countrySelect.SelectByText(countrycollege);
            }


            if (!string.IsNullOrEmpty(title))
            {
                var titleSelect = new SelectElement(_driver.FindElement(Title));
                titleSelect.SelectByText(title);
            }

            _driver.FindElement(Degree).Clear();
            _driver.FindElement(Degree).SendKeys(degree);


            if (!string.IsNullOrEmpty(yeargraduation))
            {
                var yearSelect = new SelectElement(_driver.FindElement(YearGraduation));
                yearSelect.SelectByText(yeargraduation);
            }

            _driver.FindElement(AddEducationBtn).Click();
        }

        public void DeleteEducationByUniversity(string universityName)
        {
            By educationRowLocator = By.XPath("//div[@class='twelve wide column scrollTable']//table//tbody/tr");
            foreach (var row in _driver.FindElements(educationRowLocator))
            {
                try
                {
                    var uniCell = row.FindElement(By.XPath("./td[2]"));
                    if (uniCell.Text.Equals(universityName, StringComparison.OrdinalIgnoreCase))
                    {
                        var deleteIcon = row.FindElement(By.XPath(".//i[contains(@class,'remove icon')]"));
                        _wait.Until(ExpectedConditions.ElementToBeClickable(deleteIcon)).Click();


                        _wait.Until(d => !_driver.FindElements(educationRowLocator)
                            .Any(r => r.FindElement(By.XPath("./td[2]")).Text.Equals(universityName, StringComparison.OrdinalIgnoreCase)));
                        break;
                    }
                }
                catch (StaleElementReferenceException)
                {
                    continue;
                }
                catch
                {
                    continue;
                }
            }

        }
        public void EditEducation(string university, string newUniversity = null, string newCountry = null, string newTitle = null, string newDegree = null, string newYear = null)
        {
            GoToEducationtab();
            _driver.FindElement(EditButton).Click(); 

            if (newUniversity != null)
            {
                var universityField = _driver.FindElement(UniversityName);
                universityField.Clear();
                universityField.SendKeys(newUniversity);
            }

            if (newCountry != null)
            {
                var countrySelect = new SelectElement(_driver.FindElement(Countrycollege));
                countrySelect.SelectByText(newCountry);
            }

            if (newTitle != null)
            {
                var titleSelect = new SelectElement(_driver.FindElement(Title));
                titleSelect.SelectByText(newTitle);
            }

            if (newDegree != null)
            {
                var degreeField = _driver.FindElement(Degree);
                degreeField.Clear();
                degreeField.SendKeys(newDegree);
            }

            if (newYear != null)
            {
                var yearSelect = new SelectElement(_driver.FindElement(YearGraduation));
                yearSelect.SelectByText(newYear);
            }

           _driver.FindElement(By.XPath("//input[@type='button' and @value='Update']")).Click();

            var toast = new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(By.CssSelector(".toast-message"));
                    return element.Displayed ? element.Text : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });

            if (string.IsNullOrEmpty(toast))
                throw new Exception("Toast message not found after update");
        }

        public void DeleteAllEducation()
        {

            By educationRowLocator = By.XPath("//div[@data-tab='third']//table/tbody/tr");
            By deleteIconLocator = By.XPath(".//i[contains(@class,'remove icon')]");

            while (_driver.FindElements(educationRowLocator).Count > 0)
            {

                var skillRows = _driver.FindElements(educationRowLocator);
                int initialCount = skillRows.Count;


                try
                {
                    IWebElement firstRow = skillRows[0];
                    _wait.Until(ExpectedConditions.ElementToBeClickable(firstRow.FindElement(deleteIconLocator))).Click();


                }
                catch (StaleElementReferenceException)
                {

                    continue;
                }

                _wait.Until(d => d.FindElements(educationRowLocator).Count < initialCount);
            }
        }
        public string GetToastMessage()
        {
            string message = string.Empty;
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                message = wait.Until(driver =>
                {
                    try
                    {
                        var toast = driver.FindElement(By.CssSelector(".toast-message")); 
                        return toast.Displayed ? toast.Text : null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null; 
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                message = "Toast message not found";
            }
            return message;
        }

        public List<string> GetAllEducation()
        {
            var entries = new List<string>();
            var rows = _driver.FindElements(By.XPath("//div[@data-tab='third']//table//tr"));

            foreach (var row in rows)
            {
                try
                {
                    var university = row.FindElement(By.XPath(".//td[1]")).Text;
                    entries.Add(university);
                }
                catch
                {
                    continue;
                }
            }

            return entries;
        }

       
        public void ResetAndAddEducation(List<(string university, string countryCollege, string title, string degree, string year)> education)
        {
            GoToEducationtab();
            DeleteAllEducation();

            foreach (var edu in education)
            {
                AddEducation(edu.university, edu.countryCollege, edu.title, edu.degree, edu.year);
            }
        }

        public string GetDuplicateMessage()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(DuplicatePopup)).Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string AssertDuplicateMessage()
        {
            var message = GetDuplicateMessage();

            if (string.IsNullOrEmpty(message))
                return "No duplicate message found.";

            return message.Trim();
        }


        public string GetErrorMessage()
        {
            try
            {
                var errorLocator = By.XPath("//div[contains(@class,'toast') or contains(@class,'ui message')]");
                return _wait.Until(ExpectedConditions.ElementIsVisible(errorLocator)).Text;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("No error message appeared for empty fields.");
                return string.Empty;
            }
        }


    }
}

