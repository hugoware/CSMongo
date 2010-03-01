using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CSMongo.Bson;
using CSMongo.Exceptions;
using CSMongo.Requests;
using CSMongo.Responses;
using CSMongo.Results;

namespace CSMongo.Commands {

    /// <summary>
    /// Collection of database related commands
    /// </summary>
    public static class MongoDatabaseCommands {

        #region General Commands

        /// <summary>
        /// Performs a manual log our for a connection
        /// </summary>
        public static void LogOut(MongoDatabase database) {
            try {
                MongoDatabaseCommands.RunCommand(database, new { logout = Mongo.CommandArgument });
            }
            //intentional -- ignore errors
            //I think this is a good idea...
            catch { }
        }

        /// <summary>
        /// Handles logging into the specified database
        /// </summary>
        public static MethodResult Authenticate(MongoDatabase database, string username, string password) {

            //open the connection to the database
            database.Connection.Open();

            //get the values required
            GetNonceResult nonce = MongoDatabaseCommands.GetNonce(database);
            if (!nonce.HasNonce) {
                throw new LameException("Missing nonce from database!");
            }

            //issue the actual command
            string key = MongoDatabaseCommands.HashPassword(username, password, nonce.Nonce);
            CommandResponse result = MongoDatabaseCommands.RunCommand(
                database,
                new {
                    authenticate = Mongo.CommandArgument,
                    user = username,
                    nonce = nonce.Nonce,
                    key = key
                });

            //Do not close the connection at this point
            //since we need access to keep the connection 
            //alive
            return new MethodResult(result.GetDefaultResponse());

        }

        /// <summary>
        /// Loads the 'nonce' value for password hashing
        /// </summary>
        public static GetNonceResult GetNonce(MongoDatabase database) {
            CommandResponse response = MongoDatabaseCommands.RunCommand(database, new { getnonce = Mongo.CommandArgument });
            return new GetNonceResult(response.GetDefaultResponse());
        }

        /// <summary>
        /// Sends the request to kill existing cursors
        /// </summary>
        public static void KillCursors(MongoDatabase database, IEnumerable<MongoCursor> cursors) {
            MongoDatabaseCommands.KillCursors(database, cursors.Where(item => item.Cursor > 0).Select(item => item.Cursor));
        }

        /// <summary>
        /// Sends the request to kill existing cursors
        /// </summary>
        public static void KillCursors(MongoDatabase database, IEnumerable<long> cursors) {
            
            //give up on an empty cursor count
            if (cursors.Count() == 0) { return; }

            //send the command to work
            KillCursorsRequest request = new KillCursorsRequest(cursors);
            QueryResponse response = database.SendRequest(request) as QueryResponse;

        }

        /// <summary>
        /// Causes the Mongo server to shutdown - No response is expected
        /// </summary>
        public static MethodResult ResetErrors(MongoDatabase database) {
            CommandResponse response = MongoDatabaseCommands.RunCommand(database, new { reseterror = Mongo.CommandArgument });
            return new MethodResult(response.GetDefaultResponse());
        }

        /// <summary>
        /// Gets the most recent error from the database
        /// </summary>
        public static GetLastErrorResult GetLastError(MongoDatabase database) {
            CommandResponse response = MongoDatabaseCommands.RunCommand(database, new { getlasterror = Mongo.CommandArgument });
            return new GetLastErrorResult(response.GetDefaultResponse());
        }

        /// <summary>
        /// Deletes a database from the server
        /// </summary>
        public static MethodResult DropDatabase(MongoDatabase database) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { dropDatabase = Mongo.CommandArgument });
            return new MethodResult(result.GetDefaultResponse());
        }

        /// <summary>
        /// Attempts to repair the current database
        /// </summary>
        public static MethodResult RepairDatabase(MongoDatabase database, bool preserveClonedOnFailure, bool backupOriginal) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { repairDatabase = Mongo.CommandArgument, preserveClonedFilesOnFailure = preserveClonedOnFailure, backupOriginalFiles = backupOriginal });
            return new MethodResult(result.GetDefaultResponse());
        }

        /// <summary>
        /// Returns assert info for the database ??
        /// </summary>
        public static MethodResult GetAssertInfo(MongoDatabase database) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { assertinfo = Mongo.DefaultAdminDatabase });
            return new MethodResult(result.GetDefaultResponse());
        }

        /// <summary>
        /// Returns OpTime for the provided database
        /// </summary>
        public static GetOpTimeResult GetOpTime(MongoDatabase database) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { getoptime = Mongo.CommandArgument });
            return new GetOpTimeResult(result.GetDefaultResponse());
        }

        /// <summary>
        /// Clones the current database from the provided host to this server
        /// </summary>
        public static MethodResult Clone(MongoDatabase database, string host) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { clone = host });
            return new MethodResult(result.GetDefaultResponse());
        }

        /// <summary>
        /// Sets and returns the previous profiling level
        /// </summary>
        public static ProfileResult SetProfileLevel(MongoDatabase database, int level) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { profile = level });
            return new ProfileResult(result.GetDefaultResponse());
        }

        /// <summary>
        /// Returns the count of records from the specified collection
        /// </summary>
        public static CollectionCountResult CollectionCount(MongoDatabase database, string collection) {
            return MongoDatabaseCommands.CollectionCount(database, collection, new { });
        }

        /// <summary>
        /// Returns the count of records from the specified collection 
        /// that meet the criteria for the query 
        /// </summary>
        public static CollectionCountResult CollectionCount(MongoDatabase database, string collection, object query) {
            return MongoDatabaseCommands.CollectionCount(database, collection, new BsonObject(query));
        }

        /// <summary>
        /// Returns the count of records from the specified collection 
        /// that meet the criteria for the query 
        /// </summary>
        public static CollectionCountResult CollectionCount(MongoDatabase database, string collection, BsonDocument query) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { count = collection, query = query });
            return new CollectionCountResult(result.GetDefaultResponse());
        }

        #endregion

        #region Administrator Commands

        /// <summary>
        /// Returns build info for the server
        /// </summary>
        public static BuildInfoResult GetBuildInfo(MongoDatabase database) {
            CommandResponse response = MongoDatabaseCommands.RunCommand(database, new { buildinfo = Mongo.CommandArgument });
            return new BuildInfoResult(response.GetDefaultResponse());
        }

        /// <summary>
        /// Returns a list of all available databases
        /// </summary>
        public static ListDatabasesResult ListDatabases(MongoDatabase database) {
            CommandResponse response = MongoDatabaseCommands.RunCommand(database, new { listDatabases = Mongo.CommandArgument });
            return new ListDatabasesResult(response.GetDefaultResponse());
        }

        /// <summary>
        /// Performs the FSync command against the database
        /// </summary>
        public static FSyncResult FSync(MongoDatabase database, bool async) {
            CommandResponse response = MongoDatabaseCommands.RunCommand(database, new { fsync = Mongo.CommandArgument, async = async });
            return new FSyncResult(response.GetDefaultResponse());
        }

        /// <summary>
        /// Copies a database from one location to another
        /// </summary>
        public static MethodResult CopyDatabase(MongoDatabase database, string from, string to, string host, int port, string username, string password) {

            //create the request
            BsonObject parameters = new BsonObject(new {
                copydb = Mongo.CommandArgument,
                fromdb = from,
                todb = to,
                fromhost = host
            });

            //check for credentials
            username = (username ?? string.Empty).Trim();
            password = (password ?? string.Empty);

            //check for credentials
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {

                //get the connection information for the target database
                using (MongoDatabase target = new MongoDatabase(to, host)) {
                    GetNonceResult nonce = MongoDatabaseCommands.GetNonce(target);
                    string key = MongoDatabaseCommands.HashPassword(username, password, nonce.Nonce);

                    //and add it to the command
                    parameters += new {
                        username = username,
                        nonce = nonce,
                        key = key
                    };

                }

            }

            //finally, with the created value, send the command
            CommandResponse response = MongoDatabaseCommands.RunCommand(database, parameters);
            return new MethodResult(response.GetDefaultResponse());

        }

        /// <summary>
        /// Causes the Mongo server to shutdown - No response is expected
        /// </summary>
        public static void Shutdown(MongoDatabase database) {
            MongoDatabaseCommands.RunCommand(database, new { shutdown = Mongo.CommandArgument }, false);
        }

        /// <summary>
        /// Returns information about previous errors on the server
        /// </summary>
        public static GetPreviousErrorResult GetPreviousError(MongoDatabase database) {
            CommandResponse response = MongoDatabaseCommands.RunCommand(database, new { getpreverror = Mongo.CommandArgument });
            return new GetPreviousErrorResult(response.GetDefaultResponse());
        }

        /// <summary>
        /// Forces an error message on the server
        /// </summary>
        public static ForceErrorResult ForceError(MongoDatabase database) {
            CommandResponse response = MongoDatabaseCommands.RunCommand(database, new { forceerror = Mongo.CommandArgument });
            return new ForceErrorResult(response.GetDefaultResponse());
        }
        
        /// <summary>
        /// Returns uptime, lock and memory info for the database
        /// </summary>
        public static MethodResult GetServerStatus(MongoDatabase database) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { serverStatus = Mongo.CommandArgument });
            return new MethodResult(result.GetDefaultResponse());
        }

        /// <summary>
        /// Returns OpLogging information for the server
        /// </summary>
        public static MethodResult GetOpLogging(MongoDatabase database) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { opLogging = Mongo.CommandArgument });
            return new MethodResult(result.GetDefaultResponse());
        }

        /// <summary>
        /// Sets the level for the Query Trace in the database
        /// </summary>
        public static MethodResult SetQueryTraceLevel(MongoDatabase database, int level) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { queryTraceLevel = level });
            return new MethodResult(result.GetDefaultResponse());
        }

        #endregion

        #region Collection Related Commands

        /// <summary>
        /// Removes a collection entirely from the database
        /// </summary>
        public static DropCollectionResult DropCollection(MongoDatabase database, string collection) {
            CommandResponse response = MongoDatabaseCommands.RunCommand(database, new { drop = collection });
            return new DropCollectionResult(response.GetDefaultResponse());
        }

        /// <summary>
        /// Returns details about the provided collection
        /// </summary>
        public static CollectionStatsResult GetCollectionStats(MongoDatabase database, string collection) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { collstats = collection });
            return new CollectionStatsResult(result.GetDefaultResponse());
        }

        /// <summary>
        /// Gets the distinct values for the collection and key provided
        /// </summary>
        public static CollectionDistinctResult GetCollectionDistinct(MongoDatabase database, string collection, string key) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { distinct = collection, key = key });
            return new CollectionDistinctResult(result.GetDefaultResponse());
        }

        /// <summary>
        /// Removes all indexes from the provided collection
        /// </summary>
        public static DeleteCollectionIndexResult DeleteAllCollectionIndexes(MongoDatabase database, string collection) {
            return MongoDatabaseCommands.DeleteCollectionIndex(database, collection, "*");
        }

        /// <summary>
        /// Removes the index for key in the provided collection
        /// </summary>
        public static DeleteCollectionIndexResult DeleteCollectionIndex(MongoDatabase database, string collection, string index) {
            CommandResponse result = MongoDatabaseCommands.RunCommand(database, new { distinct = collection, index = index });
            return new DeleteCollectionIndexResult(result.GetDefaultResponse());
        }

        #endregion

        #region Sending Commands

        /// <summary>
        /// Executes a command against the database using the provided information
        /// </summary>
        public static CommandResponse RunCommand(MongoDatabase database, object parameters) {
            return MongoDatabaseCommands.RunCommand(database, new BsonObject(parameters));
        }

        /// <summary>
        /// Executes a command against the database using the provided information
        /// </summary>
        public static CommandResponse RunCommand(MongoDatabase database, object parameters, bool expectResponse) {
            return MongoDatabaseCommands.RunCommand(database, parameters, true);
        }

        /// <summary>
        /// Executes a command against the database using the provided information
        /// </summary>
        public static CommandResponse RunCommand(MongoDatabase database, BsonObject parameters) {
            return MongoDatabaseCommands.RunCommand(database, parameters, true);
        }

        /// <summary>
        /// Executes a command against the database using the provided information
        /// </summary>
        public static CommandResponse RunCommand(MongoDatabase database, BsonObject parameters, bool expectResponse) {

            //create the command to use
            CommandRequest request = new CommandRequest(database);
            request.Arguments.Merge(parameters);

            //send the command and check for the result
            CommandResponse response = database.SendRequest(request) as CommandResponse;
            if (response == null) {
                throw new LameException("Invoking command failed to return a response!");
            }

            //return any documents that were found
            return response;

        }

        #endregion

        #region Working With Passwords

        /// <summary>
        /// Handles preparing a password to send to the database
        /// </summary>
        public static string HashPassword(string username, string password, string nonce) {
            string hash = MongoDatabaseCommands._HashString(string.Format("{0}:mongo:{1}", username, password));
            return MongoDatabaseCommands._HashString(string.Concat(nonce, username, hash));
        }

        #endregion

        #region Private Methods

        //hashes a string quickly with MD5
        private static string _HashString(string value) {
            MD5 hash = MD5.Create();

            //hash the string into a new series of bytes
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte[] hashed = hash.ComputeHash(bytes);

            //perform additional string formatting
            return BitConverter.ToString(hashed)
                .Replace("-", "")
                .ToLower();
        }

        #endregion

    }

}
