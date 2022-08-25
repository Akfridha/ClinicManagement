using System;
using System.ComponentModel.DataAnnotations;

namespace UnicornProject.Models
{
    public class AppionmentModel
    {

        [Required(ErrorMessage = "Please enter doctorID")]
        [CustomIntValidation(ErrorMessage = "invalid DoctorId")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please enter PatientId")]
        [CustomIntValidation(ErrorMessage = " invalid PatientId")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please enter StartDateTime ")]
        [CustomDateValidation(ErrorMessage = "Start DateTime must be greater then to current DateTime")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "Please enter EndDateTime ")]
        [CustomDateValidation(ErrorMessage = "End DateTime must be greater then to current DateTime")]
        public DateTime EndDateTime { get; set; }
    }
    public class CustomDateValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime = Convert.ToDateTime(value);
            return dateTime > DateTime.Now;
        }
    }

    public class CustomIntValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            return (int)value > 0 ;
        }
    }

    public class ScheduleDate { 
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
    }

    public class AvailabilityModel { 
        public UserModel userModel { get; set; }

        public string availabilityMessage { get; set; }
    }
}
