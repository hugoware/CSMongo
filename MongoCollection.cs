using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using CSMongo.Requests;
using CSMongo.Responses;
using CSMongo.Commands;
using CSMongo.Results;
using CSMongo.Query;
using CSMongo.Bson;

namespace CSMongo {

    /// <summary>
    /// A collection of documents within a Mongo database
    /// </summary>
    public class MongoCollection {

        #region Constructors

        /// <summary>
        /// Creates a new MongoCollection - It is better to create a
        /// collection using the MongoDatabase command GetCollection
        /// so the database can keep track of multiple instances of
        /// the same collection
        /// </summary>
        public MongoCollection(MongoDatabase database, string collection) {
            this.Database = database;
            this.Name = collection;

            //set the containers for updating
            this._Inserts = new List<MongoDocument>();
            this._Deletes = new List<MongoDocument>();
            this._Updates = new List<KeyValuePair<string, MongoDocument>>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the collection to query
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The database connection information
        /// </summary>
        public MongoDatabase Database { get; private set; }

        /// <summary>
        /// Returns the connection used for this request
        /// </summary>
        public MongoConnection Connection {
            get { return this.Database.Connection; }
        }

        //update information
        internal List<MongoDocument> _Inserts;
        internal List<MongoDocument> _Deletes;

        //document with a hashcode to track changes
        internal List<KeyValuePair<string, MongoDocument>> _Updates;

        #endregion

        #region Query Records

        /// <summary>
        /// Starts a new query for this collection
        /// </summary>
        public MongoQuery Find() {
            return this.Find<MongoQuery>();
        }

        /// <summary>
        /// Starts a new query for this collection using the provider requested
        /// </summary>
        public TQueryProvider Find<TQueryProvider>() where TQueryProvider : MongoQueryBase {
            return Activator.CreateInstance(typeof(TQueryProvider), this) as TQueryProvider;
        }

        /// <summary>
        /// Returns the current count of records for the database
        /// </summary>
        public long Count() {
            return this.Find().Count();
        }

        #endregion

        #region Updating Changes

        /// <summary>
        /// Adds a record to be inserted when changes are submitted
        /// </summary>
        public void InsertOnSubmit(object document) {
            this._Inserts.Add(new MongoDocument(document));
        }

        /// <summary>
        /// Adds a record to be inserted when changes are submitted
        /// </summary>
        public void InsertOnSubmit(MongoDocument document) {
            this._Inserts.Add(document);
        }

        /// <summary>
        /// Adds a set of records to be inserted when changes are submitted
        /// </summary>
        public void InsertOnSubmit(IEnumerable<MongoDocument> documents) {
            this._Inserts.AddRange(documents);
        }

        /// <summary>
        /// Adds a record to be deleted when changes are submitted
        /// </summary>
        public void DeleteOnSubmit(MongoDocument document) {
            this._Deletes.Add(document);
        }

        /// <summary>
        /// Adds a set of records to be deleted when changes are submitted
        /// </summary>
        public void DeleteOnSubmit(IEnumerable<MongoDocument> documents) {
            this._Deletes.AddRange(documents);
        }

        /// <summary>
        /// Appends a document to monitor for changes and updates
        /// </summary>
        public void UpdateOnSubmit(MongoDocument document) {
            this.UpdateOnSubmit((new MongoDocument[] { document }).AsEnumerable());
        }

        /// <summary>
        /// Appends a document to monitor for changes and updates
        /// </summary>
        public void UpdateOnSubmit(IEnumerable<MongoDocument> documents) {

            //append each of the items to the updates
            foreach (MongoDocument update in documents) {
                if (this._Updates.Any(item => item.Equals(update))) { return; }
                this._Updates.Add(new KeyValuePair<string, MongoDocument>(update.GetObjectHash(), update));
            }


        }

        #endregion

        #region Submitting Changes

        /// <summary>
        /// Handles updating changes for the database
        /// </summary>
        public void SubmitChanges() {

            //check for changes
            if (this._Inserts.Count > 0) { this._PerformInserts(); }
            if (this._Updates.Count > 0) { this._PerformUpdates(); }
            if (this._Deletes.Count > 0) { this._PerformDeletes(); }

            //then clear the lists
            this._Inserts = new List<MongoDocument>();
            this._Updates = new List<KeyValuePair<string, MongoDocument>>();
            this._Deletes = new List<MongoDocument>();

        }

        //handles inserting records waiting
        private void _PerformInserts() {
            InsertRequest insert = new InsertRequest(this);
            insert.Documents.AddRange(this._Inserts);
            this.Connection.SendRequest(insert);
        }

        //handles updating records that are changed
        private void _PerformUpdates() {

            //check for changed items and update them now
            foreach (KeyValuePair<string, MongoDocument> item in this._Updates) {

                //if this hasn't changed then skip it
                if (item.Key.Equals(item.Value.GetObjectHash())) { continue; }
                Console.WriteLine("Updating " + item.Value.Get<string>("name"));

                this.Find().FindById(item.Value.Id).Set(item.Value);
                this.Find().FindById(item.Value.Id).Unset(item.Value.GetRemovedFields().ToArray());

                //Might want to try and merge this into the same
                //request to avoid two trips to the database -- But
                //this might cause an issue with older versions of
                //the same database since an unset call would cause
                //the inital set request to fail...

                //UpdateRequest request = new UpdateRequest(this);
                //request.Modifications["$set"] = item.Value;
                //request.Modifications["$unset"] = item.Value.GetRemovedFields();
                //request.Parameters["$in"] = new MongoOid[] { item.Value.Id };
                //this.Database.SendRequest(request);

            }
            
        }

        //handles deleting records that need to be removed
        private void _PerformDeletes() {
            IEnumerable<MongoOid> ids = this._Deletes.Select(item => item.Id);
            this.Database.Find(this.Name).In("_id", ids).Delete();
        }

        #endregion

        #region Administrative

        /// <summary>
        /// Removes a collection from the database
        /// </summary>
        public DropCollectionResult DropCollection() {
            return this.Database.DropCollection(this.Name);
        }

        /// <summary>
        /// Returns details about the status of this collection
        /// </summary>
        public CollectionStatsResult GetStatus() {
            return MongoDatabaseCommands.GetCollectionStats(this.Database, this.Name);
        }

        /// <summary>
        /// Removes all indexes from this collection
        /// </summary>
        public DeleteCollectionIndexResult DeleteIndex() {
            return this.Database.DeleteCollectionIndex(this.Name);
        }

        /// <summary>
        /// Removes the specified index from this collection
        /// </summary>
        public DeleteCollectionIndexResult DeleteIndex(string collection, string index) {
            return this.Database.DeleteCollectionIndex(collection, index);
        }

        #endregion

    }

}
