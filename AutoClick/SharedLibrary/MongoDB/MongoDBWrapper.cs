using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;
using System.Collections;

namespace SharedLibrary.MongoDB
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
        /// Checks whether the received app url is on the queue collection
        /// to be processed or not
        /// </summary>
        /// <param name="appUrl">Url of the app</param>
        /// <returns>True if it is on the queue collection, false otherwise</returns>
        public bool AppQueued (string appUrl)
        {
            var mongoQuery = Query.EQ ("Url", appUrl);

            var queryResponse = _database.GetCollection<QueuedApp> (Consts.QUEUED_APPS_COLLECTION).FindOne (mongoQuery);

            return queryResponse == null ? false : true;
        }

        /// <summary>
        /// Checks whether the received URL is on the queue to be processed already
        /// or not
        /// </summary>
        /// <param name="appUrl">URL (Key for the search)</param>
        /// <returns>True if the app is on the queue collection, false otherwise</returns>
        public bool IsAppOnQueue (string appUrl)
        {
            var mongoQuery    = Query.EQ ("Url", appUrl);

            var queryResponse = _database.GetCollection<QueuedApp> (Consts.QUEUED_APPS_COLLECTION).FindOne (mongoQuery);

            return queryResponse == null ? false : true;
        }

        /// <summary>
        /// Adds the received url to the collection
        /// of queued apps
        /// </summary>
        /// <param name="appUrl">Url of the app</param>
        /// <returns>Operation status. True if worked, false otherwise</returns>
        public bool AddToQueue (string appUrl)
        {
            return _database.GetCollection<QueuedApp>(Consts.QUEUED_APPS_COLLECTION).SafeInsert(new QueuedApp { Url = appUrl, IsBusy = false, isReviewCrawled=false});
        }

        /// <summary>
        /// Finds an app that is "Not Busy" and modifies it's status
        /// to "Busy" atomically so that no other worker will try to process it
        /// on the same time
        /// </summary>
        /// <returns>Found app, if any</returns>
        public QueuedApp FindAndModify ()
        {
            // Mongo Query
            var mongoQuery      = Query.EQ ("IsBusy", false);
            var updateStatement = Update.Set ("IsBusy", true);

            // Finding a Not Busy App, and updating its state to busy
            var mongoResponse = _database.GetCollection<QueuedApp> (Consts.QUEUED_APPS_COLLECTION).FindAndModify (mongoQuery, null, updateStatement, false);

            // Checking for query error or no app found
            if (mongoResponse == null || mongoResponse.Response == null)
            {
                return null;
            }

            // Returns the app
            return BsonSerializer.Deserialize<QueuedApp> (mongoResponse.ModifiedDocument);
        }

        public MongoCursor<QueuedApp> QueryUnCrawledApp()
        {
            //Mongo Query
            var mongoQuery = Query.EQ("isReviewCrawled", "0");
            
            var mongoResponsCursor = _database.GetCollection<QueuedApp>(Consts.QUEUED_APPS_COLLECTION).FindAs<QueuedApp>(mongoQuery);
           
            if (mongoResponsCursor == null)
            {
                return null;
            }
            //Console.WriteLine(mongoResponsCursor.Size());
            //foreach (var QueueApp in mongoResponsCursor)
            //{
            //    Console.WriteLine(QueueApp.Url);
            //}
            return mongoResponsCursor;
        }

        public void UpdateIsCrawled(string url)
        {
            var updateStatement = Update.Set("isReviewCrawled","1");
            var mongoQuery = Query.EQ("Url", url);
            _database.GetCollection<QueuedApp>(Consts.QUEUED_APPS_COLLECTION).Update(mongoQuery,updateStatement);
        }

        /// <summary>
        /// Removes the received app from the collection
        /// of queued apps
        /// </summary>
        /// <param name="url">App document to be removed</param>
        public void RemoveFromQueue (string url)
        {
            var mongoQuery = Query.EQ ("Url", url);
            _database.GetCollection<QueuedApp> (Consts.QUEUED_APPS_COLLECTION).Remove (mongoQuery);
        }

        public QueuedApp getAppUrl()
        {
            var mongoQuery = Query.EQ("isReviewCrawled", false);
            return _database.GetCollection<QueuedApp>(Consts.QUEUED_APPS_COLLECTION).(mongoQuery);
        }
    }
}
