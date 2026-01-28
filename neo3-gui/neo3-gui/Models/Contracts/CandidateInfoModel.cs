using System;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Candidate information model for CommitteeInfoContract.setInfo
    /// </summary>
    public class CandidateInfoModel
    {
        /// <summary>
        /// Candidate address (Hash160)
        /// </summary>
        public UInt160 Sender { get; set; }

        /// <summary>
        /// Candidate name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Website URL
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// GitHub profile
        /// </summary>
        public string Github { get; set; }

        /// <summary>
        /// Telegram handle
        /// </summary>
        public string Telegram { get; set; }

        /// <summary>
        /// Twitter handle
        /// </summary>
        public string Twitter { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Logo FS ID
        /// </summary>
        public string Logo { get; set; }
    }
}
