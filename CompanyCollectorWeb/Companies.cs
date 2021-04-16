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
            var options = new ChromeOptions();
            options.AddArguments("headless", "--blink-settings=imagesEnabled=false");

            _driver = new ChromeDriver(options);
            _logger = logger;
            _sendSignalMessage = sendSignalMessage;
        }

        public async Task<IEnumerable<string>> GetCompanies(Func<CompanyModel, Task<IActionResult>> companyArrived = null, int pageAmount = 1000)
        {
            string url = "https://www.europages.de/unternehmen/Produktion.html";

            var companyList = new List<string>();
            int companiesCountInPage = 0;

            try
            {
                for(int i = 1; i < pageAmount; i++)
                {
                    if(i > 1)
                    {
                        var pageCompanyList = await GetCompaniesFromPage($"https://www.europages.de/unternehmen/pg-{i}/Produktion.html");
                        pageCompanyList.ToList().ForEach(item => companyArrived(new CompanyModel {CompanyName = item}));
                        companyList.AddRange(pageCompanyList);
                    }
                    else
                    {
                        var pageCompanyList = await GetCompaniesFromPage(url);
                        if(companiesCountInPage == 0)
                        {
                            companiesCountInPage = pageAmount * pageCompanyList.Count();
                        }

                        pageCompanyList.ToList().ForEach(item => companyArrived(new CompanyModel {CompanyName = item, CompaniesCount = companiesCountInPage}));
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

        //private List<string> GetCompaniesFromPage(string url)
        //{
        //    _driver.Navigate().GoToUrl(url);

        //    //wait for 0.1 seconds
        //    Task.Delay(100).Wait();

        //    var pages = _driver.FindElements(By.XPath("//a[contains(@class,'company-name')]"));

        //    List<string> lst = new List<string>();

        //    foreach (var company in pages)
        //    {
        //        //Collecting cata of all companies from single page
        //        string companyLink = company.GetAttribute("href");
        //        ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
        //        _driver.SwitchTo().Window(_driver.WindowHandles[1]);
        //        _driver.Navigate().GoToUrl(companyLink);
        //        Task.Delay(100).Wait();
        //        lst.Add(string.Join(",", GetCompanyDetails())); //Compiling data of a single company into one string
        //        _driver.SwitchTo().Window(_driver.WindowHandles[0]);
        //        Task.Delay(100).Wait();
        //    }

        //    return lst;
        //}

        //private List<string> GetCompanyDetails()
        //{
        //    //Collecting data of a single company
        //    List<string> line = new List<string>();
        //    line.Add(_driver.FindElement(By.XPath("//div[contains(@class, 'company-content')]/h3[contains(@itemprop, 'name')]")).GetAttribute("innerText").Replace(",", "")
        //        .Replace("\r", "").Replace("\n", " "));
        //    _logger(line[0]);
        //    line.Add(_driver.FindElement(By.XPath("//dd[contains(@class, 'company-country')]/span[2]")).GetAttribute("innerText").Replace(",", "").Replace("\r", "")
        //        .Replace("\n", " "));
        //    line.Add(_driver.FindElement(By.XPath("//dd[contains(@itemprop, 'addressLocality')]/pre")).GetAttribute("innerText").Replace(",", "").Replace("\r", "")
        //        .Replace("\n", " "));
        //    try
        //    {
        //        line.Add(_driver.FindElement(By.XPath("//div[contains(@class, 'page__layout-sidebar--container-desktop')]/a[contains(@itemprop, 'url')]")).GetAttribute("href"));
        //    }
        //    catch (NoSuchElementException)
        //    {
        //        line.Add("This company has no website.");
        //    }

        //    line.Add(_driver.FindElement(By.XPath("//p[contains(@class, 'company-description')]")).GetAttribute("innerText").Replace(",", "").Replace("\r", "").Replace("\n", " "));
        //    return line;
        //}
    }
}
