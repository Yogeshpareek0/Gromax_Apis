using System;
using System.Collections.Generic;

#nullable disable

namespace GromaxMobileApis.Models
{
    public partial class Dealermaster
    {
        public Guid? Id { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string StateHead { get; set; }
        public string Am { get; set; }
        public string Tm { get; set; }
        public string Sapcode { get; set; }
        public string Ao { get; set; }
        public DateTime? CreateDate { get; set; }
    }


    public class DealerMasterDto
    {
        public string StateCode { get; set; }
    }
}
