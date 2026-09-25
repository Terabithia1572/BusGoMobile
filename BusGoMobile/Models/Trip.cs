using System;
using System.Collections.Generic;
using System.Text;

namespace BusGoMobile.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyColor { get; set; }
        public string SeatLayout { get; set; }
        public string FromCity { get; set; }
        public string FromTerminal { get; set; }
        public string ToCity { get; set; }
        public string ToTerminal { get; set; }
        public string DepartTime { get; set; }
        public string ArriveTime { get; set; }
        public string Duration { get; set; }
        public int Price { get; set; }
        public string SeatInfo { get; set; }
        public string TravelDate { get; set; }

    }
}
