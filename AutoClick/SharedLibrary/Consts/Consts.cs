using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class Consts
    {
        // Web Request Parameters and URLs
        //I got it (Thanks to @halfer) We need to do a POST to google play url:
        //https://play.google.com/store/apps/collection/topselling_free
        //And we need to send this parms with it:
        //start=0 <-- Start from 0;
        //num=60 <-- Give back 60 apps
        //numChildren=0 <-- I have no idea
        //ipf=1 <-- It seems to have a connection with the header
        //xhr=1 <-- page format
        //If we want app from 60 to 120 we need to request it like this: start=60&num=120
        public static readonly string CRAWL_URL          = "https://play.google.com/store/search?q={0}&c=apps";
        public static readonly string HOST               = "play.google.com";
        public static readonly string ORIGIN             = "https://play.google.com";
        public static readonly string REFERER            = "https://play.google.com/store/apps";
        public static readonly string INITIAL_POST_DATA  = "ipf=1&xhr=1";
        public static readonly string POST_DATA          = "start={0}&num=48&numChildren=0&ipf=1&xhr=1";
        public static readonly string APP_URL_PREFIX     = "https://play.google.com";
        public static readonly string ACCEPT_LANGUAGE    = "Accept-Language: en-US;q=0.6,en;q=0.4,es;q=0.2";
       
        //review XPaths
        //评论者姓名
        public static readonly string REVIEW_PERSON_NAME = "//div/span[@class='author-name']//a/@href";
        //评论者的主页链接
        public static readonly string REVIEW_PERSON_HMPG = "//div/span[@class='author-name']//a";
        //评论时间
        public static readonly string REVIEW_TIME        = "//div/span[@class='review-date']";
        //评论唯一ID
        public static readonly string REVIEW_UINIID = "//div/div//a";
        //用户评分 百分制：20%
        public static readonly string REVIEW_RATING = "//div/div[@class='current-rating']";
        //用户品论内容
        public static readonly string REVIEW_RATINGCONTENT = "//div[@class='review-body']";
        //用户评论title
        public static readonly string REVIEW_RATINGTITLE = "//div[@class='review-body']/span[@class='review-title']";
        //爬取App 的url 前缀 https://play.google.com/store/apps/details?id=
        public static readonly string CRAWLEDAPP_PREFIX = "https://play.google.com/store/apps/details?id=";
        // HTML Values
        public static readonly string NO_RESULT_MESSAGE = "找不到与"; // TODO: CHANGE THIS TO YOUR OWN LANGUAGE. 

        
        // Retry Values
        public static readonly int MAX_REQUEST_ERRORS   = 100;
        public static readonly int MAX_QUEUE_TRIES      = 5;

        // AWS
        public static readonly string QUEUE_NAME        = "PlayStoreQueue";

        // MongoDB
                                                             // TODO: CHANGE TO YOUR OWN SERVER CREDENTIALS IF NEEDED
                                                             // PLEASE NOTE THAT THIS USER HAS READWRITE PERMISSIONS TO THE APPS DATABASE, SO USE IT CAREFULLY
      /* 
        public static readonly string MONGO_SERVER           = "ec2-54-208-188-27.compute-1.amazonaws.com"; 
        public static readonly string MONGO_PORT             = "21766";
        public static readonly string MONGO_USER             = "GitHubCrawlerUser";
        public static readonly string MONGO_PASS             = "g22LrJvULU5B";
        public static readonly string MONGO_DATABASE         = "PlayStore";
        public static readonly string MONGO_COLLECTION       = "ProcessedApps";
        public static readonly string QUEUED_APPS_COLLECTION = "QueuedApps";
        public static readonly string MONGO_AUTH_DB          = "PlayStore";
        public static readonly int    MONGO_TIMEOUT          = 10000;
        
       */
        public static readonly string MONGO_SERVER = "192.168.10.113";
        public static readonly string MONGO_PORT = "27017";
        public static readonly string MONGO_USER = "";//admin
        public static readonly string MONGO_PASS = "";//admin
        public static readonly string MONGO_DATABASE = "test";
        public static readonly string MONGO_COLLECTION = "ProcessedApps";
        public static readonly string QUEUED_APPS_COLLECTION = "QueuedApps";
        public static readonly string MONGO_AUTH_DB = "";//PlayStore
        public static readonly int MONGO_TIMEOUT = 10000;
        
        // Date Time Format
        public static readonly string DATE_FORMAT       = "yyyy MMMM dd";
    }
}
