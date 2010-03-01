using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Types;
using CSMongo.Bson;
using System.IO;

namespace CSMongo.DataTypes {

    /// <summary>
    /// Handles working with Oid values
    /// </summary>
    public class MongoOidType : MongoDataType {

        /// <summary>
        /// Returns the correct datatype to use
        /// </summary>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Oid;
        }

        /// <summary>
        /// Returns if the value is allowed to be used
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return value is MongoOid;
        }

        /// <summary>
        /// No conversions available for this type
        /// </summary>
        protected override object ConvertValue<T>(object value) {
            return value;
        }

        /// <summary>
        /// Generates the bytes for this container
        /// </summary>
        public override byte[] ToBson() {
            return BsonTranslator.AsOid(this.Value as MongoOid ?? new MongoOid());
        }

        /// <summary>
        /// Handles reading an Oid from an incoming stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonTranslator.ReadOid(stream);
        }

    }

}
