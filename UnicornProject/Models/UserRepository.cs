using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using UnicornProject.Helper;

namespace UnicornProject.Models
{
    public class UserRepository : IUsers
    {
        private Hospital_Appointment_DBContext _hospital_Appointment_DBContext;

        public UserRepository(Hospital_Appointment_DBContext hospital_Appointment_DBContext)
        {

            _hospital_Appointment_DBContext = hospital_Appointment_DBContext;
        }


        public List<UserModel> userList()
        {
            throw new System.NotImplementedException();

        }

        public UserModel AddUser(UserModel user)
        {
            try
            {
                using (var dbContext = _hospital_Appointment_DBContext)
                {
                    var userModel = new UserMasterTable()
                    {
                        Name = user.Name,
                        Adress = user.Adress,
                        Email = user.Email,
                        Password = user.Password,
                        Phone = user.Phone,
                        RoleId = user.RoleId == 0 ? (int)RoleEnum.Patient : user.RoleId,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        Isactive = true
                    };
                    dbContext.Add(userModel);
                    dbContext.SaveChanges();
                    user.Id = userModel.Id;
                    user.RoleId = (int)userModel.RoleId;



                }
            }
            catch (Exception ex)
            {

            }
            return user;
        }

        public UserModel DeleteUser(UserModel user)
        {
            throw new System.NotImplementedException();
        }

        public UserModel FindUserByemail(string email)
        {
            throw new System.NotImplementedException();
        }

        public UserModel FindUserById(int userId)
        {
            var data = new UserModel();
            try
            {
                using (var dbContext =  new Hospital_Appointment_DBContext())
                {
                    data = (from userdata in dbContext.UserMasterTable
                            where userdata.Id.Equals(userId) && userdata.Isactive == true
                            select new UserModel
                            {
                                Id = userdata.Id,
                                Email = userdata.Email,
                                Phone = userdata.Phone,
                                Adress = userdata.Adress,
                                Name = userdata.Name
                            }).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {

            }

            return data;
        }


        public UserModel FindDoctorById(int id) {
            var data = new UserModel();
            try
            {
                using (var dbContext = new Hospital_Appointment_DBContext())
                {
                    data = (from doctor in dbContext.UserMasterTable
                            where doctor.Id.Equals(id) && doctor.Isactive == true && doctor.RoleId == (int)RoleEnum.Doctors
                            select new UserModel
                            {
                                Id = doctor.Id,
                                Email = doctor.Email,
                                Phone = doctor.Phone,
                                Adress = doctor.Adress,
                                Name = doctor.Name
                            }).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {

            }

            return data;
        }

        public UserModel UpdateUser(UserModel user)
        {
            throw new System.NotImplementedException();
        }

        public TokenObject ValidateUser(UserModel user)
        {

            TokenObject tokenObject = new TokenObject();
            try
            {
                using (var dbContext =  new Hospital_Appointment_DBContext())
                {
                    var data = (from userdata in dbContext.UserMasterTable
                                where userdata.Email.Equals(user.Email) && userdata.Password.Equals(user.Password) && userdata.Isactive == true
                                select new UserModel
                                {
                                    Id = userdata.Id,
                                    Email = userdata.Email,
                                    Name = userdata.Name,
                                    RoleId = (int)userdata.RoleId
                                }).FirstOrDefault();

                    if (data != null)
                    {

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(Startup.SECRET);

                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                   new Claim(ClaimTypes.Email, data.Email),
                   new Claim(ClaimTypes.Role, Enum.GetName(typeof(RoleEnum), data.RoleId)),
                   new Claim(ClaimTypes.Name,data.Name),
                   new Claim(ClaimTypes.Sid, data.Id.ToString())
                        }),

                            Expires = DateTime.UtcNow.AddHours(1),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        tokenObject.token = tokenHandler.WriteToken(token);
                        tokenObject.expires = token.ValidTo;
                }
                }
            }
            catch (Exception ex)
            {

            }

            return tokenObject;
        }

        public List<AvailabilityModel> GetAvailableDoctorsList(DateTime appionmentDate)
        {
            var doctorsAvailList = new List<AvailabilityModel>();
            appionmentDate = appionmentDate > DateTime.Now.Date ? appionmentDate.Date : DateTime.Now.Date;
            int hours = 8;
            try
            {
                using (var dbContext =  new Hospital_Appointment_DBContext())
                {
                   var  doctorsList1 = (from userdata in dbContext.UserMasterTable
                                   where userdata.RoleId == (int)RoleEnum.Doctors && userdata.Isactive == true
                                   select new UserModel
                                   {
                                       Id = userdata.Id,
                                       Email = userdata.Email,
                                       Phone = userdata.Phone,
                                       Adress = userdata.Adress,
                                       Name = userdata.Name
                                   }).ToList();


                    foreach (var doct in doctorsList1)
                    {


                        var doctlist1 = (from Appionment in dbContext.AppionmentBooking where Appionment.DoctorId == doct.Id && Appionment.Isactive == true && Appionment.BookingConformation == true && Appionment.StartDateTime.Value.Date >= appionmentDate.Date && Appionment.StartDateTime <= appionmentDate.AddDays(1).Date select Appionment).ToList();

                        int fractionalMinutes = 0;

                        string availAbiityMessage = string.Empty;

                        var list = new List<ScheduleDate>();

                        foreach (var doc in doctlist1)
                        {

                            TimeSpan varTime = (DateTime)doc.EndDateTime - (DateTime)doc.StartDateTime;
                            list.Add(new ScheduleDate { StartDateTime = (DateTime)doc.StartDateTime, EndDateTime = (DateTime)doc.EndDateTime });
                            fractionalMinutes += (int)varTime.TotalMinutes;
                            
                        }
                        if (fractionalMinutes < hours * 60)
                        {  // 60  is minutes
                            availAbiityMessage = availabilitymessage(list);

                            doctorsAvailList.Add( new AvailabilityModel { 
                                 userModel = doct,
                                  availabilityMessage = availAbiityMessage
                            });

                        }

                    }
                }
            }
            catch (Exception ex)
            {
            }

            return doctorsAvailList;
        }

        private string availabilitymessage(List<ScheduleDate> scheduleDates) {

            string message = "Available "; 
            var startTime = new System.TimeSpan(09, 0, 0);
            var endTime = new System.TimeSpan(18, 0, 0);
            if (scheduleDates.Count != 0)
            {
                try
                {
                    scheduleDates = scheduleDates.OrderBy(x => x.StartDateTime).ToList();
                   
                    foreach (var scheduledate in scheduleDates) {

                        var scheduleStartDateTime = new System.TimeSpan(scheduledate.StartDateTime.Hour, scheduledate.StartDateTime.Minute, scheduledate.StartDateTime.Second);
                        var scheduleEndDateTime = new System.TimeSpan(scheduledate.EndDateTime.Hour, scheduledate.EndDateTime.Minute, scheduledate.EndDateTime.Second);
                        if (scheduleStartDateTime == startTime)
                        {
                            startTime = new System.TimeSpan(scheduledate.EndDateTime.Hour, scheduledate.EndDateTime.Minute + 1, scheduledate.EndDateTime.Second);
                        }
                        else if (scheduleStartDateTime > startTime ) {

                            TimeSpan varTime = scheduledate.StartDateTime.TimeOfDay - startTime;
                            int minutes = (int)varTime.TotalMinutes;

                            if (minutes > 15) {
                                message += "from "+startTime.Hours + " : " + startTime.Minutes + " to " + scheduleStartDateTime.Hours +" : "+scheduleStartDateTime.Minutes +" :::: ";
                            }
                            startTime = new System.TimeSpan(scheduledate.EndDateTime.Hour, scheduledate.EndDateTime.Minute + 1, scheduledate.EndDateTime.Second);
                        }
                    
                    }
                    if (startTime < endTime) {
                        message += "from " + startTime.Hours + " : " + startTime.Minutes + " to " + endTime.Hours + " : " + endTime.Minutes;
                    }

                }
                catch (Exception e)
                {

                }
            }
            else { 
                message = message += "from " + startTime.Hours + " : " + startTime.Minutes + " to " + endTime.Hours + " : " + endTime.Minutes;
            }
            
            return message;
        }

        public List<UserModel> GetAllDoctorsList()
        {
            var doctorsList = new List<UserModel>();
            try
            {
                using (var dbContext = new Hospital_Appointment_DBContext())
                {
                    doctorsList = (from userdata in dbContext.UserMasterTable
                                   where userdata.RoleId == (int)RoleEnum.Doctors && userdata.Isactive == true
                                   select new UserModel
                                   {
                                       Id = userdata.Id,
                                       Email = userdata.Email,
                                       Phone = userdata.Phone,
                                       Adress = userdata.Adress,
                                       Name = userdata.Name
                                   }).ToList();
                }
            }
            catch (Exception ex)
            {

            }

            return doctorsList;
        }


        public List<UserModel> GetAllPatientsList()
        {
            var doctorsList = new List<UserModel>();
            try
            {
                using (var dbContext = new Hospital_Appointment_DBContext())
                {
                    doctorsList = (from userdata in dbContext.UserMasterTable
                                   where userdata.RoleId == (int)RoleEnum.Patient && userdata.Isactive == true
                                   select new UserModel
                                   {
                                       Id = userdata.Id,
                                       Email = userdata.Email,
                                       Phone = userdata.Phone,
                                       Adress = userdata.Adress,
                                       Name = userdata.Name
                                   }).ToList();
                }
            }
            catch (Exception ex)
            {

            }

            return doctorsList;
        }

        public List<UserModel> GetAllAdminList()
        {
            var doctorsList = new List<UserModel>();
            try
            {
                using (var dbContext = new Hospital_Appointment_DBContext())
                {
                    doctorsList = (from userdata in dbContext.UserMasterTable
                                   where userdata.RoleId == (int)RoleEnum.ClinicAdmins && userdata.Isactive == true
                                   select new UserModel
                                   {
                                       Id = userdata.Id,
                                       Email = userdata.Email,
                                       Phone = userdata.Phone,
                                       Adress = userdata.Adress,
                                       Name = userdata.Name
                                   }).ToList();
                }
            }
            catch (Exception ex)
            {

            }

            return doctorsList;
        }
    }
}
