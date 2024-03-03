using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using HelpingHands.Models;
using HelpingHands.DAL;
using Microsoft.AspNetCore.Http;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio;
using System.Text;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using AspNetCore;
using HelpingHands.ViewModels;

namespace HelpingHands.Controllers
{
    public class UserController : Controller
    {
        private readonly DataAccessLayer _dal;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INotyfService _toastNotification;
       
        public UserController(DataAccessLayer dal, 
            IHttpContextAccessor contextAccessor,
            INotyfService toastNofication
            )
        {
            _dal = dal;
            _contextAccessor = contextAccessor;
            _toastNotification = toastNofication;
            
        }

        [HttpGet]
        public IActionResult Index()
        {

            List<UserVM> userList = new List<UserVM>();
            try
            {
                userList = _dal.GetAllEmployees();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return View(userList);
			
        }

        [HttpGet]
        public IActionResult GetAllUsers(string search)
        {

            List<UserVM> userList = new List<UserVM>();

            try
            {
                if(search == null || search == "")
                {
                    userList = _dal.GetAllUsers();
                }
                else
                {
                    userList = _dal.SearchUsers(search);
                }
                                           
                
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return View(userList);

        }


        [HttpGet]
        public IActionResult Register()
        {
            var user = new User();
            return View(user);
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model is not valid";
            }
            
            if (ModelState.IsValid)
            {
                
                 bool result = _dal.RegisterUser(user);                                       
                    
                if (!result)
                {
                     _toastNotification.Success("Congratulations, your account has been registered successfully.", 5);
                    
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to register user";
                    return View(user);
                }
                        
                return RedirectToAction("Login");
                

               
            }
            TempData["SuccessMessage"] = "Account registered successfully";
            return View(user);
        }
                      

        public IActionResult AddEmployee(int id)
		{
            ViewBag.UserType = new SelectList(_dal.GetUserType(), "UserTypeId", "UserTypeDescription");
            return View();
		}

		[HttpPost]
        public IActionResult AddEmployee(User users)
        {
           
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model is not valid";
            }

            if (ModelState.IsValid)
            {
                
                bool result = _dal.AddEmployee(users);

                if (!result)
                {
                    TempData["ErrorMessage"] = "Unable to register user";
                }
                _toastNotification.Success("User has been added successfully", 4);

                return RedirectToAction("Admin", "Dashboard");
                    

                
            }
            
            ViewBag.UserType = new SelectList(_dal.GetUserType(), "UserTypeId", "UserTypeDescription");
            return View(users);
        }

		public IActionResult UpdateEmployee(int id)
		{
            try
            {

                UserVM user = _dal.GetEmployeeById(id);

                if (user.UserId == 0)
                {
                    TempData["errorMessage"] = $"Employee details with Id {id} cannot be found";
                    return RedirectToAction("Index");
                }
                ViewBag.UserType = new SelectList(_dal.GetUserType(), "UserTypeId", "UserTypeDescription");
                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.UserType = new SelectList(_dal.GetUserType(), "UserTypeId", "UserTypeDescription");
                TempData["errorMessage"] = ex.Message;
                return View();
            }
                      

			
		}

		[HttpPost]
		public IActionResult UpdateEmployee(UserVM users)
		{
			if (!ModelState.IsValid)
			{
				TempData["ErrorMessage"] = "Model is not valid";
			}

            if (ModelState.IsValid)
            {

                bool result = _dal.UpdateEmployee(users);

                if (!result)
                {
                    _toastNotification.Success("User has been updated successfully", 4);
                    
                }
                TempData["ErrorMessage"] = "Unable to update user";

                return RedirectToAction("Admin", "Dashboard");



            }


            ViewBag.UserType = new SelectList(_dal.GetUserType(), "UserTypeId", "UserTypeDescription");
			return View(users);
		}

              

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            UserVM users = _dal.GetEmployeeById(id);
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model is not valid";
            }

            if (ModelState.IsValid)
            {
                users.Status = "I";
                bool result = _dal.DeleteUser(users);

                if (!result)
                {
                    _toastNotification.Success("User has been deactivated successfully", 4);

                }
                TempData["ErrorMessage"] = "Unable to deactivated user";

                return RedirectToAction("Index", "User");



            }


            ViewBag.UserType = new SelectList(_dal.GetUserType(), "UserTypeId", "UserTypeDescription");
            return View(users);
        }

        [HttpPost]
        public IActionResult ActivateUser(int id)
        {
            UserVM users = _dal.GetEmployeeById(id);
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model is not valid";
            }

            if (ModelState.IsValid)
            {
                users.Status = "A";
                bool result = _dal.ActivateUser(users);

                if (!result)
                {
                    _toastNotification.Success("User has been activated successfully", 4);

                }
                TempData["ErrorMessage"] = "Unable to activate user";

                return RedirectToAction("GetAllUsers", "User");



            }


            ViewBag.UserType = new SelectList(_dal.GetUserType(), "UserTypeId", "UserTypeDescription");
            return View(users);
        }
          
        public IActionResult GetUsersForPassword(string search)
        {
            List<ChangePassword> userList = new List<ChangePassword>();

            try
            {
                if (search == null || search == "")
                {
                    userList = _dal.GetUsersForPassword();
                }
                

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return View(userList);
        }

        [HttpGet]
        public IActionResult ChangeUserPassword(int id)
        {
            try
            {

                ChangePassword user = _dal.GetUserById(id);

                if (id == 0)
                {
                    TempData["errorMessage"] = $"User details with Id {id} cannot be found";
                    return RedirectToAction("GetUsersForPassword","User");
                }
                ViewBag.UserType = new SelectList(_dal.GetUserType(), "UserTypeId", "UserTypeDescription");
                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.UserType = new SelectList(_dal.GetUserType(), "UserTypeId", "UserTypeDescription");
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            
        }

        [HttpPost]
        public IActionResult ChangeUserPassword(ChangePassword users)
        {

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model is not valid";
            }

            if (ModelState.IsValid)
            {

                bool result = _dal.ChangePassword(users);

                if (!result)
                {
                    _toastNotification.Success("User password has been changed successfully", 4);
                    return RedirectToAction("ChangeUserPassword", "User");
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to change password";
                    return RedirectToAction("Admin", "Dashboard");

                }


            }


            ViewBag.UserType = new SelectList(_dal.GetUserType(), "UserTypeId", "UserTypeDescription");
            return View(users);
        }

       

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }
        [HttpPost]
		public IActionResult Login(Login login)
		{
			User loggedInUser = _dal.GetLogin(login.UserName, login.Password);

			if (loggedInUser != null)
			{
				HttpContext.Session.SetInt32("UserId", loggedInUser.UserId);
                HttpContext.Session.SetString("UserName", loggedInUser.UserName);

                HttpContext.Session.SetString("UserTypeDescription", loggedInUser.UserTypeDescription);
                int userId = loggedInUser.UserId;
                
                

				if (loggedInUser.UserTypeDescription == "Admin")
				{
					return RedirectToAction("Admin", "Dashboard");
				}
				else if (loggedInUser.UserTypeDescription == "Office Manager")
				{
					return RedirectToAction("OfficeManager", "Dashboard");
				}
				else if (loggedInUser.UserTypeDescription == "Nurse")
				{
					return RedirectToAction("Nurse", "Dashboard");
				}
				else if (loggedInUser.UserTypeDescription == "Patient")
				{
					return RedirectToAction("Patient", "Dashboard");
				}
			}
			else
			{
				TempData["logError"] = "Invalid login credentials!";
			}
			return View();
		}


        [HttpGet]
        public IActionResult ChangePatientPassword()
        {

            return View();

        }

        [HttpPost]
        public IActionResult ChangePatientPassword(User user)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                TempData["ErrorMessage"] = "User session is invalid.";
                return View(user);
            }



            if (userId != 0)
            {
                //Updating User Password
                user.UserId = userId;
                bool result = _dal.UpdatePassword(user);

                if (!result)
                {
                    _toastNotification.Success("Password updated successfully.");

                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to add Patient profile.";

                }
            }

            return View(user);

        }



        [HttpGet]
        public IActionResult ChangeNursePassword()
        {

            return View();

        }

        [HttpPost]
        public IActionResult ChangeNursePassword(User user)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            
            if (userId == 0)
            {
                TempData["ErrorMessage"] = "User session is invalid.";
                return View(user);
            }

                        

            if (userId != 0)
            {
                //Updating User Password
                user.UserId = userId;
                bool result = _dal.UpdatePassword(user);

                if (!result)
                {
                    _toastNotification.Success("Password updated successfully.");

                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to add Patient profile.";
                    
                }
            }
           
            return View(user);

        }
        public bool IsImage(string name)
        {
            var extentionAccept = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var extention = Path.GetExtension(name).ToLower();
            return extentionAccept.Contains(extention);
        }

        [HttpGet]
        public IActionResult UpdatePatientProfile()
        {

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;


            try
            {

                Patient patient = _dal.GetPatientProfile(userId);

                if (patient.UserId == 0)
                {
                    return View();

                }

                return View(patient);
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        
        [HttpPost]
        public IActionResult UpdatePatientProfile(Patient patient, IFormFile imageFile)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                TempData["ErrorMessage"] = "User session is invalid.";
                return View(patient);
            }

            


            int patientProfile = _dal.GetPatientId(userId);


            if (patientProfile == 0)
            {
                // Add Patient Profile

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (IsImage(imageFile.FileName))
                    {
                        using (var stream = new MemoryStream())
                        {
                            imageFile.CopyTo(stream);
                            patient.Picture = stream.ToArray();
                        }
                        patient.UserId = userId;
                        bool result = _dal.AddPatientProfile(patient);
                        if (!result)
                        {
                            _toastNotification.Success("Profile updated successfully.");

                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Unable to add Patient profile.";

                            return RedirectToAction("Patient", "Dashboard");
                        }

                    }
                    
                }

                
            }
            else
            {

                // Update Patient Profile

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (IsImage(imageFile.FileName))
                    {
                        using (var stream = new MemoryStream())
                        {
                            imageFile.CopyTo(stream);
                            patient.Picture = stream.ToArray();
                        }

                        patient.UserId = userId;
                        bool result = _dal.UpdatePatientProfile(patient);

                        if (!result)
                        {
                            _toastNotification.Success("Profile updated successfully.");

                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Unable to update Nurse profile.";

                            return RedirectToAction("Patient", "Dashboard");
                        }
                    }

                }


               
            }


            return View(patient);
            
		}
        public IActionResult GetPatientProfile(int userId)
        {

            userId = HttpContext.Session.GetInt32("UserId") ?? 0;


            try
            {

                Patient patient = _dal.GetPatientProfile(userId);

                if (patient.UserId == 0)
                {
                    return View(patient);

                }

                return View(patient);
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }


        [HttpGet]
        public IActionResult UpdateNurse()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;


            try
            {

                Nurse nurse = _dal.GetNurseProfile(userId);

                if (nurse.UserId == 0)
                {
                    return View();

                }

                return View(nurse);
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public IActionResult UpdateNurse(Nurse nurse, IFormFile imageFile)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                TempData["ErrorMessage"] = "User session is invalid.";
                return View(nurse);
            }


            int nurseProfile = _dal.GetNurseId(userId);


            if (nurseProfile == 0)
            {
                // Add Patient Profile

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (IsImage(imageFile.FileName))
                    {
                        using (var stream = new MemoryStream())
                        {
                            imageFile.CopyTo(stream);
                            nurse.Picture = stream.ToArray();
                        }
                        nurse.UserId = userId;
                        bool result = _dal.AddNurseProfile(nurse);
                        if (!result)
                        {
                            _toastNotification.Success("Profile updated successfully.");

                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Unable to add Patient profile.";

                            return RedirectToAction("Nurse", "Dashboard");
                        }

                    }

                }


            }
            else
            {

                // Update Patient Profile

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (IsImage(imageFile.FileName))
                    {
                        using (var stream = new MemoryStream())
                        {
                            imageFile.CopyTo(stream);
                            nurse.Picture = stream.ToArray();
                        }

                        nurse.UserId = userId;
                        bool result = _dal.UpdateNurseProfile(nurse);

                        if (!result)
                        {
                            _toastNotification.Success("Profile updated successfully.");

                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Unable to update Nurse profile.";

                            return RedirectToAction("Nurse", "Dashboard");
                        }
                    }

                }



            }




            return View(nurse);
        }

        [HttpGet]
        public IActionResult GetNurseProfile(int userId)
        {

            userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            try
            {
                Nurse profile = _dal.GetNurseProfile(userId);

                if (profile.UserId == 0)
                {
                    TempData["errorMessage"] = $"Details with Id {profile} cannot be found";
                    return RedirectToAction("Nurse", "Dashboard");
                }
                return View(profile);

            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
    }
}
      