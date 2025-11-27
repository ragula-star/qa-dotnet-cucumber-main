
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter; 
using System.IO; 
using System; 

namespace qa_dotnet_cucumber.Utilities
{
	public static class ExtentReporter
	{
		private static ExtentReports? _extent;
		private static ExtentTest? _test;

		public static ExtentReports GetReporter()
		{
			if (_extent == null)
			{
				
				string reportsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
				Directory.CreateDirectory(reportsDir);
				string reportPath = Path.Combine(reportsDir, "AutomationReport.html");

				var sparkReporter = new ExtentSparkReporter(reportPath);
				_extent = new ExtentReports();
				_extent.AttachReporter(sparkReporter); 
			}
			return _extent;
		}

		public static ExtentTest CreateTest(string testName) 
		{
			_test = GetReporter().CreateTest(testName);
			return _test;
		}

		public static void LogPass(string message)
		{
			_test?.Pass(message);
		}

		public static void LogFail(string message, string screenshotPath)
		{
			_test?.Fail(message).AddScreenCaptureFromPath(screenshotPath);
		}

		public static void Flush()
		{
			_extent?.Flush();
		}
	}
}