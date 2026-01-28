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
        /// Logo FS ID (NeoFS object ID)
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// Logo file path for auto-upload to NeoFS
        /// If provided, the file will be uploaded and Logo will be set to the returned object ID
        /// </summary>
        public string LogoPath { get; set; }
    }
}
