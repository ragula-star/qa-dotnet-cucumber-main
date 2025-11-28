using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using qa_dotnet_cucumber.Pages;
using qa_dotnet_cucumber.Utilities;
using qa_dotnet_cucumber.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace qa_dotnet_cucumber.Tests
{
    [TestFixture]
    public class CertificationTests
    {
        private IWebDriver driver;
        private CertificationsPage certPage;
        private LoginPage loginPage;

        private List<CertificationModel> allCertifications;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            
            allCertifications = JsonReader.ReadData<CertificationModel>(
                "TestData/certifications.json",
                "certifications"
            );
        }


        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("http://localhost:5003/Home");

            loginPage = new LoginPage(driver);
            loginPage.LoginWithValidUser();

            certPage = new CertificationsPage(driver);
            certPage.GoToCertificationsTab();
            certPage.DeleteAllCertifications();
        }

        [Test, Category("PositiveCertifications")]
        public void AddValidCertifications()
        {
            var reporter = ExtentReporter.GetReporter();
            var test = reporter.CreateTest("Add Valid Certifications");

            var validCerts = allCertifications
                .Where(c => c.Scenario != null && c.Scenario.StartsWith("Valid Entry"))
                .ToList();

            foreach (var cert in validCerts)
            {
                try
                {
                    certPage.AddCertification(cert.CertificateOrAward, cert.CertifiedFrom, cert.Year);
                    string toast = certPage.GetToastMessage();
                    Console.WriteLine($"Toast: {toast}");

                    Assert.Multiple(() =>
                    {
                        Assert.That(toast.ToLower(), Does.Contain(cert.CertificateOrAward.ToLower()));
                        Assert.That(toast.ToLower(), Does.Contain("has been added"));
                    });

                    test.Pass($"Added Certification: {cert.CertificateOrAward}");
                }
                catch (Exception ex)
                {
                    test.Fail($"Failed: {cert.CertificateOrAward} - {ex.Message}");
                    Assert.Fail($"Failed to add {cert.CertificateOrAward}: {ex.Message}");
                }
            }
        }

        [Test, Category("NegativeCertifications")]
        public void AddCertificationsWithMissingFields()
        {
            var reporter = ExtentReporter.GetReporter();
            var test = reporter.CreateTest("Add Certifications with Missing/Empty Fields");

            var missingFields = allCertifications
                .Where(c => c.Scenario != null && (c.Scenario.Contains("Missing") || c.Scenario.Contains("Empty")))
                .ToList();

            foreach (var cert in missingFields)
            {
                try
                {
                    certPage.AddCertification(cert.CertificateOrAward, cert.CertifiedFrom, cert.Year);
                    string toast = certPage.GetErrorMessage();
                    Console.WriteLine($"Toast: {toast}");

                    Assert.That(toast, Does.Contain("Please enter all required fields"));

                    test.Pass($"Properly blocked: {cert.Scenario}");
                }
                catch (Exception ex)
                {
                    string screenshot = ScreenshotHelper.CaptureScreenshot(driver, cert.Scenario);
                    test.Fail($"Failed: {cert.Scenario} - {ex.Message}")
                        .AddScreenCaptureFromPath(screenshot);
                    Assert.Fail($"Failed scenario: {cert.Scenario} - {ex.Message}");
                }
            }
        }

        [Test, Category("NegativeInjection")]
        public void AddCertificationsWithInjectionOrLongText()
        {
            var reporter = ExtentReporter.GetReporter();
            var test = reporter.CreateTest("Add Certifications with SQL/JS Injection or Long Text");

            var injectionCases = allCertifications
                .Where(c => c.Scenario != null && (c.Scenario.Contains("SQL") || c.Scenario.Contains("XSS") || c.Scenario.Contains("Long")))
                .ToList();

            foreach (var cert in injectionCases)
            {
                try
                {
                    certPage.AddCertification(cert.CertificateOrAward, cert.CertifiedFrom, cert.Year);
                    string toast = certPage.GetErrorMessage();
                    Console.WriteLine($"Toast/Message: {toast}");
                    test.Info($"Scenario: {cert.Scenario} → Message: {toast}");
                }
                catch (Exception ex)
                {
                    string screenshot = ScreenshotHelper.CaptureScreenshot(driver, cert.Scenario);
                    test.Fail($"Error in scenario: {cert.Scenario} - {ex.Message}")
                        .AddScreenCaptureFromPath(screenshot);
                }
            }
        }

        [Test, Category("EditCertification")]
        public void EditCertificationTest()
        {
            var reporter = ExtentReporter.GetReporter();
            var test = reporter.CreateTest("Edit Certification Test");

            var editCerts = allCertifications
                .Where(c => c.Scenario != null && c.Scenario.Contains("Edit"))
                .ToList();

            foreach (var cert in editCerts)
            {
                try
                {
                    certPage.EditCertification(cert.OldCertificateOrAward, cert.CertificateOrAward, cert.CertifiedFrom, cert.Year);
                    test.Pass($"Edited Certification: {cert.OldCertificateOrAward} → {cert.CertificateOrAward}");
                }
                catch (Exception ex)
                {
                    string screenshot = ScreenshotHelper.CaptureScreenshot(driver, cert.Scenario);
                    test.Fail($"Failed to edit: {cert.Scenario} - {ex.Message}")
                        .AddScreenCaptureFromPath(screenshot);
                }
            }
        }
        [Test, Category("DuplicateCertifications")]
        public void AddDuplicateCertification()
        {
            var reporter = ExtentReporter.GetReporter();
            var test = reporter.CreateTest("Add Duplicate Certification");

                 var duplicateCerts = allCertifications
                .Where(c => c.Scenario != null && c.Scenario == "Duplicate Record")
                .ToList();

            foreach (var cert in duplicateCerts)
            {
                try
                {
                    certPage.AddCertification(cert.CertificateOrAward, cert.CertifiedFrom, cert.Year);
                    certPage.AddCertification(cert.CertificateOrAward, cert.CertifiedFrom, cert.Year);

                    string toast = certPage.GetDuplicateMessage();
                    Console.WriteLine($"Toast: {toast}");
                    Assert.That(toast.ToLower(), Does.Contain("already exists").Or.Contain("duplicate"));

                    test.Pass($"Duplicate scenario passed for: {cert.CertificateOrAward}");
                }
                catch (Exception ex)
                {
                    test.Fail($"Duplicate scenario failed for: {cert.CertificateOrAward} - {ex.Message}");
                    Assert.Fail($"Failed duplicate test for {cert.CertificateOrAward}: {ex.Message}");
                }
            }
        }
        [TearDown]
        public void TearDown()
        {
            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                string testName = TestContext.CurrentContext.Test.Name;

                if (status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    string screenshot = ScreenshotHelper.CaptureScreenshot(driver, testName);
                    ExtentReporter.LogFail(testName, screenshot);
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
