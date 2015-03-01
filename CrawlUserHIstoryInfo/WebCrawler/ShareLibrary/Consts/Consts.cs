//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// NameSpace : WebCrawler.ShareLibrary.Consts
// FileName : Consts
//
// Created by : swinghu (ogrecpp@gamil.com) at 2014/7/1 17:04:13
//
// Function : 
//
//========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLibrary
{
    class Consts
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
        public static readonly string CRAWL_URL = "https://play.google.com/store/search?q={0}&c=apps";
        //https://play.google.com/store/people/details?id=110265869400800434672
        public static readonly string CRAWL_URL_PRIFIX = "https://play.google.com/";

        //card-list 获取历史app使用记录集合
        public static readonly string APP_HISTORY_CELLOTION = "//div[@class='card one-rationale square-cover apps medium']";
        //用户头像
        public static readonly string PERSON_IMAGE_URL = "//div[@class='person-image']";
        
        //用户名字
        public static readonly string PERSON_NAME = "//div[@class='person-name']";
      
        //app 名字
        public static readonly string APP_NAME = "//a[@class='title']";

        //app url 
        public static readonly string APP_URL  = "//a[@class='title']";

        //app 的图片链接，以便下载
        public static readonly string APP_IMAGE_URL ="//img[@class='cover-image']";

        //app 打分情况 rating
        public static readonly string APP_RATING = "//span[@class='reason-body-container']//a[@class='reason-body']";
        //


        // Retry Values
        public static readonly int MAX_REQUEST_ERRORS = 100;
        public static readonly int MAX_QUEUE_TRIES = 5;

        // MongoDB configuration
        public static readonly string QUEUE_NAME = "PlayStoreQueue";


        /*win 8机器上的信息
        public static readonly string MONGO_SERVER = "192.168.10.113";
        public static readonly string MONGO_PORT = "27017";
        public static readonly string MONGO_USER = "";
        public static readonly string MONGO_PASS = "";
        public static readonly string MONGO_DATABASE = "GOOGStore";
        public static readonly string MONGO_COLLECTION = "AppReviews";
        public static readonly string MONGO_USERINFO_COLLECTION = "UserInfo";
         */
        public static readonly string MONGO_SERVER = "192.168.10.113";//192.168.10.113
        public static readonly string MONGO_PORT = "27017";
        public static readonly string MONGO_USER = "";//admin
        public static readonly string MONGO_PASS = "";//admin
        public static readonly string MONGO_DATABASE = "GOOGStore";//
        public static readonly string MONGO_COLLECTION = "UserInfo";//
        public static readonly string QUEUED_APPS_COLLECTION = "";//QueuedApps
        public static readonly string MONGO_AUTH_DB = "";//PlayStore
        public static readonly int MONGO_TIMEOUT = 10000;

        // Date Time Format
        public static readonly string DATE_FORMAT = "yyyy MMMM dd";
    }
}
