//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// NameSpace : Txt2MongoBsonDB.Models
// FileName : PerPersonReview
//
// Created by : swinghu (ogrecpp@gamil.com) at 2014/5/20 21:05:06
//
// Function : 当个用户的评论信息
//
//========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ShareLibrary.Models
{
    public class PerPersonReviewModel
    {
   
        //评论用户的名字
        public string UserName { get; set; }
        //用户的唯一googleID
        public string UserUrl { get; set; }
        //评论的app url
        public string AppUrl { get; set; }
        //评论唯一ID
        public string ReviewID { get; set; }
        //评论发生时间
        public string ReviewTime { get; set; }
        //评分
        public string Rating { get; set; }
        //评论的title ：Rating Title
        public string RatingTitle { get; set; }
        //评论内容
        public string RatingContent { get; set; }

    }
}
