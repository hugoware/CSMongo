using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CSMongo.Bson;
using System.IO;
using CSMongo.Types;

namespace CSMongo.DataTypes {

    /// <summary>
    /// Handles array value types
    /// </summary>
    public class MongoArrayType : MongoDataType {

        /// <summary>
        /// Returns the Mongo type for this data
        /// </summary>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Array;
        }

        /// <summary>
        /// Allows for collection based values regarless of content
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return !(value is BsonObject) && (
                value is IEnumerable ||
                value is ICollection ||
                value is IList
                );
        }

        /// <summary>
        /// Saves the value of an array for this type
        /// </summary>
        public override void Set<T>(T value) {
            this.Value = ((IEnumerable)value).Cast<object>().ToArray();
        }

        public override object Get<T>() {

            //get the array information to use
            Type collection = typeof(T);
            Type element = collection.IsArray ? collection.GetElementType()
                : collection.IsGenericType ? collection.GetGenericArguments().FirstOrDefault()
                : typeof(object);

            //find the collection to use to evaluate this
            T fallback = collection.IsInterface 
                ? default(T) : 
                Activator.CreateInstance<T>();
        
            //Holy boxing/unboxing Batman!
            object[] values = this.Value as object[] ?? new object[] { };
            object[] created = (object[])Array.CreateInstance(element, values.Length);
            Array.Copy(values, created, values.Length);

            //depending on the collection type used
            if (fallback is IList && collection.IsGenericType) {
                return Activator.CreateInstance(collection, (object)created);
            }
            else {
                return created;
            }

        }

        /// <summary>
        /// Return the bytes as an array value
        /// </summary>
        public override byte[] ToBson() {
            return BsonTranslator.AsArray(this.Value as IEnumerable<object> ?? (new object[] { }).AsEnumerable());
        }

        /// <summary>
        /// Reads array content from the incoming stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonTranslator.ReadArray(stream);
        }

    }

}
