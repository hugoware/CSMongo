using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Bson {
    
    /// <summary>
    /// A document that reorders values based on size
    /// </summary>
    public class BsonOrderedDocument : BsonDocument {

        #region Constructors

        /// <summary>
        /// Creates a new empty BSON document
        /// </summary>
        public BsonOrderedDocument()
            : base() {
        }

        /// <summary>
        /// Creates a new BSON document based on the source provided
        /// </summary>
        public BsonOrderedDocument(object source)
            : base(source) {
        }

        #endregion

        #region Operators

        /// <summary>
        /// Merges another object with this object - Same as calling Merge
        /// </summary>
        public static BsonOrderedDocument operator +(BsonOrderedDocument target, object value) {
            target.Merge(value);
            return target;
        }

        /// <summary>
        /// Merges another DynamicObject with this object - Same as calling Merge
        /// </summary>
        public static BsonOrderedDocument operator +(BsonOrderedDocument target, BsonObject value) {
            target.Merge(value);
            return target;
        }

        /// <summary>
        /// Removes the field provided - Same as calling Remove
        /// </summary>
        public static BsonOrderedDocument operator -(BsonOrderedDocument target, string field) {
            target.Remove(field);
            return target;
        }

        /// <summary>
        /// Removes the field provided - Same as calling Remove
        /// </summary>
        public static BsonOrderedDocument operator -(BsonOrderedDocument target, string[] fields) {
            target.Remove(fields);
            return target;
        }

        #endregion

        #region Rendering Changes

        /// <summary>
        /// Reorders the bytes into the same order as their length
        /// </summary>
        protected override IEnumerable<BsonFieldDetail> OnBeforeFinishBsonRender(IEnumerable<BsonFieldDetail> fields) {
            return fields.OrderBy(bytes => bytes.Length);
        }

        #endregion

    }

}
