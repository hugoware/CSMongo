using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo {
    
    /// <summary>
    /// Default properties and values for Mongo
    /// </summary>
    public static class Mongo {

        /// <summary>
        /// The default number to use when creating a query for the take value
        /// </summary>
        public static readonly int DefaultTakeCount = 0;

        /// <summary>
        /// The default number to use when creating a query for the skip value
        /// </summary>
        public static readonly int DefaultSkipCount = 0;

        /// <summary>
        /// The default ID value used for ObjectIds with a Mongo Document
        /// </summary>
        public static readonly string DocumentIdKey = "_id";

        /// <summary>
        /// Null value to use when creating new anonymous types
        /// since you can't assign null directly
        /// </summary>
        public static readonly object Null = null;

        /// <summary>
        /// The starting value to use when calculating dates
        /// </summary>
        public static readonly DateTime Epoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// The number that most commands use as the value for the 
        /// name of the method that is being run in on the server
        /// </summary>
        public static readonly decimal CommandArgument = 1.0M;

        /// <summary>
        /// The default port that Mongo uses for database connections
        /// </summary>
        public static readonly int DefaultPort = 27017;

        /// <summary>
        /// The name of the Mongo database to default to 
        /// </summary>
        public static string DefaultAdminDatabase = "admin";

        /// <summary>
        /// The default number of records to return on a GetMore
        /// request if no count is provided and cannot be determined
        /// from the previous request
        /// </summary>
        public static int DefaultGetMoreReturnCount {
            get { return Mongo._DefaultGetMoreReturnCount; }
            set { Mongo._DefaultGetMoreReturnCount = Math.Max(value, DEFAULT_RETURN_COUNT); }
        }
        private static int _DefaultGetMoreReturnCount = DEFAULT_RETURN_COUNT;
        private const int DEFAULT_RETURN_COUNT = 100;
    
    }

}
