using System;
using System.Collections.Generic;
using CMC_Browser.CMC_XAML;
using System.IO;
using HtmlAgilityPack;

namespace CMC_Browser
{
    public class CMC_Html
    {
        /// <summary>
        /// Coin Market Cap
        /// </summary>
        private const String CMC_URL = @"https://coinmarketcap.com/";

        //change this to a memory stream
        private readonly Uri CMC_TEMP_FILE = new Uri(Path.Combine(Directory.GetCurrentDirectory(), "CMC_Resources/CMC.html"));

        public CMC_Html()
        {

        }

        /// <summary>
        /// Returns Uri of the location of the temporar CMC.html file
        /// </summary>
        /// <returns>Uri</returns>
        public Uri GetFileLocation()
        {
            return CMC_TEMP_FILE;
        }

        /// <summary>
        /// Takes the CMC webpage and sets relative links to absolute i.e (/css/style.css) --> (CMC_url/css/style.css)
        /// </summary>
        /// <param name="webpage"></param>
        /// <returns></returns>
        private String FixLinks(String webpage)
        {
            string localHtmlLink = "href=\"/";
            string localHtmlLinkFixed = "href=\"" + CMC_URL;

            return webpage.Replace(localHtmlLink, localHtmlLinkFixed);
        }

        /// <summary>
        /// Writes the CMC collated pages to the temporary HTML file
        /// </summary>
        private void WriteHTMLToFile(String webpage)
        {
            File.WriteAllText(CMC_TEMP_FILE.LocalPath, webpage);
        }


        public void BuildCMCData(LOAD load = null)
        {
            HtmlAgilityPack.HtmlDocument CMC_MASTER = new HtmlAgilityPack.HtmlDocument() { OptionUseIdAttribute = true};
            //create a tag skeleton for the CMC page
            HtmlNode HTML_SKELETON = HtmlNode.CreateNode("<html><head></head><body></body></html>");
            CMC_MASTER.DocumentNode.AppendChild(HTML_SKELETON);
            
            //set head of the skeleton equal to the website and fix local linking
            HtmlNode cmcHead = new HtmlWeb().Load(CMC_URL).DocumentNode.SelectSingleNode("//head");
            CMC_MASTER.DocumentNode.SelectSingleNode("//head").InnerHtml = FixLinks(cmcHead.InnerHtml);

            HtmlNode cmcBody = new HtmlWeb().Load(CMC_URL).DocumentNode.SelectSingleNode("//body");
            CMC_MASTER.DocumentNode.SelectSingleNode("//body").InnerHtml = FixLinks(cmcBody.InnerHtml);

            //remove unneeded data
            CMC_MASTER.DocumentNode.SelectSingleNode("//*[@id=\"nav-main\"]").Remove();
            CMC_MASTER.DocumentNode.SelectSingleNode("//div[@class=\"row\"]//div[@class=\"col-xs-12\"]//div[@class=\"row\"]").Remove();

            List<HtmlNode> collectedPages = new List<HtmlNode>();

            for (int i = 2; i < 11; i++)
            {
                //update progress on UI thread
                load.prgLoading.Dispatcher.BeginInvoke(new Action(() =>
                {
                    load.prgLoading.Value =  i * 10;
                }));


                String currentUrl = String.Format("{0}{1}", CMC_URL, i);
                HtmlAgilityPack.HtmlDocument currentPage = new HtmlWeb().Load(currentUrl);

                //remove table head tags
                currentPage.DocumentNode.SelectSingleNode("//thead").Remove();

                collectedPages.Add(currentPage.DocumentNode.SelectSingleNode("//*[@id=\"currencies\"]"));
            }

            //append to collated html to the original currency table
            foreach (HtmlNode page in collectedPages)
            {
                CMC_MASTER.DocumentNode.SelectSingleNode("//*[@id=\"currencies\"]").AppendChild(page);
            }

            //write master page to file
            WriteHTMLToFile(CMC_MASTER.DocumentNode.OuterHtml);

            GC.Collect();
        }


    }
}
