using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.MongoDB
{
    public class MongoDbContext
    {
        #region ** Attributes **

        static string m_baseConnectionString = String.Empty;
        static string m_login = null;
        static string m_password = null;
        static MongoServer m_server = null;

        #endregion

        #region ** Core MongoDB Methods **

        /// <summary>
        /// Configures the specified login.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        /// <param name="addresses">List of addresses. Format: host1[:port1][,host2[:port2],...[,hostN[:portN]]]</param>
        /// <param name="safeMode">The safe mode. True to receive write confirmation from mongo.</param>
        /// <param name="readOnSecondary">The read on secondary. True to direct read operations to cluster secondary nodes (secondaryPreferred), else try to read from primary (primaryPreferred).</param>
        /// <param name="connectionTimeoutMilliseconds">The time to attempt a connection before timing out.</param>
        /// <param name="socketTimeoutMilliseconds">The time to attempt a send or receive on a socket before the attempt times out.</param>
        /// <param name="isReplicaSet">The is replica set hint.</param>
        /// <param name="readPreferenceTags">The read preference tags. List of a comma-separated list of colon-separated key-value pairs. Ex.: { {dc:ny,rack:1}, { dc:ny } } </param>
        public static void Configure (string login, string password, string authSrc, string addresses, bool safeMode = true, bool readOnSecondary = false, int connectionTimeoutMilliseconds = 60000, int socketTimeoutMilliseconds = 60000, IEnumerable<string> readPreferenceTags = null)
        {
            m_login = login;
            m_password = password;
            // prepares the connection string
            string connectionString = BuildConnectionString (login, password, authSrc, safeMode, readOnSecondary, addresses, connectionTimeoutMilliseconds, socketTimeoutMilliseconds, /*isReplicaSet, */ readPreferenceTags);
            // if there is any change, reconnect
            if (m_baseConnectionString != connectionString)
            {
                m_baseConnectionString = connectionString;
                Dispose ();                
            }
        }
          
        /// <summary>
        /// Releases MongoDb resources.<para/>
        /// Obs.: the last Connection String is kept, so if any request is made, the resources will be recreated and the connection opened. 
        /// </summary>
        public static void Dispose ()
        {            
            m_server = null;
            m_docStore = new Lazy<MongoClient> (OpenConnection);
        }
  
        /// <summary>
        /// Forcefully closes all connections from the connection pool.<para/>
        /// Not recommended, since can cause some connection instability issues.
        /// </summary>
        public static void ForceDisconnect ()
        {
            if (m_server != null)
            {
                m_server.Disconnect ();                
            }
        }

        /// <summary>
        /// Builds the connection string.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        /// <param name="safeMode">The safe mode. True to receive write confirmation from mongo.</param>
        /// <param name="readOnSecondary">The read on secondary. True to direct read operations to cluster secondary nodes (secondaryPreferred), else try to read from primary (primaryPreferred).</param>
        /// <param name="addresses">List of addresses. Format: host1[:port1][,host2[:port2],...[,hostN[:portN]]]</param>
        /// <param name="connectionTimeoutMilliseconds">The time to attempt a connection before timing out.</param>
        /// <param name="socketTimeoutMilliseconds">The time to attempt a send or receive on a socket before the attempt times out.</param>
        /// <param name="isReplicaSet">The is replica set hint.</param>
        /// <param name="readPreferenceTags">The read preference tags. List of a comma-separated list of colon-separated key-value pairs. Ex.: { {dc:ny,rack:1}, { dc:ny } } </param>
        public static string BuildConnectionString (string login, string password, string authSrc, bool safeMode, bool readOnSecondary, string addresses, int connectionTimeoutMilliseconds, int socketTimeoutMilliseconds,  IEnumerable<string> readPreferenceTags = null)
        {
            StringBuilder sb = new StringBuilder ("mongodb://", 230);
            // check credentials
            if (!String.IsNullOrWhiteSpace (login) && !String.IsNullOrWhiteSpace (password))
            {
                //Optional para when do not need the login and password
                //sb.Append (login).Append (':').Append (password).Append ('@');    
            }

            // address
            sb.Append (addresses).Append ("/?");

            if (!String.IsNullOrEmpty(authSrc))
            {
                sb.Append("authSource=").Append (authSrc).Append("&");
            }

            // options
            //if (isReplicaSet)
            //    sb.Append ("connect=replicaset&");
            if (safeMode)
                sb.Append ("w=1&");
            else
                sb.Append ("w=0&");
            if (readOnSecondary)
            {
                sb.Append ("readPreference=secondaryPreferred&");
            }
            else
            {
                sb.Append ("readPreference=primaryPreferred&");
            }
            if (readPreferenceTags != null)
            {
                foreach (var tag in readPreferenceTags)
                {
                    if (String.IsNullOrWhiteSpace (tag))
                        continue;
                    sb.Append ("readPreferenceTags=").Append (tag.Trim ().Replace (' ', '_')).Append ('&');
                }
            }

            sb.Append ("connectTimeoutMS=").Append (connectionTimeoutMilliseconds).Append ("&socketTimeoutMS=").Append (socketTimeoutMilliseconds);

            // generate final connection string
            return sb.ToString ();
        }

        private static Lazy<MongoClient> m_docStore = new Lazy<MongoClient> (OpenConnection);

        static DateTimeSerializer _dateTimeSerializer = null;

        private static MongoClient OpenConnection ()
        {
            // create mongo client
            MongoClient client = new MongoClient (m_baseConnectionString);
            return client;
        }

        public static MongoClient Client
        {
            get { return m_docStore.Value; }
        }

        public static MongoServer Server
        {
            get
            {
                if (m_server == null)
                    m_server = Client.GetServer ();
                return m_server;
            }
        }

        public static MongoDatabase GetDatabase (string dbName)
        {
            return Server.GetDatabase (dbName);
        }

        #endregion
    }

	/// <summary>
    /// Some MongoDb helpers methods
    /// </summary>
    public static class MongoExtensions
    {
        /// <summary>
        /// Insert an item, retrying in case of connection errors..
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="col">The collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="retryCount">The retry count in case of connection errors.</param>
        /// <param name="throwOnError">Throws an exception on error.</param>
        /// <returns></returns>
        public static bool SafeInsert<T> (this MongoCollection col, T item, int retryCount = 2, bool throwOnError = false)
        {
            int done = 0;
            // try to update/insert and 
            // retry n times in case of connection errors            
            do
            {
                try
                {
                    if (col.Insert (item).Ok)
                        return true;
                }
                catch (System.IO.IOException ex)
                {
                    // retry limit
                    if (throwOnError && done > (retryCount - 1))
                        throw;
                }
                catch (WriteConcernException wcEx)
                {
                    // duplicate id exception (no use to retry)
                    if (wcEx.CommandResult != null && (wcEx.CommandResult.Code ?? 0) == 11000)
                    {
                        if (throwOnError)
                            throw;
                        else
                            return false;
                    }
                    // retry limit
                    if (throwOnError && done > (retryCount - 1))
                        throw;
                }
            }
            while (++done < retryCount);
            // if we got here, the operation have failled
            return false;
        }

        /// <summary>
        /// Update or insert an item, retrying in case of connection errors.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="col">The collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="retryCount">The retry count in case of connection errors.</param>
        /// <param name="throwOnError">Throws an exception on error.</param>
        /// <returns></returns>
        public static bool SafeSave<T> (this MongoCollection col, T item, int retryCount = 2, bool throwOnError = false)
        {
            int done = 0;
            // try to update/insert and 
            // retry n times in case of connection errors
            do
            {
                try
                {
                    if (col.Save (item).Ok) 
                        return true;
                }
                catch (System.IO.IOException ex)
                {
                    // retry limit
                    if (throwOnError && done > (retryCount - 1))
                        throw;
                }
                catch (WriteConcernException wcEx)
                {                    
                    // retry limit
                    if (throwOnError && done > (retryCount - 1))
                        throw;
                }
            }
            while (++done < retryCount);
            // if we got here, the operation have failled
            return false;
        }

        /// <summary>
        /// Execute insert batch, retrying in case of connection errors.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="col">The collection</param>
        /// <param name="items">list of items.</param>
        /// <param name="retryCount">The retry count in case of connection errors.</param>
        /// <param name="throwOnError">Throws an exception on error.</param>
        /// <returns></returns>
        public static bool SafeInsertBatch<T> (this MongoCollection col, IList<T> items, int retryCount = 2, bool throwOnError = false)
        {
            // try to insertbatch
            try
            {
                col.InsertBatch (items);
            }
            catch (Exception ex)
            {
                // in case of a insertbatch faillure, 
                // update or insert each item individually
                for (int i = 0; i < items.Count; i++)
                {
                    if (!col.SafeSave (items[i], retryCount, throwOnError))
                    {
                        // in case of another faillure, give up saving items
                        return false;                        
                    }
                }
            }
            return true;
        }
    }
}
