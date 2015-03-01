//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// NameSpace : WebCrawler
// FileName : Util
//
// Created by : swinghu (ogrecpp@gamil.com) at 2014/6/3 15:48:41
//
// Function : Util tool class
//
//========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ShareLibrary;
using System.IO;
namespace ShareLibrary
{
    public static  class Util
    {


        /// <summary>
        /// 解析页面
        /// </summary>
        /// <param name="userHistoryPage"></param>
        /// <returns>用户的历史app记录LinkedList<string> </returns>
        public static LinkedList<string> parsePage(string userHistoryPage)
        {
            
            LinkedList<string> userHistoryList = new LinkedList<string>();

            string personInfo = null;
           
            if (!String.IsNullOrEmpty(userHistoryPage))
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
               
                doc.LoadHtml(userHistoryPage);

                HtmlAgilityPack.HtmlNode nod = doc.DocumentNode;

                //获取个人头像url
                //background-image:url(https://lh6.googleusercontent.com/-J55KRgZzylE/AAAAAAAAAAI/AAAAAAAAA2k/3r8PGvLUHfM/w150-h150-rw/photo.jpg)
                HtmlAgilityPack.HtmlNode headNode = nod.SelectSingleNode(Consts.PERSON_IMAGE_URL);
                if (headNode == null)
                {
                    return null;                   
                }
                string _userHeadUrl = headNode.Attributes["style"].Value.ToString();
                _userHeadUrl = _userHeadUrl.Split('(')[1].Replace(')',' ');

                //用户的名字
                HtmlAgilityPack.HtmlNode userNameNode = nod.SelectSingleNode(Consts.PERSON_NAME);
                string _userName = userNameNode.InnerHtml.ToString();

                personInfo = personInfo + _userName+"^" + _userHeadUrl;

                userHistoryList.AddFirst(personInfo);

                //获取app collection
                HtmlAgilityPack.HtmlNodeCollection appHistoryCollectionNode = nod.SelectNodes(Consts.APP_HISTORY_CELLOTION);
                if (appHistoryCollectionNode == null)
                {
                    return null;
                }
                foreach (var perAppItem in appHistoryCollectionNode)
	            {
                    Console.WriteLine("每个APP的历史使用记录：");
                    
                    string perAppString = perAppItem.InnerHtml.ToString();

                    //从collection中解析出用户使用的每个app的详细信息
                    string perAppContent = parsePerItem(perAppString);

                    userHistoryList.AddLast(perAppContent);
	            }
            }            
            return userHistoryList;
        }

        /// <summary>
        /// 解析单个历史app的信息
        /// </summary>
        /// <param name="perAppString"></param>
        /// <returns></returns>
        public static string parsePerItem(string perAppString)
        {
            StringBuilder appUserHistoryStr = new StringBuilder();
            if (String.IsNullOrEmpty(perAppString))
            {
                return null;                
            }
           
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(perAppString);
            HtmlAgilityPack.HtmlNode nod = doc.DocumentNode;

            //获取app的url
            HtmlAgilityPack.HtmlNode appUrl = nod.SelectSingleNode(Consts.APP_URL);
            string _appUrl = appUrl.Attributes["href"].Value.ToString();
            appUserHistoryStr.Append(_appUrl).Append("||");

            //获取app的名字
            HtmlAgilityPack.HtmlNode appName = nod.SelectSingleNode(Consts.APP_NAME);
            string _appName = appName.Attributes["title"].Value.ToString();
            appUserHistoryStr.Append(_appName).Append("||");

            //获取图片的url
            HtmlAgilityPack.HtmlNode appImageUrl = nod.SelectSingleNode(Consts.APP_IMAGE_URL);
            string _appIamgeUrl = appImageUrl.Attributes["data-cover-large"].Value.ToString();
            appUserHistoryStr.Append(_appIamgeUrl).Append("||");

            //获取打分情况
            HtmlAgilityPack.HtmlNode appRatingNode = nod.SelectSingleNode(Consts.APP_RATING);
            string _appRating = appRatingNode.InnerText.ToString();
            appUserHistoryStr.Append(_appRating).Append(" ");

            return appUserHistoryStr.ToString();
        }

        /// <summary>
        /// 将LinkList 转换成string
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static string convtList2Str(LinkedList<string> lst)
        {
            StringBuilder resStr = new StringBuilder();
            //foreach (var item in lst)
            //{
            //    resStr.Append(item).Append("\n").Append("##");
            //}
            for (int i = 0; i <= lst.Count-2; i++)
            {
                resStr.Append(lst.ElementAt(i)).Append("\n").Append("##");
            }
            resStr.Append(lst.Last());
            return resStr.ToString();
        }
        public static void saveUserHistory2Disk(string historyContent,string fileName)
        {

            //判断文件是否已经存在
            if (! isExitsInDisk(fileName))
            {

                string outFileName = "./" + "userHistory4/" + fileName + ".txt";
                FileStream fs = new FileStream(outFileName, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(historyContent);
                sw.Flush();
                sw.Close();
                fs.Close(); 
            }
        }

        public static bool isExitsInDisk(string fileName)
        {
            string currentDir = System.Environment.CurrentDirectory;
            string fullDirPath = currentDir + "\\userHistory4\\" + fileName + ".txt";
            FileInfo fileInfo = new FileInfo(fullDirPath);

            if (fileInfo.Exists)
            {
                return true;
            }
            return false;
            
        }
       
    }
}
