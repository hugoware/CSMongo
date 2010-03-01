using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Extensions.Queries {

    /// <summary>
    /// Extension methods for working with Mongo Queries
    /// </summary>
    public static class MongoQueryExtensions {

        /// <summary>
        /// Selects information from the document in a specific format
        /// </summary>
        public static IEnumerable<T> As<T>(this IEnumerable<MongoDocument> documents, T template) {
            foreach (MongoDocument document in documents) {
                yield return document.Get(template);
            }
        }

        /// <summary>
        /// Selects information from the document in a specific format
        /// </summary>
        public static IEnumerable<T> As<T>(this IEnumerable<MongoDocument> documents, string start, T template) {
            foreach (MongoDocument document in documents) {
                yield return document.Get(start, template);
            }
        }
    
    }

}
