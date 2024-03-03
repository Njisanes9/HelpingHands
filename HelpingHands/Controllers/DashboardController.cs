using AspNetCoreHero.ToastNotification.Abstractions;
using HelpingHands.DAL;
using HelpingHands.Models;
using HelpingHands.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHands.Controllers
{
    public class DashboardController : Controller
    {

        private readonly DataAccessLayer _dal;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INotyfService _toastNotification;

        public DashboardController(DataAccessLayer dal,
            IHttpContextAccessor contextAccessor,
            INotyfService toastNofication
            )
        {
            _dal = dal;
            _contextAccessor = contextAccessor;
            _toastNotification = toastNofication;

        }
        public IActionResult Admin()
        {
            return View();
        }
        public IActionResult OfficeManager()
        {
            return View();
        }
        public IActionResult Nurse(DateTime date, int nurseId)
        {

             List<UpComingVisitVM> visits = new List<UpComingVisitVM>();
			List<PrefferedSuburb> suburbList = new List<PrefferedSuburb>();

			var visit = new UpComingVisitVM();
                date = DateTime.Now;
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                
            
            var nurseDetails = _dal.GetNurseDetailsById(userId);
            var picture = _dal.GetNursePicture(nurseId);

            if (nurseDetails != null)
            {
                // The user has updated their profile
                if (!string.IsNullOrEmpty(nurseDetails.Surname))
                {
                    _contextAccessor?.HttpContext.Session.SetString("Surname", nurseDetails.Surname);
                }
                if (!string.IsNullOrEmpty(nurseDetails.Name))
                {
                    _contextAccessor?.HttpContext.Session.SetString("FirstName", nurseDetails.Name);
                }
                if (nurseDetails.Picture !=null)
                {
                    _contextAccessor?.HttpContext.Session.Set("Picture", nurseDetails.Picture);
                }
                nurseId = _dal.GetNurseId(userId);

                visits = _dal.GetUpComingVisits(nurseId, date);                   
                ViewBag.Visits = visits;
                suburbList = _dal.GetNurseSuburb(nurseId);
                ViewBag.SuburbList = suburbList;




            }
            else
            {
                // The user recently registered
                var patient = _dal.GetUserNameById(userId);
                if (patient != null && !string.IsNullOrEmpty(patient.UserName))
                {
                    _contextAccessor?.HttpContext.Session.SetString("UserName", patient.UserName);
                }
                nurseId = _dal.GetNurseId(userId);
                
                try
                {
                    visits = _dal.GetUpComingVisits(nurseId, date);
                    suburbList = _dal.GetNurseSuburb(nurseId);
                    ViewBag.SuburbList = suburbList;
                    ViewBag.Visits = visits;
                    if (visits == null)
                    {
                        TempData["errorMessage"] = $"No care visit was found nurse {nurseId}";
                    }
                    return View();
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                }

                return View(visits);
            }

            return View();
        }
         
        public IActionResult Patient()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var patientDetails = _dal.GetPatientDetailsById(userId);

            if (patientDetails != null)
            {
                // The user has updated their profile
                if (!string.IsNullOrEmpty(patientDetails.Surname))
                {
                    _contextAccessor?.HttpContext.Session.SetString("Surname", patientDetails.Surname);
                }
                if (!string.IsNullOrEmpty(patientDetails.Name))
                {
                    _contextAccessor?.HttpContext.Session.SetString("FirstName", patientDetails.Name);
                }
                if (patientDetails.Picture != null)
                {
                    _contextAccessor?.HttpContext.Session.Set("Picture", patientDetails.Picture);
                }
            }
            else
            {
                // The user recently registered
                var patient = _dal.GetUserNameById(userId);
                if (patient != null && !string.IsNullOrEmpty(patient.UserName))
                {
                    _contextAccessor?.HttpContext.Session.SetString("UserName", patient.UserName);
                }
            }

            return View();

        }
    }
}
