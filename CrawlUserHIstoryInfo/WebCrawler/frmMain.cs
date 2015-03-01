using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareLibrary;
using ShareLibrary.MongoDB;
using System.Threading;
using mshtml;
using MongoDB.Bson;
using MongoDB.Driver;
using ShareLibrary.Models;

namespace WebCrawler
{
    public partial class frmMain : Form
    {
        private MongoDBWrapper mongoDB = new MongoDBWrapper();

        private MongoCursor<UserModel> mongoCursor;

        private LinkedList<string> userUrlLinkList;
        public frmMain()
        {
            InitializeComponent();
        }

        private void web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
       
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            web.ScriptErrorsSuppressed = true;
            //web.Navigate("https://play.google.com/store/apps/details?id=com.trulia.android");
 
            web.Navigate("https://play.google.com/store/people/details?id=113610617769959990141");

            //初始化数据库
            // TO-DO  initialize the db
            //连接数据库
            string  fullServerAddress = String.Join(":", Consts.MONGO_SERVER, Consts.MONGO_PORT);

            mongoDB.ConfigureDatabase(Consts.MONGO_USER, Consts.MONGO_PASS,
                Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT,
                Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            //获取没有爬取的记录
            mongoCursor = mongoDB.QueryUnCrawledUser();

            //获取用户数量
            long userCount = mongoCursor.Count();

            //提取出用户url链接
            userUrlLinkList = new LinkedList<string>();

            foreach (var item in mongoCursor)
            {
                userUrlLinkList.AddFirst(item.UserUrl);
            }

            //从数据库中获取剩余ID的数量
            long remainingUderID = userCount;
            pFetchBar.Maximum =Convert.ToInt32(remainingUderID);
            pFetchBar.Minimum = 0;

            totalRemainBox.Text = Convert.ToString(remainingUderID);
        }

        /// <summary>
        /// 用户点击事件，解析页面内容，获取用户历史使用信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bParsePageButton_Click(object sender, EventArgs e)
        {
           string userPageHtml = web.DocumentText.ToString();

           //get the pagehtml as string ,then parse the page 
           LinkedList<string> userHistroyList = new LinkedList<string>();
           userHistroyList =  Util.parsePage(userPageHtml);
            //如果用户历史记录为空
           if (userHistroyList == null)
           {

               //不存盘，只更新数据库为已经爬取
               mongoDB.UpdateIsCrawled(userUrlLinkList.First());
               //从LinkList中删除该项
               userUrlLinkList.RemoveFirst();
           }
           else
           {
               string totalUserHistory = Util.convtList2Str(userHistroyList);

               string url = web.Url.ToString();
               string userID = url.Split('=')[1];

               string fileName = userHistroyList.First().Split('^')[0];
               fileName = fileName.Replace("\"", " ");

               //文件名包含非法字符
               if (fileName.Contains(":"))//包含非法字符
               {
                  fileName = fileName.Replace(":", " ");
               }
               if (fileName.Contains("*"))
               {
                   fileName = fileName.Replace("*", " ");
               }

               fileName = fileName + "=" + userID;
               Util.saveUserHistory2Disk(totalUserHistory, fileName);

               //更新数据库为已经爬取
               mongoDB.UpdateIsCrawled(userUrlLinkList.First());
               //从LinkList中删除该项
               userUrlLinkList.RemoveFirst();
           }
        }

        /// <summary>
        /// 用户点击，webBrowser浏览器打开下一个页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bNextPageItem_Click(object sender, EventArgs e)
        {
            //TODO 从数据库中获取用户的ID,循环调用
            string userUrl = userUrlLinkList.First();
            string crawlUrl = Consts.CRAWL_URL_PRIFIX + userUrl;

            //web 浏览器打开页面
            web.Navigate(crawlUrl);

            //更新爬取进度条
            int currentBarCount = pFetchBar.Value;

            if (++currentBarCount <= pFetchBar.Maximum)
            {
                pFetchBar.Value++;
            }
            else
            {
                MessageBox.Show("恭喜你，程序已经获取完毕！,点击【确定】退出程序","获取通知",MessageBoxButtons.OKCancel);
                Application.Exit();
            }
           

            string totalRemainCount = totalRemainBox.Text;
            int currentCount = Convert.ToInt32(totalRemainCount);
            totalRemainBox.Text = Convert.ToString(--currentCount);
        }
    }
}
