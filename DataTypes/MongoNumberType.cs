using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;
using System.IO;
using CSMongo.Types;

namespace CSMongo.DataTypes {

    /// <summary>
    /// Represents a Number within Mongo an any acceptable types
    /// </summary>
    public class MongoNumberType : MongoDataType {

        /// <summary>
        /// Returns the Mongo type for this data
        /// </summary>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Number;
        }

        /// <summary>
        /// Returns if the value passed in is allowed to be used
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return value is decimal || value is decimal? || 
                value is float || value is float? ||
                value is Single || value is Single? || 
                value is double || value is double?;
        }

        /// <summary>
        /// Sets the current value of this MongoNumber
        /// </summary>
        public override void Set<T>(T value) {
            this.Value = Convert.ToDouble(value);
        }

        /// <summary>
        /// Converts this value into a series of BSON bytes
        /// </summary>
        public override byte[] ToBson() {
            return BsonTranslator.AsNumber(this.Value as double? ?? default(double));
        }

        /// <summary>
        /// Reads the value from a stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonTranslator.ReadNumber(stream);
        }

    }

}
