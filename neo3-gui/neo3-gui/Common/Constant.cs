using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Common
{
    public static class Constant
    {
        /// <summary>
        /// Maximum GAS for test mode execution (2000 GAS)
        /// </summary>
        public const long TestMode = 2000_00000000;

        /// <summary>
        /// Default page size for pagination
        /// </summary>
        public const int DefaultPageSize = 100;

        /// <summary>
        /// Default page index (1-based)
        /// </summary>
        public const int DefaultPageIndex = 1;
    }
}
