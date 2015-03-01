using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    // Serialization Models for App Data to be stored on MongoDB
    public class AppModel
    {
        public ObjectId _id                {get;set;}
        public string   Url                {get;set;}
        public string   Name               {get;set;}
        public string   Developer          {get;set;}
        public bool     IsTopDeveloper     {get;set;}
        public string   DeveloperURL       {get;set;}
        public DateTime PublicationDate    {get;set;}
        public string   Category           {get;set;}
        public bool     IsFree             {get;set;}
        public double   Price              {get;set;}
        public string   Reviewers          {get;set;}
        public string   CoverImgUrl        {get;set;}
        public string   Description        {get;set;}
        public Score    Score              {get;set;}
        public DateTime LastUpdateDate     {get;set;}
        public Double   AppSize            {get;set;}
        public string   Instalations       {get;set;}
        public string   CurrentVersion     {get;set;}
        public string   MinimumOSVersion   {get;set;}
        public string   ContentRating      {get;set;}
        public bool     HaveInAppPurchases {get;set;}
    }

    public class Score
    {
        public double Total      {get;set;}
        public double Count      {get;set;}
        public double FiveStars  {get;set;}
        public double FourStars  {get;set;}
        public double ThreeStars {get;set;}
        public double TwoStars   {get;set;}
        public double OneStars   {get;set;}
    }
}
