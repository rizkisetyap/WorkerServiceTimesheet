using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServiceTimesheet.ViewModels
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        public int totalrecord { get; set; }
        public List<Datalist> datalist { get; set; }
    }

    public class Datalist
    {
        public int tattendreportpk { get; set; }
        public string? staffid { get; set; }
        public string? staffname { get; set; }
        public int workintime { get; set; }
        public int workouttime { get; set; }
        public long? checkin { get; set; }
        public double? checkinlong { get; set; }
        public double? checkinlat { get; set; }
        public string? tapin { get; set; }
        public long? checkout { get; set; }
        public double? checkoutlong { get; set; }
        public double? checkoutlat { get; set; }
        public string statusin { get; set; }
        public string statusout { get; set; }
        public string? attendcode { get; set; }
        public string attenddesc { get; set; }
        public long attenddate { get; set; }
        public string? attendtype { get; set; }
        public string urlimage { get; set; }
        public string shiftname { get; set; }
        public string deptname { get; set; }
        public string late { get; set; }
        public string early { get; set; }
        public string isleave { get; set; }
        public string radiusin { get; set; }
        public string locationin { get; set; }
        public string radiusout { get; set; }
        public string locationout { get; set; }
        public string totalhours { get; set; }
        public string? staffhead { get; set; }
        public string? isapprove { get; set; }
    }

    public class ReportActivity
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class ActivityTimesheet
    {
        public DateTime? Checkin { get; set; }
        public DateTime? Checkout { get; set; }
    }

}