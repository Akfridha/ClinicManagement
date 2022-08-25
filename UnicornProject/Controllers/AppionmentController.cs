using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using UnicornProject.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UnicornProject.Controllers
{
    [ApiController]
    public class AppionmentController : ControllerBase
    {
        private IAppionment _appionment;

        public AppionmentController(IAppionment appionment)
        {
            _appionment = appionment;
        }


        [Route("appionment")]
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public IActionResult BookAppionemnt(AppionmentModel appionmentModel)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var appionment = _appionment.BookAppionment(appionmentModel);
            return Ok(appionment);
        }


        [Route("appionment/cancel/{appionmentID}")]
        [HttpPost]
        [Authorize(Roles = "Doctors,ClinicAdmins")]
        public IActionResult CancelAppionment(int appionmentID)
        {
            var appionment = _appionment.CancelAppionemnt(appionmentID);
            string msg = appionment ? "Canceled Successfully" : "Not Cancelled";
            return Ok(msg);
        }


        [Route("doctors/{doctorId}/slots")]
        [HttpPost]
        [Authorize]
        public IActionResult DoctorAppionmnetList(int doctorId)
        {
            var doctorList = _appionment.AppionemntListByDoctorId(doctorId);
            return Ok(doctorList);
        }

        [Route("appionment/doctorlist/{dateTime}")]
        [HttpPost]
        [Authorize(Roles = "ClinicAdmins")]
        public IActionResult DoctorsListBasedonHours(DateTime dateTime)
        {
            var doctorList = _appionment.AppionemntDoctorListBasedOnhours(dateTime);
            return Ok(doctorList);
        }

        [Route("appionment/{id}")]
        [HttpPost]
        [Authorize]
        public IActionResult AppionmentDetailsById(int id) {
            var appionmentDetails = _appionment.AppionmentDetailsById(id);
            return Ok(appionmentDetails);
        }

        [Route("patient/history/{id}")]
        [HttpPost]
        [Authorize]
        public IActionResult PatientHistory(int id) {
            var paientList = _appionment.AppionemntListByUserId(id);
            return Ok(paientList);
        }

        [Route("appionment/most/{dateTime}")]
        [HttpPost]
        [Authorize]
        public IActionResult AppionmentBookedByMost(DateTime dateTime)
        {
            var appionmentList = _appionment.MostAppionmentBookedDoctorsList(dateTime);
            return Ok(appionmentList);
        }

    }
}
