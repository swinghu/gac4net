//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// NameSpace : Txt2MongoBsonDB.Models
// FileName : AppReviewModel
//
// Created by : swinghu (ogrecpp@gamil.com) at 2014/5/20 20:46:08
//
// Function : Modle for App reviews 
//
//========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ShareLibrary.Models
{
    public class AppReviewModel
    {

        /// MongoDB _id
        public ObjectId                   _id { get; set; }
        /// App url:/store/apps/details?id=com.jazzmoonstudio.android.buspredictions
        public string                    Url { get; set; }
        // Review Count
        public string                    ReviewCount { get; set; }
        /// app name
        public string                    AppName { get; set; }
        // Array of user reviews
        public PerPersonReviewModel[]    Reviews { get; set; }

        public  int count()
        {
            return this.Reviews.Length;
        }
    }
}
