using System;
using System.Collections.Generic;
using System.Linq;
using CSMongo.Commands;
using CSMongo.Exceptions;
using CSMongo.Requests;
using CSMongo.Responses;
using CSMongo.Results;
using CSMongo.Bson;
using CSMongo.Query;

namespace CSMongo {

    /// <summary>
    /// An connection to a Mongo Database
    /// </summary>
    public class MongoDatabase : IDisposable {

        #region Constructors

        /// <summary>
        /// Uses the connection string information to access a database
        /// </summary>
        public MongoDatabase(string connectionString) {
            MongoConnectionString connection = MongoConnectionString.Parse(connectionString);
            this._SetupDatabase(
                connection.Database, 
                connection.Host, 
                connection.Port, 
                connection.Username, 
                connection.Password, 
                connection.AutoConnect
                );
        }

        /// <summary>
        /// Uses a connection to refer to a Mongo database
        /// </summary>
        public MongoDatabase(string database, string host)
            : this(database, host, Mongo.DefaultPort, null, null, true) {
        }

        /// <summary>
        /// Uses a connection to refer to a Mongo database
        /// </summary>
        public MongoDatabase(string database, string host, bool autoConnect)
            : this(database, host, Mongo.DefaultPort, null, null, autoConnect) {
        }

        /// <summary>
        /// Uses a connection to refer to a Mongo database
        /// </summary>
        public MongoDatabase(string database, string host, int port)
            : this(database, host, port, null, null, true) {
        }

        /// <summary>
        /// Uses a connection to refer to a Mongo database
        /// </summary>
        public MongoDatabase(string database, string host, int port, bool autoConnect)
            : this(database, host, port, null, null, true) {
        }

        /// <summary>
        /// Uses a connection to refer to a Mongo database
        /// </summary>
        public MongoDatabase(string database, string host, string username, string password)
            : this(database, host, Mongo.DefaultPort, username, password, true) {
        }

        /// <summary>
        /// Uses a connection to refer to a Mongo database
        /// </summary>
        public MongoDatabase(string database, string host, int port, string username, string password, bool autoConnect) {
            this._SetupDatabase(database, host, port, username, password, autoConnect);
        }

        #endregion

        #region Additional Setup

        //registers additional events to handle 
        private void _SetupDatabase(string database, string host, int port, string username, string password, bool autoConnect) {

            //assign the connection information
            this.Name = database;
            this.Connection = new MongoConnection(host, port, autoConnect);
            this.Username = (username ?? string.Empty).Trim();
            this.Password = password ?? string.Empty;

            //setup some of the properties
            this.HasAuthenticated = false;
            this._Cursors = new List<MongoCursor>();
            this._Collections = new Dictionary<string, MongoCollection>();

            //register the events
            this.Connection.AfterConnectionOpen += (connection) => { this.Authenticate(); };
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the database
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The username to connect with
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password to connect with
        /// </summary>
        public string Password { protected get; set; }

        /// <summary>
        /// Gets or sets if this connection automatically opens
        /// when a request is made and the connection is closed
        /// </summary>
        public bool AutoConnect { get; set; }

        /// <summary>
        /// Has this user authenticated to the server - If no 
        /// username and password is set then authentication
        /// will not ever be attempted
        /// </summary>
        public bool HasAuthenticated { get; private set; }

        //keeps track of an authentication attempt since an
        //authentication attempt requires that two commands
        //and would cause the first command to execute over and over
        private bool _IsAttemptingAuthentication = false;

        /// <summary>
        /// The connection to use being used for this database
        /// </summary>
        public MongoConnection Connection { get; set; }

        /// <summary>
        /// Returns a list of currently available cursors
        /// </summary>
        public IEnumerable<MongoCursor> Cursors {
            get { return this._Cursors.AsEnumerable(); }
        }
        private List<MongoCursor> _Cursors;

        /// <summary>
        /// Checks if this connection has both a username and password provided
        /// </summary>
        public bool HasCredentials {
            get { return !(string.IsNullOrEmpty(this.Username) && string.IsNullOrEmpty(this.Password)); }
        }

        /// <summary>
        /// Returns access to a collection
        /// </summary>
        public MongoCollection this[string collection] {
            get { return this.GetCollection(collection); }
        }

        //tracking changes
        private Dictionary<string, MongoCollection> _Collections;

        #endregion

        #region Query Starters

        /// <summary>
        /// Starts a new query for a collection
        /// </summary>
        public MongoQuery Find(string collection) {
            return this.GetCollection(collection).Find();
        }

        /// <summary>
        /// Starts a new query for a collection
        /// </summary>
        public TQueryProvider Find<TQueryProvider>(string collection) where TQueryProvider : MongoQueryBase {
            return this.GetCollection(collection).Find<TQueryProvider>();
        }

        #endregion

        #region Inserting Documents

        /// <summary>
        /// Inserts a series of documents into the database
        /// </summary>
        public void Insert(string collection, object document) {
            this.Insert(collection, (new MongoDocument[] { new MongoDocument(true, document) }).AsEnumerable());
        }

        /// <summary>
        /// Inserts a series of documents into the database
        /// </summary>
        public void Insert(string collection, MongoDocument document) {
            this.Insert(collection, (new MongoDocument[] { document }).AsEnumerable());
        }

        /// <summary>
        /// Inserts a series of documents into the database
        /// </summary>
        public void Insert(string collection, params object[] documents) {
            this.Insert(collection, documents.AsEnumerable());
        }

        /// <summary>
        /// Inserts a series of documents into the database
        /// </summary>
        public void Insert(string collection, params MongoDocument[] documents) {
            this.Insert(collection, documents.AsEnumerable());
        }

        /// <summary>
        /// Inserts a series of documents into the database
        /// </summary>
        public void Insert(string collection, IEnumerable<object> documents) {
            IEnumerable<MongoDocument> inserts = documents.Select(item => new MongoDocument(true, item));
            this.Insert(collection, inserts);
        }

        /// <summary>
        /// Inserts a series of documents into the database
        /// </summary>
        public void Insert(string collection, IEnumerable<MongoDocument> documents) {
            MongoCollection set = new MongoCollection(this, collection);
            set.InsertOnSubmit(documents);
            set.SubmitChanges();
        }

        #endregion

        #region Database Functions

        /// <summary>
        /// Sends the shutdown signal for this server
        /// </summary>
        public void Shutdown() {
            MongoDatabaseCommands.Shutdown(this);
        }

        /// <summary>
        /// Sends the logout request to the server
        /// </summary>
        public void Logout() {
            MongoDatabaseCommands.LogOut(this);
        }

        /// <summary>
        /// Attempts to repair, compact the current database
        /// </summary>
        public MethodResult RepairDatabase(bool preserveClonedOnFailure, bool backupOriginal) {
            return MongoDatabaseCommands.RepairDatabase(this, preserveClonedOnFailure, backupOriginal);
        }

        /// <summary>
        /// Sets the profile level on the server and returns the previous value
        /// </summary>
        public ProfileResult SetProfileLevel(int level) {
            return MongoDatabaseCommands.SetProfileLevel(this, level);
        }

        /// <summary>
        /// Removes a collection based on the name
        /// </summary>
        public DropCollectionResult DropCollection(MongoCollection collection) {
            return this.DropCollection(collection.Name);
        }

        /// <summary>
        /// Removes a collection based on the name
        /// </summary>
        public DropCollectionResult DropCollection(string collection) {

            //remove it from the list (if it is there)
            collection = (collection ?? string.Empty).Trim();
            this._Collections.Remove(collection);

            //then send the command to remove it
            return MongoDatabaseCommands.DropCollection(this, collection);

        }

        /// <summary>
        /// Completely removes a Mongo database from the server
        /// </summary>
        public MethodResult DropDatabase() {
            return MongoDatabaseCommands.DropDatabase(this);
        }

        /// <summary>
        /// Clears any errors from the server
        /// </summary>
        public MethodResult ResetErrors() {
            return MongoDatabaseCommands.ResetErrors(this);
        }

        /// <summary>
        /// Returns the most recent error from the server
        /// </summary>
        public GetLastErrorResult GetLastError() {
            return MongoDatabaseCommands.GetLastError(this);
        }

        /// <summary>
        /// Returns a previous error message from the server
        /// </summary>
        public GetPreviousErrorResult GetPreviousError() {
            return MongoDatabaseCommands.GetPreviousError(this);
        }

        /// <summary>
        /// Causes an error on the server -- Uh... wut?
        /// </summary>
        public ForceErrorResult ForceError() {
            return MongoDatabaseCommands.ForceError(this);
        }

        /// <summary>
        /// Removes all indexes from the provided collection
        /// </summary>
        public DeleteCollectionIndexResult DeleteCollectionIndex(string collection) {
            return MongoDatabaseCommands.DeleteAllCollectionIndexes(this, collection);
        }

        /// <summary>
        /// Removes the specified index from the collection
        /// </summary>
        public DeleteCollectionIndexResult DeleteCollectionIndex(string collection, string index) {
            return MongoDatabaseCommands.DeleteCollectionIndex(this, collection, index);
        }

        /// <summary>
        /// Returns the OpTime value from the server
        /// </summary>
        public GetOpTimeResult GetOpTime() {
            return MongoDatabaseCommands.GetOpTime(this);
        }

        /// <summary>
        /// Manually invokes a command against the database
        /// </summary>
        public MethodResult RunCommand(object arguments) {
            return this.RunCommand(new BsonObject(arguments));
        }

        /// <summary>
        /// Manually invokes a command against the database
        /// </summary>
        public MethodResult RunCommand(object arguments, bool expectResponse) {
            return this.RunCommand(new BsonObject(arguments), expectResponse);
        }

        /// <summary>
        /// Manually invokes a command against the database
        /// </summary>
        public MethodResult RunCommand(BsonObject arguments) {
            return this.RunCommand(arguments, true);
        }

        /// <summary>
        /// Manually invokes a command against the database
        /// </summary>
        public MethodResult RunCommand(BsonObject arguments, bool expectResponse) {
            CommandResponse response = MongoDatabaseCommands.RunCommand(this, arguments, expectResponse);
            return new MethodResult(response.GetDefaultResponse());
        }

        #endregion

        #region Communication

        /// <summary>
        /// Sends a prepared request to the database
        /// </summary>
        public ResponseBase SendRequest(RequestBase request) {

            //check if this person needs to authenticate
            if (this.HasCredentials && !this.HasAuthenticated && !this._IsAttemptingAuthentication) {
                this._IsAttemptingAuthentication = true;
                this.Authenticate();
            }

            //send the command to the server
            return this.Connection.SendRequest(request);
        }

        /// <summary>
        /// Handles disconnecting from the database
        /// </summary>
        public void Disconnect() {
            if (!this.Connection.Connected) { return; }

            //kill off the cursors first
            this.KillCursors();

            //check if the logout command should be sent
            if (this.Connection.Connected && this.HasAuthenticated) {
                MongoDatabaseCommands.LogOut(this);
                this.HasAuthenticated = false;
            }

        }

        #endregion

        #region Updating Database

        /// <summary>
        /// Submits changes for all collections that have items queued
        /// </summary>
        public void SubmitChanges() {
            foreach (MongoCollection collection in this._Collections.Values) {
                collection.SubmitChanges();
            }
        }

        #endregion

        #region Collections

        /// <summary>
        /// Returns the reference to a collection but does not
        /// actually communicate or create the collection 
        /// </summary>
        public MongoCollection GetCollection(string collection) {

            //verify the name is okay
            collection = (collection ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(collection)) {
                throw new LameException("A collection must have a name!");
            }

            //check if this is already loaded
            if (!this._Collections.ContainsKey(collection)) {
                this._Collections.Add(collection, new MongoCollection(this, collection));
            }

            //return the collection
            return this._Collections[collection];
        }

        #endregion

        #region Users/Authentication

        /// <summary>
        /// Authenticates to this database using the current username and password
        /// </summary>
        public void Authenticate() {
            this.Authenticate(this.Username, this.Password);
        }

        /// <summary>
        /// Authenticates this request with the provided username and password
        /// </summary>
        public void Authenticate(string username, string password) {
            if (!this.HasCredentials) { return; }

            //attempt to login for this user
            try {
                this._IsAttemptingAuthentication = true;
                this.Username = (username ?? string.Empty);
                this.Password = (password ?? string.Empty);

                //handle logging into the database
                MethodResult result = MongoDatabaseCommands.Authenticate(this, this.Username, this.Password);
                this.HasAuthenticated = result.Ok;
            }
            //thanks Ryan Farley
            catch (Exception up) {
                throw up;
            }
            //make sure to note that we finished our attempt
            finally {
                this._IsAttemptingAuthentication = false;
            }
            
        }

        #endregion

        #region Working With Cursors

        /// <summary>
        /// Returns the most recently entered cursor
        /// </summary>
        public MongoCursor GetLastCursor() {

            //if there aren't any cursors throw an error
            if (this._Cursors.Count == 0) {
                throw new LameException("No cursors were found!");
            }

            //return the most recently added item
            return this._Cursors.LastOrDefault();
        }

        /// <summary>
        /// Gets more documents from the most recently added cursor
        /// </summary>
        public IEnumerable<MongoDocument> GetMore() {
            return this.GetMore(this.GetLastCursor());
        }

        /// <summary>
        /// Gets more documents from the most recently added cursor
        /// </summary>
        public IEnumerable<MongoDocument> GetMore(int count) {
            return this.GetMore(this.GetLastCursor(), count);
        }

        /// <summary>
        /// Gets more documents from the provided MongoCursor
        /// </summary>
        public IEnumerable<MongoDocument> GetMore(MongoCursor cursor) {
            return this.GetMore(cursor, -1);
        }

        /// <summary>
        /// Gets more documents from the provided MongoCursor
        /// </summary>
        public IEnumerable<MongoDocument> GetMore(MongoCursor cursor, int count) {
            
            //make sure this is really a cursor
            if (cursor == null) {
                throw new LameException("This isn't a cursor!");
            }

            //start with the cursor value
            int take = Mongo.DefaultGetMoreReturnCount;

            //if there is a defined count, use it
            if (count > 0) {
                take = count;
            }
            //check for a manual take amount
            else if (cursor.Query is QueryRequest && cursor.Query.Take > 0) {
                take = cursor.Query.Take;
            }
            //finally, use the return count if nothing
            //else can be fount
            else if (cursor.ReturnCount > 0) {
                take = cursor.ReturnCount;
            }
                

            //if there is a cursor, use it
            RequestBase request;
            MongoCollection collection = this.GetCollection(cursor.Query.Collection);
            if (cursor.HasCursor) {
                request = new GetMoreRequest(collection, cursor, take);
            }
            //otherwise, just perform the query again
            else {
                cursor.Query.Take = take;
                cursor.Query.Skip += take;
                cursor.Query.Reset();
                request = cursor.Query;
            }

            //select any records we can find
            QueryResponse response = this.SendRequest(request) as QueryResponse;

            //make sure something was found
            if (response == null) {
                throw new LameException("Failed to get more records!");
            }

            //return the found documents
            return response.HasDocuments 
                ? response.Documents 
                : (new MongoDocument[] { }).AsEnumerable();

        }

        /// <summary>
        /// Returns an instance of a cursor
        /// </summary>
        public void RegisterCursor(MongoCursor cursor) {
            if (cursor == null) { return; }
            this._Cursors.Add(cursor);
        }

        /// <summary>
        /// Kills any available cursors to the database
        /// </summary>
        public void KillCursors() {
            MongoDatabaseCommands.KillCursors(this, this._Cursors);
            this._Cursors = new List<MongoCursor>();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Handles ensuring that a connection is cleaned up when finished
        /// </summary>
        public void Dispose() {
            this.Disconnect();
            this.Connection.Dispose();
        }

        #endregion
    
    }

}
