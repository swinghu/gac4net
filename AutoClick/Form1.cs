using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using AutoClick;
using System.Linq;
using SharedLibrary.MongoDB;
using SharedLibrary;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedLibrary.Models;

namespace TianXia
{
    public partial class Form1 : Form
    {
        bool a = true;
        int b = 1;
        int X1 = 0;
        int Y1 = 0;
        int X2 = 0;
        int Y2 = 0;
        int X3 = 0;
        int Y3 = 0;
        int timer = 1;
        int count = 800000000;
        int[] intervals = { 8000, 9000, 8000 };
        bool TextBoxChange = false;
        bool NotChangeTextBox = false;

        //MongoDB Helper
        //Configure MongoDB Wrapper
        MongoDBWrapper mongoDB;
        string fullServerAddress = String.Join(":",Consts.MONGO_SERVER,Consts.MONGO_PORT);
        // configure the database
        MongoCursor<QueuedApp> mongoCursor;
        LinkedList<string> urlLinkList;
        
        Dictionary<string, string> reviewDic = new Dictionary<string, string>();
        int reviewNumCount = 0;

        [System.Runtime.InteropServices.DllImport("user32.dll")] //申明API函数 
        public static extern bool RegisterHotKey(
        IntPtr hWnd, // handle to window 
        int id, // hot key identifier 
        uint fsModifiers, // key-modifier options 
        Keys vk // virtual-key code 
        );

        [System.Runtime.InteropServices.DllImport("user32.dll")] //申明API函数 
        public static extern bool UnregisterHotKey(
        IntPtr hWnd, // handle to window 
        int id // hot key identifier 
        );

        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }

        protected override void WndProc(ref Message m)//监视Windows消息 
        {
            const int WM_HOTKEY = 0x0312; //如果m.Msg的值为0x0312那么表示用户按下了热键 
            const int WM_NCLBUTTONDBLCLK = 0xA3;


            switch (m.Msg)
            {
                case WM_HOTKEY:
                    ProcessHotkey(m); //按下热键时调用ProcessHotkey()函数
                    break;
                case WM_NCLBUTTONDBLCLK:   //双击标题栏隐藏窗体
                    notifyIcon1_MouseDoubleClick(null, null);
                    break;

            }
            base.WndProc(ref m); //将系统消息传递自父类的WndProc 
        }
        private void ProcessHotkey(Message m)
        {
            IntPtr id = m.WParam; //IntPtr用于表示指针或句柄的平台特定类型 
            //MessageBox.Show(id.ToString()); 
            string sid = id.ToString();

            if (sid == "400")// 按下Control +光标下箭头
            {
                button2_Click(null, null);
            }
            else if (sid == "300")
                button1_Click(null, null);
            else if (sid == "200")
                notifyIcon1_MouseDoubleClick(null, null);

        }


        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);


        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (b == 3)
            {
                label4.Text = Cursor.Position.X.ToString();

                label5.Text = Cursor.Position.Y.ToString();
            }
            else if (b == 5)
            {
                label22.Text = Cursor.Position.X.ToString();
                label21.Text = Cursor.Position.Y.ToString();
            }
            else if (b == 7)
            {
                label13.Text = Cursor.Position.X.ToString();
                label12.Text = Cursor.Position.Y.ToString();
            }

            if (!a)
            {
                int newcount = 0;
                string time = "";

                if (count >= 1000)
                    newcount = count / 1000;
                else
                    newcount = 0;

                time = newcount / 3600 + "小时" + (newcount % 3600) / 60 + "分" + newcount % 60 + "秒";


                if (timer == 2)

                    this.Text = time + "后点击\"坐标二\"";
                else if (timer == 3)

                    this.Text = time + "后点击\"坐标三\"";
                else if (timer == 1)
                {
                    this.Text = time + "后点击\"坐标一\"";


                    timer1.Interval = 1000;
                }

                count = count - 1000;



            }
        }

        private void button2_Click(object sender, EventArgs e) //"开始点击"按钮事件
        {

            a = !a;

            if (!a)
            {
                count = intervals[0];
                button2.Text = "停止点击";
                this.Text = "按\"Ctrl+↓\"退出点击";
                button1.Enabled = false;
                timer = 1;
                timer3.Interval = count;
                timer3.Enabled = true;

                timer1.Interval = 1;
                timer1.Enabled = true;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton5.Enabled = false;
                radioButton6.Enabled = false;
                radioButton7.Enabled = false;
                radioButton8.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;

            }
            else
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton5.Enabled = true;
                radioButton6.Enabled = true;
                radioButton7.Enabled = true;
                radioButton8.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                timer1.Enabled = false;
                timer1.Interval = 200;
                timer3.Enabled = false;
                button2.Text = "开始点击";

                this.Text = "AutoClick";

                button1.Enabled = true;
            }


        }
        private void selectCoordinate()
        {
            b = b + 1;
            if (b == 8)
            { b = 1; }

            switch (b)
            {
                case 1:
                    button2.Enabled = true;
                    timer1.Enabled = false;
                    button1.Text = "重新选择";
                    label11.Text = "坐标三";
                    X3 = Cursor.Position.X;
                    Y3 = Cursor.Position.Y;

                    break;
                case 2:
                    if (!a)
                        button2_Click(null, null);
                    button2.Enabled = false;
                    a = true;
                    button2.Text = "开始单击";

                    button1.Text = "选坐标一";
                    break;
                case 3:
                    button1.Text = "确定选择";
                    label6.Text = "当前光标";

                    timer1.Enabled = true;
                    break;
                case 4:
                    X1 = Cursor.Position.X;
                    Y1 = Cursor.Position.Y;
                    button1.Text = "选坐标二";
                    label6.Text = "坐标一";
                    timer1.Enabled = false;
                    break;
                case 5:
                    button1.Text = "确定选择";
                    label20.Text = "当前光标";

                    timer1.Enabled = true;
                    break;
                case 6:
                    X2 = Cursor.Position.X;
                    Y2 = Cursor.Position.Y;
                    button1.Text = "选坐标三";
                    label20.Text = "坐标二";
                    timer1.Enabled = false;
                    break;
                case 7:
                    button1.Text = "确定选择";
                    label11.Text = "当前光标";

                    timer1.Enabled = true;
                    break;

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            selectCoordinate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            RegisterHotKey(Handle, 300, 2, Keys.Up); // 热键一:Control +光标上箭头
            RegisterHotKey(Handle, 400, 2, Keys.Down); // 热键一:Control +光标下箭头
            //RegisterHotKey(Handle, 100, 2, Keys.Left); // 热键一:Control +光标左箭头
            RegisterHotKey(Handle, 200, 2, Keys.Right); //热键一:Control +光标右箭头
            
            //browse the url
            web.ScriptErrorsSuppressed = true;
            
            //MongoDB Helper
            //Configure MongoDB Wrapper
            mongoDB = new MongoDBWrapper();
            mongoDB.ConfigureDatabase(Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB,
                fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.QUEUED_APPS_COLLECTION);

            //get the UnCrawledApp Collections
            mongoCursor = mongoDB.QueryUnCrawledApp();
            urlLinkList = new LinkedList<string>();
            foreach (var appItem in mongoCursor)
            {
                urlLinkList.AddFirst(appItem.Url);
            }
            //mongoDB.UpdateIsCrawled("/store/apps/details?id=com.wTheUltimateGuideToGolf");
            //加载一个网页
            string urlStr = BuildUrlString(urlLinkList.First());
            web.Navigate(urlStr);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            UnregisterHotKey(Handle, 400); //缷载第4个快捷键
            UnregisterHotKey(Handle, 300);
            UnregisterHotKey(Handle, 200);
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = !this.Visible;
            if (this.Visible) { notifyIcon1.Text = "双击显示/隐藏窗体"; }
            else { notifyIcon1.Text = "按\"Ctrl+↓\"退出点击"; }


        }

        private void Form1_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Help newHelp = new Help();
            newHelp.ShowDialog();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Process currentProcess = Process.GetCurrentProcess(); //获取当前进程 
            //获取当前运行程序完全限定名 
            string currentFileName = currentProcess.MainModule.FileName;
            //获取进程名为ProcessName的Process数组。 
            Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
            //遍历有相同进程名称正在运行的进程 
            foreach (Process process in processes)
            {
                if (process.MainModule.FileName == currentFileName)
                {
                    if (process.Id != currentProcess.Id) //根据进 程ID排除当前进程 
                    {
                        MessageBox.Show("只能运行一个相同应用程序!", "系统提示", MessageBoxButtons.OK);
                        UnregisterHotKey(Handle, 400); //缷载第4个快捷键
                        UnregisterHotKey(Handle, 300);
                        UnregisterHotKey(Handle, 200);
                        this.Close();
                        return;
                    }
                }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {

            if (timer == 1)
            {
                timer = timer + 1;

                SetCursorPos(X1, Y1);
                if (radioButton1.Checked)//点击第一个坐标点
                {
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, X1, Y1, 0, (UIntPtr)0);
                }
                else
                {
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, X1, Y1, 0, (UIntPtr)0);
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, X1, Y1, 0, (UIntPtr)0);
                }
                timer3.Interval = intervals[1];
                count = intervals[1];

                parsePage();//获取页面内容
            }
            else if (timer == 2) //点击第二个坐标点
            {
                timer = timer + 1;
                SetCursorPos(X2, Y2);
                if (radioButton8.Checked)
                {
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, X2, Y2, 0, (UIntPtr)0);
                }
                else
                {
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, X2, Y2, 0, (UIntPtr)0);
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, X2, Y2, 0, (UIntPtr)0);
                }

                timer3.Interval = intervals[2];
                count = intervals[2];

                parsePage();//获取页面内容
            }
            else if (timer == 3)//点击第三个坐标点
            {
                timer = 1;
                SetCursorPos(X3, Y3);
                if (radioButton5.Checked)
                {
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, X3, Y3, 0, (UIntPtr)0);
                }
                else
                {
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, X3, Y3, 0, (UIntPtr)0);
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, X3, Y3, 0, (UIntPtr)0);
                }


                timer3.Interval = intervals[0];
                count = intervals[0];

                parsePage();//获取页面内容
            }

            
        }

        private void button3_Click(object sender, EventArgs e)
        {

            TextBoxChange = false;

            if (isTextBoxNull())
            {
                MessageBox.Show("时间间隔不能为空!", "错误", MessageBoxButtons.OK);
                button4.Enabled = true;
                return;
            }
            else
            {
                intervals[0] = Convert.ToInt16(textBox1.Text) * 1000;
                intervals[1] = Convert.ToInt16(textBox2.Text) * 1000;
                intervals[2] = Convert.ToInt16(textBox3.Text) * 1000;

                count = intervals[0];
                timer3.Interval = intervals[0];
            }

            button3.Enabled = false;
            button4.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            NotChangeTextBox = true;
            TextBoxChange = false;
            textBox1.Text = (intervals[0] / 1000).ToString();//在设置每次点击间隔时间
            textBox2.Text = (intervals[1] / 1000).ToString();
            textBox3.Text = (intervals[2] / 1000).ToString();
            NotChangeTextBox = false;
            button3.Enabled = false;
            button4.Enabled = false;

        }
        private bool isTextBoxNull()
        {
            if (textBox1.Text == "" | textBox2.Text == "" | textBox3.Text == "")
                return true;
            else
                return false;

        }
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (TextBoxChange)
            {
                if (MessageBox.Show("要保存刚才的更改吗？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    button3_Click(null, null);//“确定”按钮
                }
            }
            NotChangeTextBox = true;//阻止文本框的textchanged事件
            textBox1.Text = (intervals[0] / 1000).ToString();//显示每次点击间隔时间的textbox，转换为秒
            textBox2.Text = (intervals[1] / 1000).ToString();
            textBox3.Text = (intervals[2] / 1000).ToString();
            NotChangeTextBox = false;
            TextBoxChange = false;


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!NotChangeTextBox)
            {
                string txt = textBox1.Text;
                string done = txt;
                int i = txt.Length;
                button4.Enabled = true;//"重置"按钮
                button3.Enabled = true;
                if (i < 1) return;
                for (int m = 0; m < i; m++)
                {
                    string str = txt.Substring(m, 1);
                    if (!char.IsNumber(Convert.ToChar(str)))
                    {
                        done = done.Replace(str, ""); //将非数字文本过滤掉
                    }
                }
                textBox1.Text = done;
                textBox1.SelectionStart = textBox1.Text.Length;//将光标定位到最后一位
                try
                {
                    if (Convert.ToInt32(textBox1.Text) == 0)
                    {
                        textBox1.Text = "";
                        button3.Enabled = false;
                    }

                }
                catch { }
                try { TextBoxChange = true; }
                catch { }

            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!NotChangeTextBox)
            {
                string txt = textBox2.Text;
                string done = txt;
                button4.Enabled = true;
                button3.Enabled = true;
                int i = txt.Length;
                if (i < 1) return;
                for (int m = 0; m < i; m++)
                {
                    string str = txt.Substring(m, 1);
                    if (!char.IsNumber(Convert.ToChar(str)))
                    {
                        done = done.Replace(str, ""); //将非数字文本过滤掉                   
                    }
                }
                textBox2.Text = done;
                textBox2.SelectionStart = textBox2.Text.Length;//将光标定位到最后一位
                try
                {
                    if (Convert.ToInt32(textBox2.Text) == 0)
                    {
                        textBox2.Text = "";
                        button3.Enabled = false;
                    }

                }
                catch { }
                try { TextBoxChange = true; }
                catch { }

            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!NotChangeTextBox)
            {
                string txt = textBox3.Text;
                string done = txt;
                button4.Enabled = true;
                button3.Enabled = true;
                int i = txt.Length;
                if (i < 1) return;
                for (int m = 0; m < i; m++)
                {
                    string str = txt.Substring(m, 1);
                    if (!char.IsNumber(Convert.ToChar(str)))
                    {
                        done = done.Replace(str, ""); //将非数字文本过滤掉


                    }
                }
                textBox3.Text = done;
                textBox3.SelectionStart = textBox3.Text.Length;//将光标定位到最后一位
                try
                {
                    if (Convert.ToInt32(textBox3.Text) == 0)
                    {
                        textBox3.Text = "";
                        button3.Enabled = false;
                    }
                }
                catch { }
                try { TextBoxChange = true; }
                catch { }

            }
        }


        private void tabControl1_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon1_MouseDoubleClick(null, null);
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon1_MouseDoubleClick(null, null);
        }

        private void label6_DoubleClick(object sender, EventArgs e) //坐标1
        {
            notifyIcon1_MouseDoubleClick(null, null);
        }

        private void tabControl1_DoubleClick_1(object sender, EventArgs e)
        {
            notifyIcon1_MouseDoubleClick(null, null);
        }

        private void label26_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon1_MouseDoubleClick(null, null);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if(web.ReadyState == WebBrowserReadyState.Complete)
            {
                System.Windows.Forms.HtmlDocument document = this.web.Document;
                document.Window.ScrollTo(320, 1020);
            }
        }

        private void parsePage()
        {
            HtmlElementCollection collection = web.Document.GetElementsByTagName("div");

            foreach (HtmlElement element in collection)
            {
                //获取到总评论数据
                if (element.GetAttribute("className").Equals("score-container"))
                {
                    string revCountStr = "";
                    revCountStr += element.OuterHtml.ToString();
                    string dstNum = parseReviewNum(revCountStr);
                    if (dstNum != String.Empty)
                    {
                        reviewNumCount = int.Parse(dstNum); 
                    }
                    
                }
                //获取到评论的div
                if (element.GetAttribute("className").Equals("single-review") ||
                   element.GetAttribute("className").Equals("featured-review clickable"))          //获取评论
                {
                    string reviewContent = "";
                    foreach (HtmlElement reviewElem in element.Children)
                    {
                        if (reviewElem.GetAttribute("className").Equals("review-header"))   //评论头部
                        {
                            reviewContent += reviewElem.OuterHtml.ToString();
                            //Console.WriteLine("评论-HEADER" + reviewElem.InnerHtml.ToString() + "\n");
                        }
                        if (reviewElem.GetAttribute("className").Equals("review-body"))     //评论尾部
                        {
                            //Console.WriteLine("评论-BODY:" + reviewElem.InnerHtml.ToString() + "\n");
                            reviewContent += reviewElem.OuterHtml.ToString()+"\n";
                        }
                    }
                    if (reviewContent != String.Empty)
                    {
                        
                        string resultStr = parseHtml(reviewContent);
                        char[] delima = { '\t' };
                        string prefixName = resultStr.Split(delima)[0];

                        if (!reviewDic.ContainsKey(prefixName))
                        {
                            reviewDic.Add(prefixName,resultStr);
                        }
                        //save2File(resultStr);
                    }
                    reviewContent = "";

                    if (reviewNumCount == 0)                                  //评论总数目为0，跳转到其他页面     
                    {

                        string tempStr = web.Url.ToString();

                        string destName = parseAppID(tempStr);
                        //没有人评论，评论为0，写入0
                        string str2txt  ="0";
                        save2File(str2txt, destName);
                        //清空评论字典集，总评论数
                        reviewDic.Clear();
                        reviewNumCount = 0;
                        //设置为已经爬取网页
                        mongoDB.UpdateIsCrawled(urlLinkList.First());
                        //从LinkList中删除该项
                        urlLinkList.RemoveFirst();
                        //跳转到其他页面web.Navigate();                       
                        web.Navigate(BuildUrlString(urlLinkList.First()));
                    }
                    else if (reviewNumCount <= 100)
                    {
                        string str2txt = "";
                        foreach (KeyValuePair<string, string> item in reviewDic)
                        {
                            str2txt += item.Key.ToString() + "\t" + item.Value.ToString();
                        }
                        //保存到本地文件内
                        string tempStr = web.Url.ToString();

                        string destName = parseAppID(tempStr);

                        save2File(str2txt, destName);

                        //清空Dic字典内的记录
                        reviewDic.Clear();
                        reviewNumCount = 0;

                        //设置为已经爬取网页
                        mongoDB.UpdateIsCrawled(urlLinkList.First());
                        //从LinkList中删除该项
                        urlLinkList.RemoveFirst();
                        //跳转到其他页面web.Navigate();                       
                        web.Navigate(BuildUrlString(urlLinkList.First()));

                    }
                    else if (reviewNumCount > 100 && reviewNumCount <= 200 && reviewDic.Count == 40)
                    {
                        string str2txt = "";
                        foreach (KeyValuePair<string, string> item in reviewDic)
                        {
                            str2txt += item.Key.ToString() + "\t" + item.Value.ToString();
                        }
                        //保存到本地文件内
                        string tempStr = web.Url.ToString();

                        string destName = parseAppID(tempStr);

                        save2File(str2txt, destName);

                        //清空Dic字典内的记录
                        reviewDic.Clear();
                        reviewNumCount = 0;

                        //设置为已经爬取网页
                        mongoDB.UpdateIsCrawled(urlLinkList.First());
                        //从LinkList中删除该项
                        urlLinkList.RemoveFirst();
                        //跳转到其他页面web.Navigate();                       
                        web.Navigate(BuildUrlString(urlLinkList.First()));
                    }
                    else if (reviewDic.Count >= 200 && reviewNumCount >= 200) //评论总数目大于200，就获取200个
                    {
                        string str2txt = "";
                        foreach (KeyValuePair<string,string> item in reviewDic)
                        {
                            str2txt += item.Key.ToString()+"\t" + item.Value.ToString();
                        }
                        //保存到本地文件内
                        string tempStr = web.Url.ToString();

                        string destName = parseAppID(tempStr);

                        save2File(str2txt, destName);
              
                        //清空Dic字典内的记录
                        reviewDic.Clear();
                        reviewNumCount = 0;
                        //设置为已经爬取网页
                        mongoDB.UpdateIsCrawled(urlLinkList.First());
                        //从LinkList中删除该项
                        urlLinkList.RemoveFirst();
                        //跳转到下一个页面web.Navigate();                       
                        web.Navigate(BuildUrlString(urlLinkList.First()));

                    }
                   
                }
            }//foreach
 
        }
        /**
         * 解析html文本 利用HtmlAgilityPack与 XPath
         * 
         */ 
        private string parseHtml(string html)
        {
            string resstr = "";
            if(html == String.Empty)
            {
                return null;
            }
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            HtmlAgilityPack.HtmlNode nod = doc.DocumentNode;

            //评论者姓名
            HtmlAgilityPack.HtmlNode revPersonNameNod = nod.SelectSingleNode(Consts.REVIEW_PERSON_NAME);
            //评论者的主页链接
            HtmlAgilityPack.HtmlNode revPersonPageNod = nod.SelectSingleNode(Consts.REVIEW_PERSON_HMPG);
            //评论时间
            HtmlAgilityPack.HtmlNode revTimeNod = nod.SelectSingleNode(Consts.REVIEW_TIME);
            //评论唯一ID
            HtmlAgilityPack.HtmlNode revIDNod = nod.SelectNodes(Consts.REVIEW_UINIID)[1];
            //用户评分 百分制：20%
            HtmlAgilityPack.HtmlNode ratingNod = nod.SelectSingleNode(Consts.REVIEW_RATING);
            //用户品论内容
            HtmlAgilityPack.HtmlNode ratingContent = nod.SelectSingleNode(Consts.REVIEW_RATINGCONTENT);
            //y用户评论title
            HtmlAgilityPack.HtmlNode ratingContentTitle = nod.SelectSingleNode(Consts.REVIEW_RATINGTITLE);
            
            string revPersonName = null;
            string revPersonPageLinkStr = null;
            string revPersonUnnId = null;
            string revTimer = null;
            string revRatingVal = null;
            string revRatingContentTitle = null;
            string revRatingContent = null;

            if(revPersonNameNod != null)
            {
                revPersonName = revPersonNameNod.InnerHtml;
            }
            //string revPersonName = revPersonNameNod.InnerHtml;
            if (revPersonPageNod != null)
            {
                revPersonPageLinkStr = revPersonPageNod.Attributes["href"].Value.ToString();
            }
            if (revIDNod != null)
            {
                revPersonUnnId = revIDNod.Attributes["href"].Value.ToString();//

            }
            if (revTimeNod != null)
            {
                revTimer = revTimeNod.InnerHtml;
            }
            if (ratingNod != null)
            {
                 revRatingVal = ratingNod.Attributes["style"].Value.ToString();
            }
            string revRatingStr = splitOutRatingVal(revRatingVal);
            if (ratingContentTitle != null)
            {
                 revRatingContentTitle = ratingContentTitle.InnerHtml;
            }

            if (ratingContent != null)
            {
                 revRatingContent = ratingContent.InnerText;
            }
            return resstr = revPersonName + "\t " + revPersonPageLinkStr + " \t" + revTimer + "\t " + revPersonUnnId + "\t" + revRatingStr + "\t" + revRatingContentTitle + "\t" + revRatingContent + "\n";
        }

        private string parseReviewNum(string reviewhtmlstr)
        {
            if (reviewhtmlstr == null)
            {
                return null;
            }
            string revNum = "";

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(reviewhtmlstr);
            HtmlAgilityPack.HtmlNode nod = doc.DocumentNode;

            HtmlAgilityPack.HtmlNode revIDNod = nod.SelectNodes("//div/meta")[1];
            revNum = revIDNod.Attributes["content"].Value.ToString();
            Console.WriteLine(revIDNod.ToString());
            return revNum;
        }
        private void save2File(string reviewContent,string filename)
        {
            string outFileName = "./" +"crawledReview/"+ filename + ".txt";
            FileStream fs = new FileStream(outFileName, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(reviewContent);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        private string splitOutRatingVal(string str)
        {
            char[] delimiterChar = { ':' };
            string[] words = str.Split(delimiterChar);
            if(words.Length != 2)
            {
                return null;
            }
            else
            {
                return words[1].Replace(" ", "");
            }
        }

        private string parseAppID(string url)
        {
            string destStr = null;
            int token = url.IndexOf("=");
            token += 1;
            destStr = url.Substring(token, url.Length - token);
            return destStr;
        }

        //build the url string
        private string BuildUrlString(string id)
        {
            return Consts.ORIGIN + id;
        }
    }

        
}