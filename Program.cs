using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CsvHelper;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Text.Json;
using System.Text.Unicode;

// classes
class YoutubeData
{
    public string VideoID { get; set; }
    public string VideoLink { get; set; }
    public string VideoTitle { get; set; }
    public string VideoViews { get; set; }
    public string VideoUploader { get; set; }

}

class IctjobData
{
    public string JobID { get; set; }
    public string JobLink { get; set; }
    public string JobTitle { get; set; }
    public string JobCompany { get; set; }
    public string JobLocation { get; set; }
    public string JobKeywords { get; set; }

}

class StackoverflowData
{
    public string ResultID { get; set; }
    public string ResultLink { get; set; }
    public string ResultTitle { get; set; }
    public string ResultVotes { get; set; }
    public string ResultPoster { get; set; }

}

class Program
{
    static void Main(string[] args)
    {
        // initialize the program
        Console.WriteLine("Webscraper made with Selenium C# by Ben Couwberghs");
        Console.WriteLine("This webscraper was made for the platform google chrome.");
        Console.WriteLine("This webscraper was made as a study case for the course Devops & Security");
        Console.WriteLine("This webscraper supports scraping the following sites:");
        Console.WriteLine("1. youtube");
        Console.WriteLine("2. ictjobs.be");
        Console.WriteLine("3. stackoverflow, Note: please complete the captcha for the site before using the scraper on this site!");
        Console.WriteLine("******");

        Console.WriteLine("The following output methods can be used:");
        Console.WriteLine("1. console");
        Console.WriteLine("2. csv file");
        Console.WriteLine("3. JSON file");
        Console.WriteLine("******");

        Console.WriteLine("Note: The console will ask you to input a site selection, output selection and searchterm.");
        Console.WriteLine("Note: Filename will also be requested if applicable.");
        Console.WriteLine("Use the corresponding number!!");
        Console.WriteLine("******");

        // initiate the driver
        var options = new ChromeOptions { };
        options.AddArgument("--log-level=OFF");
        IWebDriver driver = new ChromeDriver(options);

        bool condition = true;
        while (condition)
        {
            // get the neccesary input
            Console.Write("Which site do you want to scrape (1 to 3)? ");
            string siteSelection = Console.ReadLine();
            Console.Write("Which output method do you want to use (1 to 3)? ");
            string outputSelection = Console.ReadLine();
            Console.Write("What searchterm do you want to use for the scraper? ");
            string searchterm = Console.ReadLine();

            Console.Write("What filename do you want to use (Only give something if you choose option 2 or 3 for output)? ");
            string fileName = Console.ReadLine();

            if (siteSelection == "1")
            {
                ScrapeYoutube(driver, searchterm, outputSelection, fileName);
            }
            else if (siteSelection == "2")
            {
                ScrapeIctjob(driver, searchterm, outputSelection, fileName);
            }
            else if (siteSelection == "3")
            {
                ScrapeStackoverflow(driver, searchterm, outputSelection, fileName);
            }
            else
            {
                Console.WriteLine("You messed up with the site selection...");
            }

            Console.Write("Do you want to close the webscraper console app (y or n)? ");
            string text = Console.ReadLine();
            if (text == "y")
            {
                condition = false;
            }
        }

        // shutdown the application and console window
        Console.WriteLine("Press a key to continue...");
        driver.Quit();
        Console.Read();

        
    }

    private static void ScrapeYoutube(IWebDriver driver, string searchterm, string outputSelection, string fileName)
    {
        
        driver.Navigate().GoToUrl("https://www.youtube.com/");
        var cookieElement = driver.FindElement(
        By.XPath("/html/body/ytd-app/ytd-consent-bump-v2-lightbox/tp-yt-paper-dialog/div[4]/div[2]/div[6]" +
        "/div[1]/ytd-button-renderer[2]/yt-button-shape/button"));
        cookieElement.Click();

        // replace sleep with explicit wait if time permits.
        Thread.Sleep(1000);

        var element = driver.FindElement(
        By.XPath("/html/body/ytd-app/div[1]/div/ytd-masthead/div[3]/div[2]/ytd-searchbox/form/div[1]/div[1]/input"));


        element.SendKeys(searchterm);
        element.Submit();

        Thread.Sleep(1200);

        // interact with the filters to make it be sorted by upload date!!!

        var filter = driver.FindElement(By.XPath("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]" +
            "/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[1]/div[2]" +
            "/ytd-search-sub-menu-renderer/div[1]/div/ytd-toggle-button-renderer/yt-button-shape/button"));
        filter.Click();
        filter = driver.FindElement(By.XPath("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]" +
            "/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[1]/div[2]" +
            "/ytd-search-sub-menu-renderer/div[1]/iron-collapse/div/ytd-search-filter-group-renderer[5]/ytd-search-filter-renderer[2]/a"));
        filter.Click();

        Thread.Sleep(1000);
        

        By elem_video = By.CssSelector("#contents > ytd-video-renderer");
        ReadOnlyCollection<IWebElement> videos = driver.FindElements(elem_video);

        // Create a collection of YoutubeData objects. loop over it, proceed to fill it
        // Follow up with Ifs for each output method -> neccesary variables can be scoped within the ifs, the proper method of coding!
        List<YoutubeData> listData = new List<YoutubeData> { new YoutubeData { }, new YoutubeData { }, new YoutubeData { }, new YoutubeData { }, new YoutubeData { } };

        // fill the objects with data
        foreach (YoutubeData item in listData)
        {
            int i = listData.IndexOf(item);
            item.VideoID = (i +1).ToString();

            IWebElement elem_video_link = videos[i].FindElement(By.Id("thumbnail"));
            item.VideoLink = elem_video_link.GetAttribute("href");

            IWebElement elem_video_title = videos[i].FindElement(By.CssSelector("#video-title"));
            item.VideoTitle = elem_video_title.Text;

            IWebElement elem_video_views = videos[i].FindElement(By.XPath(".//*[@id='metadata-line']/span[1]"));
            item.VideoViews = elem_video_views.Text;

            IWebElement elem_video_uploader = videos[i].FindElement(By.XPath(".//div[1]/div/div[2]/ytd-channel-name/div/div/yt-formatted-string/a"));
            item.VideoUploader = elem_video_uploader.Text;
        }

        // output methods
        if (outputSelection == "1")
        {
            // foreach loop to write output to console

            foreach (YoutubeData item in listData)
            {
                Console.WriteLine("******* Video " + (item.VideoID) + " *******");
                Console.WriteLine("Video Link: " + item.VideoLink);
                Console.WriteLine("Video Title: " + item.VideoTitle);
                Console.WriteLine("Video Views: " + item.VideoViews);
                Console.WriteLine("Video Uploader: " + item.VideoUploader);
                Console.WriteLine("\n");
            }
            
        }
        else if (outputSelection == "2")
        {
            // variables for CSV
            StreamWriter writer = new StreamWriter(fileName + ".csv");
            CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // header field
            csv.WriteField("Video_Number");
            csv.WriteField("Video_link");
            csv.WriteField("Video_Title");
            csv.WriteField("Video_Views");
            csv.WriteField("Video_Uploader");
            csv.NextRecord();

            foreach (YoutubeData item in listData)
            {
                csv.WriteField(item.VideoID);
                csv.WriteField(item.VideoLink);
                csv.WriteField(item.VideoTitle);
                csv.WriteField(item.VideoViews);
                csv.WriteField(item.VideoUploader);
                csv.NextRecord();
            }

            writer.Flush();
        }
        else if (outputSelection == "3")
        {
            // do stuff for output by JSON
            JsonSerializerOptions options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            
            string json = JsonSerializer.Serialize(listData, options);
            File.WriteAllText(fileName + ".json", json);
        }
    }
    private static void ScrapeIctjob(IWebDriver driver, string searchterm, string outputSelection, string fileName)
    {
        driver.Navigate().GoToUrl("https://www.ictjob.be/");

        var element = driver.FindElement(By.Id("keywords-input"));


        element.SendKeys(searchterm);
        element.Submit();

        Thread.Sleep(3500);

        var cookie = driver.FindElement(By.XPath("/html/body/div[2]/a"));
        cookie.Click();

        var filter = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form" +
            "/div[2]/div/div/div[2]/section/div/div[1]/div[2]/div/div[2]/span[2]/a"));
        filter.Click();

        // figure out how to explicitly wait till jobs are filtered on date
        Thread.Sleep(12000);
        By elem_job = By.CssSelector("#search-result-body > div > ul > li.search-item.clearfix");
        ReadOnlyCollection<IWebElement> jobs = driver.FindElements(elem_job);
       
        // Create a collection of YoutubeData objects. loop over it, proceed to fill it
        // Follow up with Ifs for each output method -> neccesary variables can be scoped within the ifs, the proper method of coding!
        List<IctjobData> listData = new List<IctjobData> { new IctjobData { }, new IctjobData { }, new IctjobData { }, new IctjobData { }, new IctjobData { } };

        // go through the list and fill it kwith the collected data
        foreach (IctjobData item in listData)
        {
            int i = listData.IndexOf(item);
            item.JobID = (i + 1).ToString();

            IWebElement elem_job_link = jobs[i].FindElement(By.CssSelector("#search-result-body > div > ul > li > span.job-info > a"));
            item.JobLink = elem_job_link.GetAttribute("href");

            IWebElement elem_job_title = jobs[i].FindElement(By.CssSelector("#search-result-body > div > ul > li > span.job-info > a > h2"));
            item.JobTitle = elem_job_title.Text;

            IWebElement elem_job_company = jobs[i].FindElement(By.CssSelector("#search-result-body > div > ul > li > span.job-info > span.job-company"));
            item.JobCompany = elem_job_company.Text;

            IWebElement elem_job_location = jobs[i].FindElement(By.CssSelector("#search-result-body > div > ul > li > span.job-info " +
                "> span.job-details > span.job-location > span > span"));
            item.JobLocation = elem_job_location.Text;

            IWebElement elem_job_keywords = jobs[i].FindElement(By.CssSelector("#search-result-body > div > ul > li > span.job-info" +
                " > span.job-keywords"));
            item.JobKeywords = elem_job_keywords.Text;
        }

        // output methods
        if (outputSelection == "1")
        {
            // foreach loop to write output to console

            foreach (IctjobData item in listData)
            {
                Console.WriteLine("******* job " + item.JobID + " *******");
                Console.WriteLine("details Link: " + item.JobLink);
                Console.WriteLine("Job Title: " + item.JobTitle);
                Console.WriteLine("Job Company: " + item.JobCompany);
                Console.WriteLine("Job Location: " + item.JobLocation);
                Console.WriteLine("Job Keywords: " + item.JobKeywords);
                Console.WriteLine("\n");
            }

        }
        else if (outputSelection == "2")
        {
            // variables for CSV
            StreamWriter writer = new StreamWriter(fileName + ".csv");
            CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // header field
            csv.WriteField("Job_Number");
            csv.WriteField("Job_Details");
            csv.WriteField("Job_Title");
            csv.WriteField("Job_Company");
            csv.WriteField("Job_Location");
            csv.WriteField("Job_Keywords");
            csv.NextRecord();

            foreach (IctjobData item in listData)
            {
                csv.WriteField(item.JobID);
                csv.WriteField(item.JobLink);
                csv.WriteField(item.JobTitle);
                csv.WriteField(item.JobCompany);
                csv.WriteField(item.JobLocation);
                csv.WriteField(item.JobKeywords);
                csv.NextRecord();
            }

            writer.Flush();
        }
        else if (outputSelection == "3") // check considering I don't have checks on my input...
        {
            // do stuff for output by JSON
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
            };

            string json = JsonSerializer.Serialize(listData, options);
            File.WriteAllText(fileName + ".json", json);
        }

        
    }

    private static void ScrapeStackoverflow(IWebDriver driver, string searchterm, string outputSelection, string fileName)
    {
        driver.Navigate().GoToUrl("https://www.stackoverflow.com/");

        var element = driver.FindElement(By.CssSelector("#search > div > input"));

        element.SendKeys(searchterm);
        element.Submit();

        Thread.Sleep(1500);

        // Cookie time
        var cookie = driver.FindElement(By.ClassName("js-accept-cookies"));
        cookie.Click();

        Thread.Sleep(1000);

        // well... time to fool the captcha? lol
        //IWebElement captchaContainer = driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div/div/div[2]/form/div[1]/div/div/iframe"));
        //driver.SwitchTo().Frame(captchaContainer);

        //var captcha = driver.FindElement(By.XPath("/html/body/div[2]/div[3]/div[1]/div/div/span"));
        //captcha.Click();

        //driver.SwitchTo().DefaultContent();

        // Ok I didn't know about switch to and was wasting time here damn.
        // yeah.... just ignore this ok?

        var filter = driver.FindElement(By.CssSelector("#mainbar > div.d-flex.ai-center.mb16 > div:nth-child(2) > div > a:nth-child(2)"));
        filter.Click();

        // figure out how to explicitly wait till jobs are filtered on date
        Thread.Sleep(1000);
        By elem_result = By.XPath("/html/body/div[3]/div[2]/div[1]/div[4]/div/div");
        ReadOnlyCollection<IWebElement> results = driver.FindElements(elem_result);

        // Create a collection of StackoverflowData objects. loop over it, proceed to fill it
        // Follow up with Ifs for each output method -> neccesary variables can be scoped within the ifs, the proper method of coding!
        List<StackoverflowData> listData = new List<StackoverflowData> { new StackoverflowData { }, new StackoverflowData { },
            new StackoverflowData { }, new StackoverflowData { }, new StackoverflowData { } };

        // go through the list and fill it kwith the collected data
        foreach (StackoverflowData item in listData)
        {
            int i = listData.IndexOf(item);
            item.ResultID = (i + 1).ToString();

            IWebElement elem_result_link = results[i].FindElement(By.XPath(".//div[2]/h3/a"));
            item.ResultLink = elem_result_link.GetAttribute("href");

            IWebElement elem_result_title = results[i].FindElement(By.XPath(".//div[2]/h3/a"));
            item.ResultTitle = elem_result_title.Text;

            IWebElement elem_result_votes = results[i].FindElement(By.XPath(".//div[1]/div[1]/span[1]"));
            item.ResultVotes = elem_result_votes.Text;

            IWebElement elem_result_user = results[i].FindElement(By.XPath(".//div[2]/div[2]/div[2]/div[2]/div/a"));
            item.ResultPoster = elem_result_user.Text; 
        }

        if (outputSelection == "1")
        {
            foreach (StackoverflowData item in listData)
            {
                Console.WriteLine("******* result " + item.ResultID +" *******");
                Console.WriteLine("Result Link: " + item.ResultLink);
                Console.WriteLine("Result Title: " + item.ResultTitle);
                Console.WriteLine("Result number of Votes: " + item.ResultVotes);
                Console.WriteLine("Posted by User: " + item.ResultPoster);
                Console.WriteLine("\n");
            }
        }

        // CSV
        else if (outputSelection == "2")
        {
            // variables for CSV
            StreamWriter writer = new StreamWriter(fileName + ".csv");
            CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteField("Result_Number");
            csv.WriteField("Result_Link");
            csv.WriteField("Result_Title");
            csv.WriteField("Result_no_Votes");
            csv.WriteField("Result_PostedBy");
            csv.NextRecord();

            foreach (StackoverflowData item in listData)
            {
                csv.WriteField(item.ResultID);
                csv.WriteField(item.ResultLink);
                csv.WriteField(item.ResultTitle);
                csv.WriteField(item.ResultVotes);
                csv.WriteField(item.ResultPoster);
                csv.NextRecord();
            }
            writer.Flush();
        }
        else if (outputSelection == "3") // check considering I don't have checks on my input...
        {
            // do stuff for output by JSON
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
            };

            string json = JsonSerializer.Serialize(listData, options);
            File.WriteAllText(fileName + ".json", json);
        }
    }
}

