using System;
using System.Collections.Generic;

#nullable disable

namespace GromaxMobileApis.Models
{
    public partial class VersionMaster
    {
        public Guid? Id { get; set; }
        public string VersionCode { get; set; }
        public string Url { get; set; }
        public string Platform { get; set; }
    }

    public class VersionMasterDto
    {
        public string VersionCode { get; set; }
        public string Url { get; set; }
    }
}
