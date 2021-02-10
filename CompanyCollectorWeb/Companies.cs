using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace CompanyCollectorWeb
{
    public delegate void DelLogger(string message);

    public class Companies
    {
        private IWebDriver _driver;

        private DelLogger _logger;

        public Companies(DelLogger logger = null)
        {
            //Initialize the instance
            _driver = new ChromeDriver();
            _logger = logger;
        }

        public List<string> GetCompanies(int pageAmount = 1000)
        {
            string url = "https://www.europages.de/unternehmen/Produktion.html";

            var companyList = new List<string>();

            try
            {
                for(int i = 1; i < pageAmount; i++)
                {
                    if(i > 1)
                    {
                        companyList.AddRange(GetCompaniesFromPage($"https://www.europages.de/unternehmen/pg-{i}/Produktion.html"));
                    }
                    else
                    {
                        companyList.AddRange(GetCompaniesFromPage(url));
                    }

                    if(_logger != null)
                    {
                        _logger("Proceed page nr:" + i);
                    }
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

        private List<string> GetCompaniesFromPage(string url)
        {
            //launch gmail.com
            _driver.Navigate().GoToUrl(url);

            //maximize the browser
            _driver.Manage().Window.Minimize();

            //wait for a seconds
            Task.Delay(1000).Wait();

            //find the Next Button and click on it.

            var all = _driver.FindElements(By.XPath("//a[contains(@class,'company-name')]"));

            List<string> lst = new List<string>();
            foreach(var a in all)
            {
                lst.Add(a.Text);
            }

            return lst;
        }
    }
}
