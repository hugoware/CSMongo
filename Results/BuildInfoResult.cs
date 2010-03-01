using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// Returns details about the current build information for the database
    /// </summary>
    public class BuildInfoResult : MethodResult {

        #region Constructors

        /// <summary>
        /// Creates a new BuildInfo result from the provided document
        /// </summary>
        public BuildInfoResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the Git version of this Mongo database
        /// </summary>
        public string GitVersion {
            get { return this.Response.Get<string>("gitVersion"); }
        }

        /// <summary>
        /// Returns the version number for this database
        /// </summary>
        public string Version {
            get { return this.Response.Get<string>("version"); }
        }

        /// <summary>
        /// Returns information about the system that is hosting the database
        /// </summary>
        public string SystemInformation {
            get { return this.Response.Get<string>("sysInfo"); }
        }

        /// <summary>
        /// Returns the current bit level the system is running at
        /// </summary>
        public int Bits {
            get { return this.Response.Get<int>("bits", 0); }
        }

        #endregion

    }

}
