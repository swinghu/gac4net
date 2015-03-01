using HtmlAgilityPack;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class PlayStoreParser
    {
        /// <summary>
        /// Parse App URLS from the Search Result page
        /// </summary>
        /// <param name="response">HTML of the Search Result Page</param>
        /// <returns>Url of apps it finds</returns>
        public IEnumerable<String> ParseAppUrls (string response)
        {
            // Loading Html Document with Play Store content
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (response);

            // Checking for nodes
            HtmlNodeCollection nodes = map.DocumentNode.SelectNodes (Consts.APP_LINKS);

            if (nodes == null || nodes.Count == 0)
                yield break;

            // Reaching Nodes of Interest
            foreach (var node in nodes)
            {
                // Checking if this node contains the url of an app page
                if ((node.Attributes["href"] != null) && (node.Attributes["href"].Value.Contains ("details?id=")))
                {
                    yield return node.Attributes["href"].Value;
                }
            }
        }

        /// <summary>
        /// Checks whether the HTML received indicates the search / paging have more results or not
        /// </summary>
        /// <param name="response">HTML of the Search Results Page</param>
        /// <returns>True if any result was found, false otherwise</returns>
        public bool AnyResultFound (string response)
        {
            return response.IndexOf (Consts.NO_RESULT_MESSAGE, StringComparison.OrdinalIgnoreCase) < 0;
        }

        /// <summary>
        /// Parses ALL data about the app on the App's page
        /// </summary>
        /// <param name="response">HTML Response of the App's landing page</param>
        /// <param name="pageUrl">URL of the App's landing page</param>
        /// <returns>Parsed app data structure</returns>
        public AppModel ParseAppPage (string response, string pageUrl)
        {
            AppModel parsedApp = new AppModel ();

            // Updating App Url
            parsedApp.Url = pageUrl;

            // Loading HTML Document
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (response);

            // Parsing App Name
            HtmlNode currentNode = map.DocumentNode.SelectSingleNode (Consts.APP_NAME);
            parsedApp.Name       = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

            // Parsing Cover Img Url
            currentNode           = map.DocumentNode.SelectSingleNode (Consts.APP_COVER_IMG);
            parsedApp.CoverImgUrl = currentNode == null ? String.Empty : currentNode.Attributes["src"].Value;

            // Parsing App Category
            currentNode          = map.DocumentNode.SelectSingleNode (Consts.APP_CATEGORY);

            if (currentNode != null)
            {
                string catLink = currentNode.Attributes["href"].Value;

                if (catLink.IndexOf ('/') >= 0)
                {
                    string[] catLinkSplit = catLink.Split ('/');

                    parsedApp.Category = catLinkSplit.Last ();
                }
            }
            else
            {
                parsedApp.Category = "NO_CATEGORY_FOUND";
            }

            parsedApp.Category   = currentNode == null? String.Empty : currentNode.Attributes["href"].Value;

            // Parsing App Developer/Author
            currentNode         = map.DocumentNode.SelectSingleNode (Consts.APP_DEV);
            parsedApp.Developer = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

            // Parsing If the Developer is a Top Developer
            currentNode              = map.DocumentNode.SelectSingleNode (Consts.APP_TOP_DEV);
            parsedApp.IsTopDeveloper = currentNode == null ? false : true;

            // Parsing App Developer Url
            currentNode         = map.DocumentNode.SelectSingleNode (Consts.DEV_URL);

            if (currentNode != null && currentNode.Attributes["content"] != null)
            {
                parsedApp.DeveloperURL = currentNode.Attributes["content"].Value;
            }
            else
            {
                parsedApp.DeveloperURL = String.Empty;
            }

            // Parsing Publishing Date
            currentNode               = map.DocumentNode.SelectSingleNode (Consts.APP_PUBLISH_DATE);

            if (currentNode != null)
            {
                parsedApp.PublicationDate = ParseDate (currentNode.InnerText.Replace ("-", String.Empty).Trim ());
            }


            // Parsing Free x Paid App
            currentNode               = map.DocumentNode.SelectSingleNode (Consts.APP_FREE_PAID);

            if (currentNode.Attributes["content"] != null)
            {
                string contentValue = currentNode.Attributes["content"].Value;
                parsedApp.IsFree    = contentValue.Equals ("0") ? true : false;
            }
            else
            {
                parsedApp.IsFree = true;
            }

            // Parsing App Price
            if (parsedApp.IsFree)
            {
                parsedApp.Price = 0.0;
            }
            else
            {
                double appPrice;
                string normalizedPrice = currentNode.Attributes["content"].Value.Replace ("R$", String.Empty).Replace (",", "."); // Some countries use price as 5,20 instead of 5.20

                if (Double.TryParse(normalizedPrice, out appPrice))
                {
                    parsedApp.Price = appPrice;
                }
                else
                {
                    parsedApp.Price = 0.0;
                }
            }

            // Parsing number of app reviewers
            currentNode         = map.DocumentNode.SelectSingleNode (Consts.APP_REVIEWERS);
            parsedApp.Reviewers = currentNode == null ? String.Empty : currentNode.InnerText.Trim().Trim ('(').Trim(')');

            // Parsing App Description
            currentNode           = map.DocumentNode.SelectSingleNode (Consts.APP_DESCRIPTION);
            parsedApp.Description = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

            // Checking for In app Purchases 
            if (map.DocumentNode.SelectSingleNode (Consts.IN_APP_PURCHASE) != null)
            {
                parsedApp.HaveInAppPurchases = true;
            }
            else
            {
                parsedApp.HaveInAppPurchases = false;
            }

            // Parsing App's Score
            Score score = new Score ();

            // Total Score
            currentNode = map.DocumentNode.SelectSingleNode (Consts.APP_SCORE_VALUE);
            score.Total = ParseDouble (currentNode, "content");
            
            // Rating Count
            currentNode      = map.DocumentNode.SelectSingleNode (Consts.APP_SCORE_COUNT);
            score.Count      = ParseDouble (currentNode, "content");

            // Parsing Five  Stars Count
            currentNode      = map.DocumentNode.SelectSingleNode (Consts.APP_FIVE_STARS);
            score.FiveStars  = ParseDouble (currentNode);

            // Parsing Four Stars Count
            currentNode      = map.DocumentNode.SelectSingleNode (Consts.APP_FOUR_STARS);
            score.FourStars  = ParseDouble (currentNode);

            // Parsing Three Stars Count
            currentNode      = map.DocumentNode.SelectSingleNode (Consts.APP_THREE_STARS);
            score.ThreeStars = ParseDouble (currentNode);

            // Parsing Two Stars Count
            currentNode      = map.DocumentNode.SelectSingleNode (Consts.APP_TWO_STARS);
            score.TwoStars   = ParseDouble (currentNode);

            // Parsing One Stars Count
            currentNode      = map.DocumentNode.SelectSingleNode (Consts.APP_ONE_STARS);
            score.OneStars   = ParseDouble (currentNode);
                             
            // Updating Parsed App's Score
            parsedApp.Score  = score;

            // Parsing Last Update Date
            currentNode      = map.DocumentNode.SelectSingleNode (Consts.APP_UPDATE_DATE);

            if (currentNode != null)
            {
                parsedApp.LastUpdateDate = ParseDate (currentNode.InnerText.Replace ("-", String.Empty).Trim());
            }

            // Parsing App Size
            currentNode     = map.DocumentNode.SelectSingleNode (Consts.APP_SIZE);

            if (currentNode != null)
            {
                string stringSize = currentNode.InnerText.Trim ();
                Double appSize;

                // Checking if the app size is measured in MBs, Gbs or Kbs
                if (stringSize.EndsWith ("M", StringComparison.InvariantCultureIgnoreCase)) // MegaBytes
                {
                    // TryParse raises no exception. Its safer
                    if (Double.TryParse (stringSize.Replace ("M","").Replace ("m", "") , out appSize))
                    {
                        parsedApp.AppSize = appSize;
                    }
                }
                else if (stringSize.EndsWith ("G", StringComparison.InvariantCultureIgnoreCase)) // Gigabytes
                {
                    // TryParse raises no exception. Its safer
                    if (Double.TryParse(stringSize.Replace ("G", "").Replace ("g", "") , out appSize))
                    {
                        parsedApp.AppSize = appSize * 1000; // Normalizing Gygabites to Megabytes
                    }
                }
                else if (stringSize.EndsWith ("K", StringComparison.InvariantCultureIgnoreCase)) // Kbs
                {
                    // TryParse raises no exception. Its safer
                    if (Double.TryParse (stringSize.Replace ("K", "").Replace ("k", ""), out appSize))
                    {
                        parsedApp.AppSize = appSize / 1000; // Normalizing Kbs to Megabytes
                    }
                }
                else
                {
                    parsedApp.AppSize = -1; // Meaning that "App Size Varies Per App"
                }
            }

            // Parsing App's Current Version
            currentNode              = map.DocumentNode.SelectSingleNode (Consts.APP_VERSION);
            parsedApp.CurrentVersion = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();
            
            // Parsing App's Instalation Count
            currentNode              = map.DocumentNode.SelectSingleNode (Consts.APP_INSTALLS);
            parsedApp.Instalations   = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

            // Parsing App's Content Rating
            currentNode              = map.DocumentNode.SelectSingleNode (Consts.APP_CONTENT_RATING);
            parsedApp.ContentRating  = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

             // Parsing App's OS Version Required
            currentNode                = map.DocumentNode.SelectSingleNode (Consts.APP_OS_REQUIRED);
            parsedApp.MinimumOSVersion = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

            return parsedApp;
        }

        /// <summary>
        /// Parses the page of an app for the url of another apps,
        /// it gathers both "Related Apps" and "More from Developer" apps
        /// </summary>
        /// <param name="response">HTML responde of the app's landing page</param>
        /// <returns>Url of app pages it finds</returns>
        public IEnumerable<String> ParseExtraApps (string response)
        {
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (response);

            foreach (HtmlNode extraAppNode in map.DocumentNode.SelectNodes (Consts.EXTRA_APPS))
            {
                if (extraAppNode.Attributes["href"] != null)
                {
                    yield return extraAppNode.Attributes["href"].Value;
                }
            }
        }

        /// <summary>
        /// Safely parses a double value out of an HtmlNode attribute
        /// </summary>
        /// <param name="node">HtmlNode of interest</param>
        /// <param name="attrName">Name of the attribute that contains the double value</param>
        /// <returns>Parsed value (if any) or 0.0 in case of error</returns>
        private double ParseDouble (HtmlNode node, string attrName)
        {
            if (node != null && node.Attributes[attrName] != null)
            {
                double parsedScore;

                if (Double.TryParse (node.Attributes[attrName].Value, out parsedScore))
                {
                    return parsedScore;
                }
            }

            return 0.0;
        }
        
        /// <summary>
        /// Safely parses a double value out of an HtmlNode inner text
        /// </summary>
        /// <param name="node">HtmlNode of interest</param>
        /// <returns>Parsed value (if any) or 0.0 in case of error</returns>
        private double ParseDouble (HtmlNode node)
        {
            double parsedValue;

            // Removing Dots from value to make it easier to parse its correct value
            string normalizedInnerText = node.InnerText.Replace (".", String.Empty);

            if (Double.TryParse (normalizedInnerText, out parsedValue))
            {
                return parsedValue;
            }

            return 0.0;
        }

        /// <summary>
        /// Parses and builds a date out of a string following Google Play Store
        /// format
        /// </summary>
        /// <param name="dateString">String of date to be parsed</param>
        /// <returns>Assembled DateTime instance</returns>
        private DateTime ParseDate (string dateString)
        {
            string[] datePieces = dateString.Replace(",", String.Empty).Split (' ');

            // Backing values up
            string month = datePieces[0];
            string day   = datePieces[1];
            string year  = datePieces[2];

            // Normalizing Day if needed
            if (day.Length < 2)
                day = "0" + day;

            dateString = String.Join (" ",year, month, day);

            return DateTime.ParseExact (dateString, Consts.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
