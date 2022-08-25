using System;
using System.Collections.Generic;
using UnicornProject.Helper;

namespace UnicornProject.Models
{
    public interface IUsers
    {
        public List<UserModel> userList();
        public UserModel AddUser(UserModel user);
        public UserModel UpdateUser(UserModel user);

        public UserModel DeleteUser(UserModel user);

        public UserModel FindUserById(int userId);

        public UserModel FindUserByemail(string email);

        public TokenObject ValidateUser(UserModel user);

        public List<AvailabilityModel> GetAvailableDoctorsList(DateTime appionmentDate);

        public List<UserModel> GetAllDoctorsList();

        public List<UserModel> GetAllPatientsList();

        public List<UserModel> GetAllAdminList();

        public UserModel FindDoctorById(int id);
    }
}
