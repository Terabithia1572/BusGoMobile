using System;
using System.Collections.Generic;
using System.Text;

namespace BusGoMobile.Models
{

    public class Seat
    {
        public int Number { get; set; }
        public SeatStatus Status { get; set; }

        // Boş koltuk tıklanabilir; dolu olanlar değil
        public bool IsSelectable => Status == SeatStatus.Empty || Status == SeatStatus.Selected;
    }


}
