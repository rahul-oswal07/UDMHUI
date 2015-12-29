using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNISYS.UDTS.SourceTransformationManager.LoadQueue
{
    public class UDTS_STM_LoadQueue
    {
        public string chunkNumber { get; set; }
        public string tranchNumber { get; set; }
        public string packageName { get; set; }
        public string SourceFileName { get; set; }
        public string SourceTableName { get; set; }
        public string Type { get; set; } // category
        public string Status { get; set; }
        public string Priority { get; set; }
        public string filetype { get; set; } // Added for TRG to differentiate X02 and X03

    }
}
