using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;
using System.IO;
using CSMongo.Types;

namespace CSMongo.DataTypes {

    /// <summary>
    /// Default class to handle long int values
    /// </summary>
    public class MongoInt64Type : MongoDataType {

        /// <summary>
        /// Returns the Mongo type for this data
        /// </summary>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Int64;
        }

        /// <summary>
        /// Returns if the value passed in is allowed to be used
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return value is long || value is long? ||
                value is uint || value is uint? ||
                value is ulong || value is ulong?;
        }

        /// <summary>
        /// Handles converting the value into an appropriate type
        /// </summary>
        protected override object ConvertValue<T>(object value) {
            Type change = typeof(T);
            if (change.IsEnum) {
                return Enum.Parse(change, (this.Value ?? string.Empty).ToString(), true);
            }
            else {
                return base.ConvertValue<T>(value);
            }
        }

        /// <summary>
        /// Sets the current value of this MongoNumber
        /// </summary>
        public override void Set<T>(T value) {
            this.Value = Convert.ToInt64(value);
        }

        /// <summary>
        /// Converts this value into a series of BSON bytes
        /// </summary>
        public override byte[] ToBson() {
            return BsonTranslator.AsInt64(this.Value as long? ?? default(long));
        }

        /// <summary>
        /// Reads the value from a stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonTranslator.ReadInt64(stream);
        }

    }

}
