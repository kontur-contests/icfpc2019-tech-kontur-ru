using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using lib.Models;
using MongoDB.Driver;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace pipeline
{
    public static class Extensions
    {
        public static void CheckOnline(this SolutionMeta meta, string geckodriverExecName)
        {
            meta.OnlineTime = GetOnlineTime(meta, geckodriverExecName);
            meta.IsOnlineCorrect = meta.OnlineTime == meta.OurTime;
            meta.IsOnlineChecked = true;
            meta.SaveToDb();
        }

        private static int GetOnlineTime(this SolutionMeta meta, string geckodriverExecName)
        {
            var reader = new ProblemReader(meta.ProblemPack);
            var problemPath = reader.GetProblemPath(meta.ProblemId);

            var solutionPath = Path.GetTempFileName();
            File.WriteAllText(solutionPath, meta.SolutionBlob);
            
            var driverDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var service = FirefoxDriverService.CreateDefaultService(driverDirectory, geckodriverExecName);
            var options = new FirefoxOptions { LogLevel = FirefoxDriverLogLevel.Error };
            options.AddArgument("-headless");
            var driver = new FirefoxDriver(service, options);
            driver.Navigate().GoToUrl("https://icfpcontest2019.github.io/solution_checker/");

            var problemField = driver.FindElement(By.Id("submit_task"));
            var solutionField = driver.FindElement(By.Id("submit_solution"));
            var submitButton = driver.FindElement(By.Id("execute_solution"));
            var outputElement = driver.FindElement(By.Id("output"));
        
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            problemField.SendKeys(problemPath);
            wait.Until(drv => outputElement.Text == "Done uploading task description");
        
            solutionField.SendKeys(solutionPath);
            wait.Until(drv => outputElement.Text == "Done uploading solution");
        
            submitButton.Click();
            wait.Until(drv => outputElement.Text != "Done uploading solution");

            var result = outputElement.Text;
            driver.Quit();
            
            var match = Regex.Match(result, "Success! Your solution took (\\d+) time units\\.");
            var timeUnits = int.Parse(match.Groups[1].Captures[0].Value);
        
            return timeUnits;
        }
    }
}
