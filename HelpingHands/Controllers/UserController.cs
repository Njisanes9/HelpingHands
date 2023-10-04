using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using HelpingHands.Models;
using HelpingHands.DAL;



namespace HelpingHands.Controllers
{
    public class UserController : Controller
    {
        private readonly DataAccessLayer _dal;
        private readonly IHttpContextAccessor _contextAccessor;
        
        
        public UserController(DataAccessLayer dal, IHttpContextAccessor contextAccessor)
        {
            _dal = dal;
            _contextAccessor = contextAccessor;
        }


        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
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
                    TempData["ErrorMessage"] = "Unable to register user";
                }
                return RedirectToAction("Login");
            }
            TempData["SuccessMessage"] = "Account registered successfully";
            return View(user);
        }
        public IActionResult Nurse()
        {
            return View();
        }

        public void EntryInSession(string UserName)
        {
			HttpContext.Session.SetString("UserName",UserName);
		}

        

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Login(Login login)
        {
            string loginUser = _dal.GetLogin(login.UserName, login.Password);

           if (!string.IsNullOrEmpty(loginUser))
				{
					HttpContext.Session.SetString("UserTypeDescription", loginUser);

					if (loginUser == "Admin")
					{
						return RedirectToAction("Admin", "Dashboard");
					}
					else if (loginUser == "Office Manager")
					{
						return RedirectToAction("OfficeManager", "Dashboard");

					}
					else if (loginUser == "Nurse")
					{
						return RedirectToAction("Nurse", "Dashboard");

					}
					else if (loginUser == "Patient")
					{
						return RedirectToAction("Patient", "Dashboard");

					}
                    
	       }
           else
           {
                ViewData["logError"] = "Invalid login credentials!";
           }
           return View();
            

        }
        public IActionResult Test()
        {
            return View();
        }
    }
}
      