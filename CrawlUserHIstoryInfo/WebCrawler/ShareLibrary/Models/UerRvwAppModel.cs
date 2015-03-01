//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// NameSpace : Txt2MongoBsonDB.Models
// FileName : ReviewAppModel
//
// Created by : swinghu (ogrecpp@gamil.com) at 2014/5/20 22:00:31
//
// Function : 评论用户页面中App model
//
//========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLibrary.Models
{
    public class UserRvwAppModel
    {
        public string UserName { get; set; }
        public string UserUrl { get; set; }
        public string AppUrl { get; set; }
        public string Rating { get; set; }

    }
}
