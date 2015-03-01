using BDC.BDCCommons;
using SharedLibrary;
using SharedLibrary.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtilsLib;

namespace PlayStoreCrawler
{
    class Crawler
    {
        /// <summary>
         /// Entry point of the crawler
         /// </summary>
         /// <param name="args"></param>
        static void Main (string[] args)
        {
            // Crawling App Store using all characters as the Search Input
            for (int i = 0; i < 9; i++)
            {
                CrawlStore(i.ToString());
            }
            CrawlStore("a");
            CrawlStore ("b");
            CrawlStore ("c");
            CrawlStore ("d");
            CrawlStore ("e");
            CrawlStore ("f");
            CrawlStore ("g");
            CrawlStore ("h");
            CrawlStore ("i");
            CrawlStore ("j");
            CrawlStore ("K");
            CrawlStore ("L");
            CrawlStore ("M");
            CrawlStore ("N");
            CrawlStore ("O");
            CrawlStore ("P");
            CrawlStore ("Q");
            CrawlStore ("R");
            CrawlStore ("S");
            CrawlStore ("T");
            CrawlStore ("U");
            CrawlStore ("V");
            CrawlStore ("X");
            CrawlStore ("Y");
            CrawlStore ("Z");
            CrawlStore ("W");
            /// ... Keep Adding characters / search terms in order to increase the crawler's reach
            // APP CATEGORIES
            CrawlStore ("BOOKS");
            CrawlStore ("BUSINESS");
            CrawlStore ("COMICS");
            CrawlStore ("COMMUNICATION");
            CrawlStore ("EDUCATION");
            CrawlStore ("ENTERTAINMENT");
            CrawlStore ("FINANCE");
            CrawlStore ("HEALTH");
            CrawlStore ("LIFESTYLE");
            CrawlStore ("LIVE WALLPAPER");
            CrawlStore ("MEDIA");
            CrawlStore ("MEDICAL");
            CrawlStore ("MUSIC");
            CrawlStore ("NEWS");
            CrawlStore ("PERSONALIZATION");
            CrawlStore ("PHOTOGRAPHY");
            CrawlStore ("PRODUCTIVITY");
            CrawlStore ("SHOPPING");
            CrawlStore ("SOCIAL");
            CrawlStore ("SPORTS");
            CrawlStore ("TOOLS");
            CrawlStore ("TRANSPORTATION");
            CrawlStore ("TRAVEL");
            CrawlStore ("WEATHER");
            CrawlStore ("WIDGETS");
            CrawlStore ("ARCADE");
            CrawlStore ("BRAIN");
            CrawlStore ("CASUAL");
            CrawlStore ("CARDS");
            CrawlStore ("RACING");
        }

        /// <summary>
        /// Executes a Search using the searchField as the search parameter, 
        /// paginates / scrolls the search results to the end adding all the url of apps
        /// it finds to a AWS SQS queue
        /// </summary>
        /// <param name="searchField"></param>
        private static void CrawlStore (string searchField)
        {
            // Console Feedback
            Console.WriteLine ("Crawling Search Term : [ " + searchField + " ]");

            // HTML Response
            string response;

            // MongoDB Helper
            // Configuring MongoDB Wrapper
            MongoDBWrapper mongoDB   = new MongoDBWrapper ();
            string fullServerAddress = String.Join (":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase (Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            // Response Parser
            PlayStoreParser parser = new PlayStoreParser (); 

            // Executing Web Requests
            using (WebRequests server = new WebRequests ())
            {
                // Creating Request Object
                server.Host = Consts.HOST;

                // Executing Initial Request  for example : https://play.google.com/store/search?q={0}&c=apps + ipf=1&xhr=1 
                // need to change the q={0} to other string ?
                //config the string 
                string curCrawlUrl = Consts.CRAWL_URL;
                curCrawlUrl  = curCrawlUrl.Replace("{0}",searchField);
                response     = server.Post(curCrawlUrl, Consts.INITIAL_POST_DATA);
                //response    = server.Post (Consts.CRAWL_URL, Consts.INITIAL_POST_DATA);

                // Parsing Links out of Html Page (Initial Request)                
                foreach (string url in parser.ParseAppUrls (response))
                {
                    // Checks whether the app have been already processed 
                    // or is queued to be processed
                    
                    if ((!mongoDB.AppProcessed (Consts.APP_URL_PREFIX + url)) && (!mongoDB.AppQueued (url))) 
                    {
                        // Console Feedback
                        Console.WriteLine (" . Queued App");

                        // Than, queue it :)
                        mongoDB.AddToQueue (url);
                    }
                    else
                    {
                        // Console Feedback
                        Console.WriteLine (" . Duplicated App. Skipped");
                    }
                }

                // Executing Requests for more Play Store Links
                int initialSkip       = 48;
                int currentMultiplier = 1;
                int errorsCount       = 0;
                do
                {
                    // Assembling new PostData with paging values
                    string postData = String.Format (Consts.POST_DATA, (initialSkip * currentMultiplier));

                    // Executing request for values

                    //response = server.Post (Consts.CRAWL_URL, postData);
                    response = server.Post(curCrawlUrl, postData);
                    // Checking Server Status
                    if (server.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        LogWriter.Error ("Http Error", "Status Code [ " + server.StatusCode + " ]");
                        errorsCount++;
                        continue;
                    }

                    // Parsing Links
                    foreach (string url in parser.ParseAppUrls (response))
                    {
                        // Checks whether the app have been already processed 
                        // or is queued to be processed
                        if ((!mongoDB.AppProcessed (Consts.APP_URL_PREFIX + url)) && (!mongoDB.AppQueued (url)))
                        {
                            // Console Feedback
                            Console.WriteLine (" . Queued App");

                            // Than, queue it :)
                            mongoDB.AddToQueue (url);
                        }
                        else
                        {
                            // Console Feedback
                            Console.WriteLine (" . Duplicated App. Skipped");
                        }
                    }

                    // Incrementing Paging Multiplier
                    currentMultiplier++;

                }  while (parser.AnyResultFound (response) && errorsCount <= Consts.MAX_REQUEST_ERRORS );
            }
        }
    }
}
