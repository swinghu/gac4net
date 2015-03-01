//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// NameSpace : Txt2MongoBsonDB.Models
// FileName : UserModel
//
// Created by : swinghu (ogrecpp@gamil.com) at 2014/5/20 20:54:04
//
// Function : The User page model,User Info
//
//========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;



namespace ShareLibrary.Models
{
    public class UserModel
    {
        public ObjectId _id { get; set; }
        public string UserName { get; set; }
        public string UserUrl { get; set; }
        public string UserIamgeUrl { get; set; }
        public string ReviewCount { get; set; }

        public Boolean iscrawled { get; set; }
        public UserRvwAppModel[] ReivewsApp { get; set; }

        public int getReviewAppCount()
        {
            return this.ReivewsApp.Length;
        }
    }
}
