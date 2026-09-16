using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;

namespace GromaxMobileApis.Models
{
    public class PositionFilter
    {
        public string TmMail { get; set; }
        public string AmMail { get; set; }
        public string DealerMail { get; set; }
        public string ShMail { get; set; }
        public string StateName { get; set; }
    }
    public class PositionFilterResult
    {
        public List<dynamic> StateHead { get; set; }

        public List<dynamic> AreaManagers { get; set; }
        public List<dynamic> TerritoryManagers { get; set; }
        public List<dynamic> Dealers { get; set; }
        public List<dynamic> Location { get; set; }
        public List<dynamic> States { get; set; }
    }

    public class StateCodeModel
    {
        public string StateName { get; set; }
    }

    public class DealerListByLocationModel
    {
        public string stateName { get; set; }
        public string districtName { get; set; }
        public string tehsilName { get; set; }
    }

    public class NotAssign
    {
        public int PageSize { get; set; }
        public int RowStart { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class AssignDealerModel
    {
        public List<string> Ids { get; set; }
        public string DealerCode { get; set; }
    }
}
