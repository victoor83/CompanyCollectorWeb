using OpenQA.Selenium;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using CompanyCollectorWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyCollectorWeb
{
    public delegate void DelLogger(string message);

    public class Companies
    {
        private readonly Action<string> _sendSignalMessage;
        private readonly IWebDriver _driver;

        private readonly Action<string?> _logger;

        public Companies(Action<string> sendSignalMessage, Action<string?> logger = null)
        {
            //Initialize the instance
            _sendSignalMessage = sendSignalMessage;
            _driver = new ChromeDriver();
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetCompanies(Func<CompanyModel, Task<IActionResult>> companyArrived = null, int pageAmount = 1000)
        {
            string url = "https://www.europages.de/unternehmen/Produktion.html";

            var companyList = new List<string>();

            try
            {
                for(int i = 1; i < pageAmount; i++)
                {
                    if(i > 1)
                    {
                        var pageCompanyList = await GetCompaniesFromPage($"https://www.europages.de/unternehmen/pg-{i}/Produktion.html");
                        pageCompanyList.ToList().ForEach(item => companyArrived(new CompanyModel() { Company = item }));
                        companyList.AddRange(pageCompanyList);
                    }
                    else
                    {
                        var pageCompanyList = await GetCompaniesFromPage(url);
                        pageCompanyList.ToList().ForEach(item => companyArrived(new CompanyModel() { Company = item }));
                        companyList.AddRange(pageCompanyList);
                    }

                    _logger?.Invoke("Proceed page nr:" + i);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                //close the driver instance.
                _driver.Close();

                //quit
                _driver.Quit();
            }

            return companyList;
        }

        private async Task<IEnumerable<string>> GetCompaniesFromPage(string url)
        {
            _driver.Navigate().GoToUrl(url);
            _driver.Manage().Window.Minimize();

            //wait for a seconds
            WebDriverWait _wait = new WebDriverWait(_driver, new TimeSpan(0, 1, 0));
            _wait.Until(d => d.FindElement(By.XPath("//a[contains(@class,'company-name')]")));

            //find the Next Button and click on it.

            var all = _driver.FindElements(By.XPath("//a[contains(@class,'company-name')]"));

            return all.Select(x => x.Text);
        }
    }
}
