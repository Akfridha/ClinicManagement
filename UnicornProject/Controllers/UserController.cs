using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnicornProject.Helper;
using UnicornProject.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UnicornProject.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUsers _users; 
        public UserController(IUsers userRepository) {
            _users  = userRepository;
        }

        [HttpGet]
        [Route("/")]
         public IActionResult Homepage()
        {
            return Ok(" Home page");
        }

        [Route("register")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddUser(UserModel userModel) {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var users =  _users.AddUser(userModel);
            return Ok(users);
        }

        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserModel userModel)
        {
            if(string.IsNullOrEmpty(userModel.Email) && string.IsNullOrEmpty(userModel.Password))
                return BadRequest(new { message = "User name or password is empty" });
            var token = _users.ValidateUser(userModel);
            if (token == null || token.token == String.Empty)
                return BadRequest(new { message = "User name or password is incorrect" });
            return  Ok(token);
        }

        [Route("doctors")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult AlllDoctorsList()
        { 
                var doctors = _users.GetAllDoctorsList();
                return Ok(doctors);
        }

        [Route("patient")]
        [Authorize]
        [HttpPost]
        public IActionResult AllPatientList()
        {
            var patients = _users.GetAllPatientsList();
            return Ok(patients);
        }

        [Route("admin")]
        [Authorize]
        [HttpPost]
        public IActionResult AllAdminList()
        {
            var admin = _users.GetAllAdminList();
            return Ok(admin);
        }

        [Route("doctors/{id}")]
        [HttpPost]
        public IActionResult GetDoctorById(int id)
        {
            var doctors = _users.FindDoctorById(id);
            if (doctors == null) {
                return BadRequest(new { message = "Not valid doctors ID" });
            }
            return Ok(doctors);
        }

        [Route("doctors/availablelist/{dateTime}")]
        [HttpPost]
        [Authorize]
        public IActionResult DoctorList(DateTime dateTime)
        {
            var doctors = _users.GetAvailableDoctorsList(dateTime);
            return Ok(doctors);
        }

    }
}
