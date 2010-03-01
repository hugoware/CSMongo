using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Results;
using CSMongo.Commands;
using CSMongo.Bson;
using CSMongo.Responses;

namespace CSMongo {

    /// <summary>
    /// Creates a connection to the administrative database for the Mongo instance
    /// </summary>
    public class MongoAdminDatabase : IDisposable {

        #region Constructors

        /// <summary>
        /// Creates access to the admin database using the provided information
        /// </summary>
        public MongoAdminDatabase(string connectionString) {
            MongoConnectionString connection = MongoConnectionString.Parse(connectionString);
            this._Database = new MongoDatabase(
                Mongo.DefaultAdminDatabase,
                connection.Host,
                connection.Port,
                connection.Username,
                connection.Password,
                connection.AutoConnect
                );
        }

        /// <summary>
        /// Creates access to the admin database using the provided information
        /// </summary>
        public MongoAdminDatabase(string host, bool autoConnect)
            : this(host, Mongo.DefaultPort, null, null, autoConnect) {
        }

        /// <summary>
        /// Creates access to the admin database using the provided information
        /// </summary>
        public MongoAdminDatabase(string host, int port)
            : this(host, port, null, null, true) {
        }

        /// <summary>
        /// Creates access to the admin database using the provided information
        /// </summary>
        public MongoAdminDatabase(string host, int port, bool autoConnect)
            : this(host, port, null, null, true) {
        }

        /// <summary>
        /// Creates access to the admin database using the provided information
        /// </summary>
        public MongoAdminDatabase(string host, string username, string password)
            : this(host, Mongo.DefaultPort, username, password, true)  {
        }

        /// <summary>
        /// Creates access to the admin database using the provided information
        /// </summary>
        public MongoAdminDatabase(string host, int port, string username, string password, bool autoConnect) {
            this._Database = new MongoDatabase(Mongo.DefaultAdminDatabase, host, port, username, password, autoConnect);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the connection managing this database
        /// </summary>
        public MongoConnection Connection {
            get { return this._Database.Connection; }
        }

        //the database to perform connections on behalf of
        private MongoDatabase _Database;

        #endregion

        #region Methods

        /// <summary>
        /// Sends the log out request to the server
        /// </summary>
        public void Logout() {
            this._Database.Logout();
        }

        /// <summary>
        /// Checks the status of the database for the oplogging value
        /// </summary>
        public MethodResult CheckOpLogging() {
            return MongoDatabaseCommands.GetOpLogging(this._Database);
        }

        /// <summary>
        /// Returns the build info for the current database
        /// </summary>
        public BuildInfoResult GetBuildInfo() {
            return MongoDatabaseCommands.GetBuildInfo(this._Database);
        }

        /// <summary>
        /// Gets a list of all databases available
        /// </summary>
        public ListDatabasesResult GetDatabases() {
            return MongoDatabaseCommands.ListDatabases(this._Database);
        }

        /// <summary>
        /// Performs an FSync command against the database
        /// </summary>
        public FSyncResult FSync() {
            return this.FSync(false);
        }

        /// <summary>
        /// Performs an FSync command against the database
        /// </summary>
        public FSyncResult FSync(bool async) {
            return MongoDatabaseCommands.FSync(this._Database, async);
        }

        /// <summary>
        /// Handles copying a database to another location
        /// </summary>
        public MethodResult CopyDatabase(string from, string targetConnectionString) {
            MongoConnectionString connection = MongoConnectionString.Parse(targetConnectionString);
            return this.CopyDatabase(
                from,
                connection.Database,
                connection.Host,
                connection.Port,
                connection.Username,
                connection.Password
                );
        }

        /// <summary>
        /// Handles copying a database to another location
        /// </summary>
        public MethodResult CopyDatabase(string from, string to, string host) {
            return this.CopyDatabase(from, to, host, Mongo.DefaultPort, string.Empty, string.Empty);
        }

        /// <summary>
        /// Handles copying a database to another location
        /// </summary>
        public MethodResult CopyDatabase(string from, string to, string host, string username, string password) {
            return this.CopyDatabase(from, to, host, Mongo.DefaultPort, username, password);
        }

        /// <summary>
        /// Handles copying a database to another location
        /// </summary>
        public MethodResult CopyDatabase(string from, string to, string host, int port, string username, string password) {
            return MongoDatabaseCommands.CopyDatabase(this._Database, from, to, host, port, username, password);
        }

        /// <summary>
        /// Sends the shutdown signal to the database
        /// </summary>
        public void Shutdown() {
            MongoDatabaseCommands.Shutdown(this._Database);
        }

        /// <summary>
        /// Clears any errors from the server
        /// </summary>
        public MethodResult ResetErrors() {
            return MongoDatabaseCommands.ResetErrors(this._Database);
        }

        /// <summary>
        /// Returns the most recent error from the server
        /// </summary>
        public GetLastErrorResult GetLastError() {
            return MongoDatabaseCommands.GetLastError(this._Database);
        }

        /// <summary>
        /// Returns a previous error message from the server
        /// </summary>
        public GetPreviousErrorResult GetPreviousError() {
            return MongoDatabaseCommands.GetPreviousError(this._Database);
        }

        /// <summary>
        /// Causes an error on the server -- Uh... wut?
        /// </summary>
        public ForceErrorResult ForceError() {
            return MongoDatabaseCommands.ForceError(this._Database);
        }

        /// <summary>
        /// Returns the OpTime value from the server
        /// </summary>
        public GetOpTimeResult GetOpTime() {
            return MongoDatabaseCommands.GetOpTime(this._Database);
        }

        /// <summary>
        /// Manually invokes a command against the database
        /// </summary>
        public MethodResult RunCommand(object arguments) {
            return this._Database.RunCommand(arguments);
        }

        /// <summary>
        /// Manually invokes a command against the database
        /// </summary>
        public MethodResult RunCommand(object arguments, bool expectResponse) {
            return this._Database.RunCommand(arguments, expectResponse);
        }

        /// <summary>
        /// Manually invokes a command against the database
        /// </summary>
        public MethodResult RunCommand(BsonObject arguments) {
            return this._Database.RunCommand(arguments);
        }

        /// <summary>
        /// Manually invokes a command against the database
        /// </summary>
        public MethodResult RunCommand(BsonObject arguments, bool expectResponse) {
            return this._Database.RunCommand(arguments, expectResponse);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Handles cleaning up this database connection
        /// </summary>
        public void Dispose() {
            this._Database.Dispose();
        }

        #endregion
    
    }

}
