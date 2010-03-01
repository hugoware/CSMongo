using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Exceptions;
using CSMongo.Bson;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CSMongo.IO;
using CSMongo.Types;
using CSMongo.DataTypes;

namespace CSMongo {

    /// <summary>
    /// A document to use for storage with a MongoDatabase
    /// </summary>
    public class MongoDocument : BsonDocument {

        #region Constructors

        /// <summary>
        /// Creates an empty MongoDocument with a default ID assigned
        /// </summary>
        public MongoDocument()
            : this(false, null) {
        }

        /// <summary>
        /// Creates an empty MongoDocument with a default ID assigned
        /// </summary>
        public MongoDocument(object source)
            : this(false, source) {
        }

        /// <summary>
        /// Creates an empty MongoDocument with a default ID assigned
        /// </summary>
        public MongoDocument(bool generateId)
            : this(false, null) {
        }

        /// <summary>
        /// Creates a MongoDocument using the provided object as a template
        /// </summary>
        public MongoDocument(bool generateId, object source)
            : base(source) {
            if (generateId) { this.GenerateId(); }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Merges another object with this object - Same as calling Merge
        /// </summary>
        public static MongoDocument operator +(MongoDocument target, object value) {
            target.Merge(value);
            return target;
        }

        /// <summary>
        /// Merges another DynamicObject with this object - Same as calling Merge
        /// </summary>
        public static MongoDocument operator +(MongoDocument target, BsonObject value) {
            target.Merge(value);
            return target;
        }

        /// <summary>
        /// Removes the field provided - Same as calling Remove
        /// </summary>
        public static MongoDocument operator -(MongoDocument target, string field) {
            target.Remove(field);
            return target;
        }

        /// <summary>
        /// Removes the field provided - Same as calling Remove
        /// </summary>
        public static MongoDocument operator -(MongoDocument target, string[] fields) {
            target.Remove(fields);
            return target;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an Oid for this document - MongoDB will 
        /// automatically create an Oid when needed
        /// </summary>
        public void GenerateId() {
            this.Id = new MongoOid();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Oid for a document (if any)
        /// </summary>
        public MongoOid Id {
            get { return this.Get<MongoOid>(Mongo.DocumentIdKey); }
            set {
                if (value == null && !(value is MongoOid)) {
                    throw new LameException("Must be an Oid type!");
                }
                this.Set<MongoOid>(Mongo.DocumentIdKey, value); 
            }
        }

        #endregion

        #region Rendering

        /// <summary>
        /// Handles reordering so that the Oid is at the start
        /// </summary>
        protected override IEnumerable<BsonFieldDetail> OnBeforeFinishBsonRender(IEnumerable<BsonFieldDetail> fields) {

            //convert to a list that can be modified
            List<BsonFieldDetail> list = fields.ToList();

            //get all of the ids to use
            IEnumerable<BsonFieldDetail> ids = list.Where(item => item.Type is MongoOidType);
            list.RemoveAll(item => item.Type is MongoOidType);

            //reorder the document now so the Ids are in the front
            list.OrderBy(item => item.Length);
            
            //because the items are shared in the same list
            //we have to insert these one at the time since
            //if we use AddRange an exception will be thrown
            //since the list is modified while enumerating
            //through the values. We're also going backwards
            //to make sure they retain their original order
            for (int index = ids.Count(); index-- > 0; ) {
                list.Insert(0, ids.ElementAt(index));
            }

            //then return the list
            return list.AsEnumerable();

        }

        /// <summary>
        /// Handles rendering the bytes required to create this type
        /// </summary>
        /// <returns></returns>
        public override byte[] ToBsonByteArray() {
            if (this.Id == null) { this.GenerateId(); }
            return base.ToBsonByteArray();
        }

        #endregion

    }

}
