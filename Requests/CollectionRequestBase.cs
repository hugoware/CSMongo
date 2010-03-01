using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Types;

namespace CSMongo.Requests {

    /// <summary>
    /// Default class for requests that will be targeted at a collection
    /// </summary>
    public abstract class CollectionRequestBase : RequestBase {

        #region Constants

        private const string FULL_DATABASE_NAME_FORMAT = "{0}.{1}";

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a request targeting a collection
        /// </summary>
        public CollectionRequestBase(OpCodeTypes code, MongoCollection collection)
            : this(code, collection.Database.Name, collection.Name) {
        }

        /// <summary>
        /// Creates a request targeting a collection
        /// </summary>
        public CollectionRequestBase(OpCodeTypes code, string database, string collection)
            : base(code) {
            this.Database = (database ?? string.Empty).Trim();
            this.Collection = (collection ?? string.Empty).Trim();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The target collection for this request
        /// </summary>
        public string Collection { get; protected set; }

        /// <summary>
        /// The target database for this request
        /// </summary>
        public string Database { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the full string name for the database target
        /// </summary>
        public virtual string GetDatabaseTarget() {

            //if there is no collection, just send back the database
            if (string.IsNullOrEmpty(this.Collection)) {
                return this.Database;
            }

            //otherwise return the formatted version
            return string.Format(
                FULL_DATABASE_NAME_FORMAT, 
                this.Database, 
                this.Collection
                );

        }

        #endregion

    }

}
