using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyCollectorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CompanyCollectorWeb
{
    public delegate void DelLogger(string message);

    public class Companies
    {
        private IWebDriver _driver;

        private DelLogger _logger;

        private readonly Action<string> _sendSignalMessage;

        public Companies(Action<string> sendSignalMessage, DelLogger logger = null)
        {
            //Initialize the instance

            _driver = new ChromeDriver();
            _logger = logger;
            _sendSignalMessage = sendSignalMessage;
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
                        pageCompanyList.ToList().ForEach(item => companyArrived(new CompanyModel() {Company = item}));
                        companyList.AddRange(pageCompanyList);
                    }
                    else
                    {
                        var pageCompanyList = await GetCompaniesFromPage(url);
                        pageCompanyList.ToList().ForEach(item => companyArrived(new CompanyModel() {Company = item}));
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
