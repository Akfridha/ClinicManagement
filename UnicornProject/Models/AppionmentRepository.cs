using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Claims;
using System.Threading;
using UnicornProject.Helper;

namespace UnicornProject.Models
{
    public class AppionmentRepository : IAppionment
    {
        private Hospital_Appointment_DBContext _hospital_Appointment_DBContext;

        private IUsers _users;

        public AppionmentRepository(Hospital_Appointment_DBContext hospital_Appointment_DBContext, IUsers users)
        {

            _hospital_Appointment_DBContext = hospital_Appointment_DBContext;
            _users = users;
        }

        public List<UserAppionmentViewModel> AppionemntList()
        {
            throw new System.NotImplementedException();
        }


        public bool IsBookappionment(int docterID, DateTime startDateTime, DateTime endDateTime)
        {

            bool isBookappionment = false;
            int maxhours = 8;
            try
            {
                DateTime dateOnly = startDateTime.Date;
                using (var dbContext = _hospital_Appointment_DBContext)
                {
                    var doctlist = (from Appionment in dbContext.AppionmentBooking where Appionment.DoctorId == docterID && Appionment.Isactive == true && Appionment.BookingConformation == true && Appionment.StartDateTime >= startDateTime.Date && Appionment.StartDateTime <= startDateTime.AddDays(1).Date select Appionment).ToList();

                    int fractionalMinutes = 0;

                    if (doctlist.Count < 12)
                    {

                        foreach (var doct in doctlist)
                        {

                            TimeSpan varTime = (DateTime)doct.EndDateTime - (DateTime)doct.StartDateTime;
                            fractionalMinutes += (int)varTime.TotalMinutes;
                        }

                        if (fractionalMinutes < maxhours * 60)
                        {  // 60  is minutes

                            var availablitycheck = (from doc in doctlist
                                                    where
       TimeConversion.calculateSeconds(doc.StartDateTime.Value) >= TimeConversion.calculateSeconds(startDateTime) && TimeConversion.calculateSeconds(doc.EndDateTime.Value) <= TimeConversion.calculateSeconds(endDateTime)
                                                    select doc).ToList();
                            if (availablitycheck.Count == 0)
                            {
                                isBookappionment = true;
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {

            }
            return isBookappionment;

        }

        public APIResponse BookAppionment(AppionmentModel appionment)
        {

            APIResponse aPIResponse = new APIResponse();
            aPIResponse.Status = false;
            try
            {
                TimeSpan varTime = appionment.EndDateTime - appionment.StartDateTime;
                int minutes = (int)varTime.TotalMinutes;
                if (minutes >= 15 && minutes <= 120)
                {

                    bool bookvalidation = IsBookappionment(appionment.DoctorId, appionment.StartDateTime, appionment.EndDateTime);



                    if (bookvalidation)
                    {
                        UserAppionmentViewModel userAppionmentViewModel = new UserAppionmentViewModel();
                        using (var dbContext = new Hospital_Appointment_DBContext())
                        {
                            var appionmentmodel = new AppionmentBooking()
                            {
                                DoctorId = appionment.DoctorId,
                                PatientsId = appionment.PatientId,
                                EndDateTime = appionment.EndDateTime,
                                StartDateTime = appionment.StartDateTime,
                                CreatedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now,
                                Isactive = true,
                                BookingConformation = true
                            };
                            dbContext.Add(appionmentmodel);
                            dbContext.SaveChanges();
                            userAppionmentViewModel.Id = appionmentmodel.Id;
                            userAppionmentViewModel.Doctor = _users.FindUserById(appionment.DoctorId);
                            userAppionmentViewModel.User = null;
                            userAppionmentViewModel.Appionment = new AppionmentModel { 
                                 StartDateTime = appionment.StartDateTime,
                                 EndDateTime = appionment.EndDateTime
                            };

                            aPIResponse.Status = true;
                            aPIResponse.Data = userAppionmentViewModel;
                            aPIResponse.StatusMessage = "appionment Booked successfully";
                        }
                    }
                    else
                    {
                        aPIResponse.StatusMessage = "This doctor not available your schedule date and time";
                    }
                }
                else
                {
                    aPIResponse.StatusMessage = "Booking time should be minum 15 minutes and maximum 120 minutes";
                }

            }
            catch (Exception ex)
            {
                //

                aPIResponse.StatusMessage = ex.Message;
            }
            return aPIResponse;
        }

        public bool CancelAppionemnt(int aappionmentID)
        {
            bool iscuccess = false;
            try
            {
                using (var dbContext = _hospital_Appointment_DBContext)
                {
                    var appionemnt = (from appionmentlist in dbContext.AppionmentBooking where appionmentlist.Id == aappionmentID select appionmentlist).FirstOrDefault();
                    if (appionemnt != null)
                    {
                        appionemnt.BookingConformation = false;
                        dbContext.AppionmentBooking.Attach(appionemnt);
                        dbContext.Entry(appionemnt).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        iscuccess = true;

                    }
                }
            }
            catch (Exception ex)
            {
                //
            }
            return iscuccess;
        }

        public UserAppionmentViewModel GetAppionemntDetailsByID(int id)
        {
            throw new System.NotImplementedException();
        }

        public List<UserAppionmentViewModel> AppionemntListByDoctorId(int doctorId)
        {

            var userAppionmentViewModel = new List<UserAppionmentViewModel>();

            try
            {
                using (var dbContext = _hospital_Appointment_DBContext)
                {

                    userAppionmentViewModel = (from appionment in dbContext.AppionmentBooking
                                               where appionment.BookingConformation == true && appionment.Isactive == true && appionment.DoctorId == doctorId
                                               join doctor in dbContext.UserMasterTable on appionment.DoctorId equals doctor.Id
                                               where doctor.Isactive == true
                                               join user in dbContext.UserMasterTable on appionment.PatientsId equals user.Id
                                               where user.Isactive == true 
                                               select new UserAppionmentViewModel
                                               {
                                                   Id = appionment.Id,
                                                   Appionment = new AppionmentModel
                                                   {
                                                       StartDateTime = (DateTime)appionment.StartDateTime,
                                                       EndDateTime = (DateTime)appionment.EndDateTime
                                                   },
                                                   Doctor = new UserModel
                                                   {
                                                       Id = appionment.DoctorId,
                                                       Name = doctor.Name,
                                                       Email = doctor.Email,
                                                       Adress = doctor.Adress,
                                                       Phone = doctor.Phone

                                                   },
                                                   User = new UserModel
                                                   {
                                                       Id = appionment.PatientsId,
                                                       Name = user.Name,
                                                       Email = user.Email,
                                                       Phone = user.Phone,
                                                       Adress = user.Adress
                                                   }

                                               }).ToList();

                }

            }
            catch (Exception ex)
            {
            }

            return userAppionmentViewModel;

        }

        public List<UserAppionmentViewModel> AppionemntListByUserId(int userID)
        {
            var userAppionmentViewModel = new List<UserAppionmentViewModel>();
            try
            {
                if (userID == 0)
                {
                    var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                    var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                    userID = Convert.ToInt32(sid);
                }


                using (var dbContext = _hospital_Appointment_DBContext)
                {

                    userAppionmentViewModel = (from appionment in dbContext.AppionmentBooking
                                               where appionment.PatientsId == userID && appionment.BookingConformation == true && appionment.Isactive == true
                                               join doctor in dbContext.UserMasterTable on appionment.DoctorId equals doctor.Id
                                               where doctor.Isactive == true
                                               select new UserAppionmentViewModel
                                               {
                                                   Id = appionment.Id,
                                                   Appionment = new AppionmentModel
                                                   {
                                                       StartDateTime = (DateTime)appionment.StartDateTime,
                                                       EndDateTime = (DateTime)appionment.EndDateTime
                                                   },
                                                   Doctor = new UserModel
                                                   {
                                                       Id = appionment.DoctorId,
                                                       Name = doctor.Name,
                                                       Email = doctor.Email,
                                                       Adress = doctor.Adress,
                                                       Phone = doctor.Phone

                                                   }

                                               }).ToList();

                }

            }
            catch (Exception ex)
            {
            }

            return userAppionmentViewModel;
        }

        public List<UserModel> AppionemntDoctorListBasedOnhours(DateTime dateTime)
        {
            int hours =  6 ;

            var doctList = new List<UserModel>();
            var userIdList = new List<int>();
            try
            {
                using (var dbContext = new Hospital_Appointment_DBContext())
                {
                    var doctlist = (from Appionment in dbContext.AppionmentBooking where Appionment.Isactive == true && Appionment.BookingConformation == true && Appionment.StartDateTime >= dateTime.Date && Appionment.StartDateTime <= dateTime.AddDays(1).Date group Appionment by new
                    {
                        Appionment.DoctorId
                    } into gcs select gcs.Key.DoctorId).ToList();
                  
                    foreach (var doc in doctlist)
                    {

                        var doctlist1 = (from Appionment in dbContext.AppionmentBooking where Appionment.DoctorId == doc && Appionment.Isactive == true && Appionment.BookingConformation == true && Appionment.StartDateTime >= dateTime.Date && Appionment.StartDateTime <= dateTime.AddDays(1).Date select Appionment).ToList();

                        int fractionalMinutes = 0;


                        foreach (var doct in doctlist1)
                        {

                            TimeSpan varTime = (DateTime)doct.EndDateTime - (DateTime)doct.StartDateTime;
                            fractionalMinutes += (int)varTime.TotalMinutes;
                            
                        }
                        if (fractionalMinutes >= hours * 60)
                        {  // 60  is minutes
                            userIdList.Add((int)doc);
                            continue;
                        }
                    }
                    doctList = (from userdata in dbContext.UserMasterTable
                                where userdata.RoleId == (int)RoleEnum.Doctors && userdata.Isactive == true && userIdList.Contains(userdata.Id)
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

            return doctList;
        }


        public UserAppionmentViewModel AppionmentDetailsById(int id)
        {
            UserAppionmentViewModel userAppionmentViewModel = new UserAppionmentViewModel();
            try
            {
                using (var dbContext = new Hospital_Appointment_DBContext())
                {
                    userAppionmentViewModel = (from appionemnt in dbContext.AppionmentBooking
                                               join user in dbContext.UserMasterTable on appionemnt.PatientsId equals user.Id
                                               join doctor in dbContext.UserMasterTable on appionemnt.DoctorId equals doctor.Id
                                               where appionemnt.Id == id
                                               select new UserAppionmentViewModel
                                               {
                                                   Appionment = new AppionmentModel
                                                   {
                                                       EndDateTime = (DateTime)appionemnt.EndDateTime,
                                                       StartDateTime = (DateTime)appionemnt.StartDateTime,

                                                   },
                                                   Doctor = new UserModel
                                                   {
                                                       Name = doctor.Name,
                                                       Email = doctor.Email,
                                                       Adress = doctor.Adress,
                                                       Phone = doctor.Phone
                                                   },
                                                   User = new UserModel
                                                   {
                                                       Name = user.Name,
                                                       Email = user.Email,
                                                       Adress = user.Adress,
                                                       Phone = user.Phone
                                                   }

                                               }).FirstOrDefault();


                }
            }
            catch (Exception ex)
            {
            }

            return userAppionmentViewModel;
        }

        public dynamic MostAppionmentBookedDoctorsList(DateTime date) {

            try
            {
                using (var dbContext = new Hospital_Appointment_DBContext())
                {

                    var doctlist = (from Appionment in dbContext.AppionmentBooking
                                    where Appionment.Isactive == true && Appionment.BookingConformation == true && Appionment.StartDateTime >= date.Date && Appionment.StartDateTime <= date.Date.AddDays(1).Date
                                    group Appionment by Appionment.DoctorId
                    into gcs
                                    select new
                                    {
                                        docterId = gcs.Key,
                                        appionmentCount = gcs.Count()
                                    }).OrderByDescending(gcs => gcs.appionmentCount).Take(25).ToList();
                return doctlist;
                } 

                }
            catch (Exception ex) { 


            }
            return null;
            
        }
    }
}
