using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// Returns the OpTime result from the database
    /// </summary>
    public class GetOpTimeResult : MethodResult {

        #region Constructors

        /// <summary>
        /// The information returned when calling the FSync command
        /// </summary>
        public GetOpTimeResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the number of files after the FSync command
        /// </summary>
        public long Timestamp {
            get { return this.Response.Get<long>("optime"); }
        }

        /// <summary>
        /// Returns the total time based on the timestamp
        /// </summary>
        public TimeSpan TotalTime {
            get { return DateTime.Now.Subtract(new DateTime(this.Timestamp)); }
        }

        #endregion

    }

}
