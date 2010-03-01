using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// Information related to assertions by the database
    /// </summary>
    public class AssertInfoResult : MethodResult {
        
        #region Constructors

        /// <summary>
        /// Creates a new container for an AssertInfo response
        /// </summary>
        public AssertInfoResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The actual expression used for the assertion
        /// </summary>
        public bool WasDatabaseAsserted {
            get { return this.Response.Get<bool>("dbasserted"); }
        }

        /// <summary>
        /// The actual expression used for the assertion
        /// </summary>
        public bool WasAsserted {
            get { return this.Response.Get<bool>("asserted"); }
        }

        /// <summary>
        /// The actual expression used for the assertion
        /// </summary>
        public string Assert {
            get { return this.Response.Get<string>("assert"); }
        }

        /// <summary>
        /// Unknown functionality
        /// </summary>
        public string AssertW {
            get { return this.Response.Get<string>("assertw"); }
        }

        /// <summary>
        /// Returns a message related to this request
        /// </summary>
        public string Message {
            get { return this.Response.Get<string>("assertmsg"); }
        }

        /// <summary>
        /// Returns user information for the assertion
        /// </summary>
        public string User {
            get { return this.Response.Get<string>("assertuser"); }
        }

        #endregion

    }

}
