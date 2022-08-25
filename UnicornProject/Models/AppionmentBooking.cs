using System;
using System.Collections.Generic;

namespace UnicornProject.Models
{
    public partial class AppionmentBooking
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PatientsId { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool? BookingConformation { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Isactive { get; set; }
    }
}
