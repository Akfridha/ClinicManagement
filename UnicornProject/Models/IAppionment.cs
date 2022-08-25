using System;
using System.Collections.Generic;
using UnicornProject.Helper;

namespace UnicornProject.Models
{
    public interface IAppionment
    {
        public APIResponse BookAppionment(AppionmentModel appionment);

        public bool CancelAppionemnt(int aappionmentID);

        public List<UserAppionmentViewModel> AppionemntList();

        public List<UserAppionmentViewModel> AppionemntListByDoctorId( int doctorId);

        public List<UserAppionmentViewModel> AppionemntListByUserId(int userID);

        public UserAppionmentViewModel GetAppionemntDetailsByID(int id);

        public List<UserModel> AppionemntDoctorListBasedOnhours(DateTime date);

        public UserAppionmentViewModel AppionmentDetailsById(int id);

        public dynamic MostAppionmentBookedDoctorsList(DateTime date);
    }
}
