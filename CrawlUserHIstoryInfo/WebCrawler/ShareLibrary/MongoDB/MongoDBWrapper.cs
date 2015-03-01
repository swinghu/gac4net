using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShareLibrary.Models;
using ShareLibrary;



namespace ShareLibrary.MongoDB
{
    public class MongoDBWrapper
    {
        #region ** Private Attributes **

        private string           _connString;
        private string           _collectionName;
        private MongoServer      _server;
        private MongoDatabase    _database;

        private string           _entity;

        #endregion

        /// <summary>
        /// Executes the configuration needed in order to start using MongoDB
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">User Password</param>
        /// <param name="authSrc">Database used as Authentication Provider. Default is 'admin'</param>
        /// <param name="serverAddress">IP Address of the DB Server. Format = ip:port</param>
        /// <param name="timeout">Connection Timeout</param>
        /// <param name="databaseName">Database Name</param>
        /// <param name="collectionName">Collection Name</param>
        /// <param name="entity">Not Used. Use Empty</param>
        /// 


        public void ConfigureDatabase (string username, string password, string authSrc, string serverAddress, int timeout, string databaseName, string collectionName, string entity = "")
        {
            // Reading App Config for config data            
            _connString     = MongoDbContext.BuildConnectionString (username, password, authSrc, true, false, serverAddress, timeout, timeout);                        
            _server         = new MongoClient (_connString).GetServer ();
            _database       = _server.GetDatabase (databaseName);
            _collectionName = collectionName;
            _entity         = entity;            
        }

        /// <summary>
        /// Inserts an element to the MongoDB.
        /// The type T must match the type of the target collection
        /// </summary>
        /// <typeparam name="T">Type of the object to be inserted</typeparam>
        /// <param name="record">Record that will be inserted in the database</param>
        public bool Insert<T> (T record)
        {
            return _database.GetCollection<T> (_collectionName).SafeInsert (record);
        }

        /// <summary>
        /// Judge the record whether is in the db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <param name="appUrl"></param>
        /// <returns></returns>
        public bool IsExitInDB<T>(T record,string appUrl)
        {
            var mongoQuery = Query.EQ("Url", appUrl);
            var queryResponse = _database.GetCollection<T>(_collectionName).FindOne(mongoQuery);
            return queryResponse == null? false: true;
        }

        /// <summary>
        /// If the AppReviewModel is existed in the db then only update the the Reviews
        /// A perffect function,Good Job,
        /// </summary>
        /// <param name="record"></param>
        /// <param name="appUrl"></param>
        /// <returns>bool</returns>
        public bool UpdateReviewsArr(AppReviewModel record,string appUrl)
        {
            //if the app is exit in the db ,then update the Reviews
            //update the ReviewCount
            var mongoQuery = Query.EQ("Url", appUrl);

            var updateStatement = Update.AddToSetEachWrapped<PerPersonReviewModel>("Reviews", record.Reviews);

            var updateResponse = _database.GetCollection<AppReviewModel>(_collectionName).FindAndModify(mongoQuery, null, updateStatement, false);
           
            return updateResponse == null? false : true;
        }

        /// <summary>
        /// Update the review count of the app
        /// </summary>
        /// <param name="record"></param>
        /// <param name="appUrl"></param>
        /// <returns></returns>
        public bool UpdateReviewCount(AppReviewModel record, string appUrl)
        {
            var mongoQuery = Query.EQ( "Url", appUrl);

            //fetch the app's(base on the appUrl) reviewCount
            var  responseFindOne = _database.GetCollection<AppReviewModel>(_collectionName).FindOne(mongoQuery);

            //AppReviewModel modle = bs;
            int count = responseFindOne.count();
            
            var updateStatement = Update.Set("ReviewCount",Convert.ToString(count));
            //update the review count!
            var response = _database.GetCollection<AppReviewModel>(_collectionName).FindAndModify(mongoQuery, null, updateStatement);

            return response == null? false:true ;
        }


        public MongoCursor<UserModel> QueryUnCrawledUser()
        {
            var mongoQuery = Query.EQ("iscrawled",true);

            var mongoResponseCursor = _database.GetCollection<UserModel>(Consts.MONGO_COLLECTION).FindAs<UserModel>(mongoQuery);

            return mongoResponseCursor == null ? null : mongoResponseCursor;
        }

        public void UpdateIsCrawled(string url)
        {
            var updateStatement = Update.Set("iscrawled", false);

            var mongoQuery = Query.EQ("UserUrl", url);

            _database.GetCollection<UserModel>(Consts.MONGO_COLLECTION).Update(mongoQuery, updateStatement);
        }
        /// <summary>
        /// Change the Array to BsonArray
        /// TO-DO something else ,Not be used in this project
        /// </summary>
        /// <param name="reviewArr"></param>
        /// <returns>BsonArray</returns>
        public static BsonArray ToBsonDocumentArray (AppReviewModel reviewArr)
        {
            var array = new BsonArray();

            foreach (var item in reviewArr.Reviews)
            {
                array.Add(item.ToBson());
            }
            return array;
        }
       
    }
}
