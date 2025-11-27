using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using qa_dotnet_cucumber.Pages;
using qa_dotnet_cucumber.Utilities;
using qa_dotnet_cucumber.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace qa_dotnet_cucumber.Tests
{
    [TestFixture]
    public class EducationTests
    {
        private IWebDriver driver;
        private EducationPage educationPage;
        private LoginPage loginPage;

        private List<EducationModel> allEducations;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {

            allEducations = JsonReader.ReadData<EducationModel>("TestData/education.json", "educations");
        }

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("http://localhost:5003/Home");

            loginPage = new LoginPage(driver);
            loginPage.LoginWithValidUser();

            educationPage = new EducationPage(driver);
            educationPage.GoToEducationtab();
            educationPage.DeleteAllEducation();
        }

        [Test, Category("Positive")]
        public void AddValidEducation()
        {
            var reporter = ExtentReporter.GetReporter();
            var test = reporter.CreateTest("Add Valid Education");
            var positiveEducations = allEducations
                .Where(e => e.Scenario != null && e.Scenario.StartsWith("Valid Entry"))
                .ToList();

            foreach (var edu in positiveEducations)
            {
                try
                {
                    Console.WriteLine($"Adding: {edu.University} - {edu.Country}");

                    educationPage.AddEducation(edu.University, edu.Country, edu.Title, edu.Degree, edu.Year);
                    string toast = educationPage.GetToastMessage();
                    Console.WriteLine($"Toast: {toast}");

                    Assert.That(toast, Does.Contain("Education has been added"));
                    test.Pass($"Added education: {edu.University} ({edu.Degree})");
                }
                catch (Exception ex)
                {
                    test.Fail($"Failed to add education: {edu.University} - {ex.Message}");
                    Assert.Fail($"Failed to add {edu.University}: {ex.Message}");
                }
            }
        }


        [Test, Category("Negative")]
        public void AddEducationWithEmptyFields()
        {
            var reporter = ExtentReporter.GetReporter();
            var test = reporter.CreateTest("Add Education with Empty Fields");

            var emptyFieldEducations = allEducations
                .Where(e => e.Scenario != null && e.Scenario.StartsWith("Empty"))
                .ToList();

            foreach (var edu in emptyFieldEducations)
            {
                try
                {
                    educationPage.AddEducation(edu.University, edu.Country, edu.Title, edu.Degree, edu.Year);
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    IWebElement toastElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".toast")));
                    string toast = toastElement.Text;

                    Assert.That(toast, Does.Contain("Please enter all the fields"));
                    test.Pass($"Validation toast appeared: {toast}");
                }
                catch (WebDriverTimeoutException)
                {
                    test.Fail("Toast message not found - validation may have failed");
                }
                catch (Exception ex)
                {
                    test.Fail($"Unexpected error: {ex.Message}");
                }
            }
        }


        [Test, Category("SQLInjection")]
        public void AddEducationWithInjection()
        {
            var test = ExtentReporter.GetReporter().CreateTest("SQL Injection Tests");

            var injectionTests = allEducations
                .Where(e => e.Scenario != null && e.Scenario.Contains("Injection"))
                .ToList();

            foreach (var edu in injectionTests)
            {
                try
                {
                    educationPage.AddEducation(edu.University, edu.Country, edu.Title, edu.Degree, edu.Year);

                    string toast = educationPage.GetToastMessage();
                    var entries = educationPage.GetAllEducation();

                    bool isSaved = entries.Any(x => x.Contains(edu.University));

                    if (isSaved)
                    {
                        test.Fail($"Injection ACCEPTED for scenario: {edu.Scenario}");
                        Assert.Fail($"Injection accepted for: {edu.Scenario}");
                    }
                    else
                    {
                        test.Pass($"Injection blocked successfully: {edu.Scenario}");
                    }
                }
                catch (Exception ex)
                {
                    test.Fail($"Error while testing injection for: {edu.Scenario} - {ex.Message}");
                    Assert.Fail($"Error while testing injection for: {edu.Scenario} - {ex.Message}");
                }
            }
        }

        [Test, Category("Boundary")]
        public void AddEducationWithLongText()
        {
            var reporter = ExtentReporter.GetReporter();
            var test = reporter.CreateTest("Add Education with Long Text");

            var longTextEducations = allEducations.Where(e =>
                e.Scenario != null && e.Scenario.StartsWith("Long")).ToList();

            foreach (var edu in longTextEducations)
            {
                try
                {
                    educationPage.AddEducation(edu.University, edu.Country, edu.Title, edu.Degree, edu.Year);
                    string toast = educationPage.GetToastMessage();
                    test.Pass($"Added long text: {edu.Scenario} - Toast: {toast}");
                }
                catch (Exception ex)
                {
                    test.Fail($"Failed long text: {edu.Scenario} - {ex.Message}");
                }
            }
        }

        [Test, Category("Duplicate")]
        public void AddDuplicateEducation()
        {
            var reporter = ExtentReporter.GetReporter();
            var test = reporter.CreateTest("Add Duplicate Education");

            var duplicateEducations = allEducations.Where(e =>
                e.Scenario != null && e.Scenario.StartsWith("Duplicate")).ToList();

            foreach (var edu in duplicateEducations)
            {
                try
                {
                    educationPage.AddEducation(edu.University, edu.Country, edu.Title, edu.Degree, edu.Year);
                    string toast = educationPage.GetToastMessage();
                    test.Pass($"Duplicate scenario processed: {edu.Scenario} - Toast: {toast}");
                }
                catch (Exception ex)
                {
                    test.Fail($"Duplicate scenario failed: {edu.Scenario} - {ex.Message}");
                }
            }
        }
        [Test, Category("Edit")]
        public void EditEducationTest()
        {
            var reporter = ExtentReporter.GetReporter();
            var test = reporter.CreateTest("Edit Education Test");
            var existingEducations = educationPage.GetAllEducation();
            if (!existingEducations.Contains("Test University"))
            {
                educationPage.AddEducation("Test University", "Angola", "BFA", "Computer Science", "2023");
            }

            try
            {
                educationPage.EditEducation(
                 university: "Test University",
                 newUniversity: "Updated University",
                 newCountry: "Canada",
                 newTitle: "PHD",
                 newDegree: "Software Engineering",
                 newYear: "2024"
             );


                string toastMessage = educationPage.GetToastMessage();
                Assert.That(toastMessage, Does.Contain("updated successfully"));
                test.Pass($"Education updated successfully: {toastMessage}");
            }
            catch (Exception ex)
            {
                test.Fail($"Edit education failed: {ex.Message}");
                throw;
            }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;

                if (status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    string testName = TestContext.CurrentContext.Test.Name;
                    string screenshotPath = ScreenshotHelper.CaptureScreenshot(driver, testName);
                    ExtentReporter.LogFail(testName, screenshotPath);
                }
            }
            finally
            {
                driver.Quit();
                driver.Dispose();
                ExtentReporter.Flush();
            }
        }
    }
}

