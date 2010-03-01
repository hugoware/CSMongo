using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Types;
using CSMongo.Bson;
using System.IO;

namespace CSMongo.DataTypes {
    
    /// <summary>
    /// Handles working with large enumerated value types
    /// </summary>
    public class LargeEnumeratedType : MongoDataType {

        #region Methods

        /// <summary>
        /// Returns the correct data type to store this value
        /// </summary>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Int64;
        }

        /// <summary>
        /// Determines if the value provided can be used 
        /// for this value
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {

            //make sure it is an enumerated value
            Type type = value.GetType();
            if (!type.IsEnum) { return false; }

            //next, check for the range
            Type underlying = Enum.GetUnderlyingType(type);

            //check if it is an unallowed type
            return (underlying.Equals(SmallEnumeratedType._Int64) ||
                underlying.Equals(SmallEnumeratedType._UInt32) ||
                underlying.Equals(SmallEnumeratedType._UInt64));

        }

        /// <summary>
        /// Handles getting the current value for this field
        /// </summary>
        public override object Get<T>() {
            T fallback = default(T);

            //try and read the value
            try {

                //make sure this can be parsed
                Type change = typeof(T);
                if (change.IsEnum) {
                    return Enum.Parse(change, (this.Value ?? string.Empty).ToString(), true);
                }
                //otherwise, just use the default method
                else {
                    return this.Get<T>();
                }

            }
            //if it fails just grab the fallback value
            catch {
                return fallback;
            }

        }

        /// <summary>
        /// Converts the enumerated value to a safe level
        /// </summary>
        public override void Set<T>(T value) {
            this.Value = Convert.ToInt64(value);
        }

        /// <summary>
        /// Performs any conversions for a number
        /// </summary>
        protected override object ConvertValue<T>(object value) {
            return value;
        }

        /// <summary>
        /// Converts the value into an appropriate byte value
        /// </summary>
        public override byte[] ToBson() {
            return BsonTranslator.AsInt64(this.Value as int? ?? 0);
        }

        /// <summary>
        /// Reads the value from an incoming stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonTranslator.ReadInt64(stream);
        }

        #endregion

    }

}
