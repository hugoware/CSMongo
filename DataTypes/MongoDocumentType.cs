using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Types;
using CSMongo.Bson;
using System.IO;
using System.Collections;

namespace CSMongo.DataTypes {

    /// <summary>
    /// Represents a sub document type
    /// </summary>
    public class MongoDocumentType : MongoDataType {

        /// <summary>
        /// Returns the correct MongoType for this object
        /// </summary>
        /// <returns></returns>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Object;
        }

        /// <summary>
        /// Returns the allowed types for this object
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return value is BsonObject || value is object;
        }

        /// <summary>
        /// No conversions required for this object
        /// </summary>
        public override object Get<T>() {
            BsonDocument document = this.Value as BsonDocument;
            return document is BsonDocument ? document : new BsonDocument();
        }

        /// <summary>
        /// Type should always be a BSON document type
        /// </summary>
        public override void Set<T>(T value) {
            this.Value = value is BsonObject
                ? value
                : (object)new BsonDocument(value);
        }

        /// <summary>
        /// Writes the bytes for this document
        /// </summary>
        public override byte[] ToBson() {
            BsonDocument document = this.Get<BsonDocument>() as BsonDocument;
            return document.ToBsonByteArray();
        }

        /// <summary>
        /// Reads a document from the stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonDocument.FromStream(stream);
        }

    }

}
