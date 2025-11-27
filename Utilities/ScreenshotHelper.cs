using OpenQA.Selenium;
using System;
using System.IO;
using System;
using System.IO;

namespace qa_dotnet_cucumber.Utilities
{
    public static class ScreenshotHelper
    {
        public static string CaptureScreenshot(IWebDriver driver, string testName)
        {
            string folderPath = @"C:\Screenshots";
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

            File.WriteAllBytes(filePath, screenshot.AsByteArray);

            return filePath;
        }
    }
}


